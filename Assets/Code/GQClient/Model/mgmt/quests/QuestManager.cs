using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.FileIO;
using GQClient.Model;
using Code.GQClient.Model.pages;
using Newtonsoft.Json;
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
            set => _instance = value;
        }

        public static void Reset()
        {
            _instance = null;
        }

        private static QuestManager _instance;

        internal bool MediaStoreIsDirty { get; set; }

        private QuestManager()
        {
            _currentQuest = Quest.Null;
            PageReadyToStart = true;
        }

        #endregion


        #region quest management functions

        public string CurrentQuestName4User { get; private set; }

        private Quest _currentQuest = Quest.Null;

        public Quest CurrentQuest
        {
            get => _currentQuest;
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

        public Page CurrentPage => CurrentQuest.CurrentPage;

        public Scene CurrentScene
        {
            get
            {
                var curScene = SceneManager.GetSceneByName(CurrentPage.PageSceneName);
                return curScene;
            }
        }

        #endregion


        #region Quest Access

        public static string GetQuestUri(int questId)
        {
            var uri = $"{ConfigurationManager.GetGQServerBaseURL()}/editor/{questId}/clientxml";
            return uri;
        }

        /// <summary>
        /// Gets the local quest dir path.
        /// </summary>
        /// <returns>The local quest dir path.</returns>
        /// <param name="questId">Quest I.</param>
        public static string GetLocalPath4Quest(int questId)
        {
            return QuestInfoManager.LocalQuestsPath + questId + "/";
        }

        public static string GetRuntimeMediaPath(int questId)
        {
            var path = Files.CombinePath(QuestManager.GetLocalPath4Quest(questId), "runtime");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public const string QUEST_FILE_NAME = "game.xml";

        #endregion


        #region Parsing

        private static Quest _currentlyParsingQuest;

        public static Quest CurrentlyParsingQuest
        {
            get
            {
                if (_currentlyParsingQuest == null)
                    _currentlyParsingQuest = Quest.Null;
                return _currentlyParsingQuest;
            }
            set => _currentlyParsingQuest = value;
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
        public void SetCurrentQuestFromXml(string xml)
        {
            CurrentQuest = DeserializeQuest(xml);
        }

        public static Quest DeserializeQuest(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Log.SignalErrorToDeveloper("Tried to deserialize quest from empty or null xml.");
                return Quest.Null;
            }

            using (var reader = XmlReader.Create(new StringReader(xml)))
            {
                return new Quest(reader);
            }
        }

        #endregion


        #region Media

        private Dictionary<string, MediaInfo> _mediaStore = null;

        public Dictionary<string, MediaInfo> MediaStore
        {
            get
            {
                if (_mediaStore == null)
                {
                    InitMediaStore();
                }

                return _mediaStore;
            }
        }

        public List<MediaInfo> GetListOfGlobalMediaInfos()
        {
            return MediaStore.Values.ToList<MediaInfo>();
        }


        private void InitMediaStore()
        {
            _mediaStore = new Dictionary<string, MediaInfo>();

            var mediaJSON = "";
            try
            {
                mediaJSON = File.ReadAllText(GlobalMediaJsonPath);
            }
            catch (FileNotFoundException)
            {
                mediaJSON = @"[]"; // we use an empty list then
            }
            catch (Exception e)
            {
                Log.SignalErrorToDeveloper($"Error reading global media.json: {e.Message}");
                mediaJSON = @"[]"; // we use an empty list then
            }

            var mediaInfos = JsonConvert.DeserializeObject<List<MediaInfo>>(mediaJSON);

            foreach (var info in mediaInfos)
            {
                _mediaStore.Add(info.Url, info);
            }

            MediaStoreIsDirty = false;
        }

        public MediaInfo AddMedia(string url, string contextDescription = "no context given")
        {
            if (string.IsNullOrEmpty(url))
                return null;

            if (!MediaStore.TryGetValue(url, out var info))
            {
                AddNewMedia(new MediaInfo(QuestInfoManager.QuestsRelativeBasePath, url));
            }

            return info;
        }

        public void DecreaseMediaUsage(string url)
        {
            if (MediaStore.TryGetValue(url, out var info))
            {
                info.UsageCounter--;
                if (info.UsageCounter <= 0)
                {
                    try
                    {
                        File.Delete(info.LocalPath);
                    }
                    catch (Exception e)
                    {
                        Log.SignalErrorToDeveloper($"Error while deleting media file {info.LocalPath} : {e.Message}");
                    }

                    MediaStore.Remove(url);
                }

                Instance.MediaStoreIsDirty = true;
            }
        }

        /// <summary>
        /// Either increases the counter for the existing entry or creates a new entry in the QM MediaStore.
        /// </summary>
        /// <param name="newUsedMediaInfo"></param>
        /// <returns>null if the counter of an existing media info was increased,
        /// or the file name for the newly created media info.</returns>
        public string IncreaseMediaUsage(MediaInfo newUsedMediaInfo)
        {
            MediaStoreIsDirty = true;

            if (MediaStore.TryGetValue(newUsedMediaInfo.Url, out var info))
            {
                info.UsageCounter++;
                return null;
            }

            return AddNewMedia(newUsedMediaInfo);
        }

        private string AddNewMedia(MediaInfo newInfo)
        {
            // TODO can be optimized from O(n) to O(log_n) by using a persisting hash set of filenames.
            var occupiedFileNames = new HashSet<string>();
            foreach (var info in Instance.MediaStore.Values)
            {
                occupiedFileNames.Add(info.LocalFileName);
            }

            var fileName = Files.FileName(newInfo.Url);
            var fileTypeExtension = Files.Extension(fileName);
            var fileNameWithoutExtension = Files.StripExtension(fileName);
            var fileNameCandidate = fileName;
            var discriminationNr = 1;
            while (occupiedFileNames.Contains(fileNameCandidate))
            {
                var discriminationAppendix = $"-{discriminationNr++}";
                fileNameCandidate = $"{fileNameWithoutExtension}{discriminationAppendix}.{fileTypeExtension}";
            }

            newInfo.LocalFileName = fileNameCandidate;
            newInfo.UsageCounter = 1;

            MediaStore.Add(newInfo.Url, newInfo);
            return newInfo.LocalFileName;
        }

        public static string MediaJsonPath4Quest(int questId) =>
            Files.CombinePath(GetLocalPath4Quest(questId: questId), "media.json");

        public static string GlobalMediaJsonPath => Files.CombinePath(QuestInfoManager.LocalQuestsPath, "media.json");

        #endregion
    }
}