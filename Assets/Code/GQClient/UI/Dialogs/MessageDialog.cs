namespace Code.GQClient.UI.Dialogs
{
	public class MessageDialog : DialogBehaviour
	{
		private string message { get; set; }
		private string buttontext { get; set; }

		public MessageDialog (string message, string buttontext = "Ok") : base (null) 
		// 'null' because we do NOT connect a Task, since message dialogs only rely on user interaction
		{
			this.message = message;
			this.buttontext = buttontext;
		}

		public override void Start ()
		{
			base.Start ();

			Dialog.Title.gameObject.SetActive (false);
			Dialog.Img.gameObject.SetActive (false);
			Dialog.Details.text = message;
			Dialog.SetYesButton (buttontext, CloseDialog);
			Dialog.NoButton.gameObject.SetActive (false);

			// show the dialog:
			Dialog.Show ();
		}
	}
}
