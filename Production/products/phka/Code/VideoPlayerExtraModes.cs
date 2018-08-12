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
                    uniWebView = containerWebPlayer.AddComponent<UniWebView>();
                    containerWebPlayer.SetActive(true);
                    float headerHeight = LayoutConfig.Units2Pixels(LayoutConfig.HeaderHeightUnits) + 30;
                    uniWebView.Frame = new Rect(0, headerHeight, Device.width, Device.height - headerHeight);
                    string videoHtml = string.Format(YoutubeHTMLFormatString, myPage.VideoFile);
                    uniWebView.LoadHTMLString(videoHtml, "https://www.youtube.com/");
                    uniWebView.Show();
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

    }
}
