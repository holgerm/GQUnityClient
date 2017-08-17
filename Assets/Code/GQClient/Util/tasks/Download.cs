using System;
using UnityEngine;
using System.Collections;
using System.Diagnostics;
using GQ.Client.Err;
using GQ.Client.Util;
using GQ.Client.UI;
using System.IO;

namespace GQ.Util {
	public class Download : Task {
		string url;

		public long Timeout { get; set; }

		public string TargetPath { get; set; }

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

		public WWW Www { get; set; }

		public override object Result { get; protected set; }

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
		/// <param name="timeout">Taregt path where the downloaded file will be stored (optional).</param>
		public Download (
			string url, 
			long timeout = 0,
			string targetPath = null) : base()
		{
			Result = "";
			this.url = url;
			Timeout = timeout;
			TargetPath = targetPath;
			stopwatch = new Stopwatch();
			OnStart += defaultStartHandling;
			OnError += defaultErrorHandling;
			OnTimeout += defaultTimeoutHandling;
			OnSuccess += defaultSuccessHandling;
			OnProgress += defaultProgressHandling;
		}

		public override void Start(int step = 0) 
		{
			base.Start(step);

			Base.Instance.StartCoroutine(StartDownload());
		}

		public void Restart() {
			Base.Instance.StartCoroutine(StartDownload());
		}
			
		public IEnumerator StartDownload () 
		{
			Www = new WWW(url);
			stopwatch.Start();
			Raise(OnStart);

			float progress = 0f;
			while ( !Www.isDone ) {
				if ( progress < Www.progress ) {
					progress = Www.progress;
					Raise(OnProgress, new DownloadEvent(progress: progress));
				}
				if ( Timeout > 0 && stopwatch.ElapsedMilliseconds >= Timeout ) {
					stopwatch.Stop();
					Www.Dispose();
					Raise(OnTimeout, new DownloadEvent(elapsedTime: Timeout));
					RaiseTaskFailed (); 
					yield break;
				}
				if (Www == null)
					UnityEngine.Debug.Log ("Www is null"); // TODO what to do in this case?
				yield return null;
			} 

			if (TargetPath != null) {
				// we have to store the loaded file:
				try {
					UnityEngine.Debug.Log("Writing the file");
					string targetDir = Directory.GetParent(TargetPath).FullName;
					if (!Directory.Exists (targetDir))
						Directory.CreateDirectory (targetDir);
					if (File.Exists (TargetPath))
						File.Delete (TargetPath);
					File.WriteAllBytes(TargetPath, Www.bytes);
				}
				catch (Exception e) {
					Raise(OnError, new DownloadEvent(message: "Could not save downloaded file: " + e.Message));
					RaiseTaskFailed ();
				}
			}

			stopwatch.Stop();
			
			if ( Www.error != null && Www.error != "" ) {
				string errMsg = Www.error;
				Raise(OnError, new DownloadEvent(message: errMsg));
				RaiseTaskFailed ();
			}
			else {
				Result = Www.text;
				Raise(OnProgress, new DownloadEvent(progress: Www.progress));
				yield return null;
				Raise(OnSuccess, new DownloadEvent(message: Www.text));
				RaiseTaskCompleted (Result);
			}

			Www.Dispose();
			yield break;
		}

		#endregion

	}

	public class DownloadEvent : TaskEventArgs 
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

