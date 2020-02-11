using System.IO;
using System.Xml;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.FileIO;
using Code.GQClient.Model.mgmt.questinfos;
using Code.GQClient.Model.pages;
using UnityEngine.SceneManagement;

namespace Code.GQClient.Model.mgmt.quests
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
            }
        }

        private Page _currentPage;
        public Page CurrentPage
        {
            get
            {
                return CurrentQuest.CurrentPage;
            }
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

        public bool PageReadyToStart
        {
            get
            {
                return _pageReadyToStart;
            }
            internal set
            {
                _pageReadyToStart = value;
            }
        }
        private bool _pageReadyToStart;

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
            if (string.IsNullOrEmpty(xml))
            {
                Log.SignalErrorToDeveloper("Tried to deserialize quest from emty or null xml.");
                return Quest.Null;
            }
            using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            {
                return new Quest(reader);
            }
        }
        #endregion
    }

}
