using UnityEngine;
using GQ.Client.Err;
using TMPro;

namespace GQ.UI
{

    [RequireComponent (typeof(TextMeshProUGUI))]
	public class ShowVersionText : MonoBehaviour
	{

        protected TextMeshProUGUI text;

		// Use this for initialization
		void Start ()
		{
            text = GetComponent<TextMeshProUGUI>();

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

}