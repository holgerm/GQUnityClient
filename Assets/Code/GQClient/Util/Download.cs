using System;
using UnityEngine;
using System.Collections;
using System.Diagnostics;
using GQ.Client.Err;

namespace GQ.Util {
	public class Download {
		string url;
		long _timeout;

		public long Timeout {
			get {
				return _timeout;
			}
			set {
				_timeout = value;
			}
		}

		Stopwatch stopwatch;
		WWW _www;


		#region Callback Delegates

		public delegate void StartCallback (Download downloader);

		public delegate void ErrorCallback (Download downloader,string msg);

		public delegate bool TimeoutCallback (Download downloader,long elapsedTime);

		public delegate void SuccessCallback (Download downloader);

		public delegate void ProgressUpdate (Download downloader,float percentLoaded);

		public event StartCallback OnStart;
		public event ErrorCallback OnError;

		/// <summary>
		/// Gets or sets the on timeout. Your timeout callback bool response is used to either do the timeout and stop the download (true) or ignore the timeout (false).
		/// </summary>
		/// <value>The on timeout.</value>
		public event TimeoutCallback OnTimeout;
		public event SuccessCallback OnSuccess;
		public event ProgressUpdate OnProgress;

		public static void debugStartHandling (Download downloader) {
			string msg = String.Format("Start to download url {0}", 
				downloader.url);
			if ( downloader._timeout > 0 ) {
				msg += String.Format(", timout set to {0} ms.", downloader._timeout);
			}
			Log.InformUser (msg);
		}

		public static void defaultErrorHandling (Download downloader, string msg) {
			Log.SignalErrorToUser("Encountered a problem during download of url {0}: {1}", 
				downloader.url, msg);
		}

		/// <summary>
		/// Default timeout handler. Logs a message and does not try again but stops the download.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="elapsedTime">Elapsed time.</param>
		public static bool defaultTimeoutHandling (Download downloader, long elapsedTime) {
			Log.InformUser("Timeout: already {1} ms elapsed while trying to download url {0}", 
				downloader.url, elapsedTime);
			return true; // do timeout
		}

		public static void defaultSuccessHandling (Download downloader) {
			Log.InformUser("Download completed. (URL: {0})", 
				downloader.url);
		}

		public static void defaultProgressHandling (Download downloader, float progress) {
			Log.InformUser("Downloading: URL {0}, got {1:N2}%", 
				downloader.url, progress * 100);
		}

		#endregion


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
		/// You can start the download as Coroutine: StartCoroutine(download.startDownload).
		/// All callbacks are intialized with defaults. You can customize the behaviour via method delegates 
		/// onStart, onError, onTimeout, onSuccess, onProgress.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="timeout">Timout in milliseconds (optional).</param>
		public Download (
			string url, 
			long timeout = 0) {
			this.url = url;
			Timeout = timeout;
			stopwatch = new Stopwatch();
			OnStart += debugStartHandling;
			OnError += defaultErrorHandling;
			OnTimeout += defaultTimeoutHandling;
			OnSuccess += defaultSuccessHandling;
			OnProgress += defaultProgressHandling;
		}

		public IEnumerator StartDownload () {
			Www = new WWW(url);
			stopwatch.Start();
			if ( OnStart != null ) {
				OnStart(this);
			}

			float progress = 0f;
			while ( !Www.isDone ) {
				if ( OnProgress != null && progress < Www.progress ) {
					progress = Www.progress;
					OnProgress(this, progress);
				}
				if ( Timeout > 0 && stopwatch.ElapsedMilliseconds >= Timeout ) {
					if ( OnTimeout != null ) {
						if ( OnTimeout(this, stopwatch.ElapsedMilliseconds) ) {
							stopwatch.Stop();
							OnError(this, String.Format("Client side timeout. Download not completed after {0} ms", 
								Timeout));
							Www.Dispose();
							yield break;
						}
					}
				}
				yield return null;
			} 
			stopwatch.Stop();
			
			if ( Www.error != null && Www.error != "" ) {
				if ( OnError != null ) {
					OnError(this, Www.error);
				}
				Www.Dispose();
			}
			else {
				if ( OnProgress != null ) {
					OnProgress(this, Www.progress);
				}
				yield return null;
				if ( OnSuccess != null ) {
					OnSuccess(this);
				}
			}

			yield break;
		}

		#endregion

	}

}

