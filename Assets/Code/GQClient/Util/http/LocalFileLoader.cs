using System;
using System.Collections;
using Code.GQClient.Err;
using Code.GQClient.FileIO;
using UnityEngine;

namespace Code.GQClient.Util.http
{
	public class LocalFileLoader : AbstractDownloader
	{
		public string filePath;

		WWW _www;


		#region Default Handler

		public static void defaultLogInformationHandler (AbstractDownloader d, DownloadEvent e)
		{
			Log.InformUser (e.Message + " frame#: " + Time.frameCount);
		}

		public static void defaultLogErrorHandler (AbstractDownloader d, DownloadEvent e)
		{
			Log.SignalErrorToUser (e.Message);
		}

		#endregion


		#region Public Interface

		/// <summary>
		/// Initializes a new Downloader object. 
		/// You can start the download as Coroutine: StartCoroutine(download.startDownload).
		/// All callbacks are intialized with defaults. You can customize the behaviour via method delegates 
		/// onStart, onError, onTimeout, onSuccess, onProgress.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="timeout">Timout in milliseconds (optional).</param>
		/// <param name="timeout">Target path where the downloaded file will be stored (optional).</param>
		public LocalFileLoader (
			string filePath) : base (true)
		{
			Result = "";
			this.filePath = filePath;
			OnStart += defaultLogInformationHandler;
			OnError += defaultLogErrorHandler;
			OnSuccess += defaultLogInformationHandler;
			OnProgress += defaultLogInformationHandler;
		}

		protected override IEnumerator DoTheWork ()
		{
			var url = Files.AbsoluteLocalPath (filePath);

			Www = new WWW (url);

			var msg = String.Format ("Start to load local file {0}", filePath);
			Raise (DownloadEventType.Start, new DownloadEvent (message: msg));

			var progress = 0f;
			while (!Www.isDone) {
				if (progress < Www.progress) {
					progress = Www.progress;
					msg = string.Format ("Loading local file: URL {0}, got {1:N2}%", filePath, progress * 100);
					Raise (DownloadEventType.Progress, new DownloadEvent (progress: progress, message: msg));
				}
				if (Www == null)
					UnityEngine.Debug.Log ("Www is null"); // TODO what to do in this case?
				yield return null;
			} 
				
			if (Www.error != null && Www.error != "") {
				Raise (DownloadEventType.Error, new DownloadEvent (message: Www.error + " file: " + filePath));
				RaiseTaskFailed ();
			} else {
				Result = Www.text;

				msg = string.Format ("Loading local file: URL {0}, got {1:N2}%", filePath, progress * 100);
				Raise (DownloadEventType.Progress, new DownloadEvent (progress: Www.progress, message: msg));

				msg = string.Format ("Loading local file completed. (URL: {0})", 
					filePath);
				Raise (DownloadEventType.Success, new DownloadEvent (message: msg));
				RaiseTaskCompleted (Result);
			}

			Www.Dispose ();
			yield break;
		}

		#endregion

	}

}

