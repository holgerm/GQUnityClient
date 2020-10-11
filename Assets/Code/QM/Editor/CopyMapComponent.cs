using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using UnityEditorInternal;

namespace DefaultNamespace
{
    public class CopyMapComponent
    {
        static OnlineMaps mapComponent;
        private static string json;

        [MenuItem("SmartCopy/Copy Map Component")]
        private static void CopyAllComponents()
        {
            if (Selection.gameObjects.Length != 0)
            {
                mapComponent = GameObject.Find("Map").GetComponent<OnlineMaps>();
                // mapComponent = Selection.activeGameObject.GetComponent<OnlineMaps>();
                // ComponentUtility.CopyComponent(mapComponent);
                json = EditorJsonUtility.ToJson(mapComponent, true);
                Debug.Log($"JSON: {json}");
           }
            else
            {
                Debug.Log("Please select GameObject to copy components from.");
            }
        }

        [MenuItem("SmartCopy/Paste Map Component")]
        private static void PasteAllComponents()
        {
            if (Selection.gameObjects.Length != 0)
            {
                if (mapComponent != null)
                {
                    // ComponentUtility.PasteComponentValues(mapComponent);
                    EditorJsonUtility.FromJsonOverwrite(json, mapComponent);
                    EditorUtility.SetDirty(mapComponent);
                    return;
                }

                Debug.Log("No Component to paste.");
                return;
            }

            Debug.Log("No GameObject Selected to paste Components.");
        }

       private static bool FillerFunc(Component c)
        {
            return true;
        }
    }
}