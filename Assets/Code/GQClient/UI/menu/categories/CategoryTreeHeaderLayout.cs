using System.Collections;
using System.Collections.Generic;
using GQ.Client.Conf;
using UnityEngine;

namespace GQ.Client.UI
{
	public class CategoryTreeHeaderLayout : LayoutConfig
	{

		public override void layout ()
		{
			// set heights of text and image:
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, fgColor: ConfigurationManager.Current.menuBGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Name");
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Button");
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Button/Hint", sizeScaleFactor: 0.6f);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Button/OnOff");
		}

	}
}
