using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;
using UnityEngine.UI;
using Candlelight.UI;
using GQ.Client.Util;
using GQ.Client.Err;
using GQ.Client.Conf;
using System;

namespace GQ.Client.UI
{
	
	public class NPCTalkController : PageController
	{

		#region Inspector Fields

		public RawImage image;
		public GameObject imagePanel;
		public GameObject contentPanel;
		public Transform dialogItemContainer;
		public Text forwardButtonText;

		#endregion

		#region Runtime API

		protected PageNPCTalk npcPage;

		/// <summary>
		/// Is called during Start() of the base class, which is a MonoBehaviour.
		/// </summary>
		public override void Initialize ()
		{
			npcPage = (PageNPCTalk)page;

			// show the content:
			ShowImage ();
			ClearText ();
			AddCurrentText ();
			UpdateForwardButton ();
		}

		public override void OnForward ()
		{
			if (npcPage.HasMoreDialogItems ()) {
				npcPage.Next ();
				// update the content:
				AddCurrentText ();
				UpdateForwardButton ();
			} else {
				npcPage.End ();
			}
		}

		#endregion

		#region View Update Methods

		void ShowImage ()
		{
			// show (or hide completely) image:
			if (npcPage.ImageUrl == "") {
				imagePanel.SetActive (false);
				return;
			} 

			AbstractDownloader loader;
			if (npcPage.Parent.MediaStore.ContainsKey (npcPage.ImageUrl)) {
				MediaInfo mediaInfo;
				npcPage.Parent.MediaStore.TryGetValue (npcPage.ImageUrl, out mediaInfo);
				loader = new LocalFileLoader (mediaInfo.LocalPath);
			} else {
				loader = new Downloader (
					url: npcPage.ImageUrl, 
					timeout: ConfigurationManager.Current.timeoutMS,
					maxIdleTime: ConfigurationManager.Current.maxIdleTimeMS
				);
			}
			loader.OnSuccess += (AbstractDownloader d, DownloadEvent e) => {
                float imageAreaHeight = fitInAndShowImage(d.Www.texture);

                imagePanel.GetComponent<LayoutElement>().flexibleHeight = LayoutConfig.Units2Pixels(imageAreaHeight);
                contentPanel.GetComponent<LayoutElement>().flexibleHeight = CalculateMainAreaHeight(imageAreaHeight);

                // Dispose www including it s Texture and take some logs for preformace surveillance:
                d.Www.Dispose ();
			};
			loader.Start ();
		}

		float fitInAndShowImage(Texture2D texture) {
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

			//imagePanel.GetComponent<LayoutElement> ().flexibleHeight = LayoutConfig.Units2Pixels (imageAreaHeight);
			//contentPanel.GetComponent<LayoutElement> ().flexibleHeight = CalculateMainAreaHeight (imageAreaHeight);

			fitter.aspectRatio = imageRatio; // i.e. the adjusted image area aspect ratio
			fitter.aspectMode = 
				ConfigurationManager.Current.fitExceedingImagesIntoArea 
				? AspectRatioFitter.AspectMode.FitInParent 
				: AspectRatioFitter.AspectMode.EnvelopeParent;

			image.texture = texture;
			imagePanel.SetActive (true);

            return imageAreaHeight;

        }

		void ClearText ()
		{
			foreach (Transform dialogItem in dialogItemContainer) {
				GameObject.Destroy (dialogItem.gameObject);
			}

            Image bgImg = contentPanel.GetComponent<Image>();
            if (bgImg != null) {
                bgImg.color = ConfigurationManager.Current.mainBgColor;
            }
		}

		void AddCurrentText ()
		{
			// decode text for HyperText Component:
			string currentText = npcPage.CurrentDialogItem.Text.Decode4HyperText();

			// create dialog item GO from prefab:
			HypertextchunkCtrl.Create (dialogItemContainer, currentText);
		}

		void UpdateForwardButton ()
		{
			// update forward button text:
			forwardButtonText.text = npcPage.HasMoreDialogItems () ? npcPage.NextDialogButtonText : npcPage.EndButtonText;
		}

		#endregion
	}

}