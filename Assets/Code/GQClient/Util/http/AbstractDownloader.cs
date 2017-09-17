using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using GQ.Client.Err;

namespace GQ.Client.Util {
	public abstract class AbstractDownloader : Task {

		public long Timeout { get; set; }

		protected Stopwatch stopwatch;

		#region Callback Delegates

		public delegate void DownloadCallback (AbstractDownloader d, DownloadEvent e);

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

		protected virtual void Raise(DownloadEventType eventType, DownloadEvent e = DownloadEvent.EMPTY) {
			switch (eventType) {
			case DownloadEventType.Start:
				Raise (OnStart, e);
				break;
			case DownloadEventType.Progress:
				Raise (OnProgress, e);
				break;
			case DownloadEventType.Error:
				Raise (OnError, e);
				break;
			case DownloadEventType.Timeout:
				Raise (OnTimeout, e);
				break;
			case DownloadEventType.Success:
				Raise (OnSuccess, e);
				break;
			default:
				Log.SignalErrorToDeveloper ("Tried to raise unknown event type in Downloader.");
				break;
			}
		}

		#endregion


		#region Starting

		public override bool Run() 
		{
			Base.Instance.StartCoroutine(StartDownload());

			return true;
		}

		public abstract IEnumerator StartDownload ();

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
		Start, Progress, Timeout, Error, Success
	}


}
