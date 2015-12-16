using System;
using UnityEngine;
using System.Collections;
using System.Diagnostics;

namespace GQ.Util
{
	public class Download
	{
		string url;
		long timeout;
		Stopwatch stopwatch;
		WWW _www;

		StartCallback onStart;
		ErrorCallback onError;
		TimeoutCallback onTimeout;
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
		/// You can start the download afterthat by calling <see cref="GQ.Util.Downloader.startDownload()"/> class.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="timeout">Timout in milliseconds (optional).</param>
		/// <param name="onStart">Call back function which is called when the download has started (optional).</param>
		/// <param name="onSuccess">Call back function which is called when the download has successfully finished (optional).</param>
		/// <param name="onError">Call back function which is called when the download encounters an error (optional).</param>
		/// <param name="onTimeout">Call back function which is called when the download takes longer than the given timeout (optional).</param>
		/// <param name="onProgress">Call back function which is called when the download has progresses (optional). 
		/// Gives you a float between 0 (no progress yet) and 1 (download completed)</param>
		public Download (
			string url, 
			long timeout = 0,
			StartCallback onStart = null,
			SuccessCallback onSuccess = null, 
			ErrorCallback onError = null, 
			TimeoutCallback onTimeout = null,
			ProgressUpdate onProgress = null)
		{
			this.url = url;
			this.timeout = timeout;
			stopwatch = new Stopwatch ();

			if (onStart != null)
				this.onStart = onStart;
			else
				this.onStart = defaultStartHandling;

			if (onError != null)
				this.onError = onError;
			else
				this.onError = defaultErrorHandling;
			
			if (onTimeout != null)
				this.onTimeout = onTimeout;
			else
				this.onTimeout = defaultTimeoutHandling;

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
			stopwatch.Start();
			onStart(this);

			float progress = 0f;
			while (!Www.isDone) {
				if (onProgress != null && progress < Www.progress) {
					progress = Www.progress;
					onProgress(this, progress);
				}
				if (timeout > 0 && stopwatch.ElapsedMilliseconds >= timeout) {
					if (onTimeout != null) {
						if (onTimeout(this, stopwatch.ElapsedMilliseconds)) {
							stopwatch = Stopwatch.StartNew();
						} else {
							stopwatch.Stop();
							Www.Dispose();
							onError(this, String.Format("Client side timeout. Download not completed after {0} ms", 
							        			 		timeout));
							yield break;
						}
					}
				}
				yield return null;
			} 
			stopwatch.Stop();
			
			if (Www.error != null) {
				onError(this, Www.error);
			} else {
				if (onProgress != null)
					onProgress(this, Www.progress);
				yield return null;
				onSuccess(this);
			}

			yield break;
		}

		#endregion

		#region Callbacks Defaults
		
		public static void defaultStartHandling (Download downloader)
		{
			string msg = String.Format("Start to download url {0}", 
			                           downloader.url);
			if (downloader.timeout > 0) {
				msg += String.Format(", timout set to {0} ms.", downloader.timeout);
			}
			UnityEngine.Debug.Log(msg);
		}
		
		public static void defaultErrorHandling (Download downloader, string msg)
		{
			UnityEngine.Debug.LogWarning(String.Format("Encountered a problem during download of url {0}: {1}", 
			                                           downloader.url, msg));
		}
		
		/// <summary>
		/// Default timeout handler. Logs a message and does not try again but stops the download.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="elapsedTime">Elapsed time.</param>
		public static bool defaultTimeoutHandling (Download downloader, long elapsedTime)
		{
			UnityEngine.Debug.LogWarning(String.Format("Timeout: already {1} ms elapsed while trying to download url {0}", 
			                                           downloader.url, elapsedTime));
			return false; // do not repeat timeout
		}

		public static void defaultSuccessHandling (Download downloader)
		{
			UnityEngine.Debug.Log(String.Format("Download completed. (URL: {0})", 
			                                    downloader.url));
		}
		
		public static void defaultProgressHandling (Download downloader, float progress)
		{
			UnityEngine.Debug.Log(String.Format("Downloading: URL {0}, got {1:N2}%", 
			                                    downloader.url, progress * 100));
		}
		
		#endregion
		

		public delegate void StartCallback (Download downloader);
		
		public delegate void ErrorCallback (Download downloader, string msg);
		
		public delegate bool TimeoutCallback (Download downloader, long elapsedTime);

		public delegate void SuccessCallback (Download downloader);
		
		public delegate void ProgressUpdate (Download downloader, float percentLoaded);

	}

}

