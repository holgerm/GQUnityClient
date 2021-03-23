using Code.GQClient.Conf;
using UnityEditor;

namespace GQEditor.UI {

	[CustomEditor(typeof(Configuration))]
	public class ConfigurationEditor : Editor {

		private int selectedProductIndex;

		public override void OnInspectorGUI () {
			serializedObject.Update();
			DrawDefaultInspector();
		}
	}
}
