using UnityEngine;
using System.Collections;
using System;
using GQ.Client.Conf;
using GQ.Util;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Util;

namespace GQ.Client.UI.Dialogs {
	
	public class UpdateQuestInfoDialogBehaviour : DialogBehaviour {

		Download DownloadTask { get; set; }

		public UpdateQuestInfoDialogBehaviour(Task task) : base(task) {

			if (Task is Download) {
				DownloadTask = Task as Download;
			}

			// to prevent registering the same listeners multiple times, in case we initialize multiple times ...
			detachUpdateListeners ();
			attachUpdateListeners ();
		} 

//		/// <summary>
//		/// Idempotent init method that hides both buttons and ensures that our 
//		/// behaviour callback are registered with the InfoManager exactly once.
//		/// </summary>
//		/// TODO: Initialize überdenken!
//		public override void Initialize ()
//		{
//			base.Initialize ();
//		}

		public override void TearDown()
		{
			base.TearDown ();

			detachUpdateListeners ();
		}

		void attachUpdateListeners ()
		{
			DownloadTask.OnStart += InitializeLoadingScreen;
			DownloadTask.OnProgress += UpdateLoadingScreenProgress;
			DownloadTask.OnSuccess += CloseDialog;
			DownloadTask.OnError += UpdateLoadingScreenError;
		}			

		void detachUpdateListeners ()
		{
			DownloadTask.OnStart -= InitializeLoadingScreen;
			DownloadTask.OnProgress -= UpdateLoadingScreenProgress;
			DownloadTask.OnSuccess -= CloseDialog;
			DownloadTask.OnError -= UpdateLoadingScreenError;
		}

		const string BASIC_TITLE = "Updating quests";

		/// <summary>
		/// Callback for the OnUpdateStart event.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		public void InitializeLoadingScreen(object callbackSender, DownloadEvent args)
		{
			Debug.Log ("STEP: " + DownloadTask.Step);
			if (DownloadTask.Step == 0) {
				Dialog.Title.text = 
					string.Format (BASIC_TITLE);
			} else {
				Dialog.Title.text = 
				string.Format (BASIC_TITLE + " (step {0})", DownloadTask.Step);
			}
			Dialog.Details.text = "Start downloading data ...";

			// now we show the dialog:
			Dialog.Show();
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
//					Initialize();
//					ServerQuestInfoLoader retryLoader = new ServerQuestInfoLoader();
//					retryLoader.Behaviour = new UpdateQuestInfoDialogBehaviour ();
					DownloadTask.Start();
				}
			);
		}

	}

}