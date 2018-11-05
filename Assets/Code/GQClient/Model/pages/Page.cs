using GQ.Client.Model;
using GQ.Client.Err;
using UnityEngine.SceneManagement;
using System.Xml;
using System.Xml.Serialization;
using System;
using UnityEngine;
using GQ.Client.Util;
using GQ.Client.UI;
using System.Collections.Generic;
using System.IO;
using GQ.Client.FileIO;
using GQ.Client.Conf;

namespace GQ.Client.Model
{
    [XmlRoot(GQML.PAGE)]
    public abstract class Page : IPage
    {

        #region XML Parsing

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            Debug.LogWarning("WriteXML not implemented for " + GetType().Name);
        }

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
        public void ReadXml(XmlReader reader)
        {
            GQML.AssertReaderAtStart(reader, GQML.PAGE);

            ReadAttributes(reader);

            if (reader.IsEmptyElement)
            {
                reader.Read();
                return;
            }

            // consume the Begin Action Element:
            reader.Read();

            XmlRootAttribute xmlRootAttr = new XmlRootAttribute();
            xmlRootAttr.IsNullable = true;

            while (!GQML.IsReaderAtEnd(reader, GQML.PAGE))
            {

                if (reader.NodeType == XmlNodeType.Element)
                    ReadContent(reader, xmlRootAttr);
            }

            // consume the closing action tag (if not empty page element)
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
                Log.SignalErrorToDeveloper("Id for a page could not be parsed. We found: " + reader.GetAttribute(GQML.ID));
            }

            PageType = GQML.GetStringAttribute(GQML.PAGE_TYPE, reader);

        }

        /// <summary>
        /// If you enhance this method by overriding it in subtypes to process additional content, 
        /// you should first process the additional content or alternatives and at the end 
        /// call this implementation with base.ReadContent() as fallback.
        /// </summary>
        /// <param name="reader">Reader.</param>
        /// <param name="xmlRootAttr">Xml root attr.</param>
        protected virtual void ReadContent(XmlReader reader, XmlRootAttribute xmlRootAttr)
        {
            XmlSerializer serializer;

            switch (reader.LocalName)
            {
                case GQML.ON_START:
                    xmlRootAttr.ElementName = GQML.ON_START;
                    serializer = new XmlSerializer(typeof(Trigger), xmlRootAttr);
                    StartTrigger = (Trigger)serializer.Deserialize(reader);
                    StartTrigger.Parent = this;
                    break;
                case GQML.ON_END:
                    xmlRootAttr.ElementName = GQML.ON_END;
                    serializer = new XmlSerializer(typeof(Trigger), xmlRootAttr);
                    EndTrigger = (Trigger)serializer.Deserialize(reader);
                    EndTrigger.Parent = this;
                    break;
                // UNKOWN CASE:
                default:
                    Log.WarnDeveloper("Page {0} has additional unknown {1} element. (Ignored)", Id, reader.LocalName);
                    reader.Skip();
                    break;
            }
        }

        protected Trigger StartTrigger = Trigger.Null;
        protected Trigger EndTrigger = Trigger.Null;

        #endregion


        #region State

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

        private string result = "";

        public virtual string Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
                Variables.SetInternalVariable("$_mission_" + Id + ".result", new Value(value));
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

        #endregion

        #region Runtime API

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

        // called when a scene has been loaded:
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.SetActiveScene(scene);
            foreach (Scene sceneToUnload in scenesToUnload)
            {
                if (sceneToUnload.isLoaded)
                {
                    SceneManager.UnloadSceneAsync(sceneToUnload);
                }
            }
            scenesToUnload.Clear();
            SceneManager.sceneLoaded -= OnSceneLoaded;
            
            Resources.UnloadUnusedAssets();
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

        public virtual void Start()
        {
            if (!CanStart())
                return;

            // set this quest as current in QM
            QuestManager.Instance.CurrentQuest = Parent;

            // clean up old page and set this as new:
            if (QuestManager.Instance.CurrentPage != null)
            {
                QuestManager.Instance.CurrentPage.CleanUp();
            }
            QuestManager.Instance.CurrentPage = this;
            State = GQML.STATE_RUNNING;

            Resources.UnloadUnusedAssets();

            // ensure that the adequate scene is loaded:
            Scene scene = SceneManager.GetActiveScene();

            if (!scene.name.Equals(PageSceneName))
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.LoadSceneAsync(PageSceneName, LoadSceneMode.Additive);
                if (scene.name != Base.FOYER_SCENE_NAME)
                {
                    scenesToUnload.Add(scene);
                }
            }
            else
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

            // Trigger OnStart Actions of this page:
            StartTrigger.Initiate();
        }

        public virtual void End()
        {
            State = GQML.STATE_SUCCEEDED;

            if (EndTrigger == Trigger.Null)
            {
                Log.SignalErrorToAuthor(
                    "Quest {0} ({1}, page {2} has no actions onEnd defined, hence we end the quest here.",
                    Quest.Name, Quest.Id,
                    Id
                );
                Quest.End();
            }
            else
            {
                EndTrigger.Initiate();
            }
            Debug.Log("#### PAGE.End() before Resources.UnloadUnusedAssets().");
            Resources.UnloadUnusedAssets();
        }

        public void SaveResultInVariable()
        {
            Variables.SetInternalVariable("$_mission_" + Id + ".result", new Value(Result));
        }

        public void CleanUp()
        {
            if (PageCtrl != null)
            {
                PageCtrl.CleanUp();
            }
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
                Parent.CurrentPage = this;
            }

            public override Quest Parent
            {
                get
                {
                    return Quest.Null;
                }
            }

            public override void Start()
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
