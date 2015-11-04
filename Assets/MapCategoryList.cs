using UnityEngine;

using System.Collections;

public class MapCategoryList : MonoBehaviour
{


	public GameObject prefab;
	// Use this for initialization
	void Start ()
	{
	



		foreach (MarkerCategorySprite mcs in Configuration.instance.categoryMarker) {

			if (mcs.showInList) {

				GameObject go = (GameObject)Instantiate (prefab);
//				go.transform.parent = this.transform;
				go.transform.SetParent (this.transform, false);
				go.transform.localScale = new Vector3 (1f, 1f, 1f);
				go.GetComponent<MapCategoryMenuEntry> ().markerCategory = mcs;

			}

		}
	}
	

}
