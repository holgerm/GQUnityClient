using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GQ.Client.UI
{
	public class CategoryFolderLayout : LayoutConfig
	{
		public const float FolderImageScaleFactor = 0.65f;

		public override void layout ()
		{
			// set heights of text and image:
			MenuLayoutConfig.SetMenuEntryHeight (gameObject);
			MenuLayoutConfig.SetMenuEntryHeight (gameObject, "FolderImage", sizeScaleFactor: FolderImageScaleFactor);
			MenuLayoutConfig.SetMenuEntryHeight (gameObject, "Name");
			MenuLayoutConfig.SetMenuEntryHeight (gameObject, "Number");
		}

	}
}
