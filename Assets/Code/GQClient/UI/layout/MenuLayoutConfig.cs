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

			// set menu width:
			setWidth ();

			// set interaction option:
			string[] goPaths = 
				new string[] { "MenuPanel/SideImage", "MenuPanel/MenuScrollView", "MenuPanel/MenuScrollView/Viewport" };
			foreach (string path in goPaths) {
				Transform menuScrollT = transform.Find (path);
				if (menuScrollT != null) {
					Image im = menuScrollT.GetComponent<Image> ();
					if (im != null) {
						im.raycastTarget = ConfigurationManager.Current.menuInhibitsInteraction;
					}
				}
			}
		}

		void setWidth ()
		{
			Transform menuScrollT = transform.Find ("MenuPanel/MenuScrollView");
			if (menuScrollT != null) {
				LayoutElement layElem = menuScrollT.GetComponent<LayoutElement> ();
				if (layElem != null) {
					layElem.minWidth = Units2Pixels (MenuEntryWidthUnits);
					layElem.preferredWidth = Units2Pixels (MenuEntryWidthUnits);
				}
			}
		}

		static public float MenuEntryHeightUnits {
			get {
				return 
					calculateRestrictedHeight (
					ConfigurationManager.Current.menuEntryHeightUnits,
					ConfigurationManager.Current.menuEntryHeightMinMM,
					ConfigurationManager.Current.menuEntryHeightMaxMM
				);
			}
		}

		static public float MenuEntryWidthUnits {
			get {
				return 
					calculateRestrictedHeight (
					ConfigurationManager.Current.menuEntryWidthUnits,
					ConfigurationManager.Current.menuEntryWidthMinMM,
					ConfigurationManager.Current.menuEntryWidthMaxMM
				);
			}
		}

		static public void SetMenuEntryHeight (GameObject menuEntry, string gameObjectPath = null, float sizeScaleFactor = 1f)
		{
			ScreenLayoutConfig.SetEntryHeight (MenuEntryHeightUnits, menuEntry, gameObjectPath, sizeScaleFactor: sizeScaleFactor);
		}

	}

}