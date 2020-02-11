using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Conf;

namespace GQ.Client.UI.Dialogs
{
	/// <summary>
	/// Should be applied to the Dialog Prefab, which is placed at runtime into the DialogCanvas.
	/// </summary>
	public class DialogLayout : LayoutConfig
	{
        public Image TopLogoImage;

		public override void layout ()
		{
			// set frame color (implemented as background of the enveloping dialog panel):
			Image image = GetComponent<Image> ();
			if (image != null) {
				image.color = ConfigurationManager.Current.mainFgColor;
			}

			// set content background color:
			Transform contentPanelT = transform.Find ("Panel");
			if (contentPanelT == null)
				return;

			Image contentImage = contentPanelT.GetComponent<Image> ();
			if (contentImage != null) {
				contentImage.color = ConfigurationManager.Current.contentBackgroundColor;
			}

            TopLogoImage.sprite = Resources.Load<Sprite>(ConfigurationManager.Current.topLogo.path);
		}
	}
}