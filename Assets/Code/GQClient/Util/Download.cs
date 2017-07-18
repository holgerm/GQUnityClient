using System;
using UnityEngine;
using System.Collections;
using System.Diagnostics;
using GQ.Client.Err;

namespace GQ.Util {
	public class Download {
		string url;

		public long Timeout { get; set; }

		Stopwatch stopwatch;
		WWW _www;


		#region Callback Delegates

		public delegate void DownloadCallback (Download d, DownloadEvent e);

		public event DownloadCallback OnStart;
		public event DownloadCallback OnError;
		public event DownloadCallback OnTimeout;
		public event DownloadCallback OnSuccess;
		public event DownloadCallback OnProgress;

		/// <summary>
		/// Raises an event based on DownloadCallback delegate type, e.g. OnUpdateStart, OnUpdateProgress, etc.
		/// </summary>
		/// <param name="callback">Callback.</param>
		/// <param name="e">E.</param>
		protected virtual void Raise (DownloadCallback callback, DownloadEvent e = DownloadEvent.EMPTY)
		{
			if (callback != null)
				callback (this, e);
		}


		public static void defaultStartHandling (Download d, DownloadEvent e) {
			string msg = String.Format("Start to download url {0}", 
				d.url);
			if ( d.Timeout > 0 ) {
				msg += String.Format(", timout set to {0} ms.", d.Timeout);
			}
			Log.InformUser (msg);
		}

		public static void defaultErrorHandling (Download d, DownloadEvent e) {
			Log.SignalErrorToUser("Encountered a problem during download of url {0}: {1}", 
				d.url, e.Message);
		}

		public static void defaultTimeoutHandling (Download d, DownloadEvent e) {
			Log.InformUser("Timeout: already {1} ms elapsed while trying to download url {0}", 
				d.url, e.ElapsedTime);
		}

		public static void defaultSuccessHandling (Download d, DownloadEvent e) {
			Log.InformUser("Download completed. (URL: {0})", 
				d.url);
		}

		public static void defaultProgressHandling (Download d, DownloadEvent e) {
			Log.InformUser("Downloading: URL {0}, got {1:N2}%", 
				d.url, e.Progress * 100);
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
			OnStart += defaultStartHandling;
			OnError += defaultErrorHandling;
			OnTimeout += defaultTimeoutHandling;
			OnSuccess += defaultSuccessHandling;
			OnProgress += defaultProgressHandling;
		}

		public IEnumerator StartDownload () {
			Www = new WWW(url);
			stopwatch.Start();
			Raise(OnStart, DownloadEvent.EMPTY);

			float progress = 0f;
			while ( !Www.isDone ) {
				if ( progress < Www.progress ) {
					progress = Www.progress;
					Raise(OnProgress, new DownloadEvent(progress: progress));
				}
				if ( Timeout > 0 && stopwatch.ElapsedMilliseconds >= Timeout ) {
					stopwatch.Stop();
					Raise(OnTimeout, new DownloadEvent(elapsedTime: Timeout));
					Www.Dispose();
					yield break;
				}
				yield return null;
			} 
			stopwatch.Stop();
			
			if ( Www.error != null && Www.error != "" ) {
				string errMsg = Www.error;
				Www.Dispose();
				Raise(OnError, new DownloadEvent(message: errMsg));
			}
			else {
				Raise(OnProgress, new DownloadEvent(progress: Www.progress));
				yield return null;
				Raise(OnSuccess);
			}

			yield break;
		}

		#endregion

	}

	public class DownloadEvent : EventArgs 
	{
		public string Message { get; protected set; }
		public float Progress { get; protected set; }
		public long ElapsedTime { get; protected set; }
		public DownloadEventType ChangeType { get; protected set; }

		public DownloadEvent(
			string message = "", 
			float progress = 0f, 
			long elapsedTime = 0,
			DownloadEventType Type = DownloadEventType.Start
		)
		{
			Message = message;
			Progress = progress;
			ElapsedTime = elapsedTime;
		}

		public const DownloadEvent EMPTY = null;
	}

	public enum DownloadEventType {
		Start, Progress, Timeout, Error
	}

}

