using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;
using UnityEngine.UI;
using GQ.Client.Err;
using GQ.Client.UI.Dialogs;

namespace GQ.Client.UI
{

	public class MultipleChoiceQuestionController : QuestionController
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

            // shuffle anwers:
            if (mcqPage.Shuffle)
                Shuffle<MCQAnswer>(mcqPage.Answers);

            // show answers:
            foreach (MCQAnswer a in mcqPage.Answers) {
				// create dialog item GO from prefab:
				AnswerCtrl.Create (mcqPage, answersContainer, a);
			}
        }

        private static System.Random rng = new System.Random();

        private static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        #endregion

    }
}
