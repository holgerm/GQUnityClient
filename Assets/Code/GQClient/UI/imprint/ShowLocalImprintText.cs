using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Candlelight.UI;
using System.IO;
using UnityEngine.UI;

public class ShowLocalImprintText : MonoBehaviour {

	public Text imprintText;

	// Use this for initialization
	void Start () {
		TextAsset imprintTA = Resources.Load<TextAsset> ("imprint");
		if (imprintTA != null) {
			imprintText.text = imprintTA.text;
		}
	
	}
	
}
