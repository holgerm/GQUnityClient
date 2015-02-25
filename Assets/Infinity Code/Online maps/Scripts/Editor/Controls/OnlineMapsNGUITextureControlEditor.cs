/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OnlineMapsNGUITextureControl))]
public class OnlineMapsNGUITextureControlEditor : Editor
{
    public override void OnInspectorGUI()
    {
#if !NGUI && !UNITY_3_5 && !UNITY_3_5_5
        if (GUILayout.Button("Enable NGUI"))
        {
            string currentDefinitions =
            PlayerSettings.GetScriptingDefineSymbolsForGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup);
            if (currentDefinitions != "") currentDefinitions += ";";
            currentDefinitions += "NGUI";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currentDefinitions);
        }
#else
        base.OnInspectorGUI();
#endif
    }
}