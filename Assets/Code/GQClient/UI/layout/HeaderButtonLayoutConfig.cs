using System;
using Code.GQClient.Conf;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.layout
{

	/// <summary>
	/// Configures the header layout based on the seetings in the current apps config data. Attach this script to all header game objects.
	/// </summary>
	[RequireComponent (typeof(Image)), RequireComponent (typeof(LayoutElement))]
	public class HeaderButtonLayoutConfig : LayoutConfig
	{
        // TODO: This empty Start method seems necessary to allow for adapting layout when
        // e.g. a webview is shown and this button should be disabled and greyed out.
        // I do not know why!!??
        public void Start()
        {
            //Debug.Log("HeaderButtonLayoutConfig.Start() on " + name);
        }

        public override void layout ()
		{
			// set background color:
			Image bgImage = GetComponent<Image> ();
			if (bgImage != null) {
				bgImage.color = Config.Current.headerButtonBgColor;
			}

			// set foreground color in Image:
			try {
				Image fgImage = transform.Find ("Image").GetComponent<Image> ();
				if (fgImage != null) {
					fgImage.color = Config.Current.headerButtonFgColor;
				}
			} catch (Exception) {
			}	

			// set foreground color as font color in Text:
			try {
				Text fgText = transform.Find ("Text").GetComponent<Text> ();
				if (fgText != null) {
					fgText.color = Config.Current.headerButtonFgColor;
				}
			} catch (Exception) {
			}	
		}
	}

}
