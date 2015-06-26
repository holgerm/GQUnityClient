using UnityEngine;
using System.Collections;

public class toggleImpressum : MonoBehaviour {

	private showimpressum impressum;
	// Use this for initialization
	void Start () {
		if (GameObject.Find ("ImpressumCanvas") != null) {

			impressum = GameObject.Find ("ImpressumCanvas").GetComponent<showimpressum>();
		}
	}
	
	public void toggle(){


		impressum.toggleImpressum ();

	}
}
