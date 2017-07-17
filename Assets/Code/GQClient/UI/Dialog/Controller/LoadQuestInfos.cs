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
			base.OnEnable ();

			// Initally we do not need Buttons:
			YesButton.gameObject.SetActive(false);
			NoButton.gameObject.SetActive(false);

			// Start Upadte Quest Info List
			QuestInfoManager.Instance.UpdateQuestInfoList(
				onStart: InitializeLoadingScreen,
				onProgress: UpdateLoadingScreenProgress,
				onSuccess: UpdateLoadingScreenSucceeded,
				onError: UpdateLoadingScreenError
			);
		}
			

		void OnDisable()
		{
			/// Detach all registered listener methods:
			QuestInfoManager.Instance.OnUpdateStart -= InitializeLoadingScreen;
			QuestInfoManager.Instance.OnUpdateProgress -= UpdateLoadingScreenProgress;
			QuestInfoManager.Instance.OnUpdateSuccess -= UpdateLoadingScreenSucceeded;
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
		/// Callbakc for the OnUpdateSuccess event.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		public void UpdateLoadingScreenSucceeded(object callbackSender, UpdateQuestInfoEventArgs args)
		{
			gameObject.SetActive (false);
		}

		/// <summary>
		/// Callback for the OnUpdateError event.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		public void UpdateLoadingScreenError(object callbackSender, UpdateQuestInfoEventArgs args)
		{
			Details.text = String.Format ("Error: {0}", args.Message);

			Text buttonText = YesButton.transform.Find ("Text").GetComponent<Text>();
			buttonText.text = "Ok";
			OnYesButtonClicked += (GameObject sender, EventArgs e) => 
			{
				// in error case when user clicks the ok button, we just close the dialog:
				gameObject.SetActive (false);
			};
			YesButton.gameObject.SetActive (true);
		}
	}
}