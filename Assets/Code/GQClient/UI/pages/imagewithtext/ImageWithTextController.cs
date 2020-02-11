using Code.GQClient.Conf;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Model.pages;
using Code.GQClient.Util;
using Code.GQClient.Util.http;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.pages.imagewithtext
{

    public class ImageWithTextController : PageController
	{

		#region Inspector Fields

		public RawImage image;
		public TextMeshProUGUI text;

		#endregion


		#region Runtime API

		protected PageImageWithText iwtPage;

		public override void InitPage_TypeSpecific ()
		{
            iwtPage = (PageImageWithText)page;

			// show text:
			text.text = TextHelper.Decode4TMP(iwtPage.Text);

			// show (or hide completely) image:
			GameObject imagePanel = image.transform.parent.gameObject;

            // allow for variables inside the image url:
            string rtImageUrl = iwtPage.ImageUrl.MakeReplacements();

            if (rtImageUrl == "") {
				imagePanel.SetActive (false);
				return;
			} else {
				imagePanel.SetActive (true);
				AbstractDownloader loader;
				if (iwtPage.Parent.MediaStore.ContainsKey (rtImageUrl)) {
					MediaInfo mediaInfo;
					iwtPage.Parent.MediaStore.TryGetValue (rtImageUrl, out mediaInfo);
					loader = new LocalFileLoader (mediaInfo.LocalPath);
				} else {
					loader = 
						new Downloader (
						url: rtImageUrl, 
						timeout: ConfigurationManager.Current.timeoutMS,
						maxIdleTime: ConfigurationManager.Current.maxIdleTimeMS
					);
					// TODO store the image locally ...
				}
				loader.OnSuccess += (AbstractDownloader d, DownloadEvent e) => {
					AspectRatioFitter fitter = image.GetComponent<AspectRatioFitter> ();
					fitter.aspectRatio = (float)d.Www.texture.width / (float)d.Www.texture.height;
					image.texture = d.Www.texture;
				};
				loader.Start ();
			}
		}

		#endregion


	}
}