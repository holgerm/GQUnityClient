using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GQ.Client.Event;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Util;
using GQ.Client.Util;

namespace GQ.Client.UI.Dialogs {

	public abstract class DialogBehaviour : UIBehaviour {

		/// <summary>
		/// Mutually connects this Behaviour with a Dialog Controller and initliazes the behaviour.
		/// </summary>
		public DialogBehaviour(Task task) : base(task) {
			Dialog = DialogController.Instance;
			Dialog.Behaviour = this;

			HideAndClearButtons ();

			// Initally both Buttons are connected to our standard events:
			Dialog.YesButton.onClick.RemoveAllListeners();
			Dialog.YesButton.onClick.AddListener (RaiseYesButtonClicked);

			Dialog.NoButton.onClick.RemoveAllListeners ();
			Dialog.NoButton.onClick.AddListener (RaiseNoButtonClicked);
		}

		// Basic setting for all modes, buttons have no events and are hidden. Modes must set them afterwards appropriately.
		protected void HideAndClearButtons ()
		{
			// initially we do not have listeners:
			OnYesButtonClicked = null;
			OnNoButtonClicked = null;
			// Initally we hide Buttons:
			Dialog.YesButton.gameObject.SetActive (false);
			Dialog.NoButton.gameObject.SetActive (false);
		}

		public DialogController Dialog { get; set; }

		public event ClickCallBack OnYesButtonClicked;
		public event ClickCallBack OnNoButtonClicked;

		/// <summary>
		/// Step counter that can be used
		/// </summary>
		protected int step;

		public override void Start() 
		{
			base.Start ();
		}

		/// <summary>
		/// Should be called before the dialog is made invisible or disposed.
		/// </summary>
		public virtual void Stop() 
		{
			HideAndClearButtons ();

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
		protected void CloseDialog(object callbackSender, EventArgs args)
		{
			Stop ();
			Dialog.Hide();
		}

	}

}
