using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GQ.Client.Event;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Util;
using GQ.Client.Util;

namespace GQ.Client.UI.Dialogs {

	public abstract class DialogBehaviour : UIBehaviour {

		/// <summary>
		/// Mutually connects this Behaviour with a Dialog Controller and initliazes the behaviour.
		/// </summary>
		public DialogBehaviour(Task task) : base(task) {
			Dialog = Dialog.Instance;
			Dialog.Instance.Behaviour = this;

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
//		public override void Initialize () {
////			// initially we do not have listeners:
////			OnYesButtonClicked = null;
////			OnNoButtonClicked = null;
////
////			// Initally we do not need Buttons:
////			Dialog.YesButton.gameObject.SetActive(false);
////			Dialog.NoButton.gameObject.SetActive(false);
////
////			Dialog.YesButton.onClick.RemoveAllListeners();
////			Dialog.YesButton.onClick.AddListener (RaiseYesButtonClicked);
////
////			Dialog.NoButton.onClick.RemoveAllListeners ();
////			Dialog.NoButton.onClick.AddListener (RaiseNoButtonClicked);
//		}

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
		/// Should only be called from the Dialog UI Component.
		/// </summary>
		public void RaiseYesButtonClicked() {
			if (OnYesButtonClicked != null)
				OnYesButtonClicked (Dialog.YesButton.gameObject, EventArgs.Empty);
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
		/// TODO move to Dialog class and change event args to some more generic type
		protected void CloseDialog(object callbackSender, DownloadEvent args)
		{
			TearDown ();
			Dialog.Hide();
		}

	}

}
