using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;
using UnityEngine.UI;

namespace GQ.Client.UI
{

	public class MultipleChoiceQuestionController : PageController
	{
		#region Inspector Features

		public Text questionText;
		public Transform answersContainer;

		#endregion

		#region Runtime API

		protected PageMultipleChoiceQuestion mcqPage;

		/// <summary>
		/// Is called during Start() of the base class, which is a MonoBehaviour.
		/// </summary>
		public override void Initialize ()
		{
			mcqPage = (PageMultipleChoiceQuestion)page;

			// show the question:
			questionText.text = mcqPage.Question;

			// show the answers:
			foreach (MCQAnswer a in mcqPage.Answers) {
				// create dialog item GO from prefab:
				AnswerCtrl.Create (mcqPage, answersContainer, a);
			}
		}

		#endregion

	}
}
