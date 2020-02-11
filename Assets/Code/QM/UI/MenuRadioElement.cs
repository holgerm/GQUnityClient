using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Err;

namespace QM.UI
{
	[RequireComponent (typeof(Button))]
	public class MenuRadioElement : MonoBehaviour
	{

		public GameObject activation;
		MenuRadioGroup radioGroup;

		void Start ()
		{
			Button button = GetComponent<Button> ();

			if (button == null) {
				Log.SignalErrorToDeveloper (
					"{0} script could not be activated: missing a Button script on gameobject {1}.",
					this.GetType ().Name,
					gameObject.name
				);
				return;
			} 

			button.onClick.AddListener (SwitchOn);


			radioGroup = transform.parent.GetComponent<MenuRadioGroup> ();

			if (radioGroup == null) {
				Log.SignalErrorToDeveloper (
					"{0} script could not be activated: missing a MenuRadioGroup script on parent of this MenuRadioElement {1}.",
					this.GetType ().Name,
					gameObject.name
				);
				return;
			} 
		}

		void SwitchOn ()
		{
			activation.SetActive (true);
			radioGroup.HideMenu ();
			foreach (MenuRadioElement radioElement in radioGroup.GetComponentsInChildren<MenuRadioElement>()) {
				if (radioElement != this)
					radioElement.SwitchOff ();
			}
		}

		void SwitchOff ()
		{
			activation.SetActive (false);
		}
	}
}
