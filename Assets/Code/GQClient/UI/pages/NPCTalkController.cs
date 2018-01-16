using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;
using UnityEngine.UI;
using Candlelight.UI;
using GQ.Client.Util;
using GQ.Client.Err;
using System.Text.RegularExpressions;
using GQ.Client.Conf;

namespace GQ.Client.UI
{
	
	public class NPCTalkController : PageController
	{

		#region Inspector Fields

		public RawImage image;
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
				for (int i = 0; i < match.Groups.Count; i++) {
					Debug.Log ("   #### group " + i + " : " + match.Groups [i]);
				}
				href = match.Groups ["href"].ToString ();
				if (!href.StartsWith ("http://") && !href.StartsWith ("https://")) {
					href = "http://" + href;
				}
			}
			return href;
		}

		public override void OnForward () {
			if (npcPage.HasMoreDialogItems()) {
				npcPage.Next ();
				// update the content:
				AddCurrentText ();
				UpdateForwardButton ();
			}
			else {
				npcPage.End ();
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
			}
			else {
				imagePanel.SetActive (true);
				AbstractDownloader loader;
				if (npcPage.Parent.MediaStore.ContainsKey (npcPage.ImageUrl)) {
					MediaInfo mediaInfo;
					npcPage.Parent.MediaStore.TryGetValue (npcPage.ImageUrl, out mediaInfo);
					loader = new LocalFileLoader (mediaInfo.LocalPath);
				}
				else {
					loader = new Downloader (url: npcPage.ImageUrl, timeout: ConfigurationManager.Current.timeoutMS);
					// TODO store the image locally ...
				}
				loader.OnSuccess += (AbstractDownloader d, DownloadEvent e) =>  {
					AspectRatioFitter fitter = image.GetComponent<AspectRatioFitter> ();
					fitter.aspectRatio = (float)d.Www.texture.width / (float)d.Www.texture.height;
					image.texture = d.Www.texture;
					// Dispose www including it s Texture and take some logs for preformace surveillance:
					d.Www.Dispose ();
				};
				loader.Start ();
			}
		}

		void AddCurrentText() {
			// decode text for HyperText Component:
			string currentText = TextHelper.Decode4HyperText (npcPage.CurrentDialogItem.Text);

			// create dialog item GO form prefab:
			Debug.Log("TODO: Add dialog Item: " + currentText);
			DialogItemCtrl.Create(dialogItemContainer, currentText);
		}

		void UpdateForwardButton() {
			// update forward button text:
			forwardButtonText.text = npcPage.HasMoreDialogItems() ? npcPage.NextDialogButtonText : npcPage.EndButtonText;
		}
			
		#endregion

	}

}