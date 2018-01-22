using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Conf;
using System;
using GQ.Client.Err;

namespace GQ.Client.UI
{
	
	// TODO move to ScreenLayout.

	/// <summary>
	/// Configures the header layout based on the seetings in the current apps config data. Attach this script to all header game objects.
	/// </summary>
	[RequireComponent (typeof(Image)), RequireComponent (typeof(LayoutElement))]
	public class HeaderLayoutConfig : LayoutConfig
	{

		/// <summary>
		/// Onyl layout sizes, configure no media etc. This is used for invisble placeholders that have no logos etc.
		/// </summary>
		public bool layoutOnlyNoMedia = false;

		protected override void layout ()
		{
			// set background color:
			Image image = GetComponent<Image> ();
			if (image != null) {
				image.color = ConfigurationManager.Current.headerBgColor;
			}

			// set height:
			LayoutElement layElem = GetComponent<LayoutElement> ();
			if (layElem != null) {
				layElem.flexibleHeight = PageController.HeaderHeight;
			}

			if (layoutOnlyNoMedia)
				return;

			// here follows media stuff like logos etc.:
			
			// set MiddleTopLogo:
			try {
				Transform middleTopLogo = transform.Find ("ButtonPanel/MiddleTopLogo");
				Image mtlImage = middleTopLogo.GetComponent<Image> ();
				mtlImage.sprite = Resources.Load<Sprite> ("TopLogo");
			} catch (Exception e) {
				Log.SignalErrorToDeveloper ("Could not set Middle Top Logo Image. Exception occurred: " + e.Message);
			}
		}
	}

}
