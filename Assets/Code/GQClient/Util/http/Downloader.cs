using System;
using UnityEngine;
using System.Collections;
using System.Diagnostics;
using GQ.Client.Err;
using GQ.Client.UI;
using System.IO;

namespace GQ.Client.Util {
	public class Downloader : AbstractDownloader {
		protected string url;

		public string TargetPath { get; set; }

		WWW _www;


		#region Default Handler

		public static void defaultLogInformationHandler (AbstractDownloader d, DownloadEvent e) {
			Log.InformUser (e.Message);
		}

		public static void defaultLogErrorHandler (AbstractDownloader d, DownloadEvent e) {
			Log.SignalErrorToUser (e.Message);
		}

		#endregion


		#region Public Interface

		public WWW Www { get; set; }

		/// <summary>
		/// The elapsed time the download is/was active in milliseconds.
		/// </summary>
		/// <value>The elapsed time.</value>
		public long elapsedTime {
			get {
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
		/// <param name="timeout">Timout in milliseconds (optional).</param>
		/// <param name="timeout">Target path where the downloaded file will be stored (optional).</param>
		public Downloader (
			string url, 
			long timeout = 0,
			string targetPath = null) 
		{
			Result = "";
			this.url = url;
			Timeout = timeout;
			TargetPath = targetPath;
			stopwatch = new Stopwatch();
			OnStart += defaultLogInformationHandler;
			OnError += defaultLogErrorHandler;
			OnTimeout += defaultLogErrorHandler;
			OnSuccess += defaultLogInformationHandler;
			OnProgress += defaultLogInformationHandler;
		}

		public void Restart() {
			Base.Instance.StartCoroutine(StartDownload());
		}
			
		// TODO reduce accessibility to protected and make test use it by reflection.
		public override IEnumerator StartDownload () 
		{
			Www = new WWW(url);
			stopwatch.Start();

			string msg = String.Format("Start to download url {0}", url);
			if ( Timeout > 0 ) {
				msg += String.Format(", timout set to {0} ms.", Timeout);
			}
			Raise(DownloadEventType.Start, new DownloadEvent(message: msg));

			float progress = 0f;
			while ( !Www.isDone ) {
				if ( progress < Www.progress ) {
					progress = Www.progress;
					msg = string.Format ("Downloading: URL {0}, got {1:N2}%", url, progress * 100);
					Raise(DownloadEventType.Progress, new DownloadEvent(progress: progress, message: msg));
				}
				if ( Timeout > 0 && stopwatch.ElapsedMilliseconds >= Timeout ) {
					stopwatch.Stop();
					Www.Dispose();
					msg = string.Format ("Timeout: already {1} ms elapsed while trying to download url {0}", 
							url, stopwatch.ElapsedMilliseconds);
					Raise(DownloadEventType.Timeout, new DownloadEvent(elapsedTime: Timeout, message: msg));
					yield break;
				}
				if (Www == null)
					UnityEngine.Debug.Log ("Www is null"); // TODO what to do in this case?
				yield return null;
			} 

			stopwatch.Stop();
			
			if ( Www.error != null && Www.error != "" ) {
				Raise(DownloadEventType.Error, new DownloadEvent(message: Www.error));
				RaiseTaskFailed ();
			}
			else {
				Result = Www.text;

				msg = string.Format ("Downloading: URL {0}, got {1:N2}%", url, progress * 100);
				Raise(DownloadEventType.Progress, new DownloadEvent(progress: Www.progress, message: msg));

				yield return null;

				msg = string.Format ("Saving file ...");
				Raise(DownloadEventType.Progress, new DownloadEvent(progress: Www.progress, message: msg));

				if (TargetPath != null) {
					// we have to store the loaded file:
					try {
						string targetDir = Directory.GetParent(TargetPath).FullName;
						if (!Directory.Exists (targetDir))
							Directory.CreateDirectory (targetDir);
						if (File.Exists (TargetPath))
							File.Delete (TargetPath);
						File.WriteAllBytes(TargetPath, Www.bytes);
					}
					catch (Exception e) {
						Raise(DownloadEventType.Error, new DownloadEvent(message: "Could not save downloaded file: " + e.Message));
						RaiseTaskFailed ();

						Www.Dispose();
						yield break;
					}
				}

				msg = string.Format ("Download completed. (URL: {0})", 
					url);
				Raise(DownloadEventType.Success, new DownloadEvent(message: msg));
				RaiseTaskCompleted (Result);
			}

			Www.Dispose();
			yield break;
		}

		#endregion

	}

}

