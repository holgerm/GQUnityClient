//#define DEBUG_LOG

using System.Collections;
using GQ.Client.Util;
using GQ.Client.Err;
using QM.Util;
using System.Collections.Generic;
using System;
using System.Net;
using GQ.Client.Conf;
using GQ.Client.UI.Dialogs;
using UnityEngine;

namespace GQ.Client.Model
{

    public class PrepareMediaInfoList : Task
    {

        public PrepareMediaInfoList() : base() { }

        private string gameXML { get; set; }

        protected override void ReadInput(object input)
        {
            if (input == null)
            {
                RaiseTaskFailed();
                return;
            }

            if (input is string)
            {
                gameXML = input as string;
            }
            else
            {
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
            get
            {
                return _stepsDone;
            }
            set
            {
                _stepsDone = value;
                if (OnProgress != null)
                {
#if DEBUG_LOG
                    Debug.Log("StepDone set to " + value);
#endif
                    OnProgress((100f * StepsDone) / (float)stepsTotal);
                }
            }
        }
        private int stepsTotal { get; set; }
        #endregion

        SimpleDialogBehaviour dialogBehaviour;

        protected override IEnumerator DoTheWork()
        {
            dialogBehaviour = (SimpleDialogBehaviour)behaviours[0]; // TODO dangerous. Replace by conrete DialogControllers we will write.
            OnProgress += dialogBehaviour.OnProgress;

            // step 1 deserialize game.xml:
            Quest quest = QuestManager.Instance.DeserializeQuest(gameXML);
            stepsTotal = 2 + quest.MediaStore.Count;
            StepsDone++;
            yield return null;

            // step 2 import local media info:
            quest.ImportLocalMediaInfo();
            StepsDone++;
            yield return null;

            // step 3 include remote media info:
            Result = GetListOfFilesNeedDownload(quest);

            // TODO BAD HACK:
            QuestManager.Instance.CurrentQuest = quest;
        }

        /// <summary>
        /// This is step 3 of 4 during quest media sync. Downloads or updates the media files needed for this quest.
        /// </summary>
        public List<MediaInfo> GetListOfFilesNeedDownload(Quest quest)
        {
            // 1. we create a list of files to be downloaded / updated (as Dictionary with all neeeded data for multi downloader:
            List<MediaInfo> filesToDownload = new List<MediaInfo>();

            int infoNotReceived = 0;
            float summedSize = 0f;

            MediaInfo info;
            foreach (KeyValuePair<string, MediaInfo> kvpEntry in quest.MediaStore)
            {
#if DEBUG_LOG
                Debug.Log(("GetListOfFIlesNeedDownload: look at " + kvpEntry.Key).Yellow());
#endif
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
                        500,
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
                    httpWReq.Method = "HEAD";
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
                StepsDone++;
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
    }
}
