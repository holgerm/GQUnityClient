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
            MenuLayoutConfig.SetMenuEntryLayout (gameObject, fgColor: ConfigurationManager.Current.categoryEntryBGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "FolderImage", sizeScaleFactor: CategoryFolderLayout.FolderImageScaleFactor, fgColor: ConfigurationManager.Current.menuFGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Name", fgColor: ConfigurationManager.Current.menuFGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Number", fgColor: ConfigurationManager.Current.menuFGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Symbol", fgColor: ConfigurationManager.Current.menuFGColor);
		}

	}
}
