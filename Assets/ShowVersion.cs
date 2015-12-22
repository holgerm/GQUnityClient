using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ShowVersion : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Text text = GetComponent<Text>();
		if ( text != null ) {
			text.text = String.Format("{0}.{1}", Configuration.instance.appVersion.ToString(), Configuration.instance.subversionNumber);
		}
		else {
			Debug.LogError("Text component missing or not initialized (should be used for showing version number)");
		}
	}
	
}
