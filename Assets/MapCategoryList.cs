using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class MapCategoryList : MonoBehaviour
{

	bool currentToggleState = true;

	public Button toggleAllButon;

	public GameObject prefab;
	// Use this for initialization
	void Start ()
	{
	



		foreach (MarkerCategorySprite mcs in Configuration.instance.categoryMarker) {

			if (mcs.showInList) {

				GameObject go = (GameObject)Instantiate (prefab);
				go.transform.SetParent (this.transform, false);
				go.transform.localScale = new Vector3 (1f, 1f, 1f);
				go.GetComponent<MapCategoryMenuEntry> ().markerCategory = mcs;

			}

		}
	}

	public void whatever ()
	{


		foreach (Transform child in transform) {


			child.GetComponent<MapCategoryMenuEntry> ().onChange ();
			toggleAllButon.GetComponentInChildren<Text> ().text = "Show All";


		}


	}
	

}
