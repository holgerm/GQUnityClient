using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Candlelight.UI;
using System.IO;
using UnityEngine.UI;
using GQ.Client.Err;

public class ShowLocalImprintText : MonoBehaviour
{

	protected Text imprintText;

	// Use this for initialization
	void Start ()
	{
		imprintText = GetComponent<Text> ();

		if (imprintText == null) {
			Log.SignalErrorToDeveloper ("Imprint: Text Component missing.");
			return;
		}

		TextAsset imprintTA = Resources.Load<TextAsset> ("imprint");
		if (imprintTA != null) {
			imprintText.text = imprintTA.text;
		}
	
	}
	
}
