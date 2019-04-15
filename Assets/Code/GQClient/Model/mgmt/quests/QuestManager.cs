using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using GQ.Client.Model;
using System.IO;
using GQ.Client.Err;
using System.Collections.Generic;
using GQ.Client.Conf;
using GQ.Client.FileIO;

using System;
using GQ.Client.Util;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.Net;
using QM.Util;

namespace GQ.Client.Model
{

    public class QuestManager
    {

        #region singleton

        public static QuestManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new QuestManager();
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public static void Reset()
        {
            _instance = null;
        }

        private static QuestManager _instance = null;

        private QuestManager()
        {
            _currentQuest = Quest.Null;
            PageReadyToStart = true;
        }

        #endregion


        #region quest management functions

        public string CurrentQuestName4User
        {
            get;
            private set;
        }

        private Quest _currentQuest = Quest.Null;
        public Quest CurrentQuest
        {
            get
            {
                return _currentQuest;
            }
            set
            {
                _currentQuest = value;
                if (value.IsShown)
                {
                    CurrentQuestName4User = value.Name;
                }
                //Device.location.InitLocationMock(); // TODO really? Always?
            }
        }

        private Page _currentPage;
        public Page CurrentPage
        {
            get
            {
                return CurrentQuest.CurrentPage;
            }
            //set
            //{
            //    _currentPage = value;

            //    if (_currentPage != null)
            //    {

            //        Debug.Log(("Started Page: " + _currentPage.Id + " (" + _currentPage.PageType + ")").Green());

            //    }
            //}
        }

        public Scene CurrentScene
        {
            get
            {
                Scene curScene = SceneManager.GetSceneByName(CurrentPage.PageSceneName);
                return curScene;
            }
        }

        #endregion


        #region Quest Access

        public static string GetQuestURI(int questID)
        {
            string uri = string.Format("{0}/editor/{1}/clientxml",
                             ConfigurationManager.GQ_SERVER_BASE_URL,
                             questID
                         );
            return uri;
        }

        /// <summary>
        /// Gets the local quest dir path.
        /// </summary>
        /// <returns>The local quest dir path.</returns>
        /// <param name="questID">Quest I.</param>
        public static string GetLocalPath4Quest(int questID)
        {
            return QuestInfoManager.LocalQuestsPath + questID + "/";
        }

        public static string GetRuntimeMediaPath(int questID)
        {
            string path = Files.CombinePath(QuestManager.GetLocalPath4Quest(questID), "runtime");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public const string QUEST_FILE_NAME = "game.xml";

        /// <summary>
        /// Makes the local file name from the given URL, 
        /// so that the file name is unique and reflects the filename within the url.
        /// </summary>
        /// <returns>The local file name from URL.</returns>
        /// <param name="url">URL.</param>
        public static string MakeLocalFileNameFromUrl(string url)
        {
            string filename = Files.FileName(url);
            return filename; // TODO
        }

        public string CurrentMediaJSONPath
        {
            get
            {
                return GetLocalPath4Quest(CurrentQuest.Id) + "/media.json";
            }
        }

        #endregion


        #region Parsing

        protected void serializer_UnknownNode
            (object sender, XmlNodeEventArgs e)
        {
            Log.SignalErrorToDeveloper("Unknown XML Node found in Quest XML:" + e.Name + "\t" + e.Text);
        }

        protected void serializer_UnknownAttribute
            (object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            Log.SignalErrorToDeveloper("Unknown XML Attribute found in Quest XML:" +
            attr.Name + "='" + attr.Value + "'");
        }

        private static Quest currentlyParsingQuest;

        public static Quest CurrentlyParsingQuest
        {
            get
            {
                if (currentlyParsingQuest == null)
                    currentlyParsingQuest = Quest.Null;
                return currentlyParsingQuest;
            }
            set
            {
                currentlyParsingQuest = value;
            }
        }

        public bool PageReadyToStart { get; internal set; }

        /// <summary>
        /// Reads the quest from its game.xml file and creates a complete model hierarchy in memory and 
        /// store its root the quest object as CurrentQuest.
        /// 
        /// This is step 1 of 4 in media sync (download or update of a quest).
        ///
        /// </summary>
        /// <param name="xml">Xml.</param>
        public void SetCurrentQuestFromXML(string xml)
        {
            CurrentQuest = DeserializeQuest(xml);
        }

        public Quest DeserializeQuest(string xml)
        {
            // Creates an instance of the XmlSerializer class;
            // specifies the type of object to be deserialized.
            XmlSerializer serializer = new XmlSerializer(typeof(Quest));

            // If the XML document has been altered with unknown 
            // nodes or attributes, handles them with the 
            // UnknownNode and UnknownAttribute events.
            serializer.UnknownNode += new
                XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new
                XmlAttributeEventHandler(serializer_UnknownAttribute);

            using (TextReader reader = new StringReader(xml))
            {
                return (Quest)serializer.Deserialize(reader);
            }
        }
        #endregion
    }

}
