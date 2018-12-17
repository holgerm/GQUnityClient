using UnityEngine;
using System.Collections;
using System;
using GQ.Client.Conf;
using GQ.Client.Util;
using UnityEngine.UI;
using GQ.Client.Model;

namespace GQ.Client.UI.Dialogs
{
	
	public class DownloadDialogBehaviour : DialogBehaviour
	{

		AbstractDownloader DownloadTask { get; set; }

		public DownloadDialogBehaviour (Task task, string title = "Laden ...") : base (task)
		{

			if (Task is AbstractDownloader) {
				DownloadTask = Task as AbstractDownloader;
			}

			this.title = title;
		}

		public override void Start ()
		{
			base.Start ();

			// to prevent registering the same listeners multiple times, in case we initialize multiple times ...
			detachUpdateListeners ();

			// attach listeners before the task gets started:
			attachUpdateListeners ();
		}

		public override void Stop ()
		{
			base.Stop ();

			detachUpdateListeners ();
		}

		void attachUpdateListeners ()
		{
			DownloadTask.OnStart += OnDownloadStarted;
			DownloadTask.OnProgress += UpdateLoadingScreenProgress;
			DownloadTask.OnSuccess += CloseDialog;
			DownloadTask.OnError += UpdateLoadingScreenError;
			DownloadTask.OnTimeout += UpdateLoadingScreenTimeout;
		}

		void detachUpdateListeners ()
		{
			DownloadTask.OnStart -= OnDownloadStarted;
			DownloadTask.OnProgress -= UpdateLoadingScreenProgress;
			DownloadTask.OnSuccess -= CloseDialog;
			DownloadTask.OnError -= UpdateLoadingScreenError;
			DownloadTask.OnTimeout -= UpdateLoadingScreenTimeout;
		}

		private string title;

		/// <summary>
		/// Callback for the OnUpdateStart event.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		public void OnDownloadStarted (object callbackSender, DownloadEvent args)
		{
			EnterDownloadMode ();
		}

		void EnterDownloadMode ()
		{
			HideAndClearButtons ();

			Dialog.Title.text = title;

//			if (DownloadTask.Step == 0) {
//				Dialog.Title.text = string.Format (title);
//			} else {
//				Dialog.Title.text = string.Format (title + " (Schritt {0})", DownloadTask.Step);
//			}
			Dialog.Details.text = "Download startet ...";
			// now we show the dialog:
			Dialog.Show ();
		}

		/// <summary>
		/// Callback for the OnProgress event.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		public void UpdateLoadingScreenProgress (object callbackSender, DownloadEvent args)
		{
			Dialog.Details.text = String.Format ("{0:#0.0}% erledigt", args.Progress * 100);
		}

		/// <summary>
		/// Callback for the OnError event.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		public void UpdateLoadingScreenError (object callbackSender, DownloadEvent args)
		{
			Dialog.Details.text = String.Format ("Fehler: {0}", args.Message);

			// Use No button for Giving Up:
			Dialog.SetNoButton (
				"Abbrechen",
				(GameObject sender, EventArgs e) => {
					// in error case when user clicks the give up button, we just close the dialog:
					CloseDialog (sender, new DownloadEvent ());
					Task.RaiseTaskFailed ();
				}
			);

			// Use Yes button for Retry:
			Dialog.SetYesButton (
				"Wiederholen",
				(GameObject yesButton, EventArgs e) => {
					// in error case when user clicks the retry button, we initialize this behaviour and start the update again:
					EnterDownloadMode ();
					DownloadTask.Restart ();
				}
			);
		}


		/// <summary>
		/// Callback for the OnTimeout event.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		public void UpdateLoadingScreenTimeout (object callbackSender, DownloadEvent args)
		{
			Dialog.Details.text = String.Format ("Problem: {0}", args.Message);

			// Use No button for Giving Up:
			Dialog.SetNoButton (
				"Abbrechen",
				(GameObject sender, EventArgs e) => {
					// in error case when user clicks the give up button, we just close the dialog:
					CloseDialog (sender, new DownloadEvent ());
					Task.RaiseTaskFailed ();
				}
			);

			// Use Yes button for Retry:
			Dialog.SetYesButton (
				"Wiederholen",
				(GameObject yesButton, EventArgs e) => {
					// in error case when user clicks the retry button, we initialize this behaviour and start the update again:
					EnterDownloadMode ();
					DownloadTask.Restart ();
				}
			);
		}
	}

}