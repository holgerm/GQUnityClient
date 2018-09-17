using System.Collections;
using System.Collections.Generic;
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
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Text");
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Image");
		}
	}
}