using System;
using System.Collections;
using Code.GQClient.Event;
using Code.GQClient.Util;
using Code.GQClient.Util.tasks;
using UnityEngine;

namespace Code.GQClient.UI.Dialogs
{

	public abstract class DialogBehaviour : UIBehaviour
	{
		private float showAtLeastSeconds;

		public bool keepShowing
		{
			get;
			private set;
		}

		private bool hideWhenShownLongEnough = false;

		/// <summary>
		/// Mutually connects this Behaviour with a Dialog Controller and initliazes the behaviour.
		/// </summary>
		protected DialogBehaviour (Task task = null, float showAtLeastSeconds = 0) : base (task)
		{
			Dialog = DialogController.Instance;
			Dialog.Behaviour = this;

			HideAndClearButtons ();

			// Initally both Buttons are connected to our standard events:
			Dialog.YesButton.onClick.RemoveAllListeners ();
			Dialog.YesButton.onClick.AddListener (RaiseYesButtonClicked);

			Dialog.NoButton.onClick.RemoveAllListeners ();
			Dialog.NoButton.onClick.AddListener (RaiseNoButtonClicked);

			this.showAtLeastSeconds = showAtLeastSeconds;
			
			Debug.Log("Dialog started");
		}

		private IEnumerator AllowHideAfterSeconds(float hidesAfterSeconds)
		{
			yield return new WaitForSeconds(hidesAfterSeconds);
			keepShowing = false;
			if (hideWhenShownLongEnough)
			{
				Stop ();
				Dialog.Destroy ();
			}
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

		public override void Start ()
		{
			base.Start ();

			if (showAtLeastSeconds > float.Epsilon)
			{
				keepShowing = true;
				CoroutineStarter.Run(AllowHideAfterSeconds(showAtLeastSeconds));
			}
		}

		/// <summary>
		/// Should be called before the dialog is made invisible or disposed.
		/// </summary>
		public override void Stop ()
		{
			HideAndClearButtons ();

			Dialog.YesButton.onClick.RemoveAllListeners ();
			Dialog.NoButton.onClick.RemoveAllListeners ();

			Dialog.Hide();
		}

		/// <summary>
		/// Should only be called from the Dialog UI Component.
		/// </summary>
		public void RaiseYesButtonClicked ()
		{
			if (OnYesButtonClicked != null)
				OnYesButtonClicked (Dialog.YesButton.gameObject, EventArgs.Empty);
		}

		/// <summary>
		/// Should only be called from the Dialog UI Component.
		/// </summary>
		public void RaiseNoButtonClicked ()
		{
			if (OnNoButtonClicked != null)
				OnNoButtonClicked (Dialog.NoButton.gameObject, EventArgs.Empty);
		}

		/// <summary>
		/// Closes the dialog.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		/// TODO move to Dialog class and change event args to some more generic type
		protected void CloseDialog (object callbackSender, EventArgs args)
		{
			if (keepShowing)
			{
				hideWhenShownLongEnough = true;
			}
			else
			{
				Stop ();
    			Dialog.Destroy ();
			}
		}

	}

}
