using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Conf;
using System;

namespace GQ.Client.UI
{

	/// <summary>
	/// Configures the header layout based on the seetings in the current apps config data. Attach this script to all header game objects.
	/// </summary>
	[RequireComponent (typeof(Image)), RequireComponent (typeof(LayoutElement))]
	public class MenuLayoutConfig : LayoutConfig
	{

		public HeaderLayoutConfig header;
		public GameObject MenuContent;
		public Image MenuBackgroundImage;

		protected override void layout ()
		{
			// set menu background color:
			if (MenuBackgroundImage != null) {
				MenuBackgroundImage.color = ConfigurationManager.Current.menuBGColor;
			}
	
			setContentHeight ();
		}

		private void setContentHeight ()
		{
			if (MenuContent == null)
				return;

			LayoutElement layElem = MenuContent.GetComponent<LayoutElement> ();
			if (layElem == null)
				return;

			float sh = LayoutConfig.ScreenHeightUnits;
			float hh = LayoutConfig.HeaderHeightUnits;
			float ch = LayoutConfig.ContentHeightUnits;
			layElem.flexibleHeight = LayoutConfig.ScreenHeightUnits - LayoutConfig.HeaderHeightUnits;
		}
	}

}