using GQ.Client.Err;
using GQ.Client.Model;
using GQ.Client.Util;
using UnityEngine;

namespace GQ.Client.UI
{
    public static class VideoPlayerExtraModes
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
    }
}
