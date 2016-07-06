using UnityEngine;
using System.Collections;
using UnityEditor;
using GQ.Client.Conf;

namespace GQEditor.UI {

	[CustomEditor(typeof(Configuration))]
	public class ConfigurationEditor : Editor {
		private Configuration confScript;
		private string[] productNames = new [] {
			"public",
			"lwl",
			"wcc"
		};
		private int selectedProductIndex;

		public override void OnInspectorGUI () {
			serializedObject.Update();
			confScript = (Configuration)target;
			DrawDefaultInspector();
		}
	}
}
