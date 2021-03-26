using System.Collections;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Model.pages;
using Code.GQClient.Util;
using Code.GQClient.Util.http;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.pages.startandexitscreen
{

    public class StartAndExitScreenController : PageController
	{

		#region Inspector Fields
		public RawImage image;
		#endregion

		#region Other Fields

        private PageStartAndExitScreen _myPage;
		#endregion


		#region Runtime API
		public override void InitPage_TypeSpecific ()
        {
            _myPage = (PageStartAndExitScreen)page;

            ShowImage(_myPage.ImageUrl, image.transform.parent.gameObject, layout.TopMargin);
            InitForwardButton();
        }

        private void InitForwardButton()
        {
            if (_myPage.Duration == 0)
            {
                // interactive mode => show forwrd button:
                FooterButtonPanel.transform.parent.gameObject.SetActive(true);
                layout.ContentArea.GetComponent<Button>().enabled = true;
                return;
            }

            // timed mode => hide forward button, disable touch on whole screen and start timer:
            FooterButtonPanel.transform.parent.gameObject.SetActive(false);
            layout.ContentArea.GetComponent<Button>().enabled = false;
            //forwardButton.gameObject.SetActive(false);
            CoroutineStarter.Run(ForwardAfterDurationWaited());
        }

        private bool _tapped;

        public void Tap()
        {
            _myPage.Tap();
            _tapped = true;
        }

        private IEnumerator ForwardAfterDurationWaited()
        {
            yield return new WaitForSeconds(_myPage.Duration);

            // in case we exited the page by tapping or the quest by leave-button in the meantime we have to skip performing onForward:
            if (!_tapped && QuestManager.Instance.CurrentQuest == page.Quest)
            {
                OnForward();
            }
        }
        
        protected override void ImageDownloadCallback(AbstractDownloader d, DownloadEvent e)
        {
            var fitter = image.GetComponent<AspectRatioFitter>();
            fitter.aspectRatio = d.Www.texture.width / (float) d.Www.texture.height;
            image.texture = d.Www.texture;

            // Dispose www including it s Texture and take some logs for performance surveillance:
            d.Www.Dispose();
        }

        public override void CleanUp() {
            Destroy(image.texture);
        }

        #endregion


    }
}