//#define DEBUG_LOG

using System;
using UnityEngine;
using System.Collections;
using System.Diagnostics;
using GQ.Client.Err;
using System.IO;

namespace GQ.Client.Util
{
    public class Downloader : AbstractDownloader
    {
        public string Url { get; set; }

        public string TargetPath { get; set; }

        WWW _www;


        #region Default Handler
        public static void defaultLogInformationHandler(AbstractDownloader d, DownloadEvent e)
        {
            Log.InformUser(e.Message);
        }

        public static void defaultLogErrorHandler(AbstractDownloader d, DownloadEvent e)
        {
            Log.SignalErrorToUser(e.Message);
        }
        #endregion


        #region Delegation API for Tests
        static Downloader()
        {
            CoroutineRunner = DownloadAsCoroutine;
        }

        public delegate IEnumerator DownloaderCoroutineMethod(Downloader d);

        public static DownloaderCoroutineMethod CoroutineRunner
        {
            get;
            set;
        }

        protected override IEnumerator DoTheWork()
        {
            return CoroutineRunner(this);
        }

        static public IEnumerator DownloadAsCoroutine(Downloader d)
        {
            return d.Download();
        }
        #endregion


        #region Public API
        /// <summary>
        /// The elapsed time the download is/was active in milliseconds.
        /// </summary>
        /// <value>The elapsed time.</value>
        public long elapsedTime
        {
            get
            {
                return stopwatch.ElapsedMilliseconds;
            }
        }

        /// <summary>
        /// Initializes a new Downloader object. 
        /// You can start the download as Coroutine: StartCoroutine(download.startDownload).
        /// All callbacks are intialized with defaults. You can customize the behaviour via method delegates 
        /// onStart, onError, onTimeout, onSuccess, onProgress.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="timeout">Timeout in milliseconds (optional).</param>
        /// <param name="maxIdleTime">Idle Timeout in milliseconds (optional).</param>
        /// <param name="targetPath">Target path where the downloaded file will be stored (optional).</param>
        /// <param name="weight">If bigger than zero, the progress will send delta bytes compared to last progress message.</param>
        /// <param name="verbose">If true (default) standard event managers will be used that protocol all events to the log.</param>
        /// instead of an absolute progress value in [0,1].</param>
        public Downloader(
            string url,
            long timeout = 0,
            string targetPath = null,
            long maxIdleTime = 2000L,
            bool verbose = true,
            float weight = 0f
        ) : base(true)
        {
            Result = "";
            this.Url = url;
            Timeout = timeout;
            MaxIdleTime = maxIdleTime;
            TargetPath = targetPath;
            Weight = weight;
            stopwatch = new Stopwatch();
            idlewatch = new Stopwatch();
            if (verbose)
            {
#if DEBUG_LOG
                OnStart += defaultLogInformationHandler;
                OnSuccess += defaultLogInformationHandler;
                OnProgress += defaultLogInformationHandler;
#endif
                OnError += defaultLogErrorHandler;
                OnTimeout += defaultLogErrorHandler;
            }
        }

        protected IEnumerator Download()
        {
            Www = new WWW(Url);
            stopwatch.Reset();
            stopwatch.Start();
            idlewatch.Reset();

            string msg = String.Format("Start to download url {0}", Url);
            if (Timeout > 0)
            {
                msg += String.Format(", timout set to {0} ms, idle timeout set to {1} ms.", Timeout, MaxIdleTime);
            }
            Raise(DownloadEventType.Start, new DownloadEvent(message: msg));

            float progress = 0f;
            float progressNew;

            while (!Www.isDone)
            {
                progressNew = Www.progress;
                if (progress + 0.001f < progressNew)
                {
                    // we have a PROGRESS:
                    idlewatch.Reset();
                    msg = string.Format("Lade Datei {0}, aktuell: {1:N2}%", Url, progress * 100);
                    float prog = (Weight > 0f) ? (Weight * (progressNew - progress)) : progressNew;
                    Raise(
                        DownloadEventType.Progress,
                        new DownloadEvent(
                            // when weight is given, we signal the delta of weight (e.g. bytes) which sums up from 0 to Weight:
                            progress: prog,
                            message: msg
                        )
                    );
                    progress = progressNew;
                }
                else
                {
                    // we have no progress, i.e. we are IDLE:
                    if (MaxIdleTime > 0 && idlewatch.IsRunning && idlewatch.ElapsedMilliseconds > MaxIdleTime)
                    {
                        idlewatch.Stop();
                        stopwatch.Stop();
                        Www.Dispose();
                        msg = string.Format("Idle-Timeout: schon {0} ms ohne effektiven Download vergangen",
                            idlewatch.ElapsedMilliseconds);
                        Raise(DownloadEventType.Timeout, new DownloadEvent(elapsedTime: MaxIdleTime, message: msg));
                        yield break;
                    }
                    else
                    {
                        idlewatch.Start();
                    }
                }
                if (Timeout > 0 && stopwatch.ElapsedMilliseconds >= Timeout)
                {
                    // overall time beyond TIMEOUT:
                    stopwatch.Stop();
                    idlewatch.Stop();
                    Www.Dispose();
                    msg = string.Format("Timeout: schon {0} ms vergangen",
                        stopwatch.ElapsedMilliseconds);
                    Raise(DownloadEventType.Timeout, new DownloadEvent(elapsedTime: Timeout, message: msg));
                    yield break;
                }
                if (Www == null)
                    UnityEngine.Debug.Log("Www is null".Red()); // TODO what to do in this case?
                yield return null;
            }
            progressNew = Www.progress;

            stopwatch.Stop();
            idlewatch.Stop();

            if (!string.IsNullOrEmpty(Www.error))
            {
                UnityEngine.Debug.LogWarning("ERROR loading " + Www.url + ": " + Www.error);
                string dialogMessage = Www.url.EndsWith("clientxml", StringComparison.CurrentCulture) ?
                                          "Quest nicht gefunden." : Www.error;
                Raise(DownloadEventType.Error, new DownloadEvent(message: dialogMessage));
                RaiseTaskFailed();
            }
            else
            {
                Result = Www.text;

                msg = string.Format("Lade Datei {0}, aktuell: {1:N2}%", Url, progress * 100);
                float prog = (Weight > 0f) ? (Weight * (Www.progress - progress)) : Www.progress;
                Raise(
                    DownloadEventType.Progress,
                    new DownloadEvent(
                        progress: prog,
                        message: msg
                    )
                );

                yield return null;

                msg = string.Format("Speichere Datei ... {0}", Url);
                Raise(
                    DownloadEventType.Progress,
                    new DownloadEvent(
                        progress: (Weight > 0) ? 0 : Www.progress,
                        message: msg
                    )
                );

                if (TargetPath != null)
                {
                    // we have to store the loaded file:
                    try
                    {
                        string targetDir = Directory.GetParent(TargetPath).FullName;
                        if (!Directory.Exists(targetDir))
                            Directory.CreateDirectory(targetDir);
                        if (File.Exists(TargetPath))
                            File.Delete(TargetPath);

                        File.WriteAllBytes(TargetPath, Www.bytes);
                    }
                    catch (Exception e)
                    {
                        Raise(DownloadEventType.Error, new DownloadEvent(message: "Could not save downloaded file: " + e.Message));
                        RaiseTaskFailed();

                        Www.Dispose();
                        yield break;
                    }
                }

                msg = string.Format("Download für Datei {0} abgeschlossen",
                    Url);
                Raise(DownloadEventType.Success, new DownloadEvent(message: msg));
                RaiseTaskCompleted(Result);
            }

            Www.Dispose();
            yield break;
        }
        #endregion

    }

}

