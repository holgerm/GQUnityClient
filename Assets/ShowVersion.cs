using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using GQ.Client.Conf;

/// <summary>
/// ShowVersion Component shows the version number plus subversion and additionally an automatically generated 
/// time stamp from the last build time in a text component which must be added to the same gameobject 
/// to which it is added too.
/// </summary>
public class ShowVersion : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Text text = GetComponent<Text>();
		if ( text != null ) {
			text.text = String.Format("{0}.{1} ({2})", 
				Configuration.instance.appVersion.ToString(), 
				Configuration.instance.subversionNumber,
				ProductConfigManager.Buildtime);
		}
		else {
			Debug.LogError("Text component missing or not initialized (should be used for showing version number)");
		}
	}
	
}
