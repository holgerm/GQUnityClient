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


		#region Other Fields

		protected PageNPCTalk npcPage;

		#endregion


		#region Runtime API

		/// <summary>
		/// Is called during Start() of the base class, which is a MonoBehaviour.
		/// </summary>
		public override void Initialize ()
		{
			npcPage = (PageNPCTalk)page;

			// show the content:
			ShowImage ();
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


		#region Layout

		public override int NumberOfSpacesInContent ()
		{
			return Math.Max (npcPage.NumberOfDialogItems () - 1, 0);
		}

		protected float ContentImageHeight {
			get {
				return LayoutConfig.ScreenHeightUnits - (LayoutConfig.HeaderHeightUnits + LayoutConfig.FooterHeightUnits + ContentDividerUnits);
			}
		}

		#endregion


		#region View Update Methods

		void ShowImage ()
		{
			// show (or hide completely) image:
			GameObject imagePanel = image.transform.parent.gameObject;
			if (npcPage.ImageUrl == "") {
				imagePanel.SetActive (false);
				return;
			} else {
				imagePanel.SetActive (true);
				AbstractDownloader loader;
				if (npcPage.Parent.MediaStore.ContainsKey (npcPage.ImageUrl)) {
					MediaInfo mediaInfo;
					npcPage.Parent.MediaStore.TryGetValue (npcPage.ImageUrl, out mediaInfo);
					loader = new LocalFileLoader (mediaInfo.LocalPath);
				} else {
					loader = new Downloader (url: npcPage.ImageUrl, timeout: ConfigurationManager.Current.timeoutMS);
					// TODO store the image locally ...
				}
				loader.OnSuccess += (AbstractDownloader d, DownloadEvent e) => {
					AspectRatioFitter fitter = image.GetComponent<AspectRatioFitter> ();
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
					Debug.Log ("IMAGE Ratios: max: " + ImageRatioMinimum + " real Ration: " + imageRatio + " max: " + ImageRatioMaximum);
					Debug.Log ("IMAGE AREA HEIGHT: " + imageAreaHeight + " WIDTH: " + ContentWidthUnits);
					Debug.Log ("SCREEN HEIGHT: " + LayoutConfig.ScreenHeightUnits + " WIDTH: " + LayoutConfig.ScreenWidthUnits);

					imagePanel.GetComponent<LayoutElement> ().flexibleHeight = imageAreaHeight;
					contentPanel.GetComponent<LayoutElement> ().flexibleHeight = CalculateMainAreaHeight (imageAreaHeight);

					fitter.aspectRatio = imageRatio; // i.e. the adjusted image area aspect ratio
					fitter.aspectMode = 
						ConfigurationManager.Current.fitExceedingImagesIntoArea 
						? AspectRatioFitter.AspectMode.FitInParent 
						: AspectRatioFitter.AspectMode.EnvelopeParent;

					image.texture = d.Www.texture;
					// Dispose www including it s Texture and take some logs for preformace surveillance:
					d.Www.Dispose ();
				};
				loader.Start ();
			}
		}

		void AddCurrentText ()
		{
			// decode text for HyperText Component:
			string currentText = TextHelper.Decode4HyperText (npcPage.CurrentDialogItem.Text);

			// create dialog item GO form prefab:
			Debug.Log ("TODO: Add dialog Item: " + currentText);
			DialogItemCtrl.Create (dialogItemContainer, currentText);
		}

		void UpdateForwardButton ()
		{
			// update forward button text:
			forwardButtonText.text = npcPage.HasMoreDialogItems () ? npcPage.NextDialogButtonText : npcPage.EndButtonText;
		}

		#endregion

	}

}