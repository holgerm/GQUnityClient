using UnityEngine;
using System.Collections;
using System;
using GQ.Client.Conf;
using GQ.Util;
using UnityEngine.UI;
using GQ.Client.Model;

namespace GQ.Client.UI.Controller {
	
	public class Dialog : MonoBehaviour {

		public Text Details;
		public Text Title;
		public Button YesButton;
		public Button NoButton;

		protected const string DETAILS_PATH = "Panel/TextScrollView/Viewport/Content/DetailsText";
		protected const string TITLE_PATH = "Panel/TitleText";
		protected const string YES_BUTTON_PATH = "Panel/Buttons/YesButton";
		protected const string NO_BUTTON_PATH = "Panel/Buttons/NoButton";

		public delegate void ClickCallBack (GameObject sender, EventArgs e);

		public event ClickCallBack OnYesButtonClicked;
		public event ClickCallBack OnNoButtonClicked;

		public virtual void Reset()
		{
			if (Details == null)
			{
				Transform textGo = transform.Find (DETAILS_PATH);
				if (textGo == null) {
					Debug.LogErrorFormat ("Dialog must contain a Panel with a DetailsText inside (at path {0}).", DETAILS_PATH);
					return;
				}

				Details = textGo.GetComponent<Text> ();
			}

		
			if (Title == null)
			{
				Transform textGo = transform.Find (TITLE_PATH);
				if (textGo == null) {
					Debug.LogErrorFormat ("Dialog must contain a Panel with a TitleText inside (at path {0}).", TITLE_PATH);
					return;
				}

				Title = textGo.GetComponent<Text> ();
			}


			if (YesButton == null)
			{
				Transform buttonT = transform.Find (YES_BUTTON_PATH);
				if (buttonT == null) {
					Debug.LogErrorFormat ("Dialog must contain an Image with a Yes-Button inside (at path {0}).", YES_BUTTON_PATH);
					return;
				}

				YesButton = buttonT.GetComponent<Button>();
			}


			if (NoButton == null)
			{
				Transform buttonT = transform.Find (NO_BUTTON_PATH);
				if (buttonT == null) {
					Debug.LogErrorFormat ("Dialog must contain an Image with a No-Button inside (at path {0}).", NO_BUTTON_PATH);
					return;
				}

				NoButton = buttonT.GetComponent<Button>();
			}
		}

		protected virtual void OnEnable()
		{
			InitializeDialogListeners ();
		}

		protected virtual void InitializeDialogListeners ()
		{
			YesButton.onClick.AddListener (YesButtonClicked);
			OnYesButtonClicked = null;

			NoButton.onClick.AddListener (NoButtonClicked);
			OnNoButtonClicked = null;
		}

		public void YesButtonClicked() {
			if (OnYesButtonClicked != null)
				OnYesButtonClicked (YesButton.gameObject, EventArgs.Empty);
		}

		public void NoButtonClicked() {
			if (OnNoButtonClicked != null)
				OnNoButtonClicked (NoButton.gameObject, EventArgs.Empty);
		}

		protected void SetYesButton(string description, ClickCallBack yesButtonClicked) {
			Text buttonText = YesButton.transform.Find ("Text").GetComponent<Text>();
			buttonText.text = description;

			OnYesButtonClicked += yesButtonClicked;
			YesButton.gameObject.SetActive (true);	
			YesButton.interactable = true;
		}

		protected void SetNoButton(string description, ClickCallBack noButtonClicked) {
			Text buttonText = NoButton.transform.Find ("Text").GetComponent<Text>();
			buttonText.text = description;

			OnNoButtonClicked += noButtonClicked;
			NoButton.gameObject.SetActive (true);
			NoButton.interactable = true;
		}


		/// <summary>
		/// Callback for the OnUpdateSuccess event.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		public void CloseDialog(object callbackSender, UpdateQuestInfoEventArgs args)
		{
			gameObject.SetActive (false);
		}


	}
}