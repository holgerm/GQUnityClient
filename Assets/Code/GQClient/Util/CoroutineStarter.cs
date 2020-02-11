using System.Collections;
using UnityEngine;

namespace Code.GQClient.Util
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
