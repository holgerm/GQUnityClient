using UnityEngine;
using System.Collections;
using System;
using GQ.Client.Conf;
using GQ.Util;
using UnityEngine.UI;
using GQ.Client.Model;

namespace GQ.Client.UI.Dialogs {
	
	public class LoadInfoDialogBehaviour : DialogBehaviour {

		/// <summary>
		/// Idempotent init method that hides both buttons and ensures that our behaviour callback are registered with the InfoManager exactly once.
		/// </summary>
		public override void Initialize ()
		{
			base.Initialize ();

			// to prevent registering the same listeners multiple times, in case we initialize multiple times ...
			detachUpdateListeners ();
			attachUpdateListeners ();
		}

		public override void TearDown()
		{
			base.TearDown ();

			detachUpdateListeners ();
		}

		void attachUpdateListeners ()
		{
			QuestInfoManager.Instance.OnUpdateStart += InitializeLoadingScreen;
			QuestInfoManager.Instance.OnUpdateProgress += UpdateLoadingScreenProgress;
			QuestInfoManager.Instance.OnUpdateSuccess += CloseDialog;
			QuestInfoManager.Instance.OnUpdateError += UpdateLoadingScreenError;
		}			

		void detachUpdateListeners ()
		{
			QuestInfoManager.Instance.OnUpdateStart -= InitializeLoadingScreen;
			QuestInfoManager.Instance.OnUpdateProgress -= UpdateLoadingScreenProgress;
			QuestInfoManager.Instance.OnUpdateSuccess -= CloseDialog;
			QuestInfoManager.Instance.OnUpdateError -= UpdateLoadingScreenError;
		}

		/// <summary>
		/// Callback for the OnUpdateStart event.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		public void InitializeLoadingScreen(object callbackSender, UpdateQuestInfoEventArgs args)
		{
			Dialog.Title.text = args.Message;
			Dialog.Details.text = "Starting ...";

			// now we show the dialog:
			Dialog.gameObject.SetActive(true);
		}	

		/// <summary>
		/// Callback for the OnUpdateProgress event.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		public void UpdateLoadingScreenProgress(object callbackSender, UpdateQuestInfoEventArgs args)
		{
			Dialog.Details.text = String.Format ("{0:#0.0}% done", args.Progress * 100);
		}
		/// <summary>
		/// Callback for the OnUpdateError event.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		public void UpdateLoadingScreenError(object callbackSender, UpdateQuestInfoEventArgs args)
		{
			Dialog.Details.text = String.Format ("Error: {0}", args.Message);

			// Use No button for Giving Up:
			SetNoButton(
				"Give Up",
				(GameObject sender, EventArgs e) => {
					// in error case when user clicks the give up button, we just close the dialog:
					CloseDialog(sender, new UpdateQuestInfoEventArgs ());
				}
			);

			// Use Yes button for Retry:
			SetYesButton (
				"Retry",
				(GameObject yesButton, EventArgs e) => {
//					// inhibit multiple clicks by disabling the button first:
//					Dialog.YesButton.interactable = false;
//					Dialog.YesButton.gameObject.SetActive(false);
//
//					// hide No Button, too:
//					Dialog.NoButton.gameObject.SetActive(false);

					// in error case when user clicks the retry button, we initialize this behaviour and start the update again:
					Initialize();
					QuestInfoManager.Instance.UpdateQuestInfoList ();
				}
			);
		}
	}
}