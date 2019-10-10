using System.Collections;
using UnityEngine;
using GQ.Client.Model;
using UnityEngine.UI;
using GQ.Client.Util;
using GQ.Client.Conf;

namespace GQ.Client.UI
{

    public class StartAndExitScreenController : PageController
	{

		#region Inspector Fields
		public RawImage image;
		#endregion

		#region Other Fields
		protected PageStartAndExitScreen myPage;
		#endregion


		#region Runtime API
		public override void InitPage_TypeSpecific ()
        {
            myPage = (PageStartAndExitScreen)page;

            showImage();
            initForwardButton();
        }

        private void initForwardButton()
        {
            if (myPage.Duration == 0)
            {
                // interactive mode => show forwrd button:
                FooterButtonPanel.transform.parent.gameObject.SetActive(true);
                layout.ContentArea.GetComponent<Button>().enabled = true;
                return;
            } else
            {
                // timed mode => hide forward button, disable touch on whole screen and start timer:
                FooterButtonPanel.transform.parent.gameObject.SetActive(false);
                layout.ContentArea.GetComponent<Button>().enabled = false;
                //forwardButton.gameObject.SetActive(false);
                CoroutineStarter.Run(forwardAfterDurationWaited());
            }
        }

        private bool tapped = false;

        public void Tap()
        {
            myPage.Tap();
            tapped = true;
        }

        private IEnumerator forwardAfterDurationWaited()
        {
            yield return new WaitForSeconds(myPage.Duration);

            // in case we exited the page by tapping or the quest by leave-button in the meantime we have to skip performing onForward:
            if (!tapped && QuestManager.Instance.CurrentQuest == page.Quest)
            {
                OnForward();
            }
        }

        private void showImage()
        {
            // show (or hide completely) image:
            GameObject imagePanel = image.transform.parent.gameObject;

            // allow for variables inside the image url:
            string rtImageUrl = myPage.ImageUrl.MakeReplacements();

            if (rtImageUrl == "")
            {
                imagePanel.SetActive(false);
                return;
            }
            else
            {
                imagePanel.SetActive(true);
                AbstractDownloader loader;
                if (myPage.Parent.MediaStore.ContainsKey(rtImageUrl))
                {
                    MediaInfo mediaInfo;
                    myPage.Parent.MediaStore.TryGetValue(rtImageUrl, out mediaInfo);
                    loader = new LocalFileLoader(mediaInfo.LocalPath);
                }
                else
                {
                    loader =
                        new Downloader(
                        url: rtImageUrl,
                        timeout: ConfigurationManager.Current.timeoutMS,
                        maxIdleTime: ConfigurationManager.Current.maxIdleTimeMS
                    );
                    // TODO store the image locally ...
                }
                loader.OnSuccess += (AbstractDownloader d, DownloadEvent e) =>
                {
                    AspectRatioFitter fitter = image.GetComponent<AspectRatioFitter>();
                    fitter.aspectRatio = (float)d.Www.texture.width / (float)d.Www.texture.height;
                    image.texture = d.Www.texture;
                };
                loader.Start();
            }
        }
        #endregion


    }
}