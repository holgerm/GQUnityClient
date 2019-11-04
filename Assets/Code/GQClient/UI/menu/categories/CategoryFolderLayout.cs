using System.Collections;
using System.Collections.Generic;
using GQ.Client.Conf;
using UnityEngine;

namespace GQ.Client.UI
{
	public class CategoryFolderLayout : LayoutConfig
	{
		public const float FolderImageScaleFactor = 0.65f;

		public override void layout ()
		{
			// set heights of text and image:
            MenuLayoutConfig.SetMenuEntryLayout (gameObject, fgColor: ConfigurationManager.Current.categoryFolderBGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "FolderImage", sizeScaleFactor: FolderImageScaleFactor, fgColor: ConfigurationManager.Current.menuFGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Name", fgColor: ConfigurationManager.Current.menuFGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Number", fgColor: ConfigurationManager.Current.menuFGColor);
		}

	}
}
