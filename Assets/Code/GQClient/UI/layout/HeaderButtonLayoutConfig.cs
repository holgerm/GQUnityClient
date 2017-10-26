using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Conf;
using System;

namespace GQ.Client.UI {

	/// <summary>
	/// Configures the header layout based on the seetings in the current apps config data. Attach this script to all header game objects.
	/// </summary>
	[RequireComponent (typeof(Image)), RequireComponent (typeof(LayoutElement))]
	public class HeaderButtonLayoutConfig : LayoutConfig {

		protected override void layout() {
			// set background color:
			Image bgImage = GetComponent<Image> ();
			if (bgImage != null) {
				bgImage.color = ConfigurationManager.Current.headerButtonBgColor;
			}

			// set foreground color in Image:
			try {
				Image fgImage = transform.Find("Image").GetComponent<Image> ();
				if (fgImage != null) {
					fgImage.color = ConfigurationManager.Current.headerButtonFgColor;
				}
			}
			catch (Exception) {}	

			// set foreground color as font color in Text:
			try {
				Text fgText = transform.Find("Text").GetComponent<Text> ();
				if (fgText != null) {
					fgText.color = ConfigurationManager.Current.headerButtonFgColor;
				}
			}
			catch (Exception) {}	
		}
	}

}
