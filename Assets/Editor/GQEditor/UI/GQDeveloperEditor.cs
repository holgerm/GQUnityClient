using UnityEditor;
using UnityEngine;

using System;
using System.Reflection;


namespace GQ.Editor.UI {

	public class GQDeveloperEditor : EditorWindow {

		[MenuItem("Window/GeoQuest Developer Editor")]
		public static void  Init () {
			Assembly editorAsm = typeof(UnityEditor.Editor).Assembly;
			Type inspectorWindowType = editorAsm.GetType("UnityEditor.ConsoleWindow");
			GQDeveloperEditor editor;
			if ( inspectorWindowType != null )
				editor = EditorWindow.GetWindow<GQDeveloperEditor>("GQ Develop", true, inspectorWindowType);
			else
				editor = EditorWindow.GetWindow<GQDeveloperEditor>(typeof(GQDeveloperEditor));
			editor.Show();
			Instance = editor;
		}

		private static GQDeveloperEditor _instance = null;

		public static GQDeveloperEditor Instance {
			get {
				return _instance;
			}
			private set {
				_instance = value;
			}
		}

		public void OnEnable () {
			Instance = this;
		}


		void OnGUI () {
			GUILayout.Label("Player Prefs", EditorStyles.boldLabel);
			EditorGUILayout.BeginHorizontal();
			{
				if ( GUILayout.Button("Clear") ) {

					PlayerPrefs.DeleteAll();

				}

				if ( GUILayout.Button("Save") ) {
					PlayerPrefs.Save();
				}
			}
			EditorGUILayout.EndHorizontal();
				
		}
	}
}
