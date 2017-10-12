using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using QM.UI;
using GQ.Client.Err;

namespace QM.UI
{
	// TODO can we react on changes in the hierarchy? E.g. perform Reset()?
	// For now we react only if we beforehand have eanbled this editor, i.e. we need to touch the Toggle gameobject in the hierarchy.

	[CustomEditor (typeof(SeriesToggleButton))]
	public class SeriesToggleButtonEditor : Editor
	{
		SeriesToggleButton script = null;
		//		bool automaticResetListenerRegistered = false;
		//
		//		void OnEnable ()
		//		{
		//			if (target != null && !automaticResetListenerRegistered) {
		//				EditorApplication.hierarchyWindowChanged += ((SeriesToggleButton)target).Reset;
		//				automaticResetListenerRegistered = true;
		//				// TODO this happens very often, whenever we click on the go in the hierarchy.
		//			}
		//		}

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();

			script = (SeriesToggleButton)target;

			if (script.shownObjects.Length > 0) {
				// first object should be active:
				bool onlyFirstObjectActive = script.shownObjects [0].activeSelf;
				// all other should be inactive:
				for (int i = 1; i < script.shownObjects.Length; i++) {
					onlyFirstObjectActive &= !script.shownObjects [i].activeSelf;
				}
				if (!onlyFirstObjectActive) {
					EditorGUILayout.HelpBox (
						"Only the first ShownObject will initially be active. Consider to reset the script to automatically adjust that in editor mode too.", 
						MessageType.Warning
					);
				}
			}

			if (script.shownObjects.Length < 2) {
				EditorGUILayout.HelpBox (
					"Toggle should have at least two objects to show in order to make sense.", 
					MessageType.Warning
				);
			}

			bool shownObjectMissing = false;
			foreach (GameObject so in script.shownObjects) {
				shownObjectMissing |= so == null;
			}
			if (shownObjectMissing) {
				EditorGUILayout.HelpBox (
					"All ShownObjects must be set. Consider to reset the script to automatically select all direct children as ShownObjects.", 
					MessageType.Error
				);
			}


		}
	}

}