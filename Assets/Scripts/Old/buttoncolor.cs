using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class buttoncolor : MonoBehaviour {
	public Color default_c;
	public Color disabled_c;


	public Button button;
	void Update(){

		if (button.interactable) {


			GetComponent<Text>().color = default_c;
				} else {
			GetComponent<Text>().color  = disabled_c;

				}

	}
}
