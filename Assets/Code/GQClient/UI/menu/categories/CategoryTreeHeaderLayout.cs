using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GQ.Client.UI
{
	public class CategoryTreeHeaderLayout : LayoutConfig
	{

		public override void layout ()
		{
			// set heights of text and image:
			MenuLayoutConfig.SetMenuEntryHeight (gameObject);
			MenuLayoutConfig.SetMenuEntryHeight (gameObject, "Name");
			MenuLayoutConfig.SetMenuEntryHeight (gameObject, "Button");
			MenuLayoutConfig.SetMenuEntryHeight (gameObject, "Button/Hint", sizeScaleFactor: 0.6f);
			MenuLayoutConfig.SetMenuEntryHeight (gameObject, "Button/OnOff");
		}

	}
}
