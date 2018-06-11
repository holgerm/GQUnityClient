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
using GQ.Client.Conf;

namespace GQ.Client.Util
{
	
	public class MultiDownloader : AbstractDownloader
	{

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
			int maxParallelDownloads = 15, 
			long timeout = 0, 
			List<MediaInfo> files = null) : base (true)
		{
			if (files != null) {
				listOfFilesNotStartedYet = files;
			}
			Result = "";
			MaxParallelDownloads = maxParallelDownloads;
			Timeout = timeout;
			stopwatch = new Stopwatch ();
		}

		List<MediaInfo> listOfFilesNotStartedYet;

		public override void ReadInput (object sender, TaskEventArgs e)
		{
			if (e.Content is List<MediaInfo>) {
				listOfFilesNotStartedYet = e.Content as List<MediaInfo>;
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
			if (listOfFilesNotStartedYet == null || listOfFilesNotStartedYet.Count == 0) {
				RaiseTaskCompleted ();
				yield break;
			}

			int totalNrOfFilesToLoad = listOfFilesNotStartedYet.Count;
			int nrOfFilesCompleted = 0;

			CurrentlyRunningDownloads = 0;
			stopwatch.Start ();
			Dictionary<Downloader, MediaInfo> filesCurrentlyDownloading = new Dictionary<Downloader, MediaInfo> ();

			while (listOfFilesNotStartedYet.Count > 0 || filesCurrentlyDownloading.Count > 0) {
				// wait until a place for download is free:
				while (LimitOfParallelDownloadsExceeded && !TimeIsUp) {
					yield return null;
				}
				UnityEngine.Debug.Log(("Start loop again: " + listOfFilesNotStartedYet.Count + ", " + filesCurrentlyDownloading.Count + ", " + nrOfFilesCompleted).Yellow());
				yield return null;


				if (TimeIsUp) {
					UnityEngine.Debug.Log(("Time is up.").Yellow());
					stopwatch.Stop ();
//					string msg = 
//						string.Format (
//							"Timeout: Schon {0} ms vergangen", 
//							stopwatch.ElapsedMilliseconds
//						);
					Raise (DownloadEventType.Timeout, new DownloadEvent (elapsedTime: Timeout));
					RaiseTaskFailed (); 
					yield break;
				}

				if (listOfFilesNotStartedYet.Count > 0) {
					// now we can start the next file downloader:
					MediaInfo info = listOfFilesNotStartedYet [listOfFilesNotStartedYet.Count - 1];
					info.LocalFileName = QuestManager.MakeLocalFileNameFromUrl (info.Url);
					Downloader d = 
						new Downloader (
							url: info.Url, 
							timeout: 0L, 
							maxIdleTime: ConfigurationManager.Current.maxIdleTimeMS, 
							targetPath: info.LocalPath,
							verbose: false
						);
					filesCurrentlyDownloading.Add (d, info);
					CurrentlyRunningDownloads++;
					d.OnTimeout += (AbstractDownloader ad, DownloadEvent e) => {
						MediaInfo infoToRestart;
						if (filesCurrentlyDownloading.TryGetValue (d, out infoToRestart)) {
							listOfFilesNotStartedYet.Add (infoToRestart);
							filesCurrentlyDownloading.Remove (d);
							UnityEngine.Debug.Log ("Restarted to load file: " + d.Url);
						}
					};
					d.OnTaskEnded += (object sender, TaskEventArgs e) => {
						CurrentlyRunningDownloads--;
//						UnityEngine.Debug.Log (("Files still waiting to load: " + listOfFilesNotStartedYet.Count).Yellow ());
//						UnityEngine.Debug.Log (("Files currently loading: " + filesCurrentlyDownloading.Count).Yellow ());
					};
					d.OnTaskCompleted += (object sender, TaskEventArgs e) => {
						info.LocalSize = info.RemoteSize;
						info.LocalTimestamp = info.RemoteTimestamp;
						filesCurrentlyDownloading.Remove (d);
						nrOfFilesCompleted++;
						UnityEngine.Debug.Log (("completed: " + d.Url).Yellow ());
					};
					listOfFilesNotStartedYet.Remove (info);
					d.Start ();
				}
			}
			UnityEngine.Debug.Log("Exited load loop.".Yellow());

//			// wait until the last download is finished:
//			while (CurrentlyRunningDownloads > 0) {
//				UnityEngine.Debug.Log("wait for last downloads.".Yellow());
//
//				yield return null;
//			}
			RaiseTaskCompleted ();
		}

		public override object Result { get; set; }

	}
}

