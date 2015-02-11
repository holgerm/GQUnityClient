using UnityEngine;
using System.Collections;

public class keepalive : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
				if (GameObject.Find ("MsgCanvas") != gameObject) {
						Destroy (gameObject);		
				} else {
						DontDestroyOnLoad (gameObject);
		
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
