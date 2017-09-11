using System;
using UnityEngine;
using System.Collections;
using System.Diagnostics;
using GQ.Client.Err;
using GQ.Client.UI;
using System.IO;
using System.Collections.Generic;
using GQ.Client.Model;

namespace GQ.Client.Util {
	
	public class SerialDownloader : AbstractDownloader {

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
		public SerialDownloader (int maxParallelDownloads, long timeout = 0) 
		{
			Result = "";
			MaxParallelDownloads = maxParallelDownloads;
			Timeout = timeout;
			stopwatch = new Stopwatch();
		}


		List<MediaInfo> FileInfoList;

		public override void StartCallback(object sender, TaskEventArgs e) {
			if (e.Content is string) {
				FileInfoList = e.Content as List<MediaInfo>;
			}
			this.Start(e.Step + 1);
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
		public override IEnumerator StartDownload () 
		{
			if (FileInfoList == null || FileInfoList.Count == 0) {
				yield break;
			}

			foreach (MediaInfo info in FileInfoList) {
				// wait until a place for download is free:
				while (LimitOfParallelDownloadsExceeded && !TimeIsUp)
					yield return null;

				if (TimeIsUp) {
					stopwatch.Stop();
					Raise(DownloadEventType.Timeout, new DownloadEvent(elapsedTime: Timeout));
					RaiseTaskFailed (); 
					yield break;
				}

				// now we can start the next file downloader:
				Downloader d = new Downloader(info.Url, targetPath: MakeLocalFileNameFromUrl(info.Url));

			}

		}

		/// <summary>
		/// Makes the local file name from the given URL, 
		/// so that the file name is unique and reflects the filename within the url.
		/// </summary>
		/// <returns>The local file name from URL.</returns>
		/// <param name="url">URL.</param>
		public string MakeLocalFileNameFromUrl(string url) {
			return url; // TODO
		}

		public override object Result { get; protected set; }

	}
}

