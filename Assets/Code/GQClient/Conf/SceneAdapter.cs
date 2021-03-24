using Code.GQClient.Err;
using Code.GQClient.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.GQClient.Conf
{

	public class SceneAdapter
	{

		public const string EXTENSION_TAG = "Extension";

		public static void OnSceneLoaded (Scene scene, LoadSceneMode mode)
		{
			OnSceneActivated (scene);
		}

		public static void OnSceneActivated (Scene scene)
		{
			RemoveAllSceneExtensions (scene);

			foreach (SceneExtension extension in Config.Current.sceneExtensions) {
				if (extension.scene.ToLower() != scene.path.ToLower())
					// skip extension on other scenes:
					continue;
				
				GameObject prefab = Resources.Load (extension.prefab) as GameObject;
				if (prefab == null) {
					Log.SignalErrorToDeveloper (
						"Extending scene {0}: prefab '{1}' could not be loaded.", 
						extension.scene, 
						extension.prefab
					);
					return;
				}
				GameObject root = GameObject.Find (extension.root);
				GameObject go = (GameObject)Base.Instantiate (
					                prefab,
					                root.transform
				                );
				go.tag = EXTENSION_TAG;
			}
		}

		public static void RemoveAllSceneExtensions (Scene scene)
		{
			foreach (GameObject go in GameObject.FindGameObjectsWithTag(EXTENSION_TAG)) {
				GameObject.DestroyImmediate (go);
			}
		}

	}

}
