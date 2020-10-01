//#define DEBUG_LOG

using Code.GQClient.Model.pages;
using Code.GQClient.UI.pages.videoplayer;
using Code.GQClient.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.pages
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
                return myPage.FullscreenLandscape == false;
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
            var forwardButtonText = forwardButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            if (myPage.AllowLeaveOnUrlContains != "")
            {
                forwardButtonText.text = myPage.EndButtonTextWhenClosed.Decode4TMP(false);
                forwardButton.interactable = false;
                backButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "";
                backButton.interactable = false;
            } else
            {
                forwardButtonText.text = myPage.EndButtonText.Decode4TMP(false);
                forwardButton.interactable = true;
            }

            if (myPage.FullscreenLandscape)
            {
                Screen.orientation = ScreenOrientation.Landscape;
                HeaderButtonPanel.gameObject.SetActive(false);
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
            HeaderButtonPanel.gameObject.SetActive(true);
            HeaderButtonPanel.SetInteractable(true);
            Screen.orientation = ScreenOrientation.Portrait;
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
