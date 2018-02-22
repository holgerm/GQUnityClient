using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.UI;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Conf;
using GQ.Client.Err;

namespace GQ.Client.Model
{

	public class Rights : MonoBehaviour
	{
		public Image copyrightTextBG4FittingImage;
		public Text copyrightText4FittingImage;
		public Image copyrightTextBG4EnvelopingImage;
		public Text copyrightText4EnvelopingImage;

		private const string COPYRIGHT_PREFIX = "© ";

		void Start ()
		{
			if (copyrightText4FittingImage == null || copyrightText4EnvelopingImage == null) {
				Log.SignalErrorToDeveloper ("Rights component not set properly. Set both Text references.");
				return;
			}

			RectTransform rt = GetComponent<RectTransform> ();

			if (ConfigurationManager.Current.fitExceedingImagesIntoArea) {
				copyrightText4FittingImage.gameObject.SetActive (true);
				copyrightText4EnvelopingImage.gameObject.SetActive (false);

				if (rt != null) {
					copyrightTextBG4FittingImage.rectTransform.sizeDelta = 
						new Vector2 (0, copyrightText4FittingImage.fontSize * 1.45f);
//					copyrightText4FittingImage.rectTransform.sizeDelta = 
//						new Vector2 (0, copyrightText4FittingImage.fontSize * 1.2f);
				}
			} else {
				copyrightTextBG4FittingImage.gameObject.SetActive (false);
				copyrightTextBG4EnvelopingImage.gameObject.SetActive (true);

				if (rt != null) {
					copyrightTextBG4EnvelopingImage.rectTransform.sizeDelta = 
						new Vector2 (0, copyrightText4EnvelopingImage.fontSize * 1.45f);
//					copyrightText4EnvelopingImage.rectTransform.sizeDelta = 
//						new Vector2 (0, copyrightText4EnvelopingImage.fontSize * 1.2f);
				}
			}


			switch (ConfigurationManager.Current.id) {
			case "wcc":
				start_wcc ();
				break;
			default:
				gameObject.SetActive (false);
				break;
			}	
		}

		void start_wcc ()
		{
			// FOR WCC PRODUCT:

			/// get copyright text from variable value:
			string copyright = Variables.GetValue ("bildrechte").AsString ();

			// normalize copyright text:
			if (copyright == "") {
				copyright = "© WCC";
			} else {
				if (copyright.StartsWith ("©")) {
					copyright = copyright.Substring (1);
					copyright.TrimStart (' ');
				} else {
					copyright = COPYRIGHT_PREFIX + copyright;
				}
			}

			// replace andy line breaks by one space:
			copyright.Replace ('\n', ' ');

			// set copyright text in game objects:
			copyrightText4FittingImage.text = copyright;
			copyrightText4EnvelopingImage.text = copyright;
		}


	}
}
