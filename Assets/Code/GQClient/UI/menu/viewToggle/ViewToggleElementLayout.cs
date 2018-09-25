using System.Collections;
using System.Collections.Generic;
using GQ.Client.Conf;
using UnityEngine;

namespace GQ.Client.UI
{
	/// <summary>
	/// A GQ view-specific layout script for toggling the view between map and list etc.
	/// </summary>
	public class ViewToggleElementLayout : LayoutConfig
	{

		/// <summary>
		/// Layouts this toggle element.
		/// </summary>
		public override void layout ()
		{
            // set heights of text and image:
            MenuLayoutConfig.SetMenuEntryLayout(gameObject, fgColor: ConfigurationManager.Current.menuBGColor);
            MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Text", fgColor: ConfigurationManager.Current.menuFGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Image", fgColor: ConfigurationManager.Current.menuFGColor);
		}
	}
}