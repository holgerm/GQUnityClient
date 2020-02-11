//#define DEBUG_LOG

using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.Model.expressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.parts.imageWithRights
{

    public class Rights : MonoBehaviour
	{
		public Image copyrightTextBG4FittingImage;
		public TextMeshProUGUI copyrightText4FittingImage;
		public Image copyrightTextBG4EnvelopingImage;
		public TextMeshProUGUI copyrightText4EnvelopingImage;

		private const string COPYRIGHT_PREFIX = "© ";

		void Start ()
		{
			if (copyrightText4FittingImage == null || copyrightText4EnvelopingImage == null) {
				Log.SignalErrorToDeveloper ("Rights component not set properly. Set both Text references.");
				return;
			}

			RectTransform rt = GetComponent<RectTransform> ();

            float yDelta = copyrightText4FittingImage.fontSize * 1.45f;
#if DEBUG_LOG
            Debug.Log("DELTA: " + yDelta);
#endif


            if (ConfigurationManager.Current.fitExceedingImagesIntoArea) {
				copyrightText4FittingImage.gameObject.SetActive (true);
				copyrightText4EnvelopingImage.gameObject.SetActive (false);

				if (rt != null) {
					copyrightTextBG4FittingImage.rectTransform.sizeDelta = 
						new Vector2 (-5, yDelta);
                    copyrightText4FittingImage.rectTransform.sizeDelta = new Vector2(0, 0);
				}
			} else {
				copyrightTextBG4FittingImage.gameObject.SetActive (false);
				copyrightTextBG4EnvelopingImage.gameObject.SetActive (true);

				if (rt != null) {
					copyrightTextBG4EnvelopingImage.rectTransform.sizeDelta = 
						new Vector2 (-5, yDelta);
                    copyrightText4EnvelopingImage.rectTransform.sizeDelta = new Vector2(0, 0);
                }
            }


			switch (ConfigurationManager.Current.id) {
			case "wcc":
				start_wcc ();
				break;
			case "slsspiele":
				start_slsspiele ();
				break;
			default:
				// disable the Rights component and deactivate its gameobjects:
				copyrightTextBG4FittingImage.gameObject.SetActive (false);
				copyrightTextBG4EnvelopingImage.gameObject.SetActive (false);
				enabled = false;
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

		void start_slsspiele ()
		{
			// for slsspiel Product:

			/// get copyright text from variable value:
			string copyright = Variables.GetValue ("bildrechte").AsString ();

			// normalize copyright text:
			if (copyright == "") {
				copyright = "© SLS";
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
