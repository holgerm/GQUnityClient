using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Model;

namespace GQ.Client.UI
{

	public class SLS_PageController : PageController
	{
	
		#region Inspector Fields

		public Text titleText;

		#endregion

		#region Runtime API

		PageSLS_Spielbeschreibung slsPage;

		/// <summary>
		/// Is called during Start() of the base class, which is a MonoBehaviour.
		/// </summary>
		public override void Initialize ()
		{
			slsPage = (PageSLS_Spielbeschreibung)page;

			// set title:
			titleText.text = page.Quest.Name;
		}

		#endregion

	}

}
