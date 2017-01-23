using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class SampleListDivider : MonoBehaviour {


	public string title;

	public Text label;

	void Start(){

		label = GetComponentInChildren<Text> ();
	}
	// Update is called once per frame
	void Update () {
	
		if (label.text != "--------------------------- "+title+" ---------------------------") {

			label.text = "--------------------------- "+title+" ---------------------------";
				}
	
	}
}
