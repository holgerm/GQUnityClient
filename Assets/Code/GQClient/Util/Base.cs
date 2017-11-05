using UnityEngine;
using System.Collections;
using System.Globalization;
using System.Threading;
using GQ.Client.Err;
using GQ.Client.UI.Dialogs;
using UnityEngine.SceneManagement;
using GQ.Client.Conf;

namespace GQ.Client.Util
{
	
	public class Base : MonoBehaviour
	{

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
				}
				return _instance;
			}
		}

		#endregion


		#region Globals

		public const string FOYER_SCENE = "Scenes/Foyer";

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

		// Use this for initialization
		void Start ()
		{
		}

		#endregion


	}

}
