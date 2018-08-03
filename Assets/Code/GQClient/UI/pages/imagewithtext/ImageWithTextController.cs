using UnityEngine;
using GQ.Client.Model;
using UnityEngine.UI;
using Candlelight.UI;
using GQ.Client.Util;
using System.Text.RegularExpressions;
using GQ.Client.Conf;

namespace GQ.Client.UI
{

    public class ImageWithTextController : PageController
	{

		#region Inspector Fields

		public RawImage image;
		public HyperText text;

		#endregion


		#region Runtime API

		protected PageImageWithText iwtPage;

		public override void Initialize ()
		{
			iwtPage = (PageImageWithText)page;

			// show text:
			text.text = TextHelper.Decode4HyperText (iwtPage.Text);

			// show (or hide completely) image:
			GameObject imagePanel = image.transform.parent.gameObject;
			if (iwtPage.ImageUrl == "") {
				imagePanel.SetActive (false);
				return;
			} else {
				imagePanel.SetActive (true);
				AbstractDownloader loader;
				if (iwtPage.Parent.MediaStore.ContainsKey (iwtPage.ImageUrl)) {
					MediaInfo mediaInfo;
					iwtPage.Parent.MediaStore.TryGetValue (iwtPage.ImageUrl, out mediaInfo);
					loader = new LocalFileLoader (mediaInfo.LocalPath);
				} else {
					loader = 
						new Downloader (
						url: iwtPage.ImageUrl, 
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

		public void OnLinkClicked (HyperText text, Candlelight.UI.HyperText.LinkInfo linkInfo)
		{
			string href = extractHREF (linkInfo);
			if (href != null) {
				Application.OpenURL (href);
			}
		}

		private string extractHREF (Candlelight.UI.HyperText.LinkInfo info)
		{
			string href = null;

			string pattern = @".*?href=""(?'href'[^""]*?)(?:["" \s]|$)";
			Match match = Regex.Match (info.Name, pattern);
			if (match.Success) {
				href = match.Groups ["href"].ToString ();
				if (!href.StartsWith ("http://") && !href.StartsWith ("https://")) {
					href = "http://" + href;
				}
			}
			return href;
		}

		#endregion


	}
}