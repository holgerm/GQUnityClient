#define DEBUG_LOG

using UnityEngine.UI;
using GQ.Client.Model;
using TMPro;
using GQ.Client.Util;
using UnityEngine;
using QM.Util;

namespace GQ.Client.UI
{
    public class WebPageController : PageController
	{
		
		#region Inspector Fields
		public RectTransform webContainer;
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

        protected PageWebPage myPage;

		/// <summary>
		/// Is called during Start() of the base class, which is a MonoBehaviour.
		/// </summary>
		public override void InitPage_TypeSpecific ()
		{
            myPage = (PageWebPage)page;

            // show the forward button text:
            TextMeshProUGUI forwardButtonText = forwardButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            forwardButtonText.text = myPage.EndButtonText.Decode4TMP(false);

            // show the content:
//            UniWebView webView = webContainer.GetComponent<UniWebView>();
//            if (webView == null)
//            {
//                webView = webContainer.gameObject.AddComponent<UniWebView>();
//            }

//#if DEBUG_LOG
//            UniWebViewLogger.Instance.LogLevel = UniWebViewLogger.Level.Verbose;

//            webView.OnPageStarted += (view, url) => {
//                print("Loading started for url: " + url);
//            };

//            webView.OnPageFinished += (view, statusCode, url) => {
//                print(statusCode);
//                print("Web view loading finished for: " + url);
//            };
//#endif

//            webView.OnPageErrorReceived += (view, error, message) => {
//                // TODO show error message also to user.
//                print("Error: " + message);
//            };

//            float headerHeight = LayoutConfig.Units2Pixels(LayoutConfig.HeaderHeightUnits);
//            float footerHeight = LayoutConfig.Units2Pixels(LayoutConfig.FooterHeightUnits);
//            webView.Frame =
//                new Rect(
//                    0, headerHeight,
//                    Device.width, Device.height - (headerHeight + footerHeight)
//                );
//            webView.SetShowSpinnerWhileLoading(true);
//            webView.Show(true);
            HeaderButtonPanel.SetInteractable(false); // disable top buttons
            //webView.Load(myPage.URL);
            VideoPlayerExtraModes.Initialize(webContainer, myPage.URL);
        }

        /// <summary>
        /// Override this method to react on Forward Button Click (or similar events).
        /// </summary>
        public override void OnForward()
        {
            HeaderButtonPanel.SetInteractable(true);
            base.OnForward();
        }
        #endregion
    }
}
