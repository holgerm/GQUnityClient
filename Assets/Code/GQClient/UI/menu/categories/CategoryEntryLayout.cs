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
			MenuLayoutConfig.SetMenuEntryHeight (gameObject);
			MenuLayoutConfig.SetMenuEntryHeight (gameObject, "FolderImage", sizeScaleFactor: CategoryFolderLayout.FolderImageScaleFactor);
			MenuLayoutConfig.SetMenuEntryHeight (gameObject, "Name");
			MenuLayoutConfig.SetMenuEntryHeight (gameObject, "Number");
			MenuLayoutConfig.SetMenuEntryHeight (gameObject, "Symbol");
		}

	}
}
