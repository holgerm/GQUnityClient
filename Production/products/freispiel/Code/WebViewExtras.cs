using System;
using System.Collections;
using Code.GQClient.Err;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Model.pages;
using Code.GQClient.UI.layout;
using Code.GQClient.Util;
using Code.QM.Util;
using Paroxe.PdfRenderer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Code.GQClient.UI.pages.videoplayer
{
    public static class WebViewExtras
    {
        public static UniWebView uniWebView;

        public static void Initialize(VideoPlayController vpCtrl)
        {
            switch (vpCtrl.MyPage.VideoType)
            {
                case GQML.PAGE_VIDEOPLAY_VIDEOTYPE_YOUTUBE:
                    // USE HTML WEBVIEW FOR VIDEO:                    
                    uniWebView = vpCtrl.containerWebPlayer.GetComponent<UniWebView>();
                    if (uniWebView == null)
                    {
                        uniWebView = vpCtrl.containerWebPlayer.AddComponent<UniWebView>();
                    }

                    uniWebView.OnPageErrorReceived += (UniWebView webView, int errorCode, string errorMessage) =>
                    {
                        Log.SignalErrorToDeveloper("YOUTUBE PLAYER: OnPageErrorReceived errCode: " + errorCode
                            + "\n\terrMessage: " +
                            errorMessage);
                    };
                    uniWebView.OnShouldClose += (webView) =>
                    {
                        Debug.Log("YOUTUBE PLAYER: OnShouldClose.");
                        //webView = null;
                        vpCtrl.containerWebPlayer.SetActive(false);
                        return true;
                    };
                    
                    UniWebViewLogger.Instance.LogLevel = UniWebViewLogger.Level.Verbose;

                    vpCtrl.containerWebPlayer.SetActive(true);

                    vpCtrl.FooterButtonPanel =
                        vpCtrl.webPlayerFooterButtonPanel;
                    Transform backButtonGO = vpCtrl.FooterButtonPanel.transform.Find("BackButton");
                    backButtonGO.gameObject.SetActive(vpCtrl.MyPage.Quest.History.CanGoBackToPreviousPage);

                    float headerHeight = LayoutConfig.Units2Pixels(LayoutConfig.HeaderHeightUnits);
                    float footerHeight = LayoutConfig.Units2Pixels(LayoutConfig.FooterHeightUnits);
                    uniWebView.Frame =
                        new Rect(
                            0, headerHeight,
                            Screen.width, Screen.height - (headerHeight + footerHeight)
                        );
                    Debug.Log($"WATCH UNIWEBVIEW frame: {uniWebView.Frame} screen: h: {Screen.height}, w: {Screen.width}");

                    AllowLeavePage(vpCtrl);
                    
                    //VideoPlayController vpCtrl = (VideoPlayController)myPage.PageCtrl;
                    //uniWebView.ReferenceRectTransform = vpCtrl.webPlayerContent;
                    uniWebView.SetShowSpinnerWhileLoading(true);
                    uniWebView.Show(true);

                    string videoHtml = string.Format(YOUTUBE_HTML_FORMAT_STRING, vpCtrl.MyPage.VideoFile);
                    uniWebView.LoadHTMLString(videoHtml, "https://www.youtube.com/");
                    break;
                default:
                    Log.SignalErrorToAuthor("Unknown video type {0} used on page {1}", vpCtrl.MyPage.VideoType, vpCtrl.MyPage.Id);
                    break;
            }
        }

        private const string YOUTUBE_HTML_FORMAT_STRING = @"<html>
                <head></head>
                <body style=""margin:0\"">
                    <iframe width = ""100%"" height=""100%"" 
                        src=""https://www.youtube.com/embed/{0}"" frameborder=""0"" 
                        allow=""autoplay; encrypted-media"" allowfullscreen>
                    </iframe>
                </body>
            </html>";

        public static void Initialize(WebPageController pageCtrl, RectTransform webContainer, string url)
        {
            if (PageWebPage.PdfUrlRegex.IsMatch(url))
            {
                InitializePdfView(pageCtrl, webContainer, url);
            }
            else
            {
                InitializeWebView(pageCtrl, webContainer, url);
            }
        }

        private static void InitializeWebView(WebPageController pageCtrl, RectTransform webContainer, string url)
        {
            // show the content:
            var webView = webContainer.GetComponent<UniWebView>();
            if (webView == null)
            {
                webView = webContainer.gameObject.AddComponent<UniWebView>();
            }

            if (ShouldCheckToAllowLeavePage(pageCtrl))
            {
                webView.OnPageFinished += (view, statusCode, curUrl) =>
                {
                    if (!CheckUrlToAllowForwardButton(pageCtrl, curUrl))
                        view.GetHTMLContent((html) => { CheckHtmlToAllowForwardButton(pageCtrl, html); });
                };

                webView.OnPageStarted += (view, curUrl) => { CheckUrlToAllowForwardButton(pageCtrl, curUrl); };
            }

            webView.OnPageErrorReceived += (view, error, message) =>
            {
                // TODO show error message also to user.
                Debug.Log("Error: " + message);
            };
            
            var headerHeight = LayoutConfig.Units2Pixels(LayoutConfig.HeaderHeightUnits);
            var footerHeight = LayoutConfig.Units2Pixels(LayoutConfig.FooterHeightUnits);
            
            void SetFrameSize()
            {
                webView.Frame =
                    pageCtrl.myPage.FullscreenLandscape
                        ? new Rect(
                            0, 0,
                            Screen.width, Screen.height)
                        : new Rect(
                            0, headerHeight,
                            Screen.width, Screen.height - (headerHeight + footerHeight));
                Debug.Log(
                    $"Webview Frame width: {webView.Frame.width} , height: {webView.Frame.height}");
            }

            SetFrameSize();
            webView.OnOrientationChanged += (view, orientation) => { SetFrameSize(); };
            
            webView.SetShowSpinnerWhileLoading(true);
            webView.SetZoomEnabled(true);
            webView.Show(true);
            webView.Load(url);
        }

        private static void InitializePdfView(WebPageController pageCtrl, RectTransform pdfContainer, string url)
        {
            var match = PageWebPage.PdfUrlRegex.Match(url);
            if (!match.Groups["url"].Success)
                return;

            var pdfUrl = match.Groups["url"].Value;
            var pageNr = 1;
            if (match.Groups["page"].Success)
                pageNr = int.Parse(match.Groups["page"].Value);

            var pdfViewer = pdfContainer.GetChild(0).GetComponent<PDFViewer>();
            var cover = pdfContainer.GetChild(1).gameObject;
            cover.SetActive(true); // cover the pdf until it is readily loaded etc.

            QuestManager.Instance.MediaStore.TryGetValue(pdfUrl, out var mediaInfo);
            if (mediaInfo != null)
            {
                // pdf locally loaded:
                pdfViewer.FileSource = PDFViewer.FileSourceType.FilePath;
                pdfViewer.FilePath = mediaInfo.LocalPath;
            }
            else
            {
                // pdf remotely to load via url:
                pdfViewer.FileSource = PDFViewer.FileSourceType.Web;
                pdfViewer.FileURL = url;
            }

            pdfViewer.gameObject.SetActive(true);
            Base.Instance.StartCoroutine(PdfGoToPage(pdfViewer, pageNr, cover));
        }

        private static IEnumerator PdfGoToPage(PDFViewer pdfViewer, int pageNr, GameObject cover)
        {
            while (!pdfViewer.IsLoaded)
            {
                yield return null;
            }

            pdfViewer.GoToPage(pageNr - 1);

            yield return new WaitForEndOfFrame();

            // uncover the pdf after it has been set to the correct page:
            cover.SetActive(false);
        }

        private static bool ShouldCheckToAllowLeavePage(WebPageController pageCtrl)
        {
            var shouldCheck = pageCtrl.myPage.AllowLeaveOnUrlContains != "";
            shouldCheck |= pageCtrl.myPage.AllowLeaveOnUrlDoesNotContain != "";
            shouldCheck |= pageCtrl.myPage.AllowLeaveOnHtmlContains.Count > 0;
            shouldCheck |= pageCtrl.myPage.AllowLeaveOnHtmlDoesNotContain.Count > 0;
            return shouldCheck;
        }

        private static bool CheckUrlToAllowForwardButton(WebPageController pageCtrl, string myUrl)
        {
            var allowLeave = myUrl.Contains(pageCtrl.myPage.AllowLeaveOnUrlContains) ||
                             !myUrl.Contains(pageCtrl.myPage.AllowLeaveOnUrlDoesNotContain);
            if (allowLeave)
            {
                AllowLeavePage(pageCtrl);
            }

            return allowLeave;
        }

        private static void AllowLeavePage(WebPageController pageCtrl)
        {
            pageCtrl.ForwardButton.interactable = true;
            var forwardButtonText = pageCtrl.ForwardButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            forwardButtonText.text = pageCtrl.myPage.EndButtonText.Decode4TMP(false);
            pageCtrl.BackButton.interactable =
                true; // might be set even if back button is not shown but does not matter
            pageCtrl.BackButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "<";

            if (pageCtrl.myPage.LeaveOnAllow)
            {
                pageCtrl.myPage.End(false);
            }
        }

        private static void AllowLeavePage(VideoPlayController pageCtrl)
        {
            Debug.Log($"ALLOW_LEAVE_PAGE ForwardButton: {pageCtrl.ForwardButton != null} text: {pageCtrl.ForwardButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text}");
            Debug.Log($"ALLOW_LEAVE_PAGE ForwardButton: Active In Hierarcy {pageCtrl.ForwardButton.gameObject.activeInHierarchy}");
            pageCtrl.ForwardButton.interactable = true;
            pageCtrl.ForwardButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = ">>>";
            pageCtrl.BackButton.interactable =
                true; // might be set even if back button is not shown but does not matter
            pageCtrl.BackButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "<<<";
            
            // DEBUG:
            pageCtrl.ForwardButton.onClick.AddListener(() => Debug.Log("WATCH: CLICKED FORWARD BUTTON"));
        }

        private static void CheckHtmlToAllowForwardButton(WebPageController pageCtrl, string html)
        {
            var success = false;

            foreach (var searchString in pageCtrl.myPage.AllowLeaveOnHtmlContains)
            {
                success |= html.Contains(searchString);
            }

            foreach (var searchString in pageCtrl.myPage.AllowLeaveOnHtmlDoesNotContain)
            {
                success &= !html.Contains(searchString);
            }


            if (success)
            {
                AllowLeavePage(pageCtrl);
            }
        }

        public static void CleanUp(GameObject containerWebPlayer)
        {
            if (uniWebView != null)
            {
                uniWebView.Stop();
                uniWebView.Hide(true);
                Object.Destroy(uniWebView);
            }

            if (containerWebPlayer != null)
            {
                containerWebPlayer.SetActive(false);
            }
        }
    }
}