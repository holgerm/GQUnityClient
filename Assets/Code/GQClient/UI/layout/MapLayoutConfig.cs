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

		public override void layout ()
		{
			base.layout ();

			// TODO set background color for button panel:

			// set button background color & height:
			for (int i = 0; i < MapButtonPanel.transform.childCount; i++) {
				GameObject perhapsAButton = MapButtonPanel.transform.GetChild (i).gameObject;
				Image buttonBGImage = perhapsAButton.GetComponent<Image> ();
				if (buttonBGImage != null) {
					Debug.Log (string.Format ("Color before: {0}, {1}, {2}, {3}", 
						buttonBGImage.color.r, buttonBGImage.color.g, buttonBGImage.color.b, buttonBGImage.color.a));
					buttonBGImage.color = ConfigurationManager.Current.mapButtonBGColor;
					Debug.Log (string.Format ("Color after: {0}, {1}, {2}, {3}", 
						buttonBGImage.color.r, buttonBGImage.color.g, buttonBGImage.color.b, buttonBGImage.color.a));
				} 
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
			GameObject b = GameObject.Find ("MapCanvas/MapScreen/MapArea (invisible)/MapButtonPanel/CenteringButton");
			Image img = b.GetComponent<Image> ();
			Debug.Log(string.Format ("Resulting Color for Center Button: {0}, {1}, {2}, {3}", 
				img.color.r, img.color.g, img.color.b, img.color.a));
		}

		static public float MapButtonHeightUnits {
			get {
				float result = 
					calculateRestrictedHeight (
						ConfigurationManager.Current.mapButtonHeightUnits,
						ConfigurationManager.Current.mapButtonHeightMinMM,
						ConfigurationManager.Current.mapButtonHeightMaxMM
					);
				return result;
			}
		}

		static public float MarkerHeightUnits {
			get {
				float result = 
					calculateRestrictedHeight (
						ConfigurationManager.Current.markerHeightUnits,
						ConfigurationManager.Current.markerHeightMinMM,
						ConfigurationManager.Current.markerHeightMaxMM
					);
				return result;
			}
		}

	}

}