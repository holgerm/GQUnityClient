using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Util;
using GQ.Client.Conf;

namespace GQ.Client.UI
{
	public class WebPageController : PageController
	{
		
		#region Inspector Fields

		public GameObject contentPanel;
		public Text infoText;
		public Text forwardButtonText;

		#endregion


		#region Runtime API

		protected PageWebPage myPage;

		/// <summary>
		/// Is called during Start() of the base class, which is a MonoBehaviour.
		/// </summary>
		public override void Initialize ()
		{
			myPage = (PageWebPage)page;

			// show the content:
			showInfo ();
			forwardButtonText.text = "Ok";
		}

		void showInfo() {
			infoText.text = 
				"Diese Funktion steht leider noch nicht zur Verfügung. Hier werden als Test die Informationen angezeigt, die in der Quest-Seite gespeichert wurden:\n\n" +
				"type:\t" + myPage.PageType + "\n" +
				"id:\t\t\t" + myPage.Id + "\n" +
				"file:\t" + myPage.File + "\n" +
				"url:\t\t" + myPage.URL; 
		}

		#endregion
	}
}
