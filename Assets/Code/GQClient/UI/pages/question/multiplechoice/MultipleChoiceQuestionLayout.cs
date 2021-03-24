using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Model.pages;
using Code.GQClient.Util.http;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.pages.question.multiplechoice
{
    [RequireComponent(typeof(MultipleChoiceQuestionController))]
    public class MultipleChoiceQuestionLayout : PageLayout
    {
        public RawImage BackgroundImage;
        public Image QuestionBackgroundImage;

        PageMultipleChoiceQuestion myPage;

        protected override void setMainBackground()
        {
            Image image = ContentArea.GetComponent<Image>();
            if (image == null)
            {
                Log.SignalErrorToDeveloper(
                    "Scene MultipleChoiceQuestion broken: ContentArea must have an Image component!");
                return;
            }

            myPage = gameObject.GetComponent<MultipleChoiceQuestionController>().mcqPage;

            if (myPage == null || string.IsNullOrEmpty(myPage.BackGroundImage))
            {
                // NO Background Image given => 
                // - we use standard bg color:
                image.color = Config.Current.contentBackgroundColor;
                image.enabled = true;

                // - and hide Background Image:
                BackgroundImage.gameObject.SetActive(false);

                // - and do not use questionBG:
                QuestionBackgroundImage.enabled = false;
            }
            else
            {
                // A Backgroudn Image is given =>
                // - we disabe normal bg image:
                image.enabled = false;

                // - we do use questionBG:
                QuestionBackgroundImage.enabled = true;
                QuestionBackgroundImage.color = new Color(
                    Config.Current.contentBackgroundColor.r / 256f,
                    Config.Current.contentBackgroundColor.g / 256f,
                    Config.Current.contentBackgroundColor.b / 256f,
                    a: 200f / 256f // make question background semi transparent
                );

                // - we load Texture and set to Background Image:
                BackgroundImage.gameObject.SetActive(true);

                AbstractDownloader loader;
                if (QuestManager.Instance.MediaStore.ContainsKey(myPage.BackGroundImage))
                {
                    QuestManager.Instance.MediaStore.TryGetValue(myPage.BackGroundImage, out var mediaInfo);
                    loader = new LocalFileLoader(mediaInfo.LocalPath);
                }
                else
                {
                    loader =
                        new Downloader(
                            url: myPage.BackGroundImage,
                            timeout: Config.Current.timeoutMS,
                            maxIdleTime: Config.Current.maxIdleTimeMS
                        );
                    // TODO store the image locally ...
                }

                loader.OnSuccess += (AbstractDownloader d, DownloadEvent e) =>
                {
                    var fitter = BackgroundImage.gameObject.GetComponent<AspectRatioFitter>();
                    fitter.aspectRatio = (float) d.Www.texture.width / (float) d.Www.texture.height;
                    BackgroundImage.texture = d.Www.texture;
                };
                loader.Start();
            }
        }
    }
}