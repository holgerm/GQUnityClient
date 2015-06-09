using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class loadTopLogo : MonoBehaviour {

	// Use this for initialization
	void Start () {
	


		GetComponent<Image> ().sprite = Configuration.instance.toplogo;
	}
	

}
