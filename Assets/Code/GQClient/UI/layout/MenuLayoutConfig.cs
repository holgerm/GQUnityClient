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

		public override void layout ()
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
			Transform transf = (gameObjectPath == null ? menuEntry.transform : menuEntry.transform.Find (gameObjectPath));
			if (transf != null) {
				LayoutElement layElem = transf.GetComponent<LayoutElement> ();
				if (layElem != null) {
					layElem.minHeight = LayoutConfig.Units2Pixels (LayoutConfig.MenuEntryHeightUnits);
					layElem.preferredHeight = layElem.minHeight;

					// for images we set the width too:
					if (transf.GetComponent<Image> () != null) {
						layElem.minWidth = layElem.minHeight;
						layElem.preferredWidth = layElem.minHeight;
					}

					// for texts we adapt the font size to be at most one third of the container element height:
					Text text = transf.GetComponent<Text> ();
					if (text != null) {
						text.fontSize = (int)Math.Floor (layElem.minHeight * 0.66f);
					}
				}
			} else {
				Log.SignalErrorToDeveloper ("In gameobject {0} path {1} did not lead to another gameobject.", menuEntry.gameObject, gameObjectPath);
			}
		}

	}

}