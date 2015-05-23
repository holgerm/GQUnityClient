using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Configuration))]
public class ConfigurationEditor : Editor
{

	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();

		Configuration confScript = (Configuration)target;

		DrawDefaultInspector ();

	}

}
