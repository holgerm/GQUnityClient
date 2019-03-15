using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Util;
using GQ.Client.Conf;

namespace GQ.Client.UI
{
	public class ReadNFCController : PageController
	{
		
		#region Inspector Fields

		public RawImage image;
		public GameObject imagePanel;
		public GameObject contentPanel;
		public Text infoText;
		public Text forwardButtonText;

		#endregion


		#region Runtime API

		protected PageReadNFC myPage;

		/// <summary>
		/// Is called during Start() of the base class, which is a MonoBehaviour.
		/// </summary>
		public override void InitPage_TypeSpecific ()
		{
            myPage = (PageReadNFC)page;

			// show the content:
			showImage ();
			showInfo ();
			forwardButtonText.text = "Ok";
		}

		void showImage ()
		{
			// show (or hide completely) image:
			if (myPage.ImageUrl == "") {
				imagePanel.SetActive (false);
				return;
			} 

			AbstractDownloader loader;
			if (myPage.Parent.MediaStore.ContainsKey (myPage.ImageUrl)) {
				MediaInfo mediaInfo;
				myPage.Parent.MediaStore.TryGetValue (myPage.ImageUrl, out mediaInfo);
				loader = new LocalFileLoader (mediaInfo.LocalPath);
			} else {
				loader = new Downloader (
					url: myPage.ImageUrl, 
					timeout: ConfigurationManager.Current.timeoutMS,
					maxIdleTime: ConfigurationManager.Current.maxIdleTimeMS
				);
			}
			loader.OnSuccess += (AbstractDownloader d, DownloadEvent e) => {
				fitInAndShowImage(d.Www.texture);

				// Dispose www including it s Texture and take some logs for preformace surveillance:
				d.Www.Dispose ();
			};
			loader.Start ();
		}

		void showInfo() {
			infoText.text = 
				"Diese Funktion steht leider noch nicht zur Verfügung. Hier werden als Test die Informationen angezeigt, die in der Quest-Seite gespeichert wurden:\n\n" +
				"type:\t\t\t\t" + myPage.PageType + "\n" +
				"id:\t\t" + myPage.Id + "\n" +
				"saveToVar:\t\t" + myPage.SaveToVar + "\n" +
				"text:\t\t" + myPage.PromptText; 
		}

		void fitInAndShowImage(Texture2D texture) {
			AspectRatioFitter fitter = image.GetComponent<AspectRatioFitter> ();
			float imageRatio = (float)texture.width / (float)texture.height;
			float imageAreaHeight = ContentWidthUnits / imageRatio;  // if image fits, so we use its height (adjusted to the area):

			if (imageRatio < ImageRatioMinimum) {
				// image too high to fit:
				imageAreaHeight = ConfigurationManager.Current.imageAreaHeightMaxUnits;
			}
			if (ImageRatioMaximum < imageRatio) {
				// image too wide to fit:
				imageAreaHeight = ConfigurationManager.Current.imageAreaHeightMinUnits;
			}

			imagePanel.GetComponent<LayoutElement> ().flexibleHeight = LayoutConfig.Units2Pixels (imageAreaHeight);
			contentPanel.GetComponent<LayoutElement> ().flexibleHeight = CalculateMainAreaHeight (imageAreaHeight);

			fitter.aspectRatio = imageRatio; // i.e. the adjusted image area aspect ratio
			fitter.aspectMode = 
				ConfigurationManager.Current.fitExceedingImagesIntoArea 
				? AspectRatioFitter.AspectMode.FitInParent 
				: AspectRatioFitter.AspectMode.EnvelopeParent;

			image.texture = texture;
			imagePanel.SetActive (true);
		}

		#endregion
	}
}
