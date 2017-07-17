using UnityEngine;
using System.Collections;
using System;
using GQ.Client.Conf;
using GQ.Util;
using UnityEngine.UI;
using GQ.Client.Model;

namespace GQ.Client.UI.Controller {
	
	public class LoadQuestInfos : Dialog {

		protected override void OnEnable()
		{
			InitializeDialogListeners();

			QuestInfoManager.Instance.UpdateQuestInfoList ();
		}

		protected override void InitializeDialogListeners ()
		{
			base.InitializeDialogListeners ();

			// Initally we do not need Buttons:
			YesButton.gameObject.SetActive(false);
			NoButton.gameObject.SetActive(false);

			attachUpdateListeners ();
		}

		void attachUpdateListeners ()
		{
			QuestInfoManager.Instance.OnUpdateStart += InitializeLoadingScreen;
			QuestInfoManager.Instance.OnUpdateProgress += UpdateLoadingScreenProgress;
			QuestInfoManager.Instance.OnUpdateSuccess += CloseDialog;
			QuestInfoManager.Instance.OnUpdateError += UpdateLoadingScreenError;
		}			

		void OnDisable()
		{
			detachUpdateListeners ();
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
			Title.text = args.Message;
			Details.text = "Starting ...";
		}	

		/// <summary>
		/// Callback for the OnUpdateProgress event.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		public void UpdateLoadingScreenProgress(object callbackSender, UpdateQuestInfoEventArgs args)
		{
			Details.text = String.Format ("{0:#0.0}% done", args.Progress * 100);
			Debug.Log ("Progress: " + args.Progress);
		}
		/// <summary>
		/// Callback for the OnUpdateError event.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		public void UpdateLoadingScreenError(object callbackSender, UpdateQuestInfoEventArgs args)
		{
			Details.text = String.Format ("Error: {0}", args.Message);

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
				(GameObject sender, EventArgs e) => {
					// inhibit multiple clicks by disabling the button first:
					YesButton.interactable = false;
					YesButton.gameObject.SetActive(false);

					// in error case when user clicks the retry button, we start the update again:
					detachUpdateListeners();
					InitializeDialogListeners();
					QuestInfoManager.Instance.UpdateQuestInfoList ();
				}
			);
		}
	}
}