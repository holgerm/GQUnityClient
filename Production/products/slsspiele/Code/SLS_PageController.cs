using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GQ.Client.UI
{

	public class SLS_PageController : NPCTalkController
	{
	
		#region Inspector Fields

		public Text titleText;

		#endregion

		#region Runtime API

		/// <summary>
		/// Is called during Start() of the base class, which is a MonoBehaviour.
		/// </summary>
		public override void Initialize ()
		{
			base.Initialize ();

			titleText.text = page.Quest.Name;
		}

		#endregion

	}

}
