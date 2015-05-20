using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class texttotext : MonoBehaviour {



public void setText(string s){

		GetComponent<Text> ().text = s;

	}
}
