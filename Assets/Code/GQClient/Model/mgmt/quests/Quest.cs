//#define DEBUG_LOG

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.FileIO;
using Code.GQClient.Model.expressions;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.pages;
using Code.GQClient.Util;
using Code.GQClient.Util.input;
using GQClient.Model;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.GQClient.Model.mgmt.quests
{
    /// <summary>
    /// The root object of a quests model at runtime. It represents all details of the quest at runtime.
    /// </summary>
    public class Quest : IComparable<Quest>
    {
        #region Attributes

        public string Name { get; set; }

        public int Id { get; set; }

        public long LastUpdate { get; set; }

        public string XmlFormat { get; set; }

        public bool IndividualReturnDefinitions { get; set; }

        public virtual bool IsShown
        {
            get
            {
                // TODO change the latter two checks to test a flag stored in game.xml base element as an attribute and move to QuestInfo
                var shown = ConfigurationManager.Current.ShowHiddenQuests || (Name != null && !Name.StartsWith("---"));
                return shown;
            }
        }

        #endregion

        #region State Pages

        private Dictionary<int, Page> pageDict = new Dictionary<int, Page>();

        public Page GetPageWithID(int id)
        {
            if (pageDict.TryGetValue(id, out var page))
            {
                return page;
            }
            else
            {
                return Page.Null;
            }
        }

        private Page StartPage { get; set; }

        protected Page currentPage;

        public Page CurrentPage
        {
            get { return currentPage; }
            internal set { currentPage = value; }
        }

        private QuestHistory _history = null;

        internal QuestHistory History
        {
            get
            {
                if (_history == null)
                {
                    _history = new QuestHistory(this);
                }

                return _history;
            }
            private set { _history = value; }
        }

        #endregion

        #region Hotspots

        private Dictionary<int, Hotspot> _hotspotDict = new Dictionary<int, Hotspot>();

        /// <summary>
        /// Adds the hotspot and starts trigger detection for entering/leaving hotspots if this is the first hotspot.
        /// </summary>
        /// <param name="hotspot">Hotspot.</param>
        protected void AddHotspot(Hotspot hotspot)
        {
            _hotspotDict[hotspot.Id] = hotspot;
        }


        /// <summary>
        /// Removes the hotspot and stops trigger detection for entering/leaving hotspots if this was the last hotspot.
        /// </summary>
        /// <param name="hotspot">Hotspot.</param>
        protected void RemoveHotspot(Hotspot hotspot)
        {
            _hotspotDict.Remove(hotspot.Id);
        }

        public void UpdateHotspotMarkers(System.Object sender, LocationSensor.LocationEventArgs e)
        {
            foreach (Hotspot h in AllHotspots)
            {
                if (h.Active)
                {
                    if (h.Status == Hotspot.StatusValue.UNDEFINED || h.Status == Hotspot.StatusValue.OUTSIDE)
                    {
                        if (h.InsideRadius(e.Location))
                        {
#if DEBUG_LOG
                            Debug.Log("ENTER HOTSPOT: " + h.Id);
#endif
                            h.Enter();
                        }
                    }

                    if (h.Status == Hotspot.StatusValue.INSIDE)
                    {
                        if (h.OutsideRadius(e.Location))
                        {
#if DEBUG_LOG
                            Debug.Log("LEAVE HOTSPOT: " + h.Id);
#endif
                            h.Leave();
                        }
                    }
                }
            }
        }

        public Hotspot GetHotspotWithID(int id)
        {
            Hotspot hotspot;
            _hotspotDict.TryGetValue(id, out hotspot);
            return hotspot;
        }

        public Dictionary<int, Hotspot>.ValueCollection AllHotspots
        {
            get { return _hotspotDict.Values; }
        }

        #endregion


        #region Metadata

        public Dictionary<string, string> metadata = new Dictionary<string, string>();

        #endregion


        #region Media

        private Dictionary<string, MediaInfo> _mediaStore = null;

        public Dictionary<string, MediaInfo> MediaStore
        {
            get
            {
                if (_mediaStore == null)
                {
                    _mediaStore = new Dictionary<string, MediaInfo>();
                }

                return _mediaStore;
            }
        }

        public void InitMediaStore()
        {
            _mediaStore = new Dictionary<string, MediaInfo>();

            var mediaJSON = "";
            try
            {
                mediaJSON = File.ReadAllText(GetMediaJsonPath(Id));
            }
            catch (FileNotFoundException)
            {
                mediaJSON = @"[]"; // we use an empty list then
            }
            catch (Exception e)
            {
                Log.SignalErrorToDeveloper("Error reading media.json for quest " + Id + ": " + e.Message);
                mediaJSON = @"[]"; // we use an empty list then
            }

            var localInfos = JsonConvert.DeserializeObject<List<LocalMediaInfo>>(mediaJSON);

            foreach (var localInfo in localInfos)
            {
                var info = new MediaInfo(localInfo);
                _mediaStore.Add(info.Url, info);
            }
        }

        public void AddMedia(string url, string contextDescription = "no context given")
        {
            if (string.IsNullOrEmpty(url) || url.StartsWith(GQML.PREFIX_RUNTIME_MEDIA))
                return;
            // TODO: we should ignore this hotspot marker in the back-end:
            if (ConfigurationManager.Current.id != "ebk" &&
                url == "https://quest-mill.intertech.de/assets/img/erzbistummarker.png")
                return;

            if (!MediaStore.ContainsKey(url))
            {
                var info = new MediaInfo(QuestInfoManager.QuestsRelativeBasePath, url);
                MediaStore.Add(url, info);
            }

            // QuestManager.Instance.AddMedia(url, contextDescription);
        }

        public static string GetMediaJsonPath(int questId) =>
            Files.CombinePath(QuestManager.GetLocalPath4Quest(questId: questId), "media.json");

        #endregion

        #region XML Reading

        public Quest(System.Xml.XmlReader reader)
        {
            QuestManager.CurrentlyParsingQuest = this; // TODO use event system instead

            // proceed to quest start element:
            while (!GQML.IsReaderAtStart(reader, GQML.QUEST))
            {
                reader.Read();
            }

            // we need the id first, because it is used in createing the media store...
            Id = GQML.GetIntAttribute(GQML.ID, reader);

            // set up the media store, depends on the id of the quest for paths:
            //InitMediaStore();

            // read all further attributes
            ReadFurtherAttributes(reader);

#if DEBUG_LOG
            Debug.Log("XML Quest Attrubutes read. Quest: " + Name + " id: " + Id);
#endif

            // Start the xml content: consume the begin quest element:
            reader.Read();

            while (!GQML.IsReaderAtEnd(reader, GQML.QUEST))
            {
                if (reader.NodeType != XmlNodeType.Element && !reader.Read())
                {
                    return;
                }

                // now we are at an element:
                switch (reader.LocalName)
                {
                    case GQML.PAGE:
                        ReadPage(reader);
                        break;
                    case GQML.HOTSPOT:
                        ReadHotspot(reader);
                        break;
                }
            }

            // we are done with this quest:
            QuestManager.CurrentlyParsingQuest = Null;
        }

        protected Quest()
        {
        }

        private void ReadFurtherAttributes(XmlReader reader)
        {
            Name = GQML.GetStringAttribute(GQML.QUEST_NAME, reader);
            XmlFormat = GQML.GetStringAttribute(GQML.QUEST_XMLFORMAT, reader);
            LastUpdate = GQML.GetLongAttribute(GQML.QUEST_LASTUPDATE, reader, 0L);
            IndividualReturnDefinitions =
                GQML.GetOptionalBoolAttribute(GQML.QUEST_INDIVIDUAL_RETURN_DEFINITIONS, reader, defaultVal: false);
        }

        private void ReadPage(XmlReader reader)
        {
            // now the reader is at a page element:
            string pageTypeName = reader.GetAttribute(GQML.PAGE_TYPE);
            if (pageTypeName == null)
            {
                Log.SignalErrorToDeveloper("Page without type attribute found.");
                reader.Skip();
                return;
            }

            // Determine the full name of the according page type (e.g. GQ.Client.Model.XML.PageNPCTalk) 
            //		where SetVariable is taken form ath type attribute of the xml action element.
            string __myTypeName = typeof(Page).FullName;
            int lastDotIndex = __myTypeName.LastIndexOf(".");
            string modelNamespace = __myTypeName.Substring(0, lastDotIndex);
            string targetScenePath = null;

            // page2scene mapping comes here:
            Dictionary<string, string> sceneMappings = ConfigurationManager.Current.GetSceneMappingsDict();
            if (sceneMappings.TryGetValue(pageTypeName, out targetScenePath))
            {
                pageTypeName = targetScenePath.Substring(
                    targetScenePath.LastIndexOf("/") + 1,
                    targetScenePath.Length - (targetScenePath.LastIndexOf("/") + 1 + ".unity".Length)
                );
            }

            pageTypeName = string.Format("{0}.Page{1}", modelNamespace, pageTypeName);
            Type pageType = Type.GetType(pageTypeName);

            if (pageType == null)
            {
                Log.SignalErrorToDeveloper("No Implementation for Page Type {0}.", pageTypeName);
                reader.Skip();
                return;
            }

            // get right constructor for page type:
            ConstructorInfo constructorInfoObj = pageType.GetConstructor(new Type[] {typeof(XmlReader)});
            if (constructorInfoObj == null)
            {
                Log.SignalErrorToDeveloper("Page {0} misses a Constructor for creating the model from XmlReader.",
                    pageTypeName);
            }

            Page page = (Page) constructorInfoObj.Invoke(new object[] {reader});
            page.Parent = this;
            if (StartPage == null && page.CanStart())
                StartPage = page;
            if (pageDict.ContainsKey(page.Id))
            {
                pageDict.Remove(page.Id);
            }

            try
            {
                pageDict.Add(page.Id, page);
            }
            catch (Exception e)
            {
                Debug.LogWarning((e.Message + " id: " + page.Id).Yellow());
            }
        }

        private void ReadHotspot(XmlReader reader)
        {
            Hotspot hotspot = new Hotspot(reader);
            hotspot.Parent = this;
            AddHotspot(hotspot);
        }

        #endregion


        #region Runtime API

        public virtual void Start()
        {
            if (StartPage == null)
            {
                Log.SignalErrorToDeveloper(
                    "Quest {0} can not be started, since the StartPage is null",
                    Id
                );
                return;
            }

            Variables.SetVariableValue("quest.name", new Value(Name));

            CurrentPage = Page.Null;

            Base.Instance.HideFoyerCanvases();
            StartPage.Start();
        }

        public void End()
        {
            End(true);
        }

        public void End(bool clearAlsoUpperCaseVariables = true)
        {
            Audio.Clear();
            Variables.Clear(clearAlsoUpperCaseVariables); // persistente variablen nicht löschen
            if (CurrentPage != null && CurrentPage.PageCtrl != null)
                CurrentPage.PageCtrl.CleanUp();
            var sceneToUnload = QuestManager.Instance.CurrentScene;
            if (sceneToUnload.isLoaded)
                SceneManager.UnloadSceneAsync(QuestManager.Instance.CurrentScene);
            QuestManager.Instance.CurrentQuest = Quest.Null;
            Base.Instance.ShowFoyerCanvases();
            Resources.UnloadUnusedAssets();
        }

        #endregion


        #region Null Object

        public static readonly Quest Null = new NullQuest();

        /// <summary>
        /// Null quest is used as a functional null object for quests. Is hidden and has some standard values (id:0, name:"NullQuest" etc.).
        /// </summary>
        private class NullQuest : Quest
        {
            public NullQuest() : base()
            {
                Name = "Null Quest";
                Id = 0;
                LastUpdate = 0;
                XmlFormat = "0";
                IndividualReturnDefinitions = false;
                CurrentPage = Page.Null;
            }

            public override void Start()
            {
                Log.WarnDeveloper("Null Quest started.");
            }

            public override bool IsShown
            {
                get { return false; }
            }
        }

        #endregion


        public int CompareTo(Quest q)
        {
            // TODO this is old. Should we change it?

            if (q == null)
            {
                return 1;
            }
            else
            {
                return this.Name.ToUpper().CompareTo(q.Name.ToUpper());
            }
        }
    }
}