using System.Diagnostics;
using Code.GQClient.Err;
using Code.GQClient.Util.tasks;
using UnityEngine;

namespace Code.GQClient.Util.http
{
	public abstract class AbstractDownloader : Task
	{

		public AbstractDownloader (bool runsAsCoroutine = true) : base (true)
		{
		}

		public WWW Www { get; set; }

		public long Timeout { get; set; }
		public long MaxIdleTime { get; set; }

		public float Weight { get; set; }
		public const float DEFAULT_WEIGHT = 8000000f; // 800 KB as default size for a file to download

		protected Stopwatch stopwatch;
		protected Stopwatch idlewatch;

		public void Restart ()
		{
			Base.Instance.StartCoroutine (RunAsCoroutine ());
			// TODO: isn't a call to Start() enough?
		}

		#region Callback Delegates
		public delegate void DownloadCallback (AbstractDownloader d,DownloadEvent e);

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

		public virtual void Raise (DownloadEventType eventType, DownloadEvent e = DownloadEvent.EMPTY)
		{
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

	}


	public class DownloadEvent : TaskEventArgs
	{
		public string Message { get; protected set; }

		public float Progress { get; protected set; }

		public long ElapsedTime { get; protected set; }

		public DownloadEventType ChangeType { get; protected set; }

		public DownloadEvent (
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


	public enum DownloadEventType
	{
		Start,
		Progress,
		Timeout,
		Error,
		Success
	}


}
