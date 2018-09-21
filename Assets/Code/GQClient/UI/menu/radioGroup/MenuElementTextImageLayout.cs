using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GQ.Client.UI
{

	public class MenuElementTextImageLayout : MenuElementLayout
	{

		public override void layout ()
		{
			base.layout ();

			// set heights of text and image:
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Text");
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Image");
		}

	}
}
