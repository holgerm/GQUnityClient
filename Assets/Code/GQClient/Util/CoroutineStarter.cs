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
	
	public class CoroutineStarter : MonoBehaviour
	{

		public const string GAMEOBJECT = "CoroutineStarter";

		private static CoroutineStarter _instance = null;

		public static CoroutineStarter Instance {
			get {
				if (_instance == null) {
					GameObject baseGO = GameObject.Find (GAMEOBJECT);

					if (baseGO == null) {
						baseGO = new GameObject (GAMEOBJECT);
						DontDestroyOnLoad (baseGO);
					}

					if (baseGO.GetComponent (typeof(CoroutineStarter)) == null)
						baseGO.AddComponent (typeof(CoroutineStarter));

					_instance = (CoroutineStarter)baseGO.GetComponent (typeof(CoroutineStarter));
				}
				return _instance;
			}
		}

		public static void Run(IEnumerator coroutine) {
			Instance.StartCoroutine (coroutine);
		}

		void Awake ()
		{
			DontDestroyOnLoad (Instance);
 		}

	}
}
