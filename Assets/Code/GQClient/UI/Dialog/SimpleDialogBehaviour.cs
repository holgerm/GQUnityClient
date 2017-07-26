using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Util;

namespace GQ.Client.UI.Dialogs {

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

			if (Task.Step == 0) {
				Dialog.Title.text = 
					string.Format (title);
			} else {
				Dialog.Title.text = 
					string.Format (title + " (step {0})", Task.Step);
			}
			Dialog.Details.text = details;

			// make completion close this dialog:
			Task.OnTaskCompleted += CloseDialog;

			// show the dialog:
			Dialog.Show ();
		}

	}
}
