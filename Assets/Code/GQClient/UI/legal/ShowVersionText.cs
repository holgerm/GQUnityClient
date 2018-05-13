using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Candlelight.UI;
using System.IO;
using UnityEngine.UI;
using GQ.Client.Err;

[RequireComponent (typeof(Text))]
public class ShowVersionText : MonoBehaviour
{

	protected Text text;

	// Use this for initialization
	void Start ()
	{
		text = GetComponent<Text> ();

		if (text == null) {
			Log.SignalErrorToDeveloper ("Version Text: Text Component missing.");
			return;
		}

		TextAsset imprintTA = Resources.Load<TextAsset> ("buildtime");
		if (imprintTA != null) {
			text.text = "Version Info: " + imprintTA.text;
		}
	
	}
	
}
