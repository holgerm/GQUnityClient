using System;
using Code.GQClient.Conf;
using UnityEditor;
using UnityEngine;

namespace QM.SC.Editor
{
    public class ScreenshotEditor : EditorWindow
    {
        [MenuItem("Window/Screenshot")]
        public static void Init()
        {
            var editorAsm = typeof(UnityEditor.Editor).Assembly;
            var inspectorWindowType = editorAsm.GetType("UnityEditor.InspectorWindow");
            ScreenshotEditor editor;
            editor = inspectorWindowType != null 
                ? GetWindow<ScreenshotEditor>("Screenshot", true, inspectorWindowType) 
                : GetWindow<ScreenshotEditor>(typeof(ScreenshotEditor));
            editor.Show();
           // Instance = editor;
        }

        public void OnGUI()
        {
            if(GUILayout.Button("Take Screenshot"))
            {
                var filename = $"{ConfigurationManager.Current.id}_{DateTime.Now:yyyy'-'MM'-'DD'_'HH'-'mm'-'ss}.png";
                ScreenCapture.CaptureScreenshot(
                    $"{Application.dataPath}/../Screenshots/{filename}");
            }
        }
    }
}