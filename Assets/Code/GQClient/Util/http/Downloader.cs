//#define DEBUG_LOG

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Code.GQClient.Err;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.UI.Dialogs;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

namespace Code.GQClient.Util.http
{
    public class Downloader : AbstractDownloader
    {
        public string Url { get; private set; }

        public string TargetPath { get; private set; }

        #region Default Handlers

        public static void defaultLogInformationHandler(AbstractDownloader d, DownloadEvent e)
        {
            Log.InformUser(e.Message);
        }

        public static void defaultLogErrorHandler(AbstractDownloader d, DownloadEvent e)
        {
            Debug.Log($"Downloader error: {e.Message} while downloading {d.WebRequest.url}");
            Log.SignalErrorToUser(e.Message);
            var message = e.Message;
            if (message.StartsWith("Cannot resolve destination host")
                || message.StartsWith("Idle-Timeout"))
                message = "Bitte prüfe die Internetverbindung.";

            var dialog = new MessageDialog(message, "Ok");
            dialog.Start();
        }

        #endregion


        #region Delegation API for Tests

        static Downloader()
        {
            CoroutineRunner = DownloadAsCoroutine;
        }

        public delegate IEnumerator DownloaderCoroutineMethod(Downloader d);

        public static DownloaderCoroutineMethod CoroutineRunner { get; set; }

        protected override IEnumerator DoTheWork()
        {
            return CoroutineRunner(this);
        }

        public static IEnumerator DownloadAsCoroutine(Downloader d)
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
            get { return stopwatch.ElapsedMilliseconds; }
        }

        /// <summary>
        /// Initializes a new Downloader object. 
        /// You can start the download as Coroutine: StartCoroutine(download.startDownload).
        /// All callbacks are intialized with defaults. You can customize the behaviour via method delegates 
        /// onStart, onError, onTimeout, onSuccess, onProgress.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="downloadHandler">e.g. generic or specific for Textures or Scripts etc.</param>
        /// <param name="timeout">Timeout in milliseconds (optional).</param>
        /// <param name="maxIdleTime">Idle Timeout in milliseconds (optional).</param>
        /// <param name="targetPath">Target path where the downloaded file will be stored (optional).</param>
        /// <param name="weight">If bigger than zero, the progress will send delta bytes compared to last progress message.</param>
        /// <param name="verbose">If true (default) standard event managers will be used that protocol all events to the log.</param>
        /// instead of an absolute progress value in [0,1].</param>
        public Downloader(
            string url,
            DownloadHandler downloadHandler,
            long timeout = 0,
            string targetPath = null,
            long maxIdleTime = 2000L,
            bool verbose = true,
            float weight = 0f
        ) : base(downloadHandler, true)
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
                
                Debug.Log($"Downloader created, type: {DownloadHandler.GetType().Name}, info: {DownloadHandler.ToString()}");
            }
        }

        protected bool uriIsWellFormed(string uri)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(uri, UriKind.Absolute, out uriResult)
                          && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps ||
                              uriResult.Scheme == Uri.UriSchemeFile);
            return result;
        }

        protected IEnumerator Download()
        {
            if (!uriIsWellFormed(Url))
            {
                Raise(DownloadEventType.Error,
                    new DownloadEvent(message: $"Can not download from malformed URI: {this.Url}"));
                Log.SignalErrorToAuthor(
                    $"Malformed url {this.Url} in quest '{QuestManager.Instance.CurrentQuest.Name}' (id: {QuestManager.Instance.CurrentQuest.Id})");
                RaiseTaskFailed();
                yield break;
            }

            UnityWebRequest webRequest = UnityWebRequest.Get(Url);
            webRequest.downloadHandler = DownloadHandler;

            stopwatch.Reset();
            stopwatch.Start();
            idlewatch.Reset();

            var msg = $"Start to download url {Url}";
            if (Timeout > 0)
            {
                msg += $", timeout set to {Timeout} ms, idle timeout set to {MaxIdleTime} ms.";
            }

            Raise(DownloadEventType.Start, new DownloadEvent(message: msg));
            webRequest.SendWebRequest();

            var progress = 0f;
            float progressNew;

            while (!webRequest.isDone || !DownloadHandler.isDone)
            {
                progressNew = webRequest.downloadProgress;
                if (progress + 0.001f < progressNew)
                {
                    // we have a PROGRESS:
                    idlewatch.Reset();
                    msg = string.Format("Lade Datei {0}, aktuell: {1:N2}%", Url, progress * 100);
                    var prog = (Weight > 0f) ? (Weight * (progressNew - progress)) : progressNew;
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
                        webRequest.Dispose();
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
                    webRequest.Dispose();
                    msg = string.Format("Timeout: schon {0} ms vergangen",
                        stopwatch.ElapsedMilliseconds);
                    Raise(DownloadEventType.Timeout, new DownloadEvent(elapsedTime: Timeout, message: msg));
                    yield break;
                }

                if (webRequest == null)
                    UnityEngine.Debug.Log("Www is null".Red()); // TODO what to do in this case?
                yield return null;
            }

            Debug.Log($"Downlooder done with url: {webRequest.url} isDone? {DownloadHandler.isDone} error: '{DownloadHandler.error}' type: {DownloadHandler.GetType().Name}");
            progressNew = webRequest.downloadProgress;

            stopwatch.Stop();
            idlewatch.Stop();

            if (!string.IsNullOrEmpty(webRequest.error))
            {
                var dialogMessage = webRequest.url.EndsWith("clientxml", StringComparison.CurrentCulture)
                    ? "Bitte prüfe die Internetverbindung."
                    : webRequest.error;
                Raise(DownloadEventType.Error, new DownloadEvent(message: dialogMessage));
                RaiseTaskFailed();
            }
            else
            {
                try
                {
                    Result = Encoding.UTF8.GetString(webRequest.downloadHandler.data);
                }
                catch (NotSupportedException)
                {
                    Result = File.ReadAllText(TargetPath, Encoding.UTF8);
                }

                msg = string.Format("Lade Datei {0}, aktuell: {1:N2}%", Url, progress * 100);
                var prog = (Weight > 0f)
                    ? (Weight * (webRequest.downloadProgress - progress))
                    : webRequest.downloadProgress;
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
                        progress: (Weight > 0) ? 0 : webRequest.downloadProgress,
                        message: msg
                    )
                );

                // if (TargetPath != null) // TODO obsolete due to DownloadHandlerFile ... ? check it!
                // {
                //     // we have to store the loaded file:
                //     try
                //     {
                //         var targetDir = Directory.GetParent(TargetPath).FullName;
                //         if (!Directory.Exists(targetDir))
                //             Directory.CreateDirectory(targetDir);
                //         if (File.Exists(TargetPath))
                //             File.Delete(TargetPath);
                //
                //         File.WriteAllBytes(TargetPath, webRequest.downloadHandler.data);
                //     }
                //     catch (Exception e)
                //     {
                //         Raise(DownloadEventType.Error,
                //             new DownloadEvent(message: "Could not save downloaded file: " + e.Message));
                //         RaiseTaskFailed();
                //
                //         webRequest.Dispose();
                //         yield break;
                //     }
                // }

                ResponseHeaders = new Dictionary<string, string>(webRequest.GetResponseHeaders());
                msg = string.Format("Download für Datei {0} abgeschlossen",
                    Url);
                Raise(DownloadEventType.Success, new DownloadEvent(message: msg));
                RaiseTaskCompleted(Result);
                
                Debug.Log($"Downloader DONE: handler says: {DownloadHandler.isDone}, has error?: {DownloadHandler.error}");
            }

            webRequest.Dispose();
            yield break;
        }

        #endregion

        public Dictionary<string, string> ResponseHeaders { get; private set; }
    }
}