using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GQ.Client.Err;

namespace GQ.Client.Conf
{

	public class SceneAdaptation
	{
		public static void OnSceneLoaded (Scene scene, LoadSceneMode mode)
		{
			OnSceneActivated (scene);
		}

		public static void OnSceneActivated (Scene scene)
		{
			Debug.Log (string.Format ("OnSceneLoaded: name {0}, path {1} isloaded: {2}, current product id: {3}", 
				scene.name, scene.path, scene.isLoaded, ConfigurationManager.Current.id).Yellow ());
		}

	}

}
