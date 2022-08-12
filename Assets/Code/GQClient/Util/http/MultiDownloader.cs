using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.UI;
using Code.GQClient.Util.tasks;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

namespace Code.GQClient.Util.http
{
    public class MultiDownloader : AbstractDownloader
    {
        /// <summary>
        /// Initializes a new Downloader object. 
        /// You can start the download as Coroutine either by calling StartCoroutine(download.startDownload) directly or
        /// by calling the method StartCallback(object sender, TaskEventArgs e). 
        /// The latter is used for task concatenation.
        /// 
        /// All callbacks are intialized with defaults. You can customize the behaviour via method delegates 
        /// onStart, onError, onTimeout, onSuccess, onProgress.
        /// </summary>
        /// <param name="maxParallelDownloads">Maximal number of parallel downloads.</param>
        /// <param name="timeout">Timeout in milliseconds (optional).</param>
        public MultiDownloader(
            int maxParallelDownloads = 15,
            long timeout = 0,
            List<MediaInfo> files = null) : base(new DownloadHandlerBuffer(), true)
        {
            if (files != null)
            {
                listOfFilesNotStartedYet = files;
            }

            MaxParallelDownloads = maxParallelDownloads;
            Timeout = timeout;
            stopwatch = new Stopwatch();
        }

        protected override void ReadInput(object input = null)
        {
            if (input is PrepareMediaInfoList.QuestWithMediaList questWithMediaList)
            {
                listOfFilesNotStartedYet = new List<MediaInfo>(questWithMediaList.MediaInfoList);
                Result = questWithMediaList;
            }
            else
            {
                Log.SignalErrorToDeveloper("MultiDownloader task did not receive valid MediaInfo List from Input.");
            }

            // if (listOfFilesNotStartedYet == null || listOfFilesNotStartedYet.Count == 0)
            // {
            //     RaiseTaskCompleted();
            // }
        }

        SimpleBehaviour dialogBehaviour;

        List<MediaInfo> listOfFilesNotStartedYet;

        #region Parallelization Limits

        private int MaxParallelDownloads { get; set; }

        private int CurrentlyRunningDownloads { get; set; }

        private bool LimitOfParallelDownloadsExceeded
        {
            get { return (CurrentlyRunningDownloads >= MaxParallelDownloads); }
        }

        #endregion

        private bool TimeIsUp
        {
            get { return (Timeout > 0 && stopwatch.ElapsedMilliseconds >= Timeout); }
        }

        float downloadedSumOfWeights = 0f;
        float totalSumOfWeights = 0f;

        /// <summary>
        /// Actually starts the download.
        /// </summary>
        /// <returns>The download.</returns>
        protected override IEnumerator DoTheWork()
        {
            // init SimpleBehaviour:
            dialogBehaviour = (SimpleBehaviour) behaviours[0];
            // TODO dangerous. Replace by concrete DialogControllers we will write.

            foreach (var curInfo in listOfFilesNotStartedYet)
            {
                totalSumOfWeights +=
                    ((curInfo.RemoteSize == MediaInfo.UNKNOWN) ? DEFAULT_WEIGHT : curInfo.RemoteSize);
            }

            CurrentlyRunningDownloads = 0;
            stopwatch.Start();
            var filesCurrentlyDownloading = new Dictionary<Downloader, MediaInfo>();

            while ((null != listOfFilesNotStartedYet && listOfFilesNotStartedYet.Count > 0) ||
                   filesCurrentlyDownloading.Count > 0)
            {
                // wait until a place for download is free:
                while (LimitOfParallelDownloadsExceeded && !TimeIsUp)
                {
                    yield return null;
                }

                yield return null;


                if (TimeIsUp)
                {
                    stopwatch.Stop();
                    Raise(DownloadEventType.Timeout, new DownloadEvent(elapsedTime: Timeout));
                    RaiseTaskFailed();
                    yield break;
                }

                if (listOfFilesNotStartedYet.Count > 0)
                {
                    // now we can start the next file downloader:
                    var infoToLoad = listOfFilesNotStartedYet[listOfFilesNotStartedYet.Count - 1];
                    // TODO use only string list with urls in listOfFilesNotStartedYet.

                    QuestManager.Instance.MediaStore.TryGetValue(infoToLoad.Url, out var info);
                    if (info == null)
                    {
                        QuestManager.Instance.IncreaseMediaUsage(infoToLoad);
                        info = infoToLoad;
                    }

                    var d =
                        new Downloader(
                            url: info.Url,
                            new DownloadHandlerFile(info.LocalPath),
                            timeout: 0L,
                            maxIdleTime: Config.Current.maxIdleTimeMS,
                            targetPath: info.LocalPath,
                            verbose: false,
                            weight: ((info.RemoteSize == MediaInfo.UNKNOWN) ? DEFAULT_WEIGHT : info.RemoteSize)
                        );
                    filesCurrentlyDownloading.Add(d, info);
                    CurrentlyRunningDownloads++;
                    d.OnProgress += ContributeToTotalProgress;
                    d.OnTimeout += (AbstractDownloader ad, DownloadEvent e) =>
                    {
                        if (filesCurrentlyDownloading.TryGetValue(d, out var infoToRestart))
                        {
                            listOfFilesNotStartedYet.Add(infoToRestart);
                            filesCurrentlyDownloading.Remove(d);
                        }
                    };
                    d.OnTaskCompleted += (object sender, TaskEventArgs e) =>
                    {
                        if (d.ResponseHeaders != null)
                        {
                            d.ResponseHeaders.TryGetValue("Content-Length", out var contentLength);
                            info.RemoteSize = contentLength == null ? MediaInfo.UNKNOWN : long.Parse(contentLength);
                            d.ResponseHeaders.TryGetValue("Last-Modified", out var timestamp);
                            info.RemoteTimestamp = PrepareMediaInfoList.ParseLastModifiedHeader(timestamp);
                            info.LocalSize = info.RemoteSize;
                            info.LocalTimestamp = info.RemoteTimestamp;
                        }
                    };
                    d.OnTaskEnded += (object sender, TaskEventArgs e) =>
                    {
                        filesCurrentlyDownloading.Remove(d);
                        CurrentlyRunningDownloads--;
                    };
                    listOfFilesNotStartedYet.Remove(infoToLoad);
                    d.Start();
                }
            }

            RaiseTaskCompleted();
        }

        private void ContributeToTotalProgress(object callbackSender, DownloadEvent args)
        {
            downloadedSumOfWeights += args.Progress;
            var percent = Math.Min(100.0f, (downloadedSumOfWeights / totalSumOfWeights) * 100f);
            dialogBehaviour.Progress(percent);
        }

        public override object Result { get; set; }
    }
}