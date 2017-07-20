using UnityEngine;
using System.Collections;
using System;
using GQ.Client.Conf;
using GQ.Util;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Event;

namespace GQ.Client.UI.Dialogs {

	/// <summary>
	/// Connects the Dialog UI with the behaviour implemented in a subclass of DialogBehaviour. 
	/// These behaviours are NOT MonoBehaviours but one of them must be set as connected in this component.
	/// 
	/// Why is this? It allows to use the dialog prefab for multiple purposes. 
	/// Therefore one has to instantiate the one dialog prefab and initialize it with aone of the available behaviours
	/// in a separate step by setting the connection. 
	/// 
	/// This can both be done by script. Manually in the editor only the first step can be done right now. 
	/// We would need a little custom editor to enable selection of available behaviours in the gui.
	/// 
	/// Anyway, we typically drive the dialog by calling some functionality, 
	/// hence it should be dynamically initialized and setup by script anyway
	/// </summary>
	public class Dialog : MonoBehaviour {

		public Text Details;
		public Text Title;
		public Button YesButton;
		public Button NoButton;

		public DialogBehaviour Behaviour { get; protected set; }

		protected const string DIALOG_PREFAB = "Dialog";
		protected const string DETAILS_PATH = "Panel/TextScrollView/Viewport/Content/DetailsText";
		protected const string TITLE_PATH = "Panel/TitleText";
		protected const string YES_BUTTON_PATH = "Panel/Buttons/YesButton";
		protected const string NO_BUTTON_PATH = "Panel/Buttons/NoButton";

		/// <summary>
		/// Shows the loading dialog ui and connects it with the given behaviour. 
		/// 
		/// This method works somewhat like a constructor as it creates an object of 
		/// the Dialog Component class and intializes this objects connection to the behaviour object given.
		/// 
		/// If a ui had already been created before it will be reused.
		/// </summary>
		public static void Show (DialogBehaviour behaviour)
		{
			if (instance != null) {
				Debug.Log ("Re-Enable Existing");
				instance.SetActive (true);
			} 
			else {
				Debug.Log ("Loading and Instatiating New");
				GameObject rootCanvas = GameObject.FindGameObjectWithTag (Tags.ROOT_CANVAS);
				instance = (GameObject) Instantiate (
					Resources.Load (DIALOG_PREFAB),
					rootCanvas.transform,
					false
				);
			}

			// Connect this controller with the given behaviour:
			behaviour.Dialog = instance.GetComponent<Dialog> ();
			behaviour.Dialog.Behaviour = behaviour;

			behaviour.Initialize ();
		}

		private static GameObject instance = null;

		public virtual void Reset()
		{
			if (Details == null)
			{
				Transform textGo = transform.Find (DETAILS_PATH);
				if (textGo == null) {
					Debug.LogErrorFormat ("Dialog must contain a Panel with a DetailsText inside (at path {0}).", DETAILS_PATH);
					return;
				}

				Details = textGo.GetComponent<Text> ();
			}

		
			if (Title == null)
			{
				Transform textGo = transform.Find (TITLE_PATH);
				if (textGo == null) {
					Debug.LogErrorFormat ("Dialog must contain a Panel with a TitleText inside (at path {0}).", TITLE_PATH);
					return;
				}

				Title = textGo.GetComponent<Text> ();
			}


			if (YesButton == null)
			{
				Transform buttonT = transform.Find (YES_BUTTON_PATH);
				if (buttonT == null) {
					Debug.LogErrorFormat ("Dialog must contain an Image with a Yes-Button inside (at path {0}).", YES_BUTTON_PATH);
					return;
				}

				YesButton = buttonT.GetComponent<Button>();
			}


			if (NoButton == null)
			{
				Transform buttonT = transform.Find (NO_BUTTON_PATH);
				if (buttonT == null) {
					Debug.LogErrorFormat ("Dialog must contain an Image with a No-Button inside (at path {0}).", NO_BUTTON_PATH);
					return;
				}

				NoButton = buttonT.GetComponent<Button>();
			}
		}
		 
//		/// <summary>
//		/// When the dialog is enabled (i.e. activated, shown, started), we connect the behaviour events with the button click unity events. 
//		/// And we initialize the behaviour.
//		/// </summary>
//		protected void OnEnable()
//		{
//			/// UI 2 CONTROLLER:
//			/// connect the behaviour to the ui buttons, i.e. make sure controller events are called when ui button pressed:
//			YesButton.onClick.RemoveAllListeners();
//			YesButton.onClick.AddListener (YesButtonClicked);
//
//			NoButton.onClick.RemoveAllListeners ();
//			NoButton.onClick.AddListener (NoButtonClicked);
//
//			/// CONTROLLER FUNCTIONS:
//			/// make sure we start clean, i.e. with no mvc listeners at all:
//			if (Behaviour != null)
//				Behaviour.Initialize ();
//		}

//		protected void OnDisable() 
//		{
//			YesButton.onClick.RemoveAllListeners();
//			NoButton.onClick.RemoveAllListeners ();
//
//			Behaviour.TearDown ();
//		}

//		protected void YesButtonClicked() {
//			Behaviour.RaiseYesButtonClicked();
//		}
//
//		protected void NoButtonClicked() {
//			Behaviour.RaiseNoButtonClicked();
//		}

	}
}