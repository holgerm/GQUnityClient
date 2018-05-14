using UnityEngine;
using System.Collections;
using System.Globalization;
using System.Threading;
using GQ.Client.Err;
using GQ.Client.UI.Dialogs;
using UnityEngine.SceneManagement;
using GQ.Client.Conf;
using GQ.Client.Model;
using GQ.Client.UI;
using System;
using QM.Util;
using System.Collections.Generic;

namespace GQ.Client.Util
{
	
	public class Base : MonoBehaviour
	{
		#region Inspector Global Values

		public GameObject ListCanvas;
		public GameObject MapCanvas;
		public GameObject MapHolder;
		public GameObject MenuCanvas;
		public GameObject ImprintCanvas;
		public GameObject PrivacyCanvas;
		public GameObject AuthorCanvas;

		#endregion


		#region Singleton

		public const string BASE = "Base";

		private static Base _instance = null;

		public static Base Instance {
			get {
				if (_instance == null) {
					GameObject baseGO = GameObject.Find (BASE);

					if (baseGO == null) {
						baseGO = new GameObject (BASE);
						Init ();
					}

					if (baseGO.GetComponent (typeof(Base)) == null)
						baseGO.AddComponent (typeof(Base));

					_instance = (Base)baseGO.GetComponent (typeof(Base));

					// Initialize QuestInfoManager:
					QuestInfoManager.Instance.UpdateQuestInfos ();
				}
				return _instance;
			}
		}

		#endregion


		#region Foyer

		public const string FOYER_SCENE = "Scenes/Foyer";
		public const string FOYER_SCENE_NAME = "Foyer";

		private bool listShown;
		private bool mapShown;
		private bool menuShown;
		private bool imprintShown;

		private Dictionary<string, bool> canvasStates;

		/// <summary>
		/// Called when we leave the foyer towards a page.
		/// </summary>
		public void HideFoyerCanvases ()
		{
			// store current show state and hide:
			GameObject[] rootGOs = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().GetRootGameObjects ();
			foreach (GameObject rootGo in rootGOs) {
				Canvas canv = rootGo.GetComponent<Canvas> ();
				if (canv != null) {
					canvasStates [canv.name] = canv.isActiveAndEnabled;
					Debug.Log ("HideFoyerCanvases: " + canv.name + " stored as: " + canvasStates [canv.name]);
					canv.gameObject.SetActive (false);
				}
			}
			Debug.Log ("FRAMES NOW: " + Time.frameCount);
		}

		/// <summary>
		/// Called when we return to the foyer from a page.
		/// </summary>
		public void ShowFoyerCanvases ()
		{
			// show again accordingg to stored state:
			GameObject[] rootGOs = UnityEngine.SceneManagement.SceneManager.GetSceneByName(FOYER_SCENE_NAME).GetRootGameObjects ();
			foreach (GameObject rootGo in rootGOs) {
				Canvas canv = rootGo.GetComponent<Canvas> ();
				bool oldCanvState;
				if (canv != null) {
					if (canvasStates.TryGetValue (canv.name, out oldCanvState)) {
						Debug.Log ("ShowFoyerCanvases: trying to read " + canv.name + " stored as: " + canvasStates [canv.name]);
						canv.gameObject.SetActive (canvasStates [canv.name]);
					}
					else {
						Debug.Log ("ShowFoyerCanvases: Canv name not found in state store: " + canv.name);
					}
				}
			}
		}

		#endregion


		#region LifeCycle

		public static void Init ()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo ("de-DE");
		}

		void Awake ()
		{
			// hide all canvases at first, we show the needed ones in initViews()
			GameObject[] rootGOs = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().GetRootGameObjects ();
			foreach (GameObject rootGo in rootGOs) {
				Canvas canv = rootGo.GetComponent<Canvas> ();
				if (canv != null) {
					if ("DialogCanvas".Equals(canv.name)) {
						canv.gameObject.SetActive (true);
					}
					else {
						canv.gameObject.SetActive (false);
					}
				}
			}

			DontDestroyOnLoad (Instance);
			SceneManager.sceneLoaded += SceneAdapter.OnSceneLoaded;
			canvasStates = new Dictionary<string, bool> ();
		}

//		void Start() {
//			ImprintCanvas.gameObject.SetActive (false);
//			PrivacyCanvas.gameObject.SetActive (false);
//			AuthorCanvas.gameObject.SetActive (false);
//		}
//
		#endregion


		#region Global Runtime State

		string loggedInAs = null;
		public string LoggedInAs { 
			get {
				return loggedInAs;
			}
			set {
				loggedInAs = value;
			}
		}

		#endregion

	}

}
