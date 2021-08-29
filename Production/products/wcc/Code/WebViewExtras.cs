using Code.GQClient.Err;
using Code.GQClient.Model.pages;
using Code.GQClient.UI.pages;
using UnityEngine;

namespace Code.GQClient.UI.pages.videoplayer
{
    public static class WebViewExtras
    {
        public static void Initialize(VideoPlayController vpCtrl)
        {
            switch (vpCtrl.MyPage.VideoType)
            {
                default:
                    Log.SignalErrorToAuthor("Unknown video type {0} used on page {1}", vpCtrl.MyPage.VideoType, vpCtrl.MyPage.Id);
                    break;
            }
        }

        public static void CleanUp(GameObject containerWebPlayer)
        {
            Log.SignalErrorToAuthor("WebViewExtras: Nothing to cleanup.");
        }

        internal static void Initialize(WebPageController webPageController, RectTransform webContainer, string uRL)
        {
            Log.SignalErrorToAuthor("WebViewExtras: Should not be used in this App.");
        }
    }
}
