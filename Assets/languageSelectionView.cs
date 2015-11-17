using UnityEngine;
using System.Collections;

public class languageSelectionView : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		if (!Configuration.instance.languageChangableByUser) {

			gameObject.SetActive(false);

		}


	}
	

}
