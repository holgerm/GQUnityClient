/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OnlineMapsDFGUITextureControl))]
public class OnlineMapsDFGUITextureControlEditor : Editor
{
    public override void OnInspectorGUI()
    {
#if !DFGUI && !UNITY_3_5 && !UNITY_3_5_5
        if (GUILayout.Button("Enable DFGUI"))
        {
            string currentDefinitions =
            PlayerSettings.GetScriptingDefineSymbolsForGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup);
            if (currentDefinitions != "") currentDefinitions += ";";
            currentDefinitions += "DFGUI";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currentDefinitions);
        }
#else
        base.OnInspectorGUI();
#endif
    }
}