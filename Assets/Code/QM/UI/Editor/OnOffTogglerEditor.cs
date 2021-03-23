using Code.QM.UI;
using UnityEditor;

namespace QM.UI
{
	
	[CustomEditor (typeof(OnOffToggler))]
	public class OnOffTogglerEditor : Editor
	{

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();

			OnOffToggler script = (OnOffToggler)target;
			if (script.whatToToggle == null) {
				EditorGUILayout.HelpBox ("WhatToToggle must be assigend a gameobject that will be toggled on and off.", MessageType.Warning);
			}
		}
	}

}