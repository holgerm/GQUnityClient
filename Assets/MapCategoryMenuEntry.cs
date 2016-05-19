using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapCategoryMenuEntry : MonoBehaviour {

	public MarkerCategorySprite markerCategory;
	public Image markerImage;
	public Text text;

	public Image backgroundImage;
	public Image backgroundImageGradient;

	public Color fadeoutbgcolor;
	public Color fadeoutmarkercolor;
	public Color fadeoutcolor;


	// Use this for initialization
	void Start () {

		markerImage.sprite = markerCategory.sprite;
		text.text = markerCategory.anzeigename_de;
	}




	public void onChange () {

		markerCategory.showOnMap = !markerCategory.showOnMap;

		updateMarkerAndButton();

	}

	public void setState (bool newState) {

		markerCategory.showOnMap = newState;

		updateMarkerAndButton();
	}


	private void updateMarkerAndButton () {
		
		if ( GameObject.Find("PageController_Map") != null ) {
			GameObject.Find("PageController_Map").GetComponent<page_map>().updateMapMarker();
		}

		if ( !markerCategory.showOnMap ) {
			backgroundImageGradient.enabled = true;
			backgroundImage.color = fadeoutbgcolor;
			markerImage.color = fadeoutmarkercolor;
			text.color = fadeoutcolor;
		}
		else {
			backgroundImageGradient.enabled = false;

			backgroundImage.color = Color.white;
			markerImage.color = Color.white;
			text.color = Color.black;
		}
	}
}
