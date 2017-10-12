using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class scrollrectrecalculate : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		GetComponent<ScrollRect> ().vertical = false;
		GetComponent<ScrollRect> ().vertical = true;

	}
}
