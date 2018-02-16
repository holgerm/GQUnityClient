using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Conf;
using System;
using GQ.Client.Err;

namespace GQ.Client.UI
{

	/// <summary>
	/// Configures the header layout based on the seetings in the current apps config data. Attach this script to all header game objects.
	/// </summary>
	[RequireComponent (typeof(Image)), RequireComponent (typeof(LayoutElement))]
	public class MenuLayoutConfig : ScreenLayoutConfig
	{

		public Image MenuBackgroundImage;

		protected override void layout ()
		{
			// set menu background color:
			if (MenuBackgroundImage != null) {
				MenuBackgroundImage.color = ConfigurationManager.Current.menuBGColor;
			}
	
			setContentHeight ();
		}

		public static void SetEntryHeight (GameObject menuEntry, string gameObjectPath = null)
		{
			// set layout height:
			Transform t = (gameObjectPath == null ? menuEntry.transform : menuEntry.transform.Find (gameObjectPath));
			if (t != null) {
				LayoutElement layElem = t.GetComponent<LayoutElement> ();
				if (layElem != null) {
					layElem.minHeight = LayoutConfig.Units2Pixels (LayoutConfig.MenuEntryHeightUnits);
				}
			} else {
				Log.SignalErrorToDeveloper ("In gameobject {0} path {1} did not lead to another gameobject.", menuEntry.gameObject, gameObjectPath);
			}
		}


	}

}