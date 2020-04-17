//#define DEBUG_LOG

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.Model.gqml;
using Code.GQClient.UI;
using Code.GQClient.Util.http;
using Code.GQClient.Util.tasks;
using GQClient.Model;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.GQClient.Model.mgmt.quests
{
    public class PrepareMediaInfoList : Task
    {
        private List<MediaInfo> _filesCollectedForDownload;

        public PrepareMediaInfoList() : base()
        {
            _filesCollectedForDownload = new List<MediaInfo>();
        }

        private string GameXml { get; set; }

        protected override void ReadInput(object input)
        {
            switch (input)
            {
                case null:
                    RaiseTaskFailed();
                    return;
                case string s:
                    GameXml = s;
                    break;
                default:
                    Log.SignalErrorToDeveloper(
                        "Improper TaskEventArg received in SyncQuestData Task. Should be of type string but was " +
                        input.GetType().Name);
                    RaiseTaskFailed();
                    return;
            }
        }

        public delegate void PrepareMediaCallback(float percent);

        public event PrepareMediaCallback OnTimeout;

        #region Progress

        public event PrepareMediaCallback OnProgress;

        private int _stepsDone;

        private int StepsDone
        {
            get { return _stepsDone; }
            set
            {
                _stepsDone = value;
#if DEBUG_LOG
                    Debug.Log("StepDone set to " + value);
#endif
                OnProgress?.Invoke((100f * StepsDone) / (float) StepsTotal);
            }
        }

        private int StepsTotal { get; set; }

        #endregion

        private SimpleBehaviour _dialogBehaviour;

        protected override IEnumerator DoTheWork()
        {
            _dialogBehaviour = (SimpleBehaviour) behaviours[0];
            // TODO dangerous. Replace by concrete DialogControllers we will write.

            OnProgress += _dialogBehaviour.OnProgress;

            // step 1 deserialize game.xml:
            var quest = QuestManager.DeserializeQuest(GameXml);
            StepsTotal = 1 + quest.MediaStore.Count;
            StepsDone++;
            yield return null;

            var storedLocalInfos = GetStoredLocalInfosFromJson(quest.Id);
            var newLocalInfos = new Dictionary<string, MediaInfo> (quest.MediaStore);
            
            foreach (var storedLocalInfo in storedLocalInfos)
            {
                if (quest.MediaStore.TryGetValue(storedLocalInfo.url, out var info))
                {
                    // kept: still used media, we can remove it from the current media store:
                    newLocalInfos.Remove(storedLocalInfo.url);
                    collectWhenNewerOnServer(
                        QuestManager.Instance.MediaStore.TryGetValue(storedLocalInfo.url, out var globalInfo)
                            ? globalInfo
                            : info);
                }
                else
                {
                    // removed: this quest uses the media no more, so we reduce its global usage by one:
                    QuestManager.Instance.DecreaseMediaUsage(storedLocalInfo.url);
                }

                StepsDone++;
            }

            // new: now in the current media store only the new media is mentioned:
            foreach (var newMediaInfo in newLocalInfos.Values)
            {
                if (QuestManager.Instance.MediaStore.TryGetValue(newMediaInfo.Url, out var info))
                {
                    // new for this quest but already loaded on device:
                    QuestManager.Instance.IncreaseMediaUsage(info);
                    StepsTotal++; // we did not know that before
                    collectWhenNewerOnServer(info);
                    StepsDone++;
                }
                else
                {
                    Debug.Log($"ADDED NEW media: {newMediaInfo.Url}");
                    _filesCollectedForDownload.Add(newMediaInfo);
                }
            }

            Result = new QuestWithMediaList(quest, _filesCollectedForDownload);
            QuestManager.Instance.CurrentQuest = quest;
        }

        public static List<LocalMediaInfo> GetStoredLocalInfosFromJson(int questId)
        {
            string mediaJson;
            try
            {
                mediaJson = File.ReadAllText(Quest.GetMediaJsonPath(questId));
            }
            catch (FileNotFoundException)
            {
                mediaJson = @"[]"; // we use an empty list then
            }
            catch (Exception e)
            {
                Log.SignalErrorToDeveloper("Error reading media.json for quest " + questId + ": " + e.Message);
                mediaJson = @"[]"; // we use an empty list then
            }

            return JsonConvert.DeserializeObject<List<LocalMediaInfo>>(mediaJson);
        }

        private void collectWhenNewerOnServer(MediaInfo info)
        {
            if (info.Url.StartsWith(GQML.PREFIX_RUNTIME_MEDIA, StringComparison.Ordinal)) return;

            HttpWebRequest httpWReq;
            try
            {
                httpWReq =
                    (HttpWebRequest) WebRequest.Create(info.Url);
                httpWReq.Timeout = (int) Math.Min(
                    500,
                    ConfigurationManager.Current.maxIdleTimeMS
                );
            }
            catch (UriFormatException)
            {
                Log.SignalErrorToAuthor(
                    "Quest contains a wrong formatted URI: {0}.", info.Url);
                return;
            }

            HttpWebResponse httpWResp = null;
            try
            {
                httpWReq.Method = "HEAD";
                httpWResp = (HttpWebResponse) httpWReq.GetResponse();
            }
            catch (WebException)
            {
                Log.SignalErrorToDeveloper("Timeout while getting WebResponse for url {1}", HTTP.CONTENT_LENGTH,
                    info.Url);
                info.RemoteSize = MediaInfo.UNKNOWN;
                info.RemoteTimestamp = MediaInfo.UNKNOWN;
                // Since we do not know the timestamp of this file we load it:
 
                Debug.Log($"ADDED media due to TIMEOUT: {info.Url}");

                _filesCollectedForDownload.Add(info);
                httpWResp?.Close();
                return;
            }

            // got a response so we can use the data from server:
            info.RemoteSize = httpWResp.ContentLength;
            info.RemoteTimestamp = ParseLastModifiedHeader(httpWResp.GetResponseHeader("Last-Modified"));
            httpWResp.Close();

            // if the remote file is newer we update: 
            // or if media is not locally available we load it:
            if (info.RemoteTimestamp > info.LocalTimestamp || !info.IsLocallyAvailable)
            {
                Debug.Log($"ADDED media due to UPDATE: {info.Url} Times: remote:{info.RemoteTimestamp} local:{info.LocalTimestamp}");
                _filesCollectedForDownload.Add(info);
            }
         }

        public static long ParseLastModifiedHeader(string lastModHeader)
        {
            try
            {
                return Convert.ToInt64(lastModHeader);
            }
            catch (FormatException)
            {
                try
                {
                    var dt = DateTime.Parse(lastModHeader);
                    return (long) (dt - new DateTime(1970, 1, 1)).TotalMilliseconds;
                }
                catch (FormatException)
                {
                    return MediaInfo.UNKNOWN;
                }
            }
        }

        public class QuestWithMediaList
        {
            public Quest Quest { get; private set; }
            public List<MediaInfo> MediaInfoList { get; private set; }

            public QuestWithMediaList(Quest q, List<MediaInfo> mediaInfos)
            {
                Quest = q;
                MediaInfoList = mediaInfos;
            }
        }
    }
}