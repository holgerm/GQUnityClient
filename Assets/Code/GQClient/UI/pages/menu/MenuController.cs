using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;
using UnityEngine.UI;
using GQ.Client.Util;

namespace GQ.Client.UI
{

	public class MenuController : PageController
	{
		#region Inspector Features

		public Text questionText;
		public Transform choicesContainer;

		#endregion

		#region Runtime API

		protected PageMenu myPage;

		/// <summary>
		/// Is called during Start() of the base class, which is a MonoBehaviour.
		/// </summary>
		public override void Initialize ()
		{
			myPage = (PageMenu)page;

			// show the question:
			questionText.text = myPage.Question.Decode4HyperText();

			// show the answers:
			foreach (MenuChoice a in myPage.Choices) {
				// create dialog item GO from prefab:
				ChoiceCtrl.Create (myPage, choicesContainer, a);
			}
		}

		#endregion

	}
}
