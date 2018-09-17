using System.Collections;
using System.Collections.Generic;
using GQ.Client.Conf;
using UnityEngine;

namespace GQ.Client.UI
{

	public class CategoryEntryLayout : LayoutConfig
	{

		public override void layout ()
		{
			// set heights of text and image:
            MenuLayoutConfig.SetMenuEntryLayout (gameObject, fgColor: ConfigurationManager.Current.menuBGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "FolderImage", sizeScaleFactor: CategoryFolderLayout.FolderImageScaleFactor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Name");
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Number");
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Symbol");
		}

	}
}
