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
	/// 
	/// For details on how to link UI elements like this Dialog to Tasks cf. @ref TasksAndUI

	/// </summary>
	public class Dialog : PrefabController {

		#region Content and Structure

		protected static readonly string PREFAB = "Dialog";

		public Text Details;
		protected const string DETAILS_PATH = "Panel/TextScrollView/Viewport/Content/DetailsText";
		public Text Title;
		protected const string TITLE_PATH = "Panel/TitleText";
		public Button YesButton;
		protected const string YES_BUTTON_PATH = "Panel/Buttons/YesButton";
		public Button NoButton;
		protected const string NO_BUTTON_PATH = "Panel/Buttons/NoButton";

		public DialogBehaviour Behaviour { get; set; } 

		#endregion


		#region Singleton

		private static GameObject go = null;

		/// <summary>
		/// Gets the instance. If the instance is used for the first time, 
		/// it will be created from the prefab and will be inactive.
		/// </summary>
		/// <value>The instance.</value>
		public static Dialog Instance {
			get {
				return Create(PREFAB).GetComponent<Dialog> ();
			}
		}
			
		#endregion


		#region Runtime API

		/// <summary>
		/// Sets the yes button with text and callback method.
		/// </summary>
		/// <param name="description">Description.</param>
		/// <param name="yesButtonClicked">Yes button clicked.</param>
		public void SetYesButton(string description, ClickCallBack yesButtonClicked) {
			Text buttonText = YesButton.transform.Find ("Text").GetComponent<Text>();
			buttonText.text = description;

			Behaviour.OnYesButtonClicked += yesButtonClicked;
			YesButton.gameObject.SetActive (true);
			YesButton.interactable = true;
		}

		/// <summary>
		/// Sets the no button with text and callback method.
		/// </summary>
		/// <param name="description">Description.</param>
		/// <param name="noButtonClicked">No button clicked.</param>
		public void SetNoButton(string description, ClickCallBack noButtonClicked) {
			Text buttonText = NoButton.transform.Find ("Text").GetComponent<Text>();
			buttonText.text = description;

			Behaviour.OnNoButtonClicked += noButtonClicked;
			NoButton.gameObject.SetActive (true);
			NoButton.interactable = true;
		}

		#endregion


		#region Initialization in Editor

		public virtual void Reset()
		{
			Details = EnsurePrefabVariableIsSet<Text> (Details, "Details Label", DETAILS_PATH);
			Title = EnsurePrefabVariableIsSet<Text> (Title, "Title Label", TITLE_PATH);
			YesButton = EnsurePrefabVariableIsSet<Button> (YesButton, "Yes Button", YES_BUTTON_PATH);
			NoButton = EnsurePrefabVariableIsSet<Button> (NoButton, "No Button", NO_BUTTON_PATH);
		}

		#endregion
		 
	}
}