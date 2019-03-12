using GQ.Client.Conf;
using GQ.Client.Err;
using GQ.Client.Model;
using GQ.Client.Util;
using UnityEngine;
using UnityEngine.UI;

namespace GQ.Client.UI
{

    [RequireComponent(typeof(MultipleChoiceQuestionController))]
    public class MultipleChoiceQuestionLayout : PageLayout
    {
        public RawImage BackgroundImage;

        PageMultipleChoiceQuestion myPage;

        protected override void setMainBackground()
        {
            Image image = ContentArea.GetComponent<Image>();
            if (image == null)
            {
                Log.SignalErrorToDeveloper("Scene MultipleChoiceQuestion broken: ContentArea must have an Image component!");
                return;
            }

            myPage = gameObject.GetComponent<MultipleChoiceQuestionController>().mcqPage;

            if (string.IsNullOrEmpty(myPage.BackGroundImage))
            {
                // NO Background Image given => 
                // - we use standard bg color:
                image.color = ConfigurationManager.Current.contentBackgroundColor;
                image.enabled = true;
                // - and hide Background Image:
                BackgroundImage.gameObject.SetActive(false);
            }
            else
            {
                // A Backgroudn Image is given =>
                // - we disabe normal bg image:
                image.enabled = false;
                // - we load Texture and set to Background Image:
                BackgroundImage.gameObject.SetActive(true);

                AbstractDownloader loader;
                if (myPage.Parent.MediaStore.ContainsKey(myPage.BackGroundImage))
                {
                    MediaInfo mediaInfo;
                    myPage.Parent.MediaStore.TryGetValue(myPage.BackGroundImage, out mediaInfo);
                    loader = new LocalFileLoader(mediaInfo.LocalPath);
                }
                else
                {
                    loader =
                        new Downloader(
                        url: myPage.BackGroundImage,
                        timeout: ConfigurationManager.Current.timeoutMS,
                        maxIdleTime: ConfigurationManager.Current.maxIdleTimeMS
                    );
                    // TODO store the image locally ...
                }
                loader.OnSuccess += (AbstractDownloader d, DownloadEvent e) =>
                {
                    AspectRatioFitter fitter = BackgroundImage.gameObject.GetComponent<AspectRatioFitter>();
                    fitter.aspectRatio = (float)d.Www.texture.width / (float)d.Www.texture.height;
                    BackgroundImage.texture = d.Www.texture;
                };
                loader.Start();
            }
        }
    }
}
