using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GQ.Client.Conf;

public class MapCategoryMenuEntry : MonoBehaviour {

	public CategoryInfo markerCategory;
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
		text.text = markerCategory.Name;
	}




	public void onChange () {

		markerCategory.showOnMap = !markerCategory.showOnMap;

		if ( GameObject.Find("PageController_Map") != null ) {
			GameObject.Find("PageController_Map").GetComponent<page_map>().updateMapMarkerInFoyer();
		}

		updateButton();

	}

	public void updateButton () {

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
