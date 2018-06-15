using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Util;
using System;

namespace GQ.Client.UI.Dialogs {

	/// <summary>
	/// Simple dialog behaviour usable just by giving title and details text to the constructor. 
	/// 
	/// Standard behaviour will start the dialog without buttons and show the title and details. 
	/// 
	/// When the task is completed the dialog will be closed. It will be shown for at least one frame.
	/// </summary>
	public class SimpleDialogBehaviour : DialogBehaviour {

		private string title;
		private string details;

		public SimpleDialogBehaviour(Task task, string title, string details) : base(task) {
			this.title = title;
			this.details = details;
		}

		public override void Start() 
		{
			base.Start ();

			Dialog.Title.text = title;

//			if (Task.Step == 0) {
//				Dialog.Title.text = 
//					string.Format (title);
//			} else {
//				Dialog.Title.text = 
//					string.Format (title + " (Schritt {0})", Task.Step);
//			}
			Dialog.Details.text = details;

			// make completion close this dialog:
			Task.OnTaskEnded += CloseDialog;

			// show the dialog:
			Dialog.Show ();
		}

		public void UpdateLoadingScreenProgress (string percentText)
		{
			Dialog.Details.text = details + percentText;
		}

	}
}
