﻿using Code.GQClient.Conf;
using Code.GQClient.Model.pages;
using Code.GQClient.Util;
using TMPro;

namespace Code.GQClient.UI.pages.question.textquestion
{
    public class TextQuestionController : QuestionController
    {

        #region Inspector Features
        public TextMeshProUGUI questionText;
        public TMP_InputField inputField;
        public TextMeshProUGUI promptPlaceholder;
        public TextMeshProUGUI shownAnswer;
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
            questionText.color = Config.Current.mainFgColor;
            questionText.fontSize = Config.Current.mainFontSize;
            questionText.text = tqPage.Question.Decode4TMP();
            promptPlaceholder.text = tqPage.Prompt.Decode4TMP(false);
            promptPlaceholder.fontSize = Config.Current.mainFontSize;
            shownAnswer.text = "";
            inputField.text = "";
            inputField.Select();
            inputField.ActivateInputField();
            forwardButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Bestätigen";

            layout.layout();
        }

        public override void OnForward()
        {
            tqPage.Result = inputField.text;

            if (tqPage.AnswerCorrect(inputField.text))
            {
                tqPage.Succeed(alsoEnd: true);
            }
            else
            {
                if (tqPage.RepeatUntilSuccess)
                {
                    tqPage.Fail(alsoEnd: false);
                    ((TextQuestionController)tqPage.PageCtrl).Repeat();
                }
                else
                {
                    tqPage.Fail(alsoEnd: true);
                }
            }
        }
        
        public override void CleanUp() {
            Destroy(((TextQuestionLayout) layout).BackgroundImage.texture);
        }
        #endregion
    }
}
