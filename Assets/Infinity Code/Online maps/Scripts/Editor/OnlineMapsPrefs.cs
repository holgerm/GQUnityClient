/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class OnlineMapsPrefs
{
    private const string prefix = "OM_";

    static OnlineMapsPrefs()
    {
        EditorApplication.playmodeStateChanged += PlaymodeStateChanged;
        //EditorSettings.
    }

    private static void Clear()
    {
        Delete("Position");
        Delete("Zoom");
        Delete("Provider");
        Delete("CustomProviderURL");
        Delete("Type");
        Delete("Labels");
        Delete("Marker_Count");
    }

    private static void Delete(string id)
    {
        EditorPrefs.DeleteKey(prefix + id);
    }

    private static bool Exists()
    {
        return EditorPrefs.HasKey(prefix + "Provider");
    }

    private static void Load(OnlineMaps api)
    {
        api.position = LoadPref("Position", api.position);
        api.zoom = LoadPref("Zoom", api.zoom);
        api.provider = (OnlineMapsProviderEnum) LoadPref("Provider", 0);
        api.customProviderURL = LoadPref("CustomProviderURL", "http://localhost/{zoom}/{y}/{x}");
        api.type = LoadPref("Type", 0);
        api.labels = LoadPref("Labels", false);
        api.traffic = LoadPref("Traffic", false);

        api.markers = new OnlineMapsMarker[LoadPref("Marker_Count", 0)];
        for (int i = 0; i < api.markers.Length; i++) LoadMarker(api, i);

        OnlineMapsControlBase3D control = api.gameObject.GetComponent<OnlineMapsControlBase3D>();
        if (control != null)
        {
            control.markers3D = new OnlineMapsMarker3D[LoadPref("Marker3D_Count", 0)];
            for (int i = 0; i < control.markers3D.Length; i++) LoadMarker3D(i, control);
        }
    }

    private static void LoadMarker3D(int i, OnlineMapsControlBase3D control)
    {
        OnlineMapsMarker3D marker = new OnlineMapsMarker3D();
        marker.position = LoadPref("Marker3D_" + i + "_Position", marker.position);
        marker.range = LoadPref("Marker3D_" + i + "_Range", marker.range);
        Debug.Log(marker.range);
        int mid = LoadPref("Marker3D_" + i + "_Prefab", 0);
        marker.prefab = EditorUtility.InstanceIDToObject(mid) as GameObject;
        marker.label = LoadPref("Marker3D_" + i + "_Label", marker.label);
        control.markers3D[i] = marker;
    }

    private static void LoadMarker(OnlineMaps api, int i)
    {
        OnlineMapsMarker marker = new OnlineMapsMarker();
        marker.position = LoadPref("Marker_" + i + "_Position", marker.position);
        marker.range = LoadPref("Marker_" + i + "_Range", marker.range);
        int mid = LoadPref("Marker_" + i + "_Texture", 0);
        if (mid != 0) marker.texture = EditorUtility.InstanceIDToObject(mid) as Texture2D;
        marker.label = LoadPref("Marker_" + i + "_Label", marker.label);
        marker.align = (OnlineMapsAlign) LoadPref("Marker_" + i + "_Align", (int) marker.align);
        api.markers[i] = marker;
    }

    private static OnlineMapsRange LoadPref(string id, OnlineMapsRange defVal)
    {
        return new OnlineMapsRange
        {
            min = LoadPref(id + "_Min", defVal.min),
            max = LoadPref(id + "_Max", defVal.max)
        };
    }

    private static string LoadPref(string id, string defVal)
    {
        string key = prefix + id;
        return EditorPrefs.HasKey(key) ? EditorPrefs.GetString(key) : defVal;
    }

    private static bool LoadPref(string id, bool defVal)
    {
        string key = prefix + id;
        return EditorPrefs.HasKey(key) ? EditorPrefs.GetBool(key) : defVal;
    }

    private static int LoadPref(string id, int defVal)
    {
        string key = prefix + id;
        return EditorPrefs.HasKey(key) ? EditorPrefs.GetInt(key) : defVal;
    }

    private static Vector2 LoadPref(string id, Vector2 defVal)
    {
        string key = prefix + id;
        return EditorPrefs.HasKey(key + "X") ? new Vector2(EditorPrefs.GetFloat(key + "X"), EditorPrefs.GetFloat(key + "Y")) : defVal;
    }

    private static void PlaymodeStateChanged()
    {
        if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (Exists())
            {
#pragma warning disable 618
                OnlineMaps api = ((OnlineMaps[]) Object.FindSceneObjectsOfType(typeof (OnlineMaps))).FirstOrDefault();
#pragma warning restore 618
                if (api != null)
                {
                    Load(api);
                    Clear();
                    EditorUtility.SetDirty(api);
                }
            }
        }
    }

    public static void Save(OnlineMaps api)
    {
        SetPref("Position", api.position);
        SetPref("Zoom", api.zoom);
        SetPref("Provider", (int) api.provider);
        SetPref("CustomProviderURL", api.customProviderURL);
        SetPref("Type", api.type);
        SetPref("Labels", api.labels);
        SetPref("Traffic", api.traffic);

        SetPref("Marker_Count", api.markers.Length);
        for (int i = 0; i < api.markers.Length; i++) SaveMarker(api, i);

        OnlineMapsControlBase3D control = api.gameObject.GetComponent<OnlineMapsControlBase3D>();
        if (control != null)
        {
            SetPref("Marker3D_Count", control.markers3D.Length);
            for (int i = 0; i < control.markers3D.Length; i++) SaveMarker3D(control, i);
        }
    }

    private static void SaveMarker3D(OnlineMapsControlBase3D control, int i)
    {
        OnlineMapsMarker3D marker = control.markers3D[i];
        SetPref("Marker3D_" + i + "_Position", marker.position);
        SetPref("Marker3D_" + i + "_Range", marker.range);
        SetPref("Marker3D_" + i + "_Prefab", marker.prefab.GetInstanceID());
        SetPref("Marker3D_" + i + "_Label", marker.label);
    }

    private static void SaveMarker(OnlineMaps api, int i)
    {
        OnlineMapsMarker marker = api.markers[i];
        SetPref("Marker_" + i + "_Position", marker.position);
        SetPref("Marker_" + i + "_Range", marker.range);
        SetPref("Marker_" + i + "_Texture", marker.texture != null ? marker.texture.GetInstanceID() : 0);
        SetPref("Marker_" + i + "_Label", marker.label);
        SetPref("Marker_" + i + "_Align", (int) marker.align);
    }

    private static void SetPref(string id, OnlineMapsRange val)
    {
        SetPref(id + "_Min", val.min);
        SetPref(id + "_Max", val.max);
    }

    private static void SetPref(string id, bool val)
    {
        EditorPrefs.SetBool(prefix + id, val);
    }

    private static void SetPref(string id, int val)
    {
        EditorPrefs.SetInt(prefix + id, val);
    }

    private static void SetPref(string id, string val)
    {
        EditorPrefs.SetString(prefix + id, val);
    }

    private static void SetPref(string id, Vector2 val)
    {
        EditorPrefs.SetFloat(prefix + id + "X", val.x);
        EditorPrefs.SetFloat(prefix + id + "Y", val.y);
    }
}