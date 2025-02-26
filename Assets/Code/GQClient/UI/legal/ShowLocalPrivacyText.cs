﻿using Code.GQClient.Err;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.legal
{

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
}
