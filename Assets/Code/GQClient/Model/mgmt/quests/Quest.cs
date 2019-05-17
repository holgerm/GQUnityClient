using UnityEngine;
using Newtonsoft.Json;
using GQ.Client.FileIO;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System;
using GQ.Client.Conf;
using System.Collections.Generic;
using GQ.Client.Err;
using System.IO;
using System.Xml;
using GQ.Client.Util;
using System.Net;

namespace GQ.Client.Model
{

    /// <summary>
    /// The root object of a quests model at runtime. It represents all details of the quest at runtime.
    /// </summary>
    [System.Serializable]
    [XmlRoot(GQML.QUEST)]
    public class Quest : IComparable<Quest>, IXmlSerializable
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
                return ConfigurationManager.Current.ShowHiddenQuests || (Name != null && !Name.StartsWith("---"));
            }
        }

        #endregion

        #region State Pages

        protected Dictionary<int, Page> pageDict = new Dictionary<int, Page>();

        public Page GetPageWithID(int id)
        {
            Page page;
            if (pageDict.TryGetValue(id, out page))
            {
                return page;
            }
            else
            {
                return Page.Null;
            }
        }

        public Page StartPage
        {
            get;
            set;
        }

        protected Page currentPage;

        public Page CurrentPage
        {
            get
            {
                return currentPage;
            }
            internal set
            {
                currentPage = value;
            }
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
            private set
            {
                _history = value;
            }
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
                            Debug.Log("ENTER HOTSPOT: " + h.Id);
                            h.Enter();
                        }
                    }
                    if (h.Status == Hotspot.StatusValue.INSIDE)
                    {
                        if (h.OutsideRadius(e.Location))
                        {
                            Debug.Log("LEAVE HOTSPOT: " + h.Id);
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
            get
            {
                return _hotspotDict.Values;
            }
        }

        #endregion


        #region Metadata

        public Dictionary<string, string> metadata = new Dictionary<string, string>();

        #endregion


        #region Media

        private Dictionary<string, MediaInfo> _mediaStore = null;

        public Dictionary<string, MediaInfo> MediaStore {
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

            string mediaJSON = "";
            try
            {
                mediaJSON = File.ReadAllText(MediaJsonPath);
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

            List<LocalMediaInfo> localInfos = JsonConvert.DeserializeObject<List<LocalMediaInfo>>(mediaJSON);

            foreach (LocalMediaInfo localInfo in localInfos)
            {
                MediaInfo info = new MediaInfo(localInfo);
                _mediaStore.Add(info.Url, info);
            }
        }

        public void AddMedia(string url, string contextDescription = "no context given")
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            if (!MediaStore.ContainsKey(url))
            {
                MediaInfo info = new MediaInfo(Id, url);
                MediaStore.Add(url, info);
            }
        }

        public string MediaJsonPath
        {
            get
            {
                return Files.CombinePath(QuestManager.GetLocalPath4Quest(Id), "media.json");
            }
        }


        /// <summary>
        /// Imports the local media infos fomr the game-media.json file and updates the existing media store. 
        /// This is step 2 of 4 in media sync (download or update of a quest).
        /// </summary>
        public void ImportLocalMediaInfo()
        {
            string mediaJSON = "";
            try
            {
                mediaJSON = File.ReadAllText(MediaJsonPath);
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

            List<LocalMediaInfo> localInfos = JsonConvert.DeserializeObject<List<LocalMediaInfo>>(mediaJSON);

            List<string> occupiedFileNames = new List<string>();

            foreach (LocalMediaInfo localInfo in localInfos)
            {
                MediaInfo info;
                if (MediaStore.TryGetValue(localInfo.url, out info))
                {
                    // add local information to media store:
                    info.LocalDir = localInfo.absDir;
                    info.LocalFileName = localInfo.filename;
                    info.LocalSize = localInfo.size;
                    info.LocalTimestamp = localInfo.time;
                    // remember filenames as occupied for later creation of new unique filenames
                    occupiedFileNames.Add(info.LocalFileName);
                }
                else
                {
                    // this media file is not useful anymore, we delete it:
                    string filePath = localInfo.LocalPath;
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch (Exception e)
                    {
                        Log.SignalErrorToDeveloper(
                            "Error while deleting media file " + filePath +
                            " : " + e.Message);
                    }
                }
            }

            // Step 2b determine missing local filenames for new urls:
            // MediaStore now has all needed mediainfos including local data for this quest.
            foreach (KeyValuePair<string, MediaInfo> kvpEntry in MediaStore)
            {
                if (kvpEntry.Value.LocalFileName == null || kvpEntry.Value.LocalFileName == "")
                {
                    string fileName = Files.FileName(kvpEntry.Value.Url);
                    string fileNameCandidate = fileName;
                    int discriminationNr = 1;
                    string discriminiationAppendix = "";
                    while (occupiedFileNames.Contains(fileNameCandidate))
                    {
                        fileNameCandidate = fileName + discriminiationAppendix;
                        discriminiationAppendix = "-" + discriminationNr++;
                    }
                    kvpEntry.Value.LocalFileName = fileNameCandidate;
                }
            }
        }
        /// <summary>
        /// This is step 3 of 4 during quest media sync. Downloads or updates the media files needed for this quest.
        /// </summary>
        public List<MediaInfo> GetListOfFilesNeedDownload()
        {
            // 1. we create a list of files to be downloaded / updated (as Dictionary with all neeeded data for multi downloader:
            List<MediaInfo> filesToDownload = new List<MediaInfo>();

            int infoNotReceived = 0;
            float summedSize = 0f;

            MediaInfo info;
            foreach (KeyValuePair<string, MediaInfo> kvpEntry in MediaStore)
            {
                info = kvpEntry.Value;

                if (info.Url.StartsWith(GQML.PREFIX_RUNTIME_MEDIA, StringComparison.Ordinal))
                {
                    // not an url but instead a local reference to media that is created at runtime within a quest.
                    continue;
                }

                HttpWebRequest httpWReq = null;
                try
                {
                    httpWReq =
                        (HttpWebRequest)WebRequest.Create(info.Url);
                    httpWReq.Timeout = (int)Math.Min(
                        3000,
                        ConfigurationManager.Current.maxIdleTimeMS
                    );
                }
                catch (UriFormatException)
                {
                    Log.SignalErrorToAuthor("Quest contains a wrong formatted URI: {0}.", info.Url);
                    continue;
                }


                HttpWebResponse httpWResp;
                try
                {
                    httpWResp = (HttpWebResponse)httpWReq.GetResponse();
                }
                catch (WebException)
                {
                    Log.SignalErrorToDeveloper("Timeout while getting WebResponse for url {1}", HTTP.CONTENT_LENGTH, info.Url);
                    info.RemoteSize = MediaInfo.UNKNOWN;
                    info.RemoteTimestamp = MediaInfo.UNKNOWN;
                    infoNotReceived++;
                    // Since we do not know the timestamp of this file we load it:
                    filesToDownload.Add(info);
                    continue;
                }
                // got a response so we can use the data from server:
                info.RemoteSize = httpWResp.ContentLength;
                info.RemoteTimestamp = ParseLastModifiedHeader(httpWResp.GetResponseHeader("Last-Modified"));

                summedSize += info.RemoteSize;
                // if the remote file is newer we update: 
                // or if media is not locally available we load it:
                if (info.RemoteTimestamp > info.LocalTimestamp || !info.IsLocallyAvailable)
                {
                    filesToDownload.Add(info);
                }

                httpWResp.Close();
            }

            return filesToDownload;
        }

        private long ParseLastModifiedHeader(string lastmodHeader)
        {
            try
            {
                return Convert.ToInt64(lastmodHeader);
            }
            catch (FormatException)
            {
                try
                {
                    DateTime dt = DateTime.Parse(lastmodHeader);
                    return (long)(dt - new DateTime(1970, 1, 1)).TotalMilliseconds;
                }
                catch (FormatException)
                {
                    return MediaInfo.UNKNOWN;
                }
            }
        }
        #endregion

        #region XML Reading
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// This method should only be called from the XML Serialization Framework. 
        /// It will be indirectly used by the QuestManager method DeserializeQuest().
        /// </summary>
        /// <returns>The xml.</returns>
        /// <param name="reader">Reader.</param>
        public void ReadXml(System.Xml.XmlReader reader)
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

            // Start the xml content: consume the begin quest element:
            reader.Read();

            // Content:
            XmlRootAttribute xmlRootAttr = new XmlRootAttribute();
            xmlRootAttr.IsNullable = true;

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

        private void ReadFurtherAttributes(XmlReader reader)
        {
            Name = GQML.GetStringAttribute(GQML.QUEST_NAME, reader);
            XmlFormat = GQML.GetStringAttribute(GQML.QUEST_XMLFORMAT, reader);
            LastUpdate = GQML.GetLongAttribute(GQML.QUEST_LASTUPDATE, reader);
            IndividualReturnDefinitions = GQML.GetOptionalBoolAttribute(GQML.QUEST_INDIVIDUAL_RETURN_DEFINITIONS, reader, defaultVal:false);
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
            string __myTypeName = this.GetType().FullName;
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

            XmlSerializer serializer = new XmlSerializer(pageType);
            Page page = (Page)serializer.Deserialize(reader);
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
            XmlSerializer serializer = new XmlSerializer(typeof(Hotspot));
            Hotspot hotspot = (Hotspot)serializer.Deserialize(reader);
            hotspot.Parent = this;
            AddHotspot(hotspot);
        }


        public void WriteXml(System.Xml.XmlWriter writer)
        {
            Debug.LogWarning("WriteXML not implemented for " + GetType().Name);
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

        public void End(bool clearAlsoUpperCaseVariables = true)
        {
            Audio.Clear();
            Variables.Clear(clearAlsoUpperCaseVariables); // persistente variablen nicht löschen
            CurrentPage.PageCtrl.CleanUp();
            Scene sceneToUnload = QuestManager.Instance.CurrentScene;
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

            public NullQuest()
                : base()
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
                get
                {
                    return false;
                }
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

