using System;
using Code.GQClient.Conf;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.layout
{

	/// <summary>
	/// Configures the layout for overlay buttons, e.g. on a map, based on the settings in the current apps config data. 
	/// Attach this script to all overlying button game objects.
	/// </summary>
	[RequireComponent (typeof(Image)), RequireComponent (typeof(LayoutElement))]
	public class OverlayButtonLayoutConfig : LayoutConfig
	{

		private bool _enabled;

		public bool Enabled {
			get {
				return _enabled;
			}
			set {
				if (_enabled != value) {
					_enabled = value;
					Button button = GetComponent<Button> ();
					button.enabled = _enabled;
					layout ();
				}
			}
		}

		protected new void Start ()
		{
			base.Start ();

			Enabled = true;
		}

		public override void layout ()
		{
			// set background color:
			Image bgImage = GetComponent<Image> ();
			if (bgImage != null) {
				bgImage.color = Config.Current.overlayButtonBgColor;
			}

			// set foreground color in Image:
			try {
				Image fgImage = transform.Find ("Image").GetComponent<Image> ();
				if (fgImage != null) {
					fgImage.color = Enabled ? Config.Current.overlayButtonFgColor : Config.Current.overlayButtonFgDisabledColor;
				}
			} catch (Exception) {
			}	

			// set foreground color as font color in Text:
			try {
				Text fgText = transform.Find ("Text").GetComponent<Text> ();
				if (fgText != null) {
					fgText.color = Enabled ? Config.Current.overlayButtonFgColor : Config.Current.overlayButtonFgDisabledColor;
				}
			} catch (Exception) {
			}
		}
	}

}