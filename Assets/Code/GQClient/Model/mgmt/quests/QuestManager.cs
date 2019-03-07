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
            CurrentPage = Page.Null;
            PageReadyToStart = true;
        }

        #endregion


        #region quest management functions

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
                Device.location.InitLocationMock();
            }
        }

        public Page CurrentPage { get; set; }

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


        /// <summary>
        /// Imports the local media infos fomr the game-media.json file and updates the existing media store. 
        /// This is step 2 of 4 in media sync (download or update of a quest).
        /// </summary>
        public void ImportLocalMediaInfo()
        {
            string mediaJSON = "";
            try
            {
                mediaJSON = File.ReadAllText(CurrentQuest.MediaJsonPath);
            }
            catch (FileNotFoundException)
            {
                mediaJSON = @"[]"; // we use an empty list then
            }
            catch (Exception e)
            {
                Log.SignalErrorToDeveloper("Error reading media.json for quest " + CurrentQuest.Id + ": " + e.Message);
                mediaJSON = @"[]"; // we use an empty list then
            }

            List<LocalMediaInfo> localInfos = JsonConvert.DeserializeObject<List<LocalMediaInfo>>(mediaJSON);

            List<string> occupiedFileNames = new List<string>();

            foreach (LocalMediaInfo localInfo in localInfos)
            {
                MediaInfo info;
                if (CurrentQuest.MediaStore.TryGetValue(localInfo.url, out info))
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
            foreach (KeyValuePair<string, MediaInfo> kvpEntry in CurrentQuest.MediaStore)
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
            foreach (KeyValuePair<string, MediaInfo> kvpEntry in CurrentQuest.MediaStore)
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
                info.RemoteTimestamp = (long)(httpWResp.LastModified - new DateTime(1970, 1, 1)).TotalMilliseconds;

                summedSize += info.RemoteSize;
                // if the remote file is newer we update: 
                // or if media is not locally available we load it:
                if (info.RemoteTimestamp > info.LocalTimestamp || !info.IsLocallyAvailable)
                {
                    filesToDownload.Add(info);
                }

                httpWResp.Close();


                //				// Request file header
                //				// TODO WHAT IF OFFLINE?
                //				Debug.Log (("Before getHeaders for " + info.Url + " time: " + Time.time + " frame:" + Time.frameCount).Red());
                //				Dictionary<string, string> headers = HTTP.GetRequestHeaders (info.Url);
                //				Debug.Log("After getHeaders time: " + Time.time + " frame:" + Time.frameCount);
                //
                //				string headerValue;
                //				if (!headers.TryGetValue (HTTP.CONTENT_LENGTH, out headerValue)) {
                //					Log.SignalErrorToDeveloper ("{0} header missing for url {1}", HTTP.CONTENT_LENGTH, info.Url);
                //					info.RemoteSize = MediaInfo.UNKNOWN;
                //				} else {
                //					info.RemoteSize = long.Parse (headerValue);
                //				}
                //
                //				if (!headers.TryGetValue (HTTP.LAST_MODIFIED, out headerValue)) {
                //					Log.SignalErrorToDeveloper ("{0} header missing for url {1}", HTTP.LAST_MODIFIED, info.Url);
                //					info.RemoteTimestamp = MediaInfo.UNKNOWN;
                //					// Since we do not know the timestamp of this file we load it:
                //					filesToDownload.Add (info);
                //				} else {
                //					info.RemoteTimestamp = long.Parse (headerValue);
                //
                //					// if the remote file is newer we update: 
                //					// or if media is not locally available we load it:
                //					if (info.RemoteTimestamp > info.LocalTimestamp || !info.IsLocallyAvailable) {
                //						filesToDownload.Add (info);
                //					}								
                //				}
            }

            return filesToDownload;
        }

        #endregion
    }

}
