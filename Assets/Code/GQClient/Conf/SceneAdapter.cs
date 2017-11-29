using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GQ.Client.Err;
using GQ.Client.Util;

namespace GQ.Client.Conf
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

			foreach (SceneExtension extension in ConfigurationManager.Current.sceneExtensions) {
				if (extension.scene.ToLower() != scene.path.ToLower())
					// skip extension on other scenes:
					continue;
				
				Object prefab = Resources.Load (extension.prefab);
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
					                root.transform,
					                false
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
