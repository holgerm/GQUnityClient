#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;


namespace GQ.Editor.UI
{
    public class GQDeveloperEditor : EditorWindow
    {
        [MenuItem("Window/GeoQuest Developer Editor")]
        public static void Init()
        {
            Assembly editorAsm = typeof(UnityEditor.Editor).Assembly;
            Type inspectorWindowType = editorAsm.GetType("UnityEditor.ConsoleWindow");
            GQDeveloperEditor editor;
            if (inspectorWindowType != null)
                editor = EditorWindow.GetWindow<GQDeveloperEditor>("GQ Develop", true, inspectorWindowType);
            else
                editor = EditorWindow.GetWindow<GQDeveloperEditor>(typeof(GQDeveloperEditor));
            editor.Show();
        }

        private static bool _localPortalUsed;

        public static bool LocalPortalUsed()
        {
            if (!readFromPrefs)
            {
                ReadFromPrefs();
            }

            return _localPortalUsed;
        }

        private static string _portalIDString;
        private static bool readFromPrefs = false;

        private static void ReadFromPrefs()
        {
            if (EditorPrefs.HasKey("localPortalUsed"))
            {
                _localPortalUsed = EditorPrefs.GetBool("localPortalUsed");
                Debug.Log($"Read from editorprefs: localPortalUsed <-- {_localPortalUsed}");
            }
            else
            {
                _localPortalUsed = false;
                Debug.Log($"localPortalUsed initialized to {_localPortalUsed}");
            }

            if (EditorPrefs.HasKey("portalIDString"))
            {
                _portalIDString = EditorPrefs.GetString("portalIDString");
                Debug.Log($"Read from editorprefs: portalIDString <-- {_portalIDString}");
            }
            else
            {
                _portalIDString = "0";
                Debug.Log($"portalIDString initialized to {_portalIDString}");
            }

            readFromPrefs = true;
        }

        public static string LocalPortalId()
        {
            if (!readFromPrefs)
            {
                ReadFromPrefs();
            }

            return _portalIDString;
        }

        void OnGUI()
        {
            if (!readFromPrefs)
                ReadFromPrefs();
            
            GUILayout.Label("Player Prefs", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Clear"))
                {
                    PlayerPrefs.DeleteAll();
                }

                if (GUILayout.Button("Save"))
                {
                    PlayerPrefs.Save();
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Local Portal", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            {
                bool localPortalUsedNew = GUILayout.Toggle(_localPortalUsed, "On");
                if (localPortalUsedNew != _localPortalUsed)
                {
                    _localPortalUsed = localPortalUsedNew;
                    EditorPrefs.SetBool("localPortalUsed", _localPortalUsed);
                    Debug.Log($"SetBool: localPortalUsed to {_localPortalUsed}");
                }

                EditorGUI.BeginDisabledGroup(!_localPortalUsed);
                string portalIDStringNew = GUILayout.TextField(_portalIDString);
                if (portalIDStringNew != _portalIDString)
                {
                    _portalIDString = portalIDStringNew;
                    _portalIDString = _portalIDString == null ? "0" : int.Parse(_portalIDString).ToString();
                    EditorPrefs.SetString("portalIDString", _portalIDString);
                    Debug.Log($"SetBool: portalIDString to {_portalIDString}");
                }

                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}

#endif // only in Unity_Editor