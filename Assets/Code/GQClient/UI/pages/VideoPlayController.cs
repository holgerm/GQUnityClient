using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Util;
using GQ.Client.Conf;

namespace GQ.Client.UI
{
	public class VideoPlayController : PageController
	{
		
		#region Inspector Fields

		public GameObject contentPanel;
		public Text infoText;
		public Text forwardButtonText;

		#endregion


		#region Runtime API

		protected PageVideoPlay myPage;

		/// <summary>
		/// Is called during Start() of the base class, which is a MonoBehaviour.
		/// </summary>
		public override void Initialize ()
		{
			myPage = (PageVideoPlay)page;

			// show the content:
			showInfo ();
			forwardButtonText.text = "Ok";
		}

		void showInfo() {
			infoText.text = 
				"Diese Funktion steht leider noch nicht zur Verfügung. Hier werden als Test die Informationen angezeigt, die in der Quest-Seite gespeichert wurden:\n\n" +
				"type:\t\t\t" + myPage.PageType + "\n" +
				"id:\t\t\t\t\t" + myPage.Id + "\n" +
                "file:\t\t\t" + myPage.VideoFile + "\n" +
				"cotrollable:\t" + myPage.Controllable + "\n" +
				"text:\t\t\t\t" + myPage.Portrait; 
		}

		#endregion
	}
}
