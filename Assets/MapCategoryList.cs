using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using GQ.Client.Conf;

public class MapCategoryList : MonoBehaviour {

	bool currentToggleStateShowAll;

	public Button toggleAllButton;

	public GameObject prefab;

	void Awake () {
		currentToggleStateShowAll = false;
		updateToggleAllButtonText();
	}

	void Start () {

		Debug.Log("Starting to create marker categories. Size: " + Configuration.instance.categoryMarker.Count);

		foreach ( MarkerCategorySprite mcs in Configuration.instance.categoryMarker ) {

			if ( mcs.showInList ) {

				GameObject go = (GameObject)Instantiate(prefab);
				go.transform.SetParent(this.transform, false);
				go.transform.localScale = new Vector3(1f, 1f, 1f);
				go.GetComponent<MapCategoryMenuEntry>().markerCategory = mcs;

				Debug.Log("Created marker category " + mcs.anzeigename_de);
			}
		}
	}

	public void whatever () {
		MapCategoryMenuEntry[] allChildren = GetComponentsInChildren<MapCategoryMenuEntry>();

		foreach ( MapCategoryMenuEntry child in allChildren ) {
			child.setState(currentToggleStateShowAll);
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
