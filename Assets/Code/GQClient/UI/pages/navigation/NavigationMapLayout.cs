using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GQ.Client.UI
{

	public class NavigationMapLayout : PageLayout
	{

		public GameObject MapButtonPanel;

		public override void layout ()
		{
			base.layout ();

			// TODO set background color for button panel:

			// set button background height:
			for (int i = 0; i < MapButtonPanel.transform.childCount; i++) {
				GameObject perhapsAButton = MapButtonPanel.transform.GetChild (i).gameObject;
				Button button = perhapsAButton.GetComponent<Button> ();
				if (button != null) {
					LayoutElement layElem = perhapsAButton.GetComponent<LayoutElement> ();
					if (layElem != null) {
						float height = Units2Pixels (FoyerMapScreenLayout.MapButtonHeightUnits);
						SetLayoutElementHeight (layElem, height);
						SetLayoutElementWidth (layElem, height);
					}
				}
			}
		}

	}
}
