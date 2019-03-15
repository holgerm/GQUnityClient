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
		public override void InitPage_TypeSpecific ()
		{
            myPage = (PageMenu)page;

			// show the question:
			questionText.text = myPage.Question.Decode4HyperText();

            // shuffle anwers:
            if (myPage.Shuffle)
                Shuffle<MenuChoice>(myPage.Choices);

            // show the answers:
            foreach (MenuChoice a in myPage.Choices) {
				// create dialog item GO from prefab:
				ChoiceCtrl.Create (myPage, choicesContainer, a);
			}

            // footer:
            // hide footer if no return possible:
            FooterButtonPanel.transform.parent.gameObject.SetActive(myPage.Quest.History.CanGoBackToPreviousPage);
            forwardButton.gameObject.SetActive(false);
            // TODO when we enhance to real multiple choice mode we have to adapt this ...
        }

        #endregion

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

    }
}
