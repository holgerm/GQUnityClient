using UnityEngine;
using System.Collections;

public class S2GUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
		GUI.Label(new Rect(10, 10, 100, 20), "hello world!!!");
		if (GUI.Button(new Rect(10, 70, 50, 30), "Back")) {
			Application.LoadLevel(0);
		}
	}
}

