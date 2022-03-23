using System;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Model.pages;
using Code.GQClient.Util.http;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Code.GQClient.UI.pages.question.textquestion
{
    [RequireComponent(typeof(TextQuestionController))]
    public class TextQuestionLayout : PageLayout
    {
        public RawImage BackgroundImage;
        public Image QuestionBackgroundImage;

        PageTextQuestion myPage;

        protected override void setMainBackground()
        {
            Image pageBG = ContentArea.GetComponent<Image>();
            if (pageBG == null)
            {
                Log.SignalErrorToDeveloper("Scene TextQuestion broken: ContentArea must have an Image component!");
                return;
            }

            myPage = gameObject.GetComponent<TextQuestionController>().tqPage;

            if (myPage == null || string.IsNullOrEmpty(myPage.BackGroundImage))
            {
                // NO Background Image given => 
                // - we use standard bg color:
                pageBG.color = Config.Current.contentBackgroundColor;
                pageBG.enabled = true;
                // - and hide Background Image:
                BackgroundImage.gameObject.SetActive(false);
                // - and do not use questionBG:
                QuestionBackgroundImage.enabled = false;
            }
            else
            {
                // A Backgroudn Image is given =>
                // - we disabe normal bg image:
                pageBG.enabled = false;

                // - we do use questionBG:
                QuestionBackgroundImage.enabled = true;
                QuestionBackgroundImage.color = new Color(
                    (float)Config.Current.contentBackgroundColor.r / 256f,
                    (float)Config.Current.contentBackgroundColor.g / 256f,
                    (float)Config.Current.contentBackgroundColor.b / 256f,
                    a: 200f / 256f // make question background semi transparent
                );

                // - and we load Texture and set to Background Image:
                BackgroundImage.gameObject.SetActive(true);

                AbstractDownloader loader;
                if (QuestManager.Instance.MediaStore.ContainsKey(myPage.BackGroundImage))
                {
                    QuestManager.Instance.MediaStore.TryGetValue(myPage.BackGroundImage, out var mediaInfo);
                    loader = new LocalFileLoader(mediaInfo.LocalPath, new DownloadHandlerTexture());
                }
                else
                {
                    loader =
                        new Downloader(
                            url: myPage.BackGroundImage,
                            new DownloadHandlerTexture(),
                            timeout: Config.Current.timeoutMS,
                            maxIdleTime: Config.Current.maxIdleTimeMS
                        );
                    // TODO store the image locally ...
                }

                loader.OnSuccess += (AbstractDownloader d, DownloadEvent e) =>
                {
                    DownloadHandlerTexture dhTexture = null;
                    try
                    {
                        dhTexture = (DownloadHandlerTexture)d.DownloadHandler;
                    }
                    catch (InvalidCastException)
                    {
                        Log.SignalErrorToDeveloper(
                            "TextQuestionLayout tried to load bgImage without TextureDownloadHandler");
                        return;
                    }
                    
                    var fitter = BackgroundImage.gameObject.GetComponent<AspectRatioFitter>();
                    fitter.aspectRatio = (float)dhTexture.texture.width / (float)dhTexture.texture.height;
                    BackgroundImage.texture = dhTexture.texture;
                };
                loader.Start();
            }
        }
    }
}