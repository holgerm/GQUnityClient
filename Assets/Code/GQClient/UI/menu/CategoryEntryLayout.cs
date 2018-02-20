using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GQ.Client.UI
{

	public class CategoryEntryLayout : LayoutConfig
	{

		public override void layout ()
		{
			// set heights of text and image:
			MenuLayoutConfig.SetEntryHeight (gameObject);
			MenuLayoutConfig.SetEntryHeight (gameObject, "FolderImage");
			MenuLayoutConfig.SetEntryHeight (gameObject, "Name");
			MenuLayoutConfig.SetEntryHeight (gameObject, "Number");
			MenuLayoutConfig.SetEntryHeight (gameObject, "Symbol");
		}

	}
}
