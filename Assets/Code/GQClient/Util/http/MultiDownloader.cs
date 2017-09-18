using System;
using UnityEngine;
using System.Collections;
using System.Diagnostics;
using GQ.Client.Err;
using GQ.Client.UI;
using System.IO;
using System.Collections.Generic;
using GQ.Client.Model;
using GQ.Client.FileIO;

namespace GQ.Client.Util {
	
	public class MultiDownloader : AbstractDownloader {

		/// <summary>
		/// Initializes a new Downloader object. 
		/// You can start the download as Coroutine either by calling StartCoroutine(download.startDownload) directly or
		/// by calling the method StartCallback(object sender, TaskEventArgs e). 
		/// The latter is used for task concatenation.
		/// 
		/// All callbacks are intialized with defaults. You can customize the behaviour via method delegates 
		/// onStart, onError, onTimeout, onSuccess, onProgress.
		/// </summary>
		/// <param name="maxParallelDownloads">Maximal number of parallel downloads.</param>
		/// <param name="timeout">Timout in milliseconds (optional).</param>
		public MultiDownloader (
			int maxParallelDownloads = 1, 
			long timeout = 0, 
			List<MediaInfo> files = null) : base(true)
		{
			if (files != null) {
				FileInfoList = files;
			}
			Result = "";
			MaxParallelDownloads = maxParallelDownloads;
			Timeout = timeout;
			stopwatch = new Stopwatch();
		}

		List<MediaInfo> FileInfoList;

		public override void InitAfterPreviousTask(object sender, TaskEventArgs e) {
			if (e.Content is List<MediaInfo>) {
				FileInfoList = e.Content as List<MediaInfo>;
			}
		}

		#region Parallelization Limits

		private int MaxParallelDownloads  { get; set; }

		private int CurrentlyRunningDownloads { get; set; }

		private bool LimitOfParallelDownloadsExceeded {
			get {
				return (CurrentlyRunningDownloads >= MaxParallelDownloads);
			}
		}

		#endregion

		private bool TimeIsUp {
			get {
				return (Timeout > 0 && stopwatch.ElapsedMilliseconds >= Timeout);
			}
		}

		/// <summary>
		/// Actually starts the download.
		/// </summary>
		/// <returns>The download.</returns>
		public override IEnumerator RunAsCoroutine () 
		{
			CurrentlyRunningDownloads = 0;

			if (FileInfoList == null || FileInfoList.Count == 0) {
				RaiseTaskCompleted ();
				yield break;
			}

			foreach (MediaInfo info in FileInfoList) {
				// wait until a place for download is free:
				while (LimitOfParallelDownloadsExceeded && !TimeIsUp) {
					yield return null;
				}

				if (TimeIsUp) {
					stopwatch.Stop();
					Raise(DownloadEventType.Timeout, new DownloadEvent(elapsedTime: Timeout));
					RaiseTaskFailed (); 
					yield break;
				}

				// now we can start the next file downloader:
				info.LocalFileName = QuestManager.MakeLocalFileNameFromUrl(info.Url);
				Downloader d = new Downloader(info.Url, targetPath: info.LocalPath);
				CurrentlyRunningDownloads++;
				d.OnTaskEnded += (object sender, TaskEventArgs e) => {
					CurrentlyRunningDownloads--;
					UnityEngine.Debug.Log("downloader freed");
				};
				d.OnTaskCompleted += (object sender, TaskEventArgs e) => {
					info.LocalSize = info.RemoteSize;
					info.LocalTimestamp = info.RemoteTimestamp;
					UnityEngine.Debug.Log("size and time updated: new time: " + info.LocalTimestamp);
				};
				d.Start ();
			}

			// wait until the last download is finished:
			while (CurrentlyRunningDownloads > 0) {
				yield return null;
			}

			RaiseTaskCompleted ();
		}

		public override object Result { get; protected set; }

	}
}

