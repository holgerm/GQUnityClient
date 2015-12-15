using System;
using UnityEngine;
using System.Collections;

namespace GQ.Util
{
	public class Downloader
	{
		string url;
		WWW _www;

		StartCallback onStart;
		ErrorCallback onError;
		SuccessCallback onSuccess;
		ProgressUpdate onProgress;

		#region Public Interface
		
		public WWW Www {
			get {
				return _www;
			}
			private set {
				_www = value;
			}
		}
		
		/// <summary>
		/// Initializes a new Downloader object. 
		/// You can start the download afterthat by calling <see cref="GQ.Util.Downloader.startDownload()"/> class.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="onStart">Call back function which is called when the download has started.</param>
		/// <param name="onSuccess">Call back function which is called when the download has successfully finished.</param>
		/// <param name="onError">Call back function which is called when the download encounters an error.</param>
		/// <param name="onProgress">Call back function which is called when the download has progresses. 
		/// Gives you a float between 0 (no progress yet) and 1 (download completed)</param>
		public Downloader (
			string url, 
			StartCallback onStart = null,
			SuccessCallback onSuccess = null, 
			ErrorCallback onError = null, 
			ProgressUpdate onProgress = null)
		{
			this.url = url;

			if (onStart != null)
				this.onStart = onStart;
			else
				this.onStart = defaultStartHandling;

			if (onError != null)
				this.onError = onError;
			else
				this.onError = defaultErrorHandling;

			if (onSuccess != null)
				this.onSuccess = onSuccess;
			else
				this.onSuccess = defaultSuccessHandling;

			if (onProgress != null)
				this.onProgress = onProgress;
		}

		public IEnumerator startDownload ()
		{
			Www = new WWW (url);
			onStart(url);

			float progress = 0f;
			while (!Www.isDone) {
				if (onProgress != null && progress < Www.progress) {
					progress = Www.progress;
					onProgress(url, progress);
				}
				yield return null;
			} 
			
			if (Www.error != null) {
				onError(url, Www.error);
			} else {
				if (onProgress != null)
					onProgress(url, Www.progress);
				yield return null;
				onSuccess(this);
			}

			yield break;
		}

		#endregion

		#region Callbacks Defaults
		
		public static void defaultStartHandling (string url)
		{
			Debug.Log(String.Format("Start to download url {0}", url));
		}
		
		public static void defaultErrorHandling (string url, string msg)
		{
			Debug.LogWarning(String.Format("Encountered a problem during download of url {0}: {1}", url, msg));
		}
		
		public static void defaultSuccessHandling (Downloader downloader)
		{
			Debug.Log(String.Format("Download completed. (URL: {0})", downloader.Www.url));
		}
		
		public static void defaultProgressHandling (string url, float progress)
		{
			Debug.Log(String.Format("Downloading: URL {0}, got {1:N2}%", url, progress * 100));
		}
		
		#endregion
		

		public delegate void StartCallback (string url);
		
		public delegate void ErrorCallback (string url, string msg);
		
		public delegate void SuccessCallback (Downloader downloader);
		
		public delegate void ProgressUpdate (string url, float percentLoaded);

	}

}

