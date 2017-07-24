using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GQ.Client.Event;
using UnityEngine.UI;
using GQ.Client.Model;

namespace GQ.Client.UI.Dialogs {

	public abstract class DialogBehaviour {

		public Dialog Dialog { get; set; }

		public event ClickCallBack OnYesButtonClicked;
		public event ClickCallBack OnNoButtonClicked;

		/// <summary>
		/// Step counter that can be used
		/// </summary>
		protected int step;

		/// <summary>
		/// Initialize this instance. 
		/// This method is called by Show() just before the dialog will be made visible (aka enabled, SetActive).
		/// </summary>
		public virtual void Initialize () {
			// initially we do not have listeners:
			OnYesButtonClicked = null;
			OnNoButtonClicked = null;

			// Initally we do not need Buttons:
			Dialog.YesButton.gameObject.SetActive(false);
			Dialog.NoButton.gameObject.SetActive(false);

			Dialog.YesButton.onClick.RemoveAllListeners();
			Dialog.YesButton.onClick.AddListener (RaiseYesButtonClicked);

			Dialog.NoButton.onClick.RemoveAllListeners ();
			Dialog.NoButton.onClick.AddListener (RaiseNoButtonClicked);
		}

		/// <summary>
		/// Should be called before the dialog is made invisible or disposed.
		/// </summary>
		public virtual void TearDown() 
		{
			// initially we do not have listeners:
			OnYesButtonClicked = null;
			OnNoButtonClicked = null;

			// Initally we do not need Buttons:
			Dialog.YesButton.gameObject.SetActive(false);
			Dialog.NoButton.gameObject.SetActive(false);

			Dialog.YesButton.onClick.RemoveAllListeners();
			Dialog.NoButton.onClick.RemoveAllListeners ();
		}

		/// <summary>
		/// Sets the yes button with text and callback method.
		/// </summary>
		/// <param name="description">Description.</param>
		/// <param name="yesButtonClicked">Yes button clicked.</param>
		protected void SetYesButton(string description, ClickCallBack yesButtonClicked) {
			Text buttonText = Dialog.YesButton.transform.Find ("Text").GetComponent<Text>();
			buttonText.text = description;

			OnYesButtonClicked += yesButtonClicked;
			Dialog.YesButton.gameObject.SetActive (true);	
			Dialog.YesButton.interactable = true;
		}

		/// <summary>
		/// Should only be called from the Dialog UI Component.
		/// </summary>
		public void RaiseYesButtonClicked() {
			if (OnYesButtonClicked != null)
				OnYesButtonClicked (Dialog.YesButton.gameObject, EventArgs.Empty);
		}

		/// <summary>
		/// Sets the no button with text and callback method.
		/// </summary>
		/// <param name="description">Description.</param>
		/// <param name="noButtonClicked">No button clicked.</param>
		protected void SetNoButton(string description, ClickCallBack noButtonClicked) {
			Text buttonText = Dialog.NoButton.transform.Find ("Text").GetComponent<Text>();
			buttonText.text = description;

			OnNoButtonClicked += noButtonClicked;
			Dialog.NoButton.gameObject.SetActive (true);
			Dialog.NoButton.interactable = true;
		}

		/// <summary>
		/// Should only be called from the Dialog UI Component.
		/// </summary>
		public void RaiseNoButtonClicked() {
			if (OnNoButtonClicked != null)
				OnNoButtonClicked (Dialog.NoButton.gameObject, EventArgs.Empty);
		}

		/// <summary>
		/// Closes the dialog.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		protected void CloseDialog(object callbackSender, UpdateQuestInfoEventArgs args)
		{
			TearDown ();
			Dialog.gameObject.SetActive (false);
		}

	}

}
