using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Err;
using GQ.Client.Conf;

namespace GQ.Client.UI
{

	/// <summary>
	/// Add this script to the foyer list screen.
	/// </summary>
	public class FoyerListLayoutConfig : ScreenLayoutConfig
	{


		static public void SetListEntryHeight (GameObject listEntry, string gameObjectPath = null, float sizeScaleFactor = 1f)
		{
			ScreenLayoutConfig.SetEntryHeight (ListEntryHeightUnits, listEntry, gameObjectPath, sizeScaleFactor: sizeScaleFactor);
		}

		static public float ListEntryHeightUnits {
			get {
				return 
					calculateRestrictedHeight (
					ConfigurationManager.Current.listEntryHeightUnits,
					ConfigurationManager.Current.listEntryHeightMinMM,
					ConfigurationManager.Current.listEntryHeightMaxMM
				);
			}
		}

	}
}
