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
			MenuLayoutConfig.SetEntryHeight (gameObject);
			MenuLayoutConfig.SetEntryHeight (gameObject, "Name");
			MenuLayoutConfig.SetEntryHeight (gameObject, "Button");
			MenuLayoutConfig.SetEntryHeight (gameObject, "Button/Hint", sizeScaleFactor: 0.6f);
			MenuLayoutConfig.SetEntryHeight (gameObject, "Button/OnOff");
		}

	}
}
