using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
#if UNITY_3_5
[CustomEditor(typeof(VideoPlayer))]
#else
[CustomEditor(typeof(VideoPlayer), true)]
#endif
public class VideoPlayerEditor : Editor
{

	public override void OnInspectorGUI ()
	{

		GUILayout.Space(10f);
		DrawProperty("Video","Video");
		GUILayout.Space(10f);
		DrawProperty("Preload","Preload");
		DrawProperty("Loop","Loop");
		DrawProperty("Transparent","Transparent");
		DrawProperty("Additive","Additive");
		GUILayout.Space(10f);
		DrawProperty("Play","Play");
		GUILayout.Space(10f);
		DrawProperty("Scale","Scale");
		GUILayout.Space(10f);
		DrawProperty("OnReady","OnReady");
		GUILayout.Space(10f);
		DrawProperty("OnStart","OnStart");
		GUILayout.Space(10f);
		DrawProperty("OnStop","OnStop");
		GUILayout.Space(10f);


		serializedObject.ApplyModifiedProperties();
	}

	void DrawProperty(string name, string label)
	{
		SerializedProperty sp = serializedObject.FindProperty(name);
		
		if (sp != null)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(sp, new GUIContent(label), true, GUILayout.MinWidth(120f));
			EditorGUILayout.EndHorizontal();
		}
		
	}


}
