using GQ.Client.Err;
using GQ.Client.Model;
using GQ.Client.Util;
using UnityEngine;

namespace GQ.Client.UI
{
    public static class VideoPlayerExtraModes
    {
        public static UniWebView uniWebView;

        public static void Initialize(PageVideoPlay myPage, GameObject containerWebPlayer)
        {
            switch (myPage.VideoType)
            {
                case GQML.PAGE_VIDEOPLAY_VIDEOTYPE_YOUTUBE:
                    // USE HTML WEBVIEW FOR VIDEO:                    
                    Debug.Log("YOUTUBE PLAYER: start.");

                    uniWebView = containerWebPlayer.GetComponent<UniWebView>();
                    if (uniWebView == null)
                        uniWebView = containerWebPlayer.AddComponent<UniWebView>();
                    Debug.Log("YOUTUBE PLAYER: UniWebView component added.");

                    uniWebView.OnPageStarted += (view, url) => {
                        Debug.Log("YOUTUBE PLAYER: OnPageStarted with url: " + url);
                    };
                    uniWebView.OnPageFinished += (view, statusCode, url) => {
                        Debug.Log("YOUTUBE PLAYER: OnPageFinished with status code: " + statusCode);
                    };
                    uniWebView.OnPageErrorReceived += (UniWebView webView, int errorCode, string errorMessage) => {
                        Debug.Log("YOUTUBE PLAYER: OnPageErrorReceived errCode: " + errorCode
                                  + "\n\terrMessage: " + errorMessage);
                    };
                    uniWebView.OnShouldClose += (webView) => {
                        Debug.Log("YOUTUBE PLAYER: OnShouldClose.");
                        //webView = null;
                        containerWebPlayer.SetActive(false);
                        return true;
                    };

                    UniWebViewLogger.Instance.LogLevel = UniWebViewLogger.Level.Verbose;

                    containerWebPlayer.SetActive(true);
                    Debug.Log("YOUTUBE PLAYER: component set active.");
                    float headerHeight = LayoutConfig.Units2Pixels(LayoutConfig.HeaderHeightUnits); // + 30;
                    float footerHeight = LayoutConfig.Units2Pixels(LayoutConfig.FooterHeightUnits); // + 30;
                    uniWebView.Frame = 
                        new Rect(
                            50, headerHeight, 
                            Device.width, Device.height - (headerHeight + footerHeight)
                        );
                    Debug.Log("YOUTUBE PLAYER: frame set.");
                    VideoPlayController vpCtrl = (VideoPlayController)myPage.PageCtrl;
                    //uniWebView.ReferenceRectTransform = vpCtrl.webPlayerContent;
                    string videoHtml = string.Format(YoutubeHTMLFormatString, myPage.VideoFile);
                    uniWebView.LoadHTMLString(videoHtml, "https://www.youtube.com/");
                    Debug.Log("YOUTUBE PLAYER: htmlString loaded.");
                    uniWebView.SetShowSpinnerWhileLoading(true);
                    Debug.Log("YOUTUBE PLAYER: spinner set.");

                    uniWebView.Show(true);
                    Debug.Log("YOUTUBE PLAYER: after show()");
                    break;
                default:
                    Log.SignalErrorToAuthor("Unknown video type {0} used on page {1}", myPage.VideoType, myPage.Id);
                    break;
            }
        }

        private static string YoutubeHTMLFormatString =
           @"<html>
                <head></head>
                <body style=""margin:0\"">
                    <iframe width = ""100%"" height=""100%"" 
                        src=""https://www.youtube.com/embed/{0}"" frameborder=""0"" 
                        allow=""autoplay; encrypted-media"" allowfullscreen>
                    </iframe>
                </body>
            </html>";

        public static void CleanUp(GameObject containerWebPlayer) {
            if (uniWebView != null)
            {
                Debug.Log("YOUTUBE PLAYER: Hiding.");
                uniWebView.Hide(true);
                uniWebView = null;
            }

            if (containerWebPlayer != null) {
                containerWebPlayer.SetActive(false);
            }
        }
    }
}
