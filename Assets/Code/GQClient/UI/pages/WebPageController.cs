//#define DEBUG_LOG

using UnityEngine.UI;
using GQ.Client.Model;
using TMPro;
using GQ.Client.Util;
using UnityEngine;

namespace GQ.Client.UI
{
    public class WebPageController : PageController
    {

        #region Inspector Fields
        public RectTransform webContainer;
        #endregion

        internal Button ForwardButton => forwardButton;
        internal Button BackButton => backButton;


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

        internal PageWebPage myPage;

        /// <summary>
        /// Is called during Start() of the base class, which is a MonoBehaviour.
        /// </summary>
        public override void InitPage_TypeSpecific()
        {
            myPage = (PageWebPage)page;

            // show the forward button text:
            TextMeshProUGUI forwardButtonText = forwardButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            if (myPage.ShouldEndOnLoadUrlPart)
            {
                forwardButtonText.text = myPage.ForwardButtonTextBeforeFinished.Decode4TMP(false);
                forwardButton.interactable = false;
                backButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "";
                backButton.interactable = false;
            } else
            {
                forwardButtonText.text = myPage.EndButtonText.Decode4TMP(false);
                forwardButton.interactable = true;
            }
            
            // show the content:
            HeaderButtonPanel.SetInteractable(false); // disable top buttons
            WebViewExtras.Initialize(this, webContainer, myPage.URL);
        }

        /// <summary>
        /// Override this method to react on Forward Button Click (or similar events).
        /// </summary>
        public override void OnForward()
        {
            HeaderButtonPanel.SetInteractable(true);
            base.OnForward();
        }

        /// <summary>
        /// Override this method to react on Forward Button Click (or similar events).
        /// </summary>
        public override void OnBackward()
        {
            HeaderButtonPanel.SetInteractable(true);
            base.OnBackward();
        }
        #endregion
    }
}
