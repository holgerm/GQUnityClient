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

		private int MaxParallelDownloads  { get; set; }


		List<MediaInfo> FileInfoList;

		public override void StartCallback(object sender, TaskEventArgs e) {
			if (e.Content is string) {
				FileInfoList = e.Content as List<MediaInfo>;
			}
			this.Start(e.Step + 1);
		}

		private bool LimitOfParallelDownloadsExceeded {
			get {
				// TODO calculate based on some limit and counter
				return true;
			}
		}

		private bool TimeIsUp {
			get {
				return (Timeout > 0 && stopwatch.ElapsedMilliseconds >= Timeout);
			}
		}

		private int _crd;
		private int CurrentlyRunningDownloads {
			get {
				return _crd;
			}
			set {
				_crd = value;
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


			}

		}

		public override object Result { get; protected set; }

	}
}

