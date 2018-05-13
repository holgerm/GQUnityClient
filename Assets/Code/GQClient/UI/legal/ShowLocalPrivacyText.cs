using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Candlelight.UI;
using System.IO;
using UnityEngine.UI;
using GQ.Client.Err;

public class ShowLocalPrivacyText : MonoBehaviour
{

	protected Text privacyText;

	// Use this for initialization
	void Start ()
	{
		privacyText = GetComponent<Text> ();

		if (privacyText == null) {
			Log.SignalErrorToDeveloper ("Privacy Info: Text Component missing.");
			return;
		}

		TextAsset privacyTA = Resources.Load<TextAsset> ("privacy");
		if (privacyTA != null) {
			privacyText.text = privacyTA.text;
		}
	
	}
	
}
