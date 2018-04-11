using UnityEngine;
using System.Collections;
using System.Globalization;
using System.Threading;
using GQ.Client.Err;
using GQ.Client.UI.Dialogs;
using UnityEngine.SceneManagement;
using GQ.Client.Conf;
using GQ.Client.Model;

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
					QuestInfoManager.Instance.UpdateQuestInfos();
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

		public void HideFoyerCanvases() {
			// store current show state and hide:
			listShown = ListCanvas.activeSelf;
			ListCanvas.SetActive (false);
			mapShown = MapCanvas.activeSelf;
			MapCanvas.SetActive (false);
			MapHolder.SetActive (false);
			menuShown = MenuCanvas.activeSelf;
			MenuCanvas.SetActive (false);
			imprintShown = ImprintCanvas.activeSelf;
			ImprintCanvas.SetActive (false);
		}

		public void ShowFoyerCanvases() {
			// show again accordingg to stored state:
			ListCanvas.SetActive (listShown);
			MapCanvas.SetActive (mapShown);
			MapHolder.SetActive (mapShown);
			MenuCanvas.SetActive (menuShown);
			ImprintCanvas.SetActive (imprintShown);
		}
		#endregion


		#region LifeCycle
		public static void Init ()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo ("de-DE");
		}

		void Awake ()
		{
			DontDestroyOnLoad (Instance);
			SceneManager.sceneLoaded += SceneAdapter.OnSceneLoaded;
		}
		#endregion

	}

}
