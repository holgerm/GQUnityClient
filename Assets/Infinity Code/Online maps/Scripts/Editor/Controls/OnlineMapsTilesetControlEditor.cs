/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System.Diagnostics;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (OnlineMapsTileSetControl))]
public class OnlineMapsTilesetControlEditor : OnlineMapsTextureControlEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        OnlineMapsTileSetControl tilesetControl = (OnlineMapsTileSetControl) control;

        if (tilesetControl.tilesetShader == null) tilesetControl.tilesetShader = Shader.Find("Infinity Code/Online Maps/Tileset");

        tilesetControl.tileMaterial = (Material)EditorGUILayout.ObjectField("Tile material: ", tilesetControl.tileMaterial,
            typeof (Material), false);

        EditorGUI.BeginChangeCheck();
        tilesetControl.useElevation = EditorGUILayout.Toggle("Use elevation", tilesetControl.useElevation);
        if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying) control.UpdateControl();

        if (tilesetControl.useElevation)
        {
            tilesetControl.bingAPI = EditorGUILayout.TextField("BingMaps API key:", tilesetControl.bingAPI);
            if (GUILayout.Button("Create BingMaps API Key")) Process.Start("http://msdn.microsoft.com/en-us/library/ff428642.aspx");
        }

        if (GUILayout.Button("Move camera to center of Tileset"))
        {
            GameObject go = (control.activeCamera == null)? GameObject.FindGameObjectWithTag("MainCamera"): control.activeCamera.gameObject;
            OnlineMaps api = tilesetControl.GetComponent<OnlineMaps>();
            float minSide = Mathf.Min(api.tilesetSize.x, api.tilesetSize.y);
            Vector3 position = api.transform.position + new Vector3(api.tilesetSize.x / -2, minSide, api.tilesetSize.y / 2);
            go.transform.position = position;
            go.transform.rotation = Quaternion.Euler(90, 180, 0);
        }
    }
}