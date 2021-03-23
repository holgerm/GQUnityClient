using Code.GQClient.Err;
using TMPro;
using UnityEngine;

namespace Code.GQClient.UI.legal
{

    [RequireComponent (typeof(TextMeshProUGUI))]
	public class ShowVersionText : MonoBehaviour
	{
		private TextMeshProUGUI _text;

		// Use this for initialization
		private void Start ()
		{
            _text = GetComponent<TextMeshProUGUI>();

			if (_text == null) {
				Log.SignalErrorToDeveloper ("Version Text: Text Component missing.");
				return;
			}

			_text.text = "Version Info: " + Migration.Migration.CurrentAppVersion;
		}
		
	}

}