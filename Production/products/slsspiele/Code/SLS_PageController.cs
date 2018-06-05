using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Util;
using GQ.Client.Conf;

namespace GQ.Client.UI
{

	public class SLS_PageController : PageController
	{
	
		#region Inspector Fields

		public Text titleText;
		public RawImage image;
		public GameObject imagePanel;

		#endregion

		#region Runtime API

		PageSLS_Spielbeschreibung slsPage;

		/// <summary>
		/// Is called during Start() of the base class, which is a MonoBehaviour.
		/// </summary>
		public override void Initialize ()
		{
			slsPage = (PageSLS_Spielbeschreibung)page;

			// set title:
			titleText.text = page.Quest.Name;

		
			ShowImage ();
		}
			
		void ShowImage ()
		{
			// show (or hide completely) image:
			if (slsPage.ImageUrl == "") {
				imagePanel.SetActive (false);
				return;
			} else {
				AbstractDownloader loader;
				if (slsPage.Parent.MediaStore.ContainsKey (slsPage.ImageUrl)) {
					MediaInfo mediaInfo;
					slsPage.Parent.MediaStore.TryGetValue (slsPage.ImageUrl, out mediaInfo);
					loader = new LocalFileLoader (mediaInfo.LocalPath);
				} else {
					loader = new Downloader (
						url: slsPage.ImageUrl, 
						timeout: ConfigurationManager.Current.timeoutMS,
						maxIdleTime: ConfigurationManager.Current.maxIdleTimeMS
					);
				}
				loader.OnSuccess += (AbstractDownloader d, DownloadEvent e) => {

					AspectRatioFitter fitter = image.gameObject.GetComponent<AspectRatioFitter> ();
					float imageRatio = (float)d.Www.texture.width / (float)d.Www.texture.height;
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
//					contentPanel.GetComponent<LayoutElement> ().flexibleHeight = CalculateMainAreaHeight (imageAreaHeight);

					fitter.aspectRatio = imageRatio; // i.e. the adjusted image area aspect ratio
					fitter.aspectMode = 
						ConfigurationManager.Current.fitExceedingImagesIntoArea 
						? AspectRatioFitter.AspectMode.FitInParent 
						: AspectRatioFitter.AspectMode.EnvelopeParent;

					image.texture = d.Www.texture;
					// Dispose www including it s Texture and take some logs for preformace surveillance:
					d.Www.Dispose ();
					imagePanel.SetActive (true);
				};
				loader.Start ();
			}
		}

		#endregion

	}

}
