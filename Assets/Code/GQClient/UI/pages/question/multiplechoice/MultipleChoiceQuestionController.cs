using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;
using GQ.Client.Util;
using GQ.Client.Conf;
using TMPro;

namespace GQ.Client.UI
{

    public class MultipleChoiceQuestionController : QuestionController
	{
		#region Inspector Features
		public TextMeshProUGUI questionText;
		public Transform answersContainer;
        #endregion

        #region Runtime API
        /// <summary>
        /// Shows top margin:
        /// </summary>
        public override bool ShowsTopMargin
        {
            get
            {
                return true;
            }
        }

        public PageMultipleChoiceQuestion mcqPage
        {
            get;
            protected set;
        }

        /// <summary>
        /// Is called during Start() of the base class, which is a MonoBehaviour.
        /// </summary>
        public override void InitPage_TypeSpecific ()
		{
            mcqPage = (PageMultipleChoiceQuestion)page;

            // show the question:
            questionText.text = mcqPage.Question.Decode4TMP();
            questionText.color = ConfigurationManager.Current.mainFgColor;
            questionText.fontSize = ConfigurationManager.Current.mainFontSize;

            // shuffle anwers:
            if (mcqPage.Shuffle)
                Shuffle<MCQAnswer>(mcqPage.Answers);

            // clear answers (maybe we had another mcq just before ...
            foreach (Transform child in answersContainer)
            {
                Destroy(child.gameObject);
            }

            // show answers:
            foreach (MCQAnswer a in mcqPage.Answers) {
				// create dialog item GO from prefab:
				AnswerCtrl.Create (mcqPage, answersContainer, a);
			}

            // footer:
            // hide footer if no return possible:
            FooterButtonPanel.transform.parent.gameObject.SetActive(mcqPage.Quest.History.CanGoBackToPreviousPage);
            forwardButton.gameObject.SetActive(false);
            // TODO when we enhance to real multiple choice mode we have to adapt this ...

            layout.layout();
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
