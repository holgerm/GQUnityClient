using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;
using UnityEngine.UI;
using Candlelight.UI;
using GQ.Client.Util;
using GQ.Client.Err;
using System.Text.RegularExpressions;

namespace GQ.Client.UI
{
	
	public class NPCTalkController : PageController
	{

		#region Inspector Fields

		public RawImage image;
		public HyperText text;

		#endregion

		#region Other Fields

		protected PageNPCTalk npcPage;

		#endregion


		#region Runtime API

		//		// Use this for initialization
		//		public override void Start ()
		//		{
		//			base.Start ();
		//
		//			if (page == null)
		//				return;
		//
		//			Initialize ();
		//		}

		public override void Initialize ()
		{
			npcPage = (PageNPCTalk)page;

			// show text:
			text.text = TextHelper.Decode4HyperText (npcPage.CurrentDialogItem.Text);

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
					loader = new Downloader (npcPage.ImageUrl);
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
			Debug.Log ("### name = " + linkInfo.Name);
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

		#endregion

	}
}