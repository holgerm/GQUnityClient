using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Util;
using GQ.Client.Conf;
using TMPro;

namespace GQ.Client.UI
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
            questionText.text = tqPage.Question.Decode4TMP();
            promptPlaceholder.text = tqPage.Prompt;
            promptPlaceholder.fontSize = ConfigurationManager.Current.mainFontSize;
            //shownAnswer.fontSize = ConfigurationManager.Current.mainFontSize;
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
        #endregion
    }
}
