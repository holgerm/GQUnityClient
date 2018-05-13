using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GQ.Client.UI.Dialogs
{
	public class MessageDialog : DialogBehaviour
	{
		private string message { get; set; }

		public MessageDialog (string message) : base (null) 
		// 'null' because we do NOT connect a Task, sice message dialogs only rely on user interaction
		{
			this.message = message;
		}

		public override void Start ()
		{
			base.Start ();

			Dialog.Title.gameObject.SetActive (false);
			Dialog.Img.gameObject.SetActive (false);
			Dialog.Details.text = message;
			Dialog.SetYesButton ("Ok", CloseDialog);
			Dialog.NoButton.gameObject.SetActive (false);

			// show the dialog:
			Dialog.Show ();
		}
	}
}
