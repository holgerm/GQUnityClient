/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OnlineMapsIGUITextureControl))]
public class OnlineMapsIGUITextureControlEditor : Editor
{
    public override void OnInspectorGUI()
    {
#if !IGUI && !UNITY_3_5 && !UNITY_3_5_5
        if (GUILayout.Button("Enable iGUI"))
        {
            string currentDefinitions =
            PlayerSettings.GetScriptingDefineSymbolsForGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup);
            if (currentDefinitions != "") currentDefinitions += ";";
            currentDefinitions += "IGUI";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currentDefinitions);
        }
#else
        base.OnInspectorGUI();
#endif
    }
}