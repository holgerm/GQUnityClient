// #define DEBUG_LOG

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Code.GQClient.Err;
using Code.GQClient.Model.actions;
using Code.GQClient.Model.expressions;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.UI.pages;
using Code.GQClient.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using Action = System.Action;

namespace Code.GQClient.Model.pages
{
    public abstract class Page : ITriggerContainer
    {
        #region XML Parsing
        public virtual Quest Quest
        {
            get
            {
                return Parent;
            }
        }

        /// <summary>
        /// Reader must be at the page element (start). When it returns the reader is position behind the page end element. 
        /// 
        /// This is a template method. Subtypes should only override the ReadAttributes() and ReadContent() methods 
        /// and extend them by calling their base versions.
        /// </summary>
        /// <param name="reader">Reader.</param>
        public Page(XmlReader reader)
        {
#if DEBUG_LOG
            Debug.Log("XML Page start type: " + GetType());
#endif
            GQML.AssertReaderAtStart(reader, GQML.PAGE);

            ReadAttributes(reader);
#if DEBUG_LOG
            Debug.Log("XML Page Attribtes read. page id: " + Id);
            if (Id == 17945)
            {
                Debug.Log("Last order.");
            }
#endif

            if (reader.IsEmptyElement)
            {
                reader.Read();
#if DEBUG_LOG
                Debug.Log("XML Page was empty.");
#endif
                return;
            }

            // consume the Begin Element:
            reader.Read();
            while (!GQML.IsReaderAtEnd(reader, GQML.PAGE))
            {

                if (reader.NodeType == XmlNodeType.Element)
                    ReadContent(reader, new XmlRootAttribute()); // RECENTLY ADDED tha second parameter to that call.
            }
#if DEBUG_LOG
            Debug.Log("XML Page Content read.");
#endif

            // consume the closing tag (if not empty element)
            if (reader.NodeType == XmlNodeType.EndElement)
                reader.Read();
        }

        protected virtual void ReadAttributes(XmlReader reader)
        {
            // Id:
            int id;
            if (Int32.TryParse(reader.GetAttribute(GQML.ID), out id))
            {
                Id = id;
            }
            else
            {
                Log.SignalErrorToDeveloper(
                    "Id for a page could not be parsed. We found: {0}, line {1} pos {2}",
                    reader.GetAttribute(GQML.ID),
                    ((IXmlLineInfo)reader).LineNumber,
                    ((IXmlLineInfo)reader).LinePosition);
            }

            PageType = GQML.GetStringAttribute(GQML.PAGE_TYPE, reader);

        }

        /// <summary>
        /// If you enhance this method by overriding it in subtypes to process additional content, 
        /// you should first process the additional content or alternatives and at the end 
        /// call this implementation with base.ReadContent() as fallback.
        /// </summary>
        protected virtual void ReadContent(XmlReader reader, XmlRootAttribute xmlRootAttr)
        {
            ReadContent(reader);
        }
        
        protected virtual void ReadContent(XmlReader reader) {
            switch (reader.LocalName)
            {
                case GQML.ON_START:
                    StartTrigger = new Trigger(reader);
                    StartTrigger.Parent = this;
                    break;
                case GQML.ON_END:
                    EndTrigger = new Trigger(reader);
                    EndTrigger.Parent = this;
                    break;
                // UNKOWN CASE:
                default:
                    Log.WarnDeveloper("Page {0} has additional unknown {1} element. (Ignored) line {2} position {3}",
                        Id,
                        reader.LocalName,
                        ((IXmlLineInfo)reader).LineNumber,
                        ((IXmlLineInfo)reader).LinePosition);
                    reader.Skip();
                    break;
            }
        }

        protected Trigger StartTrigger = Trigger.Null;
        protected Trigger EndTrigger = Trigger.Null;

        public bool HasEndEvents()
        {
            return (EndTrigger != Trigger.Null && !EndTrigger.IsEmptyOrEndGameOnly());
        }

#endregion


#region Runtime

        public Page()
        {
            State = GQML.STATE_NEW;
            Result = "";
        }

        public virtual Quest Parent { get; set; }

        public int Id { get; protected set; }

        public string PageType { get; protected set; }

        private string state = GQML.STATE_NEW;

        public string State
        {
            get
            {
                return state;
            }
            protected set
            {
                switch (value)
                {
                    case GQML.STATE_NEW:
                    case GQML.STATE_RUNNING:
                    case GQML.STATE_SUCCEEDED:
                    case GQML.STATE_FAILED:
                        state = value;
                        Variables.SetInternalVariable("$_mission_" + Id + ".state", new Value(value));
                        break;
                    default:
                        Log.SignalErrorToDeveloper("Invalid Page State found: {0}", value);
                        break;
                }
            }
        }

        internal void TriggerOnStart()
        {
            StartTrigger.Initiate();
        }

        private string result = "";

        public string Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value ?? ""; // never store null, but an empty string instead
                Variables.SetInternalVariable("$_mission_" + Id + ".result", new Value(result));
            }
        }

        public override string ToString()
        {
            Scene scene = SceneManager.GetActiveScene();
            return "Page: "
                + base.ToString()
                      + "\n\tid: " + Id
                      + "\n\tquest: " + Quest.Id
                      + "\n\ttype: " + PageType
                      + "\n\tscene: " + scene.path
                      + "\n\tframe: " + Time.frameCount;
        }

        public PageController PageCtrl
        {
            get;
            set;
        }

        /// <summary>
        /// Maps the scene to this model for a page (mission).
        /// </summary>
        /// <value>The name of the page scene.</value>
        public virtual string PageSceneName
        {
            get
            {
                return GetType().Name.Substring(4);
            }
        }

        public Action OnPageSceneLoaded;

        // called when a scene has been loaded:
        void InitOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            OnPageSceneLoaded?.Invoke();
            OnPageSceneLoaded = null;
            
            SceneManager.SetActiveScene(scene);
            foreach (Scene sceneToUnload in scenesToUnload)
            {
                if (sceneToUnload.isLoaded)
                {
#if DEBUG_LOG
                    SceneManager.sceneUnloaded += DebugShowSceneUnloaded;
#endif
                    SceneManager.UnloadSceneAsync(sceneToUnload);
                }
            }
            scenesToUnload.Clear();
            SceneManager.sceneLoaded -= InitOnSceneLoaded;

            Resources.UnloadUnusedAssets();
            QuestManager.Instance.PageReadyToStart = true;
        }

#if DEBUG_LOG
        void DebugShowSceneUnloaded(Scene unloadedScene)
        {
            Debug.Log(
                string.Format("Scene unloaded: {0}. Frame# {1}.",
                    unloadedScene.name,
                    Time.frameCount).Yellow());
            SceneManager.sceneUnloaded -= DebugShowSceneUnloaded;
        }
#endif

        private void InitOnSceneReused(Scene scene)
        {
            // if we use the same page again, we have to initialize the UI controller again with the new data.
            GameObject goPageScreen = GameObject.Find(GO_PATH_PAGE_CONTROLLER);
            if (goPageScreen == null)
            {
                Log.SignalErrorToDeveloper(
                    "Page {0} using scene {1} does not have a PageController at {2}",
                    Id, scene.name, GO_PATH_PAGE_CONTROLLER
                );
                Quest.End();
                return;
            }

            PageController pageCtrl = goPageScreen.GetComponent<PageController>();
            if (pageCtrl == null)
            {
                Log.SignalErrorToDeveloper(
                    "Page {0} using scene {1} does not have a PageController at {2}",
                    Id, scene.name, GO_PATH_PAGE_CONTROLLER
                );
                Quest.End();
                return;
            }

            pageCtrl.InitPage();
        }

        public static List<Scene> scenesToUnload = new List<Scene>();

        private const string GO_PATH_PAGE_CONTROLLER = "PageController";

        /// <summary>
        /// Always returns true. Override this in subtypes if the according page type can not be started.
        /// </summary>
        /// <returns><c>true</c> if this instance can start; otherwise, <c>false</c>.</returns>
        public virtual bool CanStart()
        {
            return true;
        }

        public virtual void Start(bool canReturnToPrevious = false)
        {
            if (!CanStart())
                return;

            QuestManager.Instance.PageReadyToStart = false; // make new page wait for switching scene and unloading old page.
            Base.Instance.BlockInteractions(true);

            bool canReturn = canReturnToPrevious || !Quest.IndividualReturnDefinitions;
            Quest.History.Record(new PageStarted(this, canReturn));

            // set this quest as current in QM
            QuestManager.Instance.CurrentQuest = Parent;

            // clean up old page and set this as new:
            if (!Page.IsNull(Quest.CurrentPage))
            {
                Quest.CurrentPage.CleanUp();
            }
            Quest.CurrentPage = this;
            State = GQML.STATE_RUNNING;

            Resources.UnloadUnusedAssets();

            // ensure that the adequate scene is loaded:
            Scene scene = SceneManager.GetActiveScene();

            if (!scene.name.Equals(PageSceneName))
            {
                SceneManager.sceneLoaded += InitOnSceneLoaded;
                SceneManager.LoadSceneAsync(PageSceneName, LoadSceneMode.Additive);
                if (scene.name != Base.FOYER_SCENE_NAME)
                {
                    scenesToUnload.Add(scene);
                }
            }
            else
            {
                QuestManager.Instance.PageReadyToStart = true; // in case of reuse we do not need to block & wait 
                Base.Instance.BlockInteractions(false);
                InitOnSceneReused(scene);
            }
            
            StartTrigger.Initiate();
        }

        public virtual void End(Boolean leaveQuestIfEmpty = true)
        {
            State = GQML.STATE_SUCCEEDED;

            if (EndTrigger == Trigger.Null && leaveQuestIfEmpty)
            {
                Quest.End();
            }
            else
            {
                EndTrigger.Initiate();
            }
            Resources.UnloadUnusedAssets();
        }

        public void SaveResultInVariable()
        {
            Variables.SetInternalVariable("$_mission_" + Id + ".result", new Value(Result));
        }

        public virtual void CleanUp()
        {
            if (PageCtrl != null)
            {
                PageCtrl.CleanUp();
            }
        }

        public static bool IsNull(Page page)
        {
            return (page == null || page == Page.Null);
        }

#endregion


#region Null Object

        public static readonly Page Null = new NullPage();

        private class NullPage : Page
        {

            public NullPage()
                : base()
            {
                Id = 0;
                State = GQML.STATE_NEW;
            }

            public override Quest Parent
            {
                get
                {
                    return Quest.Null;
                }
            }

            public override void Start(bool canReturnToPrevious = false)
            {
                Log.WarnDeveloper("Null Page started in quest {0} (id: {1})", Parent.Name, Parent.Id);
                Parent.CurrentPage = this;
                State = GQML.STATE_RUNNING;
                StartTrigger.Initiate();
            }

        }

#endregion

    }
}
