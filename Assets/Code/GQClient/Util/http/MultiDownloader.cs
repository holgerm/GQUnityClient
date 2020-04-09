using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.UI;
using Code.GQClient.Util.tasks;

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
        /// <param name="timeout">Timout in milliseconds (optional).</param>
        public MultiDownloader(
            int maxParallelDownloads = 15,
            long timeout = 0,
            List<MediaInfo> files = null) : base(true)
        {
            if (files != null)
            {
                listOfFilesNotStartedYet = files;
            }
            Result = "";
            MaxParallelDownloads = maxParallelDownloads;
            Timeout = timeout;
            stopwatch = new Stopwatch();
        }

        protected override void ReadInput(object input = null)
        {
            if (input is List<MediaInfo>)
            {
                listOfFilesNotStartedYet = input as List<MediaInfo>;
            }
            else
            {
                Log.SignalErrorToDeveloper("MultiDownloader task did not receive valid MediaInfo List from Input.");
            }

            if (listOfFilesNotStartedYet == null || listOfFilesNotStartedYet.Count == 0)
            {
                RaiseTaskCompleted();
            }


        }

        SimpleBehaviour dialogBehaviour;

        List<MediaInfo> listOfFilesNotStartedYet;

        #region Parallelization Limits

        private int MaxParallelDownloads { get; set; }

        private int CurrentlyRunningDownloads { get; set; }

        private bool LimitOfParallelDownloadsExceeded
        {
            get
            {
                return (CurrentlyRunningDownloads >= MaxParallelDownloads);
            }
        }

        #endregion

        private bool TimeIsUp
        {
            get
            {
                return (Timeout > 0 && stopwatch.ElapsedMilliseconds >= Timeout);
            }
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
            dialogBehaviour = (SimpleBehaviour)behaviours[0]; 
            // TODO dangerous. Replace by concrete DialogControllers we will write.
                                                                    // init totalSumOfWeights:
            foreach (var curInfo in listOfFilesNotStartedYet)
            {
                totalSumOfWeights += ((curInfo.RemoteSize == MediaInfo.UNKNOWN) ? DEFAULT_WEIGHT : curInfo.RemoteSize);
            }

            CurrentlyRunningDownloads = 0;
            stopwatch.Start();
            var filesCurrentlyDownloading = new Dictionary<Downloader, MediaInfo>();

            while (listOfFilesNotStartedYet.Count > 0 || filesCurrentlyDownloading.Count > 0)
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
                    var info = listOfFilesNotStartedYet[listOfFilesNotStartedYet.Count - 1];
                    info.LocalFileName = QuestManager.MakeLocalFileNameFromUrl(info.Url);
                    var d =
                        new Downloader(
                            url: info.Url,
                            timeout: 0L,
                            maxIdleTime: ConfigurationManager.Current.maxIdleTimeMS,
                            targetPath: info.LocalPath,
                            verbose: false,
                            weight: ((info.RemoteSize == MediaInfo.UNKNOWN) ? DEFAULT_WEIGHT : info.RemoteSize)
                        );
                    filesCurrentlyDownloading.Add(d, info);
                    CurrentlyRunningDownloads++;
                    d.OnTimeout += (AbstractDownloader ad, DownloadEvent e) =>
                    {
                        if (filesCurrentlyDownloading.TryGetValue(d, out var infoToRestart))
                        {
                            listOfFilesNotStartedYet.Add(infoToRestart);
                            filesCurrentlyDownloading.Remove(d);
                        }
                    };
                    d.OnTaskEnded += (object sender, TaskEventArgs e) =>
                    {
                        CurrentlyRunningDownloads--;
                    };
                    d.OnTaskCompleted += (object sender, TaskEventArgs e) =>
                    {
                        info.LocalSize = info.RemoteSize;
                        info.LocalTimestamp = info.RemoteTimestamp;
                        filesCurrentlyDownloading.Remove(d);
                    };
                    d.OnProgress += ContributeToTotalProgress;
                    listOfFilesNotStartedYet.Remove(info);
                    d.Start();
                }
            }

            RaiseTaskCompleted();
        }

        public void ContributeToTotalProgress(object callbackSender, DownloadEvent args)
        {
            downloadedSumOfWeights += args.Progress;
            var percent = Math.Min(100.0f, (downloadedSumOfWeights / totalSumOfWeights) * 100f);
            dialogBehaviour.OnProgress(percent);
        }

        public override object Result { get; set; }

    }
}

