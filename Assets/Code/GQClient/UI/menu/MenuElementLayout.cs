using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QM.UI;
using GQ.Client.Conf;

namespace GQ.Client.UI
{

	public class MenuElementLayout : LayoutConfig
	{
		/// <summary>
		/// Sets the height of this multi toggle button as menu entry.
		/// </summary>
		public override void layout ()
		{
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, fgColor: ConfigurationManager.Current.menuBGColor);
		}

	}
}