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
		public Text copyrightText4FittingImage;
		public Text copyrightText4EnvelopingImage;

		private const string COPYRIGHT_PREFIX = "© ";

		void Start ()
		{
			if (copyrightText4FittingImage == null || copyrightText4EnvelopingImage == null) {
				Log.SignalErrorToDeveloper ("Rights component not set properly. Set both Text references.");
				return;
			}

			copyrightText4FittingImage.gameObject.SetActive (ConfigurationManager.Current.fitExceedingImagesIntoArea);
			copyrightText4EnvelopingImage.gameObject.SetActive (!ConfigurationManager.Current.fitExceedingImagesIntoArea);

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
