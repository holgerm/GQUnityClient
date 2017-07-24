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

		public DialogBehaviour Behaviour { get; set; }

		protected const string DIALOG_PREFAB = "Dialog";
		protected const string DETAILS_PATH = "Panel/TextScrollView/Viewport/Content/DetailsText";
		protected const string TITLE_PATH = "Panel/TitleText";
		protected const string YES_BUTTON_PATH = "Panel/Buttons/YesButton";
		protected const string NO_BUTTON_PATH = "Panel/Buttons/NoButton";

		private static GameObject instance = null;

		/// <summary>
		/// Gets the instance. If the instance is used for the first time, 
		/// it will be created from the prefab and will be inactive.
		/// </summary>
		/// <value>The instance.</value>
		public static Dialog Instance {
			get {
				if (instance == null) {
					GameObject rootCanvas = GameObject.FindGameObjectWithTag (Tags.ROOT_CANVAS);
					instance = (GameObject) Instantiate (
						Resources.Load (DIALOG_PREFAB),
						rootCanvas.transform,
						false
					);
					instance.SetActive (false);
					DontDestroyOnLoad (instance);
				}
				return instance.GetComponent<Dialog> ();
			}
		}

		/// <summary>
		/// Shows the loading dialog ui and connects it with the given behaviour. 
		/// 
		/// This method works somewhat like a constructor as it creates an object of 
		/// the Dialog Component class and intializes this objects connection to the behaviour object given.
		/// 
		/// The UI prefab is used like a singleton: If a ui had already been created before it will be reused.
		/// </summary>
		public static void Show (DialogBehaviour behaviour)
		{
			Instance.gameObject.SetActive (true);

			// Connect this controller with the given behaviour:
			behaviour.Dialog = Instance;
			behaviour.Dialog.Behaviour = behaviour;

			behaviour.Initialize ();
		}


		#region Initialization in Editor

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

		#endregion
		 
	}
}