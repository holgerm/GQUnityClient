using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Candlelight.UI;
using System.Text.RegularExpressions;
using GQ.Client.Util;

namespace GQ.Client.UI
{

	public class HypertextchunkCtrl : MonoBehaviour {

		#region Unity Inspektor

		public HyperText DialogItemHyperText;

		public void OnLinkClicked (HyperText text, Candlelight.UI.HyperText.LinkInfo linkInfo)
		{
			string href = extractHREF (linkInfo);
			if (href != null) {
				Application.OpenURL (href);
			}
		}

		#endregion


		public void Initialize(string itemText) {
			this.DialogItemHyperText.text = itemText.MakeReplacements();
		}

		public static HypertextchunkCtrl Create(Transform rootTransform, string text) {
			GameObject go = (GameObject)Instantiate (
				Resources.Load ("HypertextChunk"),
				rootTransform,
				false
			);
			go.SetActive (true);

			HypertextchunkCtrl diCtrl = go.GetComponent<HypertextchunkCtrl> ();
			diCtrl.Initialize (text);

			return diCtrl;
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
	}

}
