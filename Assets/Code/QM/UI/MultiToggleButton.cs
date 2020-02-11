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
	public class MultiToggleButton : MonoBehaviour
	{

		public GameObject[] shownObjects;
		public int selectedIndexAtStart = 0;

		public Button toggleButton;

		/// <summary>
		/// Register listener methods for Toggle events here. They receive a parameter set to the gameobject that 
		/// represents the button that has been pressed, e.g. a "View on Map"-Button etc.
		/// </summary>
		public ToggleEvent onToggledEvent;

		// Use this for initialization
		void Start ()
		{
			orderToggleSeries ();
			showSelectedObjectOnly ();

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

		public void SetSelectedStartIndex(int index)
		{
			if (index < 0 || index > shownObjects.Length) {
				Log.SignalErrorToDeveloper ("Tried to set the startIndex to {0} but only {1} objects are shown.", index, shownObjects.Length);
				return;
			}

			selectedIndexAtStart = index;
			showSelectedObjectOnly (); // update the view
		}


		void showSelectedObjectOnly ()
		{
			if (selectedIndexAtStart < 0 || selectedIndexAtStart > shownObjects.Length) {
				selectedIndexAtStart = 0;
			}
			
			if (shownObjects.Length > 0) {
				for (int i = 1; i < shownObjects.Length; i++) {
					shownObjects [i].SetActive (i == selectedIndexAtStart);
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
						// currently active view was not found before, hence we remember its index:
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
			int indexReferredByPressedButton = indexCurShownObject;
			indexCurShownObject = (indexCurShownObject + 1) % shownObjects.Length;
			// and set only the new object active:
			for (int i = 0; i < shownObjects.Length; i++) {
				// show next view (cyclic) in menu entry:
				shownObjects [i].SetActive (i == indexCurShownObject);
			}

			// invoke the onToggled Unity Event:
			if (indexReferredByPressedButton > -1) {
				onToggledEvent.Invoke (shownObjects [indexReferredByPressedButton]);
			}
		}

		/// <summary>
		/// Initializes the shownObjects with all direct children of this gameobject.
		/// </summary>
		public void Reset ()
		{
			orderToggleSeries ();
			showSelectedObjectOnly ();
			toggleButton = gameObject.GetComponentInChildren<Button> ();
		}
	
	}

	[System.Serializable]
	public class ToggleEvent : UnityEvent<GameObject>
	{

	}
}
