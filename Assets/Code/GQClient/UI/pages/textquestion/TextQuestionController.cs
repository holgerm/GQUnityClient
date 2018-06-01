using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Model;

namespace GQ.Client.UI
{
	public class TextQuestionController : PageController
	{
		
		#region Inspector Features

		public Text questionText;
		public Text promptPlaceholder;
		public Text answerGiven;
		public Button forwardButton;

		#endregion


		#region Runtime API

		protected PageTextQuestion tqPage;

		/// <summary>
		/// Is called during Start() of the base class, which is a MonoBehaviour.
		/// </summary>
		public override void Initialize ()
		{
			tqPage = (PageTextQuestion)page;

			// show the question:
			questionText.text = tqPage.Question;
			promptPlaceholder.text = tqPage.Prompt;
			forwardButton.transform.Find ("Text").GetComponent<Text> ().text = "Eingeben";
		}

		public override void OnForward ()
		{
			if (tqPage.AnswerCorrect (answerGiven.text)) {
				tqPage.Succeed ();
			} else {
				tqPage.Fail ();
			}
		}

		#endregion
	}
}
