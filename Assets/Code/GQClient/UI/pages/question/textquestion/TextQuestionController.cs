using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Util;
using GQ.Client.Conf;

namespace GQ.Client.UI
{
    public class TextQuestionController : QuestionController
    {

        #region Inspector Features

        public Text questionText;
        public InputField inputField;
        public Text promptPlaceholder;
        public Text answerGiven;

        #endregion


        #region Runtime API

        public PageTextQuestion tqPage
        {
            get;
            protected set;
        }


        /// <summary>
        /// Is called during Start() of the base class, which is a MonoBehaviour.
        /// </summary>
        public override void InitPage_TypeSpecific()
        {
            tqPage = (PageTextQuestion)page;

            // show the question:
            questionText.color = ConfigurationManager.Current.mainFgColor;
            questionText.fontSize = ConfigurationManager.Current.mainFontSize;
            questionText.text = tqPage.Question.Decode4HyperText();
            promptPlaceholder.text = tqPage.Prompt;
            promptPlaceholder.fontSize = ConfigurationManager.Current.mainFontSize;
            answerGiven.fontSize = ConfigurationManager.Current.mainFontSize;
            answerGiven.text = "";
            inputField.text = "";
            inputField.Select();
            inputField.ActivateInputField();
            forwardButton.transform.Find("Text").GetComponent<Text>().text = "Eingeben";

            layout.layout();
        }

        public override void OnForward()
        {
            if (tqPage.AnswerCorrect(answerGiven.text))
            {
                tqPage.Succeed();
            }
            else
            {
                if (tqPage.RepeatUntilSuccess)
                {
                    ((TextQuestionController)tqPage.PageCtrl).Repeat();
                }
                else
                {
                    tqPage.Fail();
                }

            }
        }

        #endregion
    }
}
