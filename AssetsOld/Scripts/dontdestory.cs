using UnityEngine;
using System.Collections;

public class dontdestory : MonoBehaviour {

	// Use this for initialization
	void Start () {

		DontDestroyOnLoad (gameObject);
	}



	void onDisable(){

		Debug.Log ("destroyed");

	}

}
