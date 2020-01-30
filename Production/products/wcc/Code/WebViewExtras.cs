using System;
using GQ.Client.Err;
using GQ.Client.Model;
using UnityEngine;

namespace GQ.Client.UI
{
    public static class WebViewExtras
    {
        public static void Initialize(PageVideoPlay myPage, GameObject containerWebPlayer)
        {
            switch (myPage.VideoType)
            {
                default:
                    Log.SignalErrorToAuthor("Unknown video type {0} used on page {1}", myPage.VideoType, myPage.Id);
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
