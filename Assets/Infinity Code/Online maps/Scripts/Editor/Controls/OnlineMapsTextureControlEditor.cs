/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OnlineMapsTextureControl))]
public class OnlineMapsTextureControlEditor : Editor
{
    protected OnlineMapsControlBase3D control;
    private bool showMarkers;

    private void OnEnable()
    {
        control = (OnlineMapsControlBase3D) target;
    }

    private void OnGUIMarker(int i, ref int index, ref bool hasDeleted, OnlineMaps map)
    {
        OnlineMapsMarker3D marker = control.markers3D[i];
        GUILayout.Label("Marker " + index);
        Vector2 oldPosition = marker.position;
        EditorGUI.BeginChangeCheck();
        marker.position.y = EditorGUILayout.FloatField("Latitude: ", marker.position.y);
        marker.position.x = EditorGUILayout.FloatField("Longitude: ", marker.position.x);

        float min = marker.range.min;
        float max = marker.range.max;
        EditorGUILayout.MinMaxSlider(new GUIContent(string.Format("Zooms ({0}-{1}): ", marker.range.min, marker.range.max)), ref min, ref max, 3, 20);
        if (marker.range.Update(Mathf.RoundToInt(min), Mathf.RoundToInt(max))) marker.Update(map.topLeftPosition, map.bottomRightPosition, map.zoom);

        if (Application.isPlaying && marker.position != oldPosition) marker.Update(map.topLeftPosition, map.bottomRightPosition, map.zoom);

        marker.label = EditorGUILayout.TextField("Label: ", marker.label);
        marker.scale = EditorGUILayout.FloatField("Scale: ", marker.scale);
        GameObject prefab = marker.prefab;
        marker.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab: ", marker.prefab, typeof(GameObject), false);

        if (Application.isPlaying && marker.prefab != prefab) marker.Reinit(map.topLeftPosition, map.bottomRightPosition, map.zoom);

        if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(control);

        if (GUILayout.Button("Remove"))
        {
            control.markers3D[i] = null;
            hasDeleted = true;
            if (Application.isPlaying) Destroy(marker.instance);
        }
        index++;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        control.allowUserControl = EditorGUILayout.Toggle("Allow User Control", control.allowUserControl);
        control.allowAddMarkerByM = EditorGUILayout.Toggle("Allow Add 2D marker by M", control.allowAddMarkerByM);
        control.allowAddMarker3DByN = EditorGUILayout.Toggle("Allow Add 3D marker by N", control.allowAddMarker3DByN);
        control.zoomInOnDoubleClick = EditorGUILayout.Toggle("Zoom In On Double Click", control.zoomInOnDoubleClick);
        control.activeCamera = (Camera)EditorGUILayout.ObjectField("Camera:", control.activeCamera, typeof (Camera), true);
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
        control.marker2DMode = (OnlineMapsMarker2DMode) EditorGUILayout.EnumPopup("Marker 2D Mode: ", control.marker2DMode);
        if (control.marker2DMode == OnlineMapsMarker2DMode.billboard)
        {
            control.marker2DSize = EditorGUILayout.FloatField("Marker 2D size: ", control.marker2DSize);
            if (control.marker2DSize < 1) control.marker2DSize = 1;
        }
#endif

        if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(control);

        EditorGUILayout.BeginVertical(GUI.skin.box);
        showMarkers = OnlineMapsEditor.Foldout(showMarkers, "3D markers");
        if (showMarkers) OnGUIMarkers();
        EditorGUILayout.EndVertical();
    }

    private void OnGUIMarkers()
    {
        if (control.markers3D == null) control.markers3D = new OnlineMapsMarker3D[0];

        EditorGUI.BeginChangeCheck();
        control.marker3DScale = EditorGUILayout.FloatField("Marker3D Scale: ", control.marker3DScale);
        if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(control);

        if (GUILayout.Button("Add marker"))
        {
            if (!Application.isPlaying)
            {
                OnlineMapsMarker3D marker = new OnlineMapsMarker3D { position = control.GetComponent<OnlineMaps>().position };
                List<OnlineMapsMarker3D> markers = new List<OnlineMapsMarker3D>(control.markers3D) { marker };
                control.markers3D = markers.ToArray();
            }
            else
            {
                control.AddMarker3D(OnlineMaps.instance.position, GameObject.CreatePrimitive(PrimitiveType.Cube));
            }
            EditorUtility.SetDirty(control);
        }

        int index = 1;
        bool hasDeleted = false;

        OnlineMaps map = control.GetComponent<OnlineMaps>();

        for (int i = 0; i < control.markers3D.Length; i++) OnGUIMarker(i, ref index, ref hasDeleted, map);

        if (hasDeleted)
        {
            List<OnlineMapsMarker3D> markers = control.markers3D.ToList();
            markers.RemoveAll(m => m == null);
            control.markers3D = markers.ToArray();
            if (Application.isPlaying) OnlineMaps.instance.Redraw();
            EditorUtility.SetDirty(control);
        }

        EditorGUILayout.Space();
    }
}
