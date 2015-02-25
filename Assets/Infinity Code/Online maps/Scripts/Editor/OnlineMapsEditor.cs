/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (OnlineMaps))]
// ReSharper disable once UnusedMember.Global
public class OnlineMapsEditor : Editor
{
    private OnlineMaps api;
    private readonly int[] availableSizes = {256, 512, 1024, 2048, 4096};
    private string[] availableSizesStr;
    private bool showAdvanced;
    private bool showCacheInfo;
    private bool showCreateTexture;
    private bool showMarkers;
    private bool showCustomProviderTokens;
    private int textureHeight = 512;
    private int textureWidth = 512;

    private static bool isPlay
    {
        get { return Application.isPlaying; }
    }

    private void CheckAPITextureImporter(Texture2D texture)
    {
        if (texture == null) return;

        string textureFilename = AssetDatabase.GetAssetPath(texture.GetInstanceID());
        TextureImporter textureImporter = AssetImporter.GetAtPath(textureFilename) as TextureImporter;
        if (textureImporter == null) return;

        bool needReimport = false;
        if (textureImporter.mipmapEnabled)
        {
            textureImporter.mipmapEnabled = false;
            needReimport = true;
        }
        if (!textureImporter.isReadable)
        {
            textureImporter.isReadable = true;
            needReimport = true;
        }
        if (textureImporter.textureFormat != TextureImporterFormat.RGB24)
        {
            textureImporter.textureFormat = TextureImporterFormat.RGB24;
            needReimport = true;
        }
        if (textureImporter.maxTextureSize < 256)
        {
            textureImporter.maxTextureSize = 256;
            needReimport = true;
        }

        if (needReimport) AssetDatabase.ImportAsset(textureFilename, ImportAssetOptions.ForceUpdate);
    }

    private void CheckMarkerTextureImporter(Texture2D texture)
    {
        if (texture == null) return;

        string textureFilename = AssetDatabase.GetAssetPath(texture.GetInstanceID());
        TextureImporter textureImporter = AssetImporter.GetAtPath(textureFilename) as TextureImporter;
        if (textureImporter != null)
        {
            bool needReimport = false;
            if (textureImporter.mipmapEnabled)
            {
                textureImporter.mipmapEnabled = false;
                needReimport = true;
            }
            if (!textureImporter.isReadable)
            {
                textureImporter.isReadable = true;
                needReimport = true;
            }
            if (textureImporter.textureFormat != TextureImporterFormat.ARGB32)
            {
                textureImporter.textureFormat = TextureImporterFormat.ARGB32;
                needReimport = true;
            }

            if (needReimport) AssetDatabase.ImportAsset(textureFilename, ImportAssetOptions.ForceUpdate);
        }
    }

    private void CreateTexture()
    {
        const string textureFilename = "Assets/OnlineMap.png";
        api.texture = new Texture2D(textureWidth, textureHeight);
        File.WriteAllBytes(textureFilename, api.texture.EncodeToPNG());
        AssetDatabase.Refresh();
        TextureImporter textureImporter = AssetImporter.GetAtPath(textureFilename) as TextureImporter;
        if (textureImporter != null)
        {
            textureImporter.mipmapEnabled = false;
            textureImporter.isReadable = true;
            textureImporter.textureFormat = TextureImporterFormat.RGB24;
            textureImporter.maxTextureSize = Mathf.Max(textureWidth, textureHeight);
            AssetDatabase.ImportAsset(textureFilename, ImportAssetOptions.ForceUpdate);
            api.texture = (Texture2D)AssetDatabase.LoadAssetAtPath(textureFilename, typeof(Texture2D));
        }

        EditorUtility.UnloadUnusedAssets();
    }

    public static Texture2D GetIcon(string iconName)
    {
        string[] path = Directory.GetFiles(Application.dataPath, iconName, SearchOption.AllDirectories);
        if (path.Length == 0) return null;
        string iconFile = "Assets" + path[0].Substring(Application.dataPath.Length).Replace('\\', '/');
        return AssetDatabase.LoadAssetAtPath(iconFile, typeof (Texture2D)) as Texture2D;
    }

    private static void FixImportSettings()
    {
        string resourcesFolder = Path.Combine(Application.dataPath, "Resources/OnlineMapsTiles");
        string[] tiles = Directory.GetFiles(resourcesFolder, "*.png", SearchOption.AllDirectories);
        float count = tiles.Length;
        int index = 0;
        foreach (string tile in tiles)
        {
            string shortPath = "Assets/" + tile.Substring(Application.dataPath.Length + 1);
            TextureImporter textureImporter = AssetImporter.GetAtPath(shortPath) as TextureImporter;
            EditorUtility.DisplayProgressBar("Update import settings for tiles",
                "Please wait, this may take several minutes.", index / count);
            if (textureImporter != null)
            {
                textureImporter.mipmapEnabled = false;
                textureImporter.isReadable = true;
                textureImporter.textureFormat = TextureImporterFormat.RGB24;
                textureImporter.maxTextureSize = 256;
                AssetDatabase.ImportAsset(shortPath, ImportAssetOptions.ForceSynchronousImport);
            }
            index++;
        }

        EditorUtility.ClearProgressBar();
    }

    public static bool Foldout(bool value, string text)
    {
        return GUILayout.Toggle(value, text, EditorStyles.foldout);
    }

    private void ImportFromGMapCatcher()
    {
        string folder = EditorUtility.OpenFolderPanel("Select GMapCatcher tiles folder", string.Empty, "");
        if (string.IsNullOrEmpty(folder)) return;

        string[] files = Directory.GetFiles(folder, "*.png", SearchOption.AllDirectories);
        if (files.Length == 0) return;

        const string resPath = "Assets/Resources/OnlineMapsTiles";

        foreach (string file in files) ImportTileFromGMapCatcher(file, folder, resPath);

        AssetDatabase.Refresh();
    }

    private static void ImportTileFromGMapCatcher(string file, string folder, string resPath)
    {
        string shortPath = file.Substring(folder.Length + 1);
        shortPath = shortPath.Replace('\\', '/');
        string[] shortArr = shortPath.Split(new[] {'/'});
        int zoom = 17 - int.Parse(shortArr[0]);
        int x = int.Parse(shortArr[1]) * 1024 + int.Parse(shortArr[2]);
        int y = int.Parse(shortArr[3]) * 1024 + int.Parse(shortArr[4].Substring(0, shortArr[4].Length - 4));
        string dir = Path.Combine(resPath, string.Format("{0}/{1}", zoom, x));
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        File.Copy(file, Path.Combine(dir, y + ".png"), true);
    }

    private void OnCheckUpdates()
    {
        OnlineMapsUpdater.OpenWindow();
    }

    private void OnEnable()
    {
        api = (OnlineMaps) target;
        if (api.defaultMarkerTexture == null) api.defaultMarkerTexture = GetIcon("DefaultMarker.png");
        if (api.skin == null)
        {
            api.skin =
                (GUISkin)
                    AssetDatabase.LoadAssetAtPath("Assets/Infinity Code/Online maps/Skin/DefaultSkin.guiskin",
                        typeof (GUISkin));
        }
    }

    private void OnGUIAdvanced(ref bool dirty)
    {
        EditorGUI.BeginChangeCheck();

        api.redrawOnPlay = EditorGUILayout.Toggle("Redraw on play", api.redrawOnPlay);

        if (api.target == OnlineMapsTarget.texture) api.useSmartTexture = EditorGUILayout.Toggle("Smart texture", api.useSmartTexture);

        OnGUITraffic(ref dirty);

        api.emptyColor = EditorGUILayout.ColorField("Empty color: ", api.emptyColor);

        api.defaultTileTexture = (Texture2D)EditorGUILayout.ObjectField("Default tile: ", api.defaultTileTexture, typeof(Texture2D), false);
        CheckAPITextureImporter(api.defaultTileTexture);

        api.skin = (GUISkin) EditorGUILayout.ObjectField("Skin: ", api.skin, typeof (GUISkin), false);

        api.defaultMarkerTexture = (Texture2D) EditorGUILayout.ObjectField("Default marker: ", api.defaultMarkerTexture, typeof (Texture2D), false);
        CheckMarkerTextureImporter(api.defaultMarkerTexture);

        api.defaultMarkerAlign = (OnlineMapsAlign) EditorGUILayout.EnumPopup("Markers align: ", api.defaultMarkerAlign);
        api.showMarkerTooltip = (OnlineMapsShowMarkerTooltip)EditorGUILayout.EnumPopup("Show marker tooltip: ", api.showMarkerTooltip);

        if (EditorGUI.EndChangeCheck()) dirty = true;
    }

    private void OnGUICreateTexture(ref bool dirty)
    {
        if (availableSizesStr == null) availableSizesStr = availableSizes.Select(s => s.ToString()).ToArray();

        textureWidth = EditorGUILayout.IntPopup("Width: ", textureWidth,
            availableSizesStr, availableSizes);
        textureHeight = EditorGUILayout.IntPopup("Height: ", textureHeight,
            availableSizesStr, availableSizes);

        if (GUILayout.Button("Create"))
        {
            CreateTexture();
            dirty = true;
        }

        EditorGUILayout.Space();
    }

    private bool OnGUIGeneral()
    {
        bool dirty = false;

        OnGUISource(ref dirty);
        OnGUILocation(ref dirty);
        OnGUITarget(ref dirty);

        if (EditorApplication.isPlaying && GUILayout.Button("Save state"))
        {
            SaveState();
            dirty = true;
        }
        return dirty;
    }

    private void OnGUILabels(ref bool dirty)
    {
        bool showLanguage;
        if (api.availableLabels)
        {
            bool labels = api.labels;
            api.labels = EditorGUILayout.Toggle("Labels: ", api.labels);
            if (labels != api.labels) dirty = true;
            showLanguage = api.labels;
        }
        else
        {
            showLanguage = api.enabledLabels;
            GUILayout.Label("Labels " + (showLanguage ? "enabled" : "disabled"));
        }
        if (showLanguage && api.availableLanguage)
        {
            api.language = EditorGUILayout.TextField("Language: ", api.language);
            GUILayout.Label(api.provider == OnlineMapsProviderEnum.nokia
                ? "Use three-letter code such as: eng"
                : "Use two-letter code such as: en");
        }
    }

    private void OnGUILocation(ref bool dirty)
    {
        Vector2 pos = api.position;
        Vector2 p = api.position;
        p.y = EditorGUILayout.FloatField("Latitude: ", p.y);
        p.x = EditorGUILayout.FloatField("Longitude: ", p.x);
        if (pos.x != p.x || pos.y != p.y)
        {
            dirty = true;
            api.position = p;
        }
        int zoom = api.zoom;
        api.zoom = EditorGUILayout.IntSlider("Zoom: ", api.zoom, 3, 20);
        if (zoom != api.zoom) dirty = true;
    }

    private void OnGUIMarker(int i, ref int index, ref bool hasDeleted)
    {
        OnlineMapsMarker marker = api.markers[i];
        GUILayout.Label("Marker " + index);

        EditorGUI.BeginChangeCheck();
        marker.position.y = EditorGUILayout.FloatField("Latitude: ", marker.position.y);
        marker.position.x = EditorGUILayout.FloatField("Longitude: ", marker.position.x);
        if (EditorGUI.EndChangeCheck() && Application.isPlaying) api.Redraw();

        float min = marker.range.min;
        float max = marker.range.max;
        EditorGUILayout.MinMaxSlider(new GUIContent(string.Format("Zooms ({0}-{1}): ", marker.range.min, marker.range.max) ), ref min, ref max, 3, 20);
        marker.range.min = Mathf.RoundToInt(min);
        marker.range.max = Mathf.RoundToInt(max);
        marker.rotation = Mathf.Repeat(EditorGUILayout.FloatField("Rotation (0-1): ", marker.rotation), 1);
        marker.label = EditorGUILayout.TextField("Label: ", marker.label);
        marker.align = (OnlineMapsAlign) EditorGUILayout.EnumPopup("Align: ", marker.align);
        marker.texture =
            (Texture2D) EditorGUILayout.ObjectField("Texture: ", marker.texture, typeof (Texture2D), true);

        CheckMarkerTextureImporter(marker.texture);

        if (GUILayout.Button("Remove"))
        {
            api.markers[i] = null;
            hasDeleted = true;
        }
        index++;
    }

    private void OnGUIMarkers()
    {
        if (api.markers == null) api.markers = new OnlineMapsMarker[0];
        if (GUILayout.Button("Add marker"))
        {
            if (!Application.isPlaying)
            {
                OnlineMapsMarker marker = new OnlineMapsMarker { position = api.position };
                List<OnlineMapsMarker> markers = new List<OnlineMapsMarker>(api.markers) { marker };
                api.markers = markers.ToArray();
            }
            else
            {
                api.AddMarker(api.position);
            }
        }

        int index = 1;
        bool hasDeleted = false;

        for (int i = 0; i < api.markers.Length; i++) OnGUIMarker(i, ref index, ref hasDeleted);

        if (hasDeleted)
        {
            List<OnlineMapsMarker> markers = api.markers.ToList();
            markers.RemoveAll(m => m == null);
            api.markers = markers.ToArray();
            if (Application.isPlaying) api.Redraw();
        }

        EditorGUILayout.Space();
    }

    private void OnGUIProvider(ref bool dirty)
    {
        OnlineMapsProviderEnum provider =
            (OnlineMapsProviderEnum)EditorGUILayout.EnumPopup(new GUIContent("Provider: ", "Provider of tiles"), api.provider);
        if (provider != api.provider)
        {
            api.provider = provider;
            api.type = 0;
            dirty = true;
        }

        if (provider == OnlineMapsProviderEnum.custom)
        {
            string customProviderURL = api.customProviderURL;
            api.customProviderURL = EditorGUILayout.TextField("URL: ", api.customProviderURL);
            if (customProviderURL != api.customProviderURL) dirty = true;

            EditorGUILayout.BeginVertical(GUI.skin.box);
            showCustomProviderTokens = Foldout(showCustomProviderTokens, "Available tokens");
            if (showCustomProviderTokens)
            {
                GUILayout.Label("{zoom}");
                GUILayout.Label("{x}");
                GUILayout.Label("{y}");
                GUILayout.Label("{quad}");
                GUILayout.Space(10);
            }
            EditorGUILayout.EndVertical();
        }

#if UNITY_WEBPLAYER
        if (provider != OnlineMapsProviderEnum.custom) GUILayout.Label("This provider can be slow on Webplayer");
#endif
    }

    private void OnGUISource(ref bool dirty)
    {
        EditorGUI.BeginDisabledGroup(isPlay);

        OnlineMapsSource source = (OnlineMapsSource) EditorGUILayout.EnumPopup("Source: ", api.source);
        if (source != api.source)
        {
            api.source = source;
            dirty = true;
        }

        if (source != OnlineMapsSource.Online)
        {
            if (GUILayout.Button("Fix import settings for tiles")) FixImportSettings();
            if (GUILayout.Button("Import from GMapCatcher")) ImportFromGMapCatcher();
        }

        EditorGUI.EndDisabledGroup();

        if (source != OnlineMapsSource.Resources)
        {
            OnGUIProvider(ref dirty);

            GUIContent[] aviableTypes = api.availableTypes.Select(t => new GUIContent(t)).ToArray();
            if (aviableTypes != null)
            {
                int type = api.type;
                api.type = EditorGUILayout.Popup(new GUIContent("Type: ", "Type of map texture"), api.type, aviableTypes);
                if (type != api.type) dirty = true;
            }

            OnGUILabels(ref dirty);
        }
    }

    private void OnGUITarget(ref bool dirty)
    {
        EditorGUI.BeginDisabledGroup(isPlay);

        OnlineMapsTarget mapTarget = api.target;
        api.target =
            (OnlineMapsTarget) EditorGUILayout.EnumPopup(new GUIContent("Target:", "Where will be drawn map"), api.target);
        if (mapTarget != api.target) dirty = true;

        if (api.target == OnlineMapsTarget.texture)
        {
            Texture2D texture = api.texture;
            api.texture = (Texture2D) EditorGUILayout.ObjectField("Texture: ", api.texture, typeof (Texture2D), true);
            if (texture != api.texture) dirty = true;
            CheckAPITextureImporter(api.texture);
        }
        else if (api.target == OnlineMapsTarget.tileset) OnGUITilesetProps(ref dirty);

        EditorGUI.EndDisabledGroup();
    }

    private void OnGUITilesetProps(ref bool dirty)
    {
        EditorGUI.BeginChangeCheck();
        api.tilesetWidth = EditorGUILayout.IntField("Width (pixels): ", api.tilesetWidth);
        api.tilesetHeight = EditorGUILayout.IntField("Height (pixels): ", api.tilesetHeight);
        api.tilesetSize = EditorGUILayout.Vector2Field("Size (in scene): ", api.tilesetSize);
        if (EditorGUI.EndChangeCheck()) dirty = true;

        int dts = OnlineMapsUtils.tileSize * 2;
        if (api.tilesetWidth % dts != 0) api.tilesetWidth = Mathf.FloorToInt(api.tilesetWidth / (float) dts + 0.5f) * dts;
        if (api.tilesetHeight % dts != 0)
            api.tilesetHeight = Mathf.FloorToInt(api.tilesetHeight / (float) dts + 0.5f) * dts;

        if (api.tilesetWidth <= 0) api.tilesetWidth = dts;
        if (api.tilesetHeight <= 0) api.tilesetHeight = dts;
    }

    private void OnGUIToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        GUILayout.Label("");

        if (GUILayout.Button("Help", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("View Online Documentation"), false, OnViewDocs);
            menu.AddItem(new GUIContent("View Online API Reference"), false, OnViewAPI);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Open Product Page"), false, OnProductPage);
            menu.AddItem(new GUIContent("Check Updates"), false, OnCheckUpdates);
            menu.AddItem(new GUIContent("Mail to Support"), false, OnSendMail);
            menu.ShowAsContext();
        }

        GUILayout.EndHorizontal();
    }

    private void OnGUITraffic(ref bool dirty)
    {
        bool traffic = api.traffic;
        api.traffic = EditorGUILayout.Toggle("Traffic: ", api.traffic);
        if (traffic != api.traffic) dirty = true;
    }

    public override void OnInspectorGUI()
    {
        OnGUIToolbar();

        bool dirty = OnGUIGeneral();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        showMarkers = Foldout(showMarkers, "2D Markers");
        if (showMarkers) OnGUIMarkers();
        EditorGUILayout.EndVertical();

        if (api.target == OnlineMapsTarget.texture)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            showCreateTexture = Foldout(showCreateTexture, "Create texture");
            if (showCreateTexture) OnGUICreateTexture(ref dirty);
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.BeginVertical(GUI.skin.box);
        showAdvanced = Foldout(showAdvanced, "Advanced");
        if (showAdvanced) OnGUIAdvanced(ref dirty);
        EditorGUILayout.EndVertical();

        if (dirty)
        {
            EditorUtility.SetDirty(api);
            Repaint();
        }
    }

    private void OnProductPage()
    {
        Process.Start("http://infinity-code.com/en/products/online-maps");
    }

    private void OnSendMail()
    {
        Process.Start("mailto:support@infinity-code.com?subject=Online maps");
    }

    private void OnViewAPI()
    {
        Process.Start("http://infinity-code.com/en/docs/api/online-maps");
    }

    private void OnViewDocs()
    {
        Process.Start("http://infinity-code.com/en/docs/online-maps");
    }

    private void SaveState()
    {
        OnlineMapsPrefs.Save(api);
        api.Save();
        if (api.target == OnlineMapsTarget.texture)
        {
            string path = AssetDatabase.GetAssetPath(api.texture);
            File.WriteAllBytes(path, api.texture.EncodeToPNG());
            AssetDatabase.Refresh();
        }
    }
}