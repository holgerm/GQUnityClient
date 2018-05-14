using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Candlelight.UI;
using System.IO;
using UnityEngine.UI;
using GQ.Client.Err;

namespace GQ.UI {
	
	public class ShowLocalImprintText : MonoBehaviour
	{

		protected Text imprintText;

		// Use this for initialization
		void Start ()
		{
			Debug.Log ("STARTED: IMPRINT CANVAS");
			GameObject[] rootGOs = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().GetRootGameObjects ();
			foreach (GameObject rootGo in rootGOs) {
				Canvas canv = rootGo.GetComponent<Canvas> ();
				if (canv != null) {
					Debug.Log ("Canvas " + canv.name + " on sorting order " + canv.sortingOrder + " layer ID: " + canv.sortingLayerID + " active&enabled: " + canv.isActiveAndEnabled);
				}
			}

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
}
