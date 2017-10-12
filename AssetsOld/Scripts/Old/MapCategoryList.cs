using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using GQ.Client.Conf;
using System.Diagnostics;

public class MapCategoryList : MonoBehaviour {

	bool currentToggleStateShowAll;

	public Button toggleAllButton;

	public GameObject prefab;

	void Awake () {
		currentToggleStateShowAll = false;
		updateToggleAllButtonText();
	}

	void Start () {

		foreach ( CategoryInfo mcs in ConfigurationManager.Current.markers ) {

			if ( mcs.showInList ) {

				GameObject go = (GameObject)Instantiate(prefab);
				go.transform.SetParent(this.transform, false);
				go.transform.localScale = new Vector3(1f, 1f, 1f);
				go.GetComponent<MapCategoryMenuEntry>().markerCategory = mcs;
			}
		}

	}

	public void changeAll () {
		MapCategoryMenuEntry[] allChildren = GetComponentsInChildren<MapCategoryMenuEntry>();


		foreach ( MapCategoryMenuEntry child in allChildren ) {
			child.markerCategory.showOnMap = currentToggleStateShowAll;
			child.updateButton();
		}

		if ( GameObject.Find("PageController_Map") != null ) {

			GameObject.Find("PageController_Map").GetComponent<page_map>().updateMapMarkerInFoyer();
		}


		currentToggleStateShowAll = !currentToggleStateShowAll;
		updateToggleAllButtonText();

	}

	private void updateToggleAllButtonText () {
		if ( currentToggleStateShowAll ) {
			toggleAllButton.GetComponentInChildren<Text>().text = "Alle anzeigen";
		}
		else {
			toggleAllButton.GetComponentInChildren<Text>().text = "Alle ausblenden";
		}

	}
	

}
