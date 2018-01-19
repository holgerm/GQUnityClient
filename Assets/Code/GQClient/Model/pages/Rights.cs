using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.UI;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Conf;

namespace GQ.Client.Model
{

	public class Rights : MonoBehaviour
	{
		Text copyrightText;

		private const string COPYRIGHT_PREFIX = "© ";

		void Start ()
		{
			switch (ConfigurationManager.Current.id) {
			case "wcc":
				start_wcc ();
				break;
			case "q3traunreutguide":
				start_q3traunreutguide ();
				break;
			default:
				gameObject.SetActive (false);
				break;
			}	
		}

		void start_wcc() {
			// FOR WCC PRODUCT:
			Reset ();

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
			copyright.Replace('\n', ' ');

			// set copyright text in game object:
			copyrightText.text = copyright;
		}


		void start_q3traunreutguide() {
			// FOR WCC PRODUCT:
			Reset ();

			/// get copyright text from variable value:
			string copyright = Variables.GetValue ("bildrechte").AsString ();

			// normalize copyright text:
			if (copyright == "") {
				gameObject.SetActive (false);
				return;
			} else {
				if (copyright.StartsWith ("©")) {
					copyright = copyright.Substring (1);
					copyright.TrimStart (' ');
				} else {
					copyright = COPYRIGHT_PREFIX + copyright;
				}
			}

			// replace andy line breaks by one space:
			copyright.Replace('\n', ' ');

			// set copyright text in game object:
			copyrightText.text = copyright;
		}
		void Reset() {
			copyrightText = GetComponent<Text> ();
		}

	}
}
