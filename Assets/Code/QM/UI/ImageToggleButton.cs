using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Err;

namespace QM.UI
{

	/// <summary>
	/// Add this component to a button to toggle the given GameObject on and off. 
	/// 
	/// Initial state depends on the activity setting of the gameobject whatToToggle.
	/// GameObject to toggle must be set in Inspector, otherwise the script will warn you and simply not work.
	/// </summary>
	[RequireComponent (typeof(Button)), RequireComponent (typeof(Image)), ExecuteInEditMode]
	public class ImageToggleButton : MonoBehaviour
	{
		public Button ToggleButton;
		public Image ToggleImage;

		public Sprite OnSprite;
		public Sprite OffSprite;

		public bool stateIsOn = true;

		void Start ()
		{
			ToggleButton = gameObject.GetComponentInChildren<Button> ();

			if (ToggleButton == null) {
				Log.SignalErrorToDeveloper (
					"{0} script could not be activated: missing a Button script on gameobject {1}.",
					this.GetType ().Name,
					gameObject.name
				);
				return;
			} 

			ToggleButton.onClick.AddListener (Toggle);

			ToggleImage.sprite = stateIsOn ? OnSprite : OffSprite;
		}

		public void Toggle ()
		{
			stateIsOn = !stateIsOn;
			ToggleImage.sprite = stateIsOn ? OnSprite : OffSprite;
		}

		void OnDisable()
		{
			ToggleImage.enabled = false;
			ToggleButton.enabled = false;
		}

		void OnEnable() {
			ToggleImage.enabled = true;
			ToggleButton.enabled = true;
		}
	}
}