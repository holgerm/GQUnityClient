using Code.GQClient.Conf;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.layout
{

	/// <summary>
	/// Add this script to map screens.
	/// </summary>
	public class FoyerMapScreenLayout : ScreenLayout
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
						float height = Units2Pixels (MapButtonHeightUnits);
						SetLayoutElementHeight (layElem, height);
						SetLayoutElementWidth (layElem, height);
					}
				}
			}
		}

		public static float MapButtonHeightUnits {
			get {
				float result = 
					calculateRestrictedHeight (
						Config.Current.mapButtonHeightUnits,
						Config.Current.mapButtonHeightMinMM,
						Config.Current.mapButtonHeightMaxMM
					);
				return result;
			}
		}

		public static float MarkerHeightUnits {
			get {
				float result = 
					calculateRestrictedHeight (
						Config.Current.markerHeightUnits,
						Config.Current.markerHeightMinMM,
						Config.Current.markerHeightMaxMM
					);
				return result;
			}
		}

	}

}