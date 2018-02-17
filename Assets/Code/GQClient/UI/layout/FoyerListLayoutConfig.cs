using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Err;

namespace GQ.Client.UI
{

	/// <summary>
	/// Add this script to the foyer list screen.
	/// </summary>
	public class FoyerListLayoutConfig : ScreenLayoutConfig
	{
		public static void SetEntryHeight (GameObject listEntry, string gameObjectPath = null)
		{
			// set layout height:
			Transform t = (gameObjectPath == null ? listEntry.transform : listEntry.transform.Find (gameObjectPath));
			if (t != null) {
				LayoutElement layElem = t.GetComponent<LayoutElement> ();
				if (layElem != null) {
					layElem.minHeight = LayoutConfig.Units2Pixels (LayoutConfig.ListEntryHeightUnits);
				}
			} else {
				Log.SignalErrorToDeveloper ("In gameobject {0} path {1} did not lead to another gameobject.", listEntry.gameObject, gameObjectPath);
			}
		}
	}
}
