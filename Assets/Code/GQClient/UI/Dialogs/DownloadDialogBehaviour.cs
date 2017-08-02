using UnityEngine;
using System.Collections;
using System;
using GQ.Client.Conf;
using GQ.Util;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Util;

namespace GQ.Client.UI.Dialogs {
	
	public class DownloadDialogBehaviour : DialogBehaviour {

		Download DownloadTask { get; set; }

		public DownloadDialogBehaviour(Task task, string title = "Downloading ...") : base(task) {

			if (Task is Download) {
				DownloadTask = Task as Download;
			}

			this.title = title;
		} 

		public override void Start() 
		{
			base.Start ();

			// to prevent registering the same listeners multiple times, in case we initialize multiple times ...
			detachUpdateListeners ();

			// attach listeners before the task gets started:
			attachUpdateListeners ();
		}
			
		public override void Stop()
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
		}			

		void detachUpdateListeners ()
		{
			DownloadTask.OnStart -= OnDownloadStarted;
			DownloadTask.OnProgress -= UpdateLoadingScreenProgress;
			DownloadTask.OnSuccess -= CloseDialog;
			DownloadTask.OnError -= UpdateLoadingScreenError;
		}

		private string title;

		/// <summary>
		/// Callback for the OnUpdateStart event.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		public void OnDownloadStarted(object callbackSender, DownloadEvent args)
		{
			EnterDownloadMode ();
		}	

		void EnterDownloadMode ()
		{
			HideAndClearButtons ();

			if (DownloadTask.Step == 0) {
				Dialog.Title.text = string.Format (title);
			}
			else {
				Dialog.Title.text = string.Format (title + " (step {0})", DownloadTask.Step);
			}
			Dialog.Details.text = "Start downloading data ...";
			// now we show the dialog:
			Dialog.Show ();
		}

		/// <summary>
		/// Callback for the OnUpdateProgress event.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		public void UpdateLoadingScreenProgress(object callbackSender, DownloadEvent args)
		{
			Dialog.Details.text = String.Format ("{0:#0.0}% done", args.Progress * 100);
		}

		/// <summary>
		/// Callback for the OnUpdateError event.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		public void UpdateLoadingScreenError(object callbackSender, DownloadEvent args)
		{
			Dialog.Details.text = String.Format ("Error: {0}", args.Message);

			// Use No button for Giving Up:
			Dialog.SetNoButton(
				"Give Up",
				(GameObject sender, EventArgs e) => {
					// in error case when user clicks the give up button, we just close the dialog:
					CloseDialog(sender, new DownloadEvent ());
				}
			);

			// Use Yes button for Retry:
			Dialog.SetYesButton (
				"Retry",
				(GameObject yesButton, EventArgs e) => {
					// in error case when user clicks the retry button, we initialize this behaviour and start the update again:
					EnterDownloadMode();
					DownloadTask.Restart();
				}
			);
		}

	}

}