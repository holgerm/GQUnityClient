using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QM.UI;

namespace GQ.Client.UI
{

	[RequireComponent (typeof(MultiToggleButton))]
	public class ViewToggleLayout : LayoutConfig
	{
		/// <summary>
		/// Sets the height of this multi toggle button as menu entry.
		/// </summary>
		public override void layout ()
		{
			MenuLayoutConfig.SetMenuEntryHeight (gameObject);
		}

	}
}