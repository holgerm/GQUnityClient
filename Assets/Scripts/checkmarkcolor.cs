using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class checkmarkcolor : MonoBehaviour {





	public bool mode = true;
	public Color on = Color.black;
	public Color off = Color.gray;

	// Use this for initialization
	void Start () {
	

		if (mode) {
			
			GetComponent<Image> ().color = on;
			
			
		} else {
			
			GetComponent<Image>().color = off;
			
		}





	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void setMode(bool b){
		mode = b;


		if (b) {

			GetComponent<Image> ().color = on;


		} else {

			GetComponent<Image>().color = off;

		}

	}
}
