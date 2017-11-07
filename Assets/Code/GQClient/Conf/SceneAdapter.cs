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

			Debug.Log (string.Format ("OnSceneActivated: name {0}, path {1} isloaded: {2}, current product id: {3}", 
				scene.name, scene.path, scene.isLoaded, ConfigurationManager.Current.id).Yellow ());

			foreach (SceneExtension extension in ConfigurationManager.Current.sceneExtensions) {
				Debug.Log (string.Format ("Extending Scene {0} at {1} with prefab {2}.",
					extension.scene, extension.root, extension.prefab));
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
			Debug.Log (string.Format ("RemoveAllSceneExtensions: scene {0}", scene.name).Yellow ());

			foreach (GameObject go in GameObject.FindGameObjectsWithTag(EXTENSION_TAG)) {
				Debug.Log ("\t" + go.name);
				GameObject.DestroyImmediate (go);
			}
		}

	}

}
