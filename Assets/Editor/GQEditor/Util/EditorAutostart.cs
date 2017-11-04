using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using GQ.Client.Conf;
using UnityEditor.SceneManagement;

namespace GQ.Editor.Util
{
	[InitializeOnLoad]
	public class EditorAutostart
	{
		static EditorAutostart ()
		{
			Debug.Log ("Up and running");
			EditorSceneManager.sceneOpened += OnSceneOpened;
		}

		public static void OnSceneOpened (Scene scene, OpenSceneMode mode)
		{
			SceneAdaptation.OnSceneActivated (scene);
		}


	}
}
