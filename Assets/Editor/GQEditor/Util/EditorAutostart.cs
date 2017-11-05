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
			EditorSceneManager.sceneOpened += OnSceneOpened;
		}

		public static void OnSceneOpened (Scene scene, OpenSceneMode mode)
		{
			SceneAdapter.OnSceneActivated (scene);
		}


	}
}
