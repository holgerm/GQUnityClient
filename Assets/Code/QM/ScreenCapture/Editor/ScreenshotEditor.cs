using System;
using Code.GQClient.Conf;
using GQ.Editor.Util;
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
            var editor = inspectorWindowType != null 
                ? GetWindow<ScreenshotEditor>("Screenshot", true, inspectorWindowType) 
                : GetWindow<ScreenshotEditor>(typeof(ScreenshotEditor));
            editor.Show();
           // Instance = editor;
        }

        public void OnGUI()
        {
            if(GUILayout.Button("Take Screenshot"))
            {
                if (Camera.main != null)
                {
                    var camera = Camera.main;
                    var dir =
                        Files.CombinePath(
                            $"{Application.dataPath}/../Production/products addon",
                            ConfigurationManager.Current.id,
                            "Screenshots",
                            $"{camera.pixelWidth}_{camera.pixelHeight}"
                        );
                    Files.CreateDir(dir);
                    var filename = 
                        $"{ConfigurationManager.Current.id}_{DateTime.Now:yyyy'-'MM'-'dd'_'HH'-'mm'-'ss}.png";

                    ScreenCapture.CaptureScreenshot(
                        $"{dir}/{filename}");
                }
            }
        }

        private float GetScale()
        {
            var assembly = typeof(UnityEditor.EditorWindow).Assembly;
            var type = assembly.GetType("UnityEditor.GameView");
            var v = UnityEditor.EditorWindow.GetWindow(type);
 
            var defScaleField = type.GetField("m_defaultScale", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var defaultScale = (float) defScaleField.GetValue(v);
 
            var areaField = type.GetField("m_ZoomArea", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var areaObj = areaField.GetValue(v);
 
            var scaleField = areaObj.GetType().GetField("m_Scale", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            return defaultScale;
        }
    }
}