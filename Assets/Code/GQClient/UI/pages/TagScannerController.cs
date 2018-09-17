using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Util;
using GQ.Client.Conf;

namespace GQ.Client.UI
{
	public class TagScannerController : PageController
	{
		
		#region Inspector Features

		public Text prompt;
		public Text scannedText;
		public Text forwardButtonText;

		#endregion


		#region Runtime API

		protected PageTagScanner myPage;

		/// <summary>
		/// Is called during Start() of the base class, which is a MonoBehaviour.
		/// </summary>
		public override void Initialize ()
		{
			myPage = (PageTagScanner)page;

			// show the content:
			prompt.color = ConfigurationManager.Current.mainFgColor;
			prompt.fontSize = ConfigurationManager.Current.mainFontSize;
			prompt.text = myPage.Prompt.Decode4HyperText();
			showInfo ();
			forwardButtonText.text = "Ok";
		}

		void showInfo() {
			scannedText.text = 
				"Diese Funktion steht leider noch nicht zur Verfügung. Hier werden als Test die Informationen angezeigt, die in der Quest-Seite gespeichert wurden:\n\n" +
				"type:\t\t\t\t\t\t" + myPage.PageType + "\n" +
				"id:\t\t\t\t\t\t\t\t" + myPage.Id + "\n" +
				"showTagContent:\t" + myPage.ShowTagContent + "\n" +
				"taskdescription:\t\t" + myPage.Prompt; 
		}

		public override void OnForward ()
		{
			if (myPage.AnswerCorrect (scannedText.text)) {
				myPage.Succeed ();
			} else {
				myPage.Fail ();
			}
		}


		#endregion
	}
}
