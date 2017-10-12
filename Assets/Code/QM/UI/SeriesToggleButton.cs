using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Err;
using UnityEngine.UI;
using UnityEngine.Events;

namespace QM.UI
{

	/// <summary>
	/// Toggles between two or more states as a linear series.
	/// E.g. you can select one view from multiple being offered by pressing a button as long until your preferred selection shows up.
	/// 
	/// Toggle is performed when the method toggle() is called, e.g. by a Button click event. 
	/// 
	/// You can configure any number of gameobjects as toggle series. The first of them will be shown initially.
	/// They will be activated one after the other (looping), whenever toggle() is called.
	/// 
	/// To perform further actions (beside showing the according gameobject as child of this button) this script defines an onToggledEvent which
	/// gives you the according gameobject that has been activated as parameter. Hence, you can hook your methods on this event and analyse 
	/// the parameter to perform the right action, e.g. changing a view according to the state that you toggled to.
	/// </summary>
	[RequireComponent (typeof(Button))]
	public class SeriesToggleButton : MonoBehaviour
	{

		public GameObject[] shownObjects;

		public Button toggleButton;

		public ToggleEvent onToggledEvent;

		// Use this for initialization
		void Start ()
		{
			orderToggleSeries ();
			showFirstObjectOnly ();

			toggleButton = gameObject.GetComponentInChildren<Button> ();

			if (toggleButton == null) {
				Log.SignalErrorToDeveloper (
					"{0} script could not be activated: missing a Button script on gameobject {1}.",
					this.GetType ().Name,
					gameObject.name
				);
				return;
			} 

			toggleButton.onClick.AddListener (ProceedToggle);
		}

		void showFirstObjectOnly ()
		{
			if (shownObjects.Length > 0) {
				shownObjects [0].SetActive (true);
				for (int i = 1; i < shownObjects.Length; i++) {
					shownObjects [i].SetActive (false);
				}
			}
		}

		void orderToggleSeries ()
		{
			shownObjects = new GameObject[transform.childCount];
			for (int i = 0; i < transform.childCount; i++) {
				shownObjects [i] = transform.GetChild (i).gameObject;
			}
		}

		public void ProceedToggle ()
		{
			if (shownObjects.Length < 2) {
				Log.SignalErrorToDeveloper (
					"{0} script could not be activated: At least two objects to toggle between are needed, but we got {1}.",
					this.GetType ().Name,
					shownObjects.Length
				);
				return;
			}

			// determine index of currently shown object (and set all other inactive):
			int indexCurShownObject = -1;
			for (int i = 0; i < shownObjects.Length; i++) {
				if (shownObjects [i].activeSelf) {
					if (indexCurShownObject == -1) {
						indexCurShownObject = i;
					}
					shownObjects [i].SetActive (false);
				}
			}
			if (indexCurShownObject == -1) {
				indexCurShownObject = 0;
				shownObjects [0].SetActive (true);
			}

			// increase to next index (start from beginning if the end of series is reached):
			indexCurShownObject = (indexCurShownObject + 1) % shownObjects.Length;
			// and set only the new object active:
			for (int i = 0; i < shownObjects.Length; i++) {
				shownObjects [i].SetActive (i == indexCurShownObject);
			}

			// invoke the onToggled Unity Event:
			onToggledEvent.Invoke (shownObjects [indexCurShownObject]);
		}

		/// <summary>
		/// Initializes the shownObjects with all direct children of this gameobject.
		/// </summary>
		public void Reset ()
		{
			orderToggleSeries ();
			showFirstObjectOnly ();
			toggleButton = gameObject.GetComponentInChildren<Button> ();
		}
	
	}

	[System.Serializable]
	public class ToggleEvent : UnityEvent<GameObject>
	{

	}
}
