using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Conf;
using System;

namespace GQ.Client.UI
{

	/// <summary>
	/// Configures the header layout based on the seetings in the current apps config data. Attach this script to all header game objects.
	/// </summary>
	[RequireComponent (typeof(Image)), RequireComponent (typeof(LayoutElement))]
	public class MenuLayoutConfig : ScreenLayoutConfig
	{

		public Image MenuBackgroundImage;

		protected override void layout ()
		{
			// set menu background color:
			if (MenuBackgroundImage != null) {
				MenuBackgroundImage.color = ConfigurationManager.Current.menuBGColor;
			}
	
			setContentHeight ();
		}

	}

}