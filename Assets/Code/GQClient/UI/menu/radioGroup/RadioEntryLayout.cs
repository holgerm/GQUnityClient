using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GQ.Client.UI
{

	public class RadioEntryLayout : MenuElementLayout
	{

		public override void layout ()
		{
			base.layout ();

			// set heights of text and image:
			MenuLayoutConfig.SetMenuEntryHeight (gameObject, "Text");
			MenuLayoutConfig.SetMenuEntryHeight (gameObject, "Image");
		}

	}
}
