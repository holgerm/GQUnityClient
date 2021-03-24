using Code.GQClient.Conf;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Model.pages;
using Code.GQClient.Util;
using Code.GQClient.Util.http;
using TMPro;
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

		private PageImageWithText _iwtPage;

		public override void InitPage_TypeSpecific ()
		{
            _iwtPage = (PageImageWithText)page;

			// show text:
			text.text = _iwtPage.Text.Decode4TMP();

			// show (or hide completely) image:
			var imagePanel = image.transform.parent.gameObject;

            // allow for variables inside the image url:
            var rtImageUrl = _iwtPage.ImageUrl.MakeReplacements();

            if (rtImageUrl == "") {
				imagePanel.SetActive (false);
				return;
			} else {
				imagePanel.SetActive (true);
				AbstractDownloader loader;
				if (QuestManager.Instance.MediaStore.ContainsKey (rtImageUrl)) {
					MediaInfo mediaInfo;
					QuestManager.Instance.MediaStore.TryGetValue (rtImageUrl, out mediaInfo);
					loader = new LocalFileLoader (mediaInfo.LocalPath);
				} else {
					loader = 
						new Downloader (
						url: rtImageUrl, 
						timeout: Config.Current.timeoutMS,
						maxIdleTime: Config.Current.maxIdleTimeMS
					);
					// TODO store the image locally ...
				}
				loader.OnSuccess += (AbstractDownloader d, DownloadEvent e) => {
					var fitter = image.GetComponent<AspectRatioFitter> ();
					fitter.aspectRatio = (float)d.Www.texture.width / (float)d.Www.texture.height;
					image.texture = d.Www.texture;
				};
				loader.Start ();
			}
		}
		
		public override void CleanUp() {
			Destroy(image.texture);
			// Destroy(text);
		}

		#endregion


	}
}