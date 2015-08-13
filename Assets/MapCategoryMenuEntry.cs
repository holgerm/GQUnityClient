using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapCategoryMenuEntry : MonoBehaviour {



	public MarkerCategorySprite markerCategory;
	public Image markerImage;
	public Text text;
	public Toggle toggle;


	// Use this for initialization
	void Start () {
	
		markerImage.sprite = markerCategory.sprite;
		text.text = markerCategory.anzeigename_de;

		if (!markerCategory.showOnMap) {

			toggle.isOn = false;
		}

	}



	public void onChange(bool b){


		markerCategory.showOnMap = b;

		if (GameObject.Find ("PageController_Map") != null) {
			GameObject.Find ("PageController_Map").GetComponent<page_map> ().updateMapMarker ();

		}
	}
}
