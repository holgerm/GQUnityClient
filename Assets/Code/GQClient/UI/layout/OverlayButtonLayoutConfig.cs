using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Conf;
using System;

namespace GQ.Client.UI {

	/// <summary>
	/// Configures the layout for overlay buttons, e.g. on a map, based on the settings in the current apps config data. 
	/// Attach this script to all overlying button game objects.
	/// </summary>
	[RequireComponent (typeof(Image)), RequireComponent (typeof(LayoutElement))]
	public class OverlayButtonLayoutConfig : LayoutConfig {

		private bool enabled;
		public bool Enabled {
			get {
				return enabled;
			}
			set {
				if (enabled != value) {
					enabled = value;
					Button button = GetComponent<Button> ();
					button.enabled = enabled;
					layout ();
				}
			}
		}

		protected void Start() {
			base.Start ();

			Enabled = true;
		}
			
		protected override void layout() {
			// set background color:
			Image bgImage = GetComponent<Image> ();
			if (bgImage != null) {
				bgImage.color = ConfigurationManager.Current.overlayButtonBgColor;
			}

			// set foreground color in Image:
			try {
				Image fgImage = transform.Find("Image").GetComponent<Image> ();
				if (fgImage != null) {
					fgImage.color = Enabled ? ConfigurationManager.Current.overlayButtonFgColor : ConfigurationManager.Current.overlayButtonFgDisabledColor;
				}
			}
			catch (Exception) {}	

			// set foreground color as font color in Text:
			try {
				Text fgText = transform.Find("Text").GetComponent<Text> ();
				if (fgText != null) {
					fgText.color = Enabled ? ConfigurationManager.Current.overlayButtonFgColor : ConfigurationManager.Current.overlayButtonFgDisabledColor;
				}
			}
			catch (Exception) {}
		}
	}

}