using System;
using System.Collections;
using Code.GQClient.Err;
using Code.GQClient.FileIO;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.GQClient.Util.http
{
	public class LocalFileLoader : AbstractDownloader
	{
		public string filePath;

		WWW _www;


		#region Default Handler

		public static void defaultLogInformationHandler (AbstractDownloader d, DownloadEvent e)
		{
			Log.InformUser ($"{e.Message} frame#: {Time.frameCount}");
		}

		public static void defaultLogErrorHandler (AbstractDownloader d, DownloadEvent e)
		{
			Log.SignalErrorToUser ($"{e.Message} frame#: {Time.frameCount}");
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
			string filePath, DownloadHandler downloadHandler) : base (downloadHandler, true)
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
			

			WebRequest = UnityWebRequest.Get(url);
			WebRequest.downloadHandler = DownloadHandler;

			string msg = $"Start to load local file {filePath}";
			Raise (DownloadEventType.Start, new DownloadEvent (message: msg));
			WebRequest.SendWebRequest();

			float progress = 0f;
			while (WebRequest is { isDone: false }) {
				if (progress < WebRequest.downloadProgress) {
					progress = WebRequest.downloadProgress;
					msg = string.Format ("Loading local file: URL {0}, got {1:N2}%", filePath, progress * 100);
					Raise (DownloadEventType.Progress, new DownloadEvent (progress: progress, message: msg));
				}
				if (WebRequest == null)
					UnityEngine.Debug.Log ("WebRequest is null"); // TODO what to do in this case?
				yield return null;
			} 
				
			if (WebRequest != null && WebRequest.error != null && WebRequest.error != "") {
				Raise (DownloadEventType.Error, new DownloadEvent (message: WebRequest.error + " file: " + filePath));
				RaiseTaskFailed ();
			} else {
				Result = WebRequest.downloadHandler.text;

				msg = string.Format ("Loading local file: URL {0}, got {1:N2}%", filePath, progress * 100);
				Raise (DownloadEventType.Progress, new DownloadEvent (progress: WebRequest.downloadProgress, message: msg));

				msg = string.Format ("Loading local file completed. (URL: {0})", 
					filePath);
				Raise (DownloadEventType.Success, new DownloadEvent (message: msg));
				RaiseTaskCompleted (Result);
			}

			WebRequest.Dispose ();
			yield break;
		}

		#endregion

	}

}

