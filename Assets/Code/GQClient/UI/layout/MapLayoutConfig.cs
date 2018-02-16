using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Conf;

namespace GQ.Client.UI
{

	/// <summary>
	/// Add this script to map screens.
	/// </summary>
	public class MapLayoutConfig : ScreenLayoutConfig
	{

		public GameObject MapButtonPanel;

		protected override void layout ()
		{
			base.layout ();

			// TODO set background color for button panel:

			// TODO set button background color & height:
			for (int i = 0; i < MapButtonPanel.transform.childCount; i++) {
				GameObject perhapsAButton = MapButtonPanel.transform.GetChild (i).gameObject;
				Button button = perhapsAButton.GetComponent<Button> ();
				if (button != null) {
					LayoutElement layElem = perhapsAButton.GetComponent<LayoutElement> ();
					if (layElem != null) {
						layElem.preferredHeight = LayoutConfig.Units2Pixels (LayoutConfig.MapButtonHeightUnits);
						layElem.preferredWidth = layElem.preferredHeight;
					}
				}
			}
		}
	}

}