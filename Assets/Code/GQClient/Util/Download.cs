using System;
using UnityEngine;
using System.Collections;
using System.Diagnostics;

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

		StartCallback _onStart;

		public StartCallback OnStart {
			get {
				return _onStart;
			}
			set {
				_onStart = value;
			}
		}

		ErrorCallback _onError;

		public ErrorCallback OnError {
			get {
				return _onError;
			}
			set {
				_onError = value;
			}
		}

		TimeoutCallback _onTimeout;

		/// <summary>
		/// Gets or sets the on timeout. Your timeout callback bool response is used to either do the timeout and stop the download (true) or ignore the timeout (false).
		/// </summary>
		/// <value>The on timeout.</value>
		public TimeoutCallback OnTimeout {
			get {
				return _onTimeout;
			}
			set {
				_onTimeout = value;
			}
		}

		SuccessCallback _onSuccess;

		public SuccessCallback OnSuccess {
			get {
				return _onSuccess;
			}
			set {
				_onSuccess = value;
			}
		}

		ProgressUpdate _onProgress;

		public ProgressUpdate OnProgress {
			get {
				return _onProgress;
			}
			set {
				_onProgress = value;
			}
		}

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
		/// All callbacks are intialized with defaults. You can customize the behaviour via properties 
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
			OnStart = defaultStartHandling;
			OnError = defaultErrorHandling;
			OnTimeout = defaultTimeoutHandling;
			OnSuccess = defaultSuccessHandling;
			OnProgress = defaultProgressHandling;
		}

		public IEnumerator startDownload () {
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
			
			if ( Www.error != null ) {
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

		#region Callbacks Defaults

		public static void defaultStartHandling (Download downloader) {
			string msg = String.Format("Start to download url {0}", 
				             downloader.url);
			if ( downloader._timeout > 0 ) {
				msg += String.Format(", timout set to {0} ms.", downloader._timeout);
			}
//			UnityEngine.Debug.Log (msg);
		}

		public static void defaultErrorHandling (Download downloader, string msg) {
			UnityEngine.Debug.LogWarning(String.Format("Encountered a problem during download of url {0}: {1}", 
				downloader.url, msg));
		}

		/// <summary>
		/// Default timeout handler. Logs a message and does not try again but stops the download.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="elapsedTime">Elapsed time.</param>
		public static bool defaultTimeoutHandling (Download downloader, long elapsedTime) {
			UnityEngine.Debug.LogWarning(String.Format("Timeout: already {1} ms elapsed while trying to download url {0}", 
				downloader.url, elapsedTime));
			return true; // do timeout
		}

		public static void defaultSuccessHandling (Download downloader) {
			UnityEngine.Debug.Log(String.Format("Download completed. (URL: {0})", 
				downloader.url));
		}

		public static void defaultProgressHandling (Download downloader, float progress) {
			UnityEngine.Debug.Log(String.Format("Downloading: URL {0}, got {1:N2}%", 
				downloader.url, progress * 100));
		}

		#endregion


		public delegate void StartCallback (Download downloader);

		public delegate void ErrorCallback (Download downloader,string msg);

		public delegate bool TimeoutCallback (Download downloader,long elapsedTime);

		public delegate void SuccessCallback (Download downloader);

		public delegate void ProgressUpdate (Download downloader,float percentLoaded);

	}

}

