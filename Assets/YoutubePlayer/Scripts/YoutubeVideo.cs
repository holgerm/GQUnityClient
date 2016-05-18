using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using YoutubeExtractor;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

public class YoutubeVideo : MonoBehaviour {

    public static YoutubeVideo Instance;

    void Awake()
    {
        Instance = this;
    }


    public bool drawBackground = false;
    public Texture2D backgroundImage;

    public string RequestVideo(string urlOrId, int quality)
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

        Uri uriResult;
        bool result = Uri.TryCreate(urlOrId, UriKind.Absolute, out uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

        if (!result)
            urlOrId = "https://youtube.com/watch?v=" + urlOrId;


        IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(urlOrId, false);
        VideoInfo video = null;

        //Search for video with desired format and desired resolution, you can filter if your own methods if you need.
        var enumerator = videoInfos.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.VideoType == VideoType.Mp4 && enumerator.Current.Resolution == quality)
            {
                video = enumerator.Current;
                break;
            }
        }

        if (video.RequiresDecryption)
        {
            DownloadUrlResolver.DecryptDownloadUrl(video);
        }

        Debug.Log("The mp4 is: " + video.DownloadUrl);
        return video.DownloadUrl;
    }

    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                    }
                }
            }
        }
        return isOk;
    }

	void OnGUI()
	{
		GUI.depth = 1;
		if(drawBackground)
		{
			GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), backgroundImage);
		}
	}

}
