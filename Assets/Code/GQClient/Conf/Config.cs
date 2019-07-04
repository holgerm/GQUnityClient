using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Converters;
using GQ.Client.FileIO;
using GQ.Client.UI;
using GQ.Client.Util;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GQ.Client.Conf
{
    /// <summary>
    /// Config class specifies textual parameters of a product. It is used both at runtime to initilize the app's branding details from and 
    /// at editor time to back the product editor view and store the parameters while we use the editor.
    /// </summary>
    public class Config
    {
        #region Parse Helper
        public static bool __JSON_Currently_Parsing = false;

        public static Config _doDeserializeConfig(string configText)
        {
            Config.__JSON_Currently_Parsing = true;
            Config config = JsonConvert.DeserializeObject<Config>(configText);
            Config.__JSON_Currently_Parsing = false;
            return config;
        }
        #endregion

        //////////////////////////////////
        // THE ACTUAL PRODUCT CONFIG DATA:	


        #region General

        [ShowInProductEditor(StartSection = "General:")]
        public string id { get; set; }

        [ShowInProductEditor]
        public string idExtension { get; set; }

        [ShowInProductEditor]
        public string name { get; set; }

        [ShowInProductEditor]
        public int portal { get; set; }

        [ShowInProductEditor]
        public string[] assetAddOns { get; set; }

#if UNITY_EDITOR
        [ShowInProductEditor]
        [JsonConverter(typeof(StringEnumConverter))]
        public AndroidSdkVersions androidMinSDKVersion { get; set; }
#endif

        [ShowInProductEditor]
        public int autoStartQuestID { get; set; }

        [ShowInProductEditor]
        public bool autostartIsPredeployed { get; set; }

        [ShowInProductEditor]
        public bool keepAutoStarting { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(StringEnumConverter))]
        public DownloadStrategy downloadStrategy { get; set; }

        [ShowInProductEditor]
        public long timeoutMS { get; set; }

        [ShowInProductEditor]
        public long maxIdleTimeMS { get; set; }

        [ShowInProductEditor]
        public int maxParallelDownloads { get; set; }

        [ShowInProductEditor]
        public string nameForQuestSg { get; set; }

        [ShowInProductEditor]
        public string nameForQuestsPl { get; set; }

        [ShowInProductEditor]
        public string[] questInfoViews { get; set; }

        public bool cloudQuestsVisible { get; set; }

        public bool showCloudQuestsImmediately { get; set; }

        public bool downloadAllCloudQuestOnStart { get; set; }

        [ShowInProductEditor]
        public bool localQuestsDeletable { get; set; }

        [ShowInProductEditor, JsonProperty]
        private bool showHiddenQuests { get; set; }
        [JsonIgnore]
        public bool ShowHiddenQuests
        {
            get
            {
                if (Author.LoggedIn)
                {
                    return Author.ShowHiddenQuests;
                }
                else
                {
                    return showHiddenQuests;
                }
            }
        }

        [ShowInProductEditor, JsonProperty]
        public bool showOnlyLocalQuests { get; set; }
        [JsonIgnore]
        public bool ShowOnlyLocalQuests
        {
            get
            {
                if (Author.LoggedIn)
                {
                    return Author.ShowOnlyLocalQuests;
                }
                else
                {
                    return showOnlyLocalQuests;
                }
            }
        }


        [ShowInProductEditor(StartSection = "Pages & Scenes:")]
        public string[] acceptedPageTypes { get; set; }

        public Dictionary<string, string> GetSceneMappingsDict()
        {
            Dictionary<string, string> smDict = new Dictionary<string, string>();
            foreach (SceneMapping sm in sceneMappings)
            {
                smDict.Add(sm.pageTypeName, sm.scenePath);
            }
            return smDict;
        }

        [ShowInProductEditor]
        public List<SceneMapping> sceneMappings { get; set; }

        private string[] _scenePaths;

        [ShowInProductEditor]
        public string[] scenePaths
        {
            get
            {
                if (_scenePaths == null)
                {
                    _scenePaths = new string[0];
                }
                return _scenePaths;
            }
            set
            {
                if (value != null)
                    _scenePaths = value;
            }
        }

        [ShowInProductEditor]
        public List<SceneExtension> sceneExtensions { get; set; }

        [ShowInProductEditor]
        public bool stopAudioWhenLeavingPage { get; set; }
        #endregion

        #region Synching
        [ShowInProductEditor]
        public bool autoSynchQuestInfos { get; set; }

        /// <summary>
        /// If set, quests called by StartQuest actions will update before being started if possible and load if needed.
        /// </summary>
        [ShowInProductEditor]
        public bool autoUpdateSubquests { get; set; }

        [ShowInProductEditor, JsonProperty]
        private bool offerManualUpdate4QuestInfos { get; set; }
        [JsonIgnore]
        public bool OfferManualUpdate4QuestInfos
        {
            get
            {
                if (Author.LoggedIn)
                {
                    return Author.OfferManualUpdate;
                }
                else
                {
                    return offerManualUpdate4QuestInfos;
                }
            }
        }
        #endregion


        #region Map
        [ShowInProductEditor(StartSection = "Map & Markers:")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MapProvider mapProvider { get; set; }

        [ShowInProductEditor]
        public string mapBaseUrl { get; set; }

        [ShowInProductEditor]
        public string mapKey { get; set; }

        [ShowInProductEditor]
        public string mapID { get; set; }

        [ShowInProductEditor]
        public string mapTileImageExtension { get; set; }

        public bool useMapOffline { get; set; }

        [ShowInProductEditor]
        public float mapMinimalZoom { get; set; }

        [ShowInProductEditor]
        public float mapDeltaZoom { get; set; }


        [ShowInProductEditor]
        [JsonConverter(typeof(StringEnumConverter))]
        public MapStartPositionType mapStartPositionType { get; set; }

        [ShowInProductEditor]
        public bool mapStartAtLocation { get; set; }

        [ShowInProductEditor]
        public double mapStartAtLongitude { get; set; }

        [ShowInProductEditor]
        public double mapStartAtLatitude { get; set; }

        [ShowInProductEditor]
        public ImagePath hotspotMarker
        {
            get
            {
                if (!__JSON_Currently_Parsing && _hotspotMarker == null)
                {
                    _hotspotMarker = new ImagePath(Marker.DEFAULT_MARKER_PATH);
                }
                return _hotspotMarker;
            }
            set
            {
                if (value == null || value.path == null || value.path.Equals(""))
                {
                    _hotspotMarker = new ImagePath(Marker.DEFAULT_MARKER_PATH);
                }
                else
                {
                    _hotspotMarker = value;
                }
            }
        }
        [JsonIgnore]
        private ImagePath _hotspotMarker;

        [ShowInProductEditor]
        public ImagePath marker
        {
            get
            {
                if (!__JSON_Currently_Parsing && _marker == null)
                {
                    _marker = new ImagePath(Marker.DEFAULT_MARKER_PATH);
                }
                return _marker;
            }
            set
            {
                if (value == null || value.path == null || value.path.Equals(""))
                {
                    _marker = new ImagePath(Marker.DEFAULT_MARKER_PATH);
                }
                else
                {
                    _marker = value;
                }
            }
        }
        [JsonIgnore]
        private ImagePath _marker;

        [ShowInProductEditor]
        public float markerHeightUnits { get; set; }

        [ShowInProductEditor]
        public float markerHeightMinMM { get; set; }

        [ShowInProductEditor]
        public float markerHeightMaxMM { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 markerColor { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 markerSymbolFGColor { get; set; }

        [JsonIgnore]
        private byte _markerBGAlpha = 168;

        [ShowInProductEditor]
        public byte markerBGAlpha
        {
            get
            {
                return _markerBGAlpha;
            }
            set
            {
                if (value < 0)
                {
                    _markerBGAlpha = 0;
                    return;
                }
                if (value > 255)
                {
                    _markerBGAlpha = 255;
                    return;
                }
                _markerBGAlpha = value;
            }
        }

        [ShowInProductEditor]
        public float mapButtonHeightUnits { get; set; }

        [ShowInProductEditor]
        public float mapButtonHeightMinMM { get; set; }

        [ShowInProductEditor]
        public float mapButtonHeightMaxMM { get; set; }


        [ShowInProductEditor(StartSection = "Categories & Filters:")]
        public bool foldableCategoryFilters { get; set; }

        [ShowInProductEditor]
        public bool categoryFiltersStartFolded { get; set; }

        [ShowInProductEditor]
        public bool categoryFoldersStartFolded { get; set; }

        /// <summary>
        /// Used as characterization of the quest infos, e.g. to determine the shown symbols in the foyer list.
        /// </summary>
        /// <value>The main category set.</value>
        [ShowInProductEditor]
        public string mainCategorySet { get; set; }

        public CategorySet GetMainCategorySet()
        {
            return CategorySets.Find(cat => cat.name == mainCategorySet);
        }

        [ShowInProductEditor]
        public List<CategorySet> CategorySets
        {
            get
            {
                if (_categorySets == null)
                {
                    _categorySets = new List<CategorySet>();
                }
                categoryDict = new Dictionary<string, Category>();
                foreach (CategorySet cs in _categorySets)
                {
                    foreach (Category c in cs.categories)
                    {
                        categoryDict[c.id] = c;
                    }
                }
                return _categorySets;
            }
            set
            {
                _categorySets = value;
            }
        }

        [ShowInProductEditor]
        public string defaultCategory { get; set; }

        [JsonIgnore]
        private List<CategorySet> _categorySets;

        [JsonIgnore]
        public Dictionary<string, Category> categoryDict;

        [JsonIgnore]
        private float _disabledAlpha = 0.5f;

        [ShowInProductEditor]
        public float disabledAlpha
        {
            get
            {
                return _disabledAlpha;
            }
            set
            {
                if (value < 0f)
                {
                    _disabledAlpha = 0f;
                    return;
                }
                if (value > 1f)
                {
                    _disabledAlpha = 1f;
                    return;
                }
                _disabledAlpha = value;
            }
        }
        #endregion


        #region UI Strategies
        [ShowInProductEditor(StartSection = "UI Strategies:")]
        public bool hideFooterIfPossible { get; set; }

        [ShowInProductEditor]
        public bool autoScrollNewText { get; set; }
        #endregion


        #region Layout
        [ShowInProductEditor(StartSection = "Layout & Colors:")]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 mainFgColor { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 mainBgColor { get; set; }

        [ShowInProductEditor]
        public int mainFontSize { get; set; }

        [ShowInProductEditor]
        public int maxFontSize { get; set; }

        [ShowInProductEditor]
        public bool showShadows { get; set; }

        [ShowInProductEditor]
        public ImagePath topLogo { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(StringEnumConverter))]
        public HeaderMiddleButtonPolicy headerMiddleButtonPolicy { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 headerBgColor { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 headerButtonBgColor { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 headerButtonFgColor { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 contentBackgroundColor { get; set; }

        [ShowInProductEditor]
        public float headerHeightUnits { get; set; }

        [ShowInProductEditor]
        public float headerHeightMinMM { get; set; }

        [ShowInProductEditor]
        public float headerHeightMaxMM { get; set; }

        [ShowInProductEditor]
        public float contentHeightUnits { get; set; }

        [ShowInProductEditor]
        public float contentTopMarginUnits { get; set; }

        [ShowInProductEditor]
        public float contentDividerUnits { get; set; }

        [ShowInProductEditor]
        public float contentBottomMarginUnits { get; set; }

        [ShowInProductEditor]
        public float imageAreaHeightMinUnits { get; set; }

        [ShowInProductEditor]
        public float imageAreaHeightMaxUnits { get; set; }

        [ShowInProductEditor]
        public bool fitExceedingImagesIntoArea { get; set; }

        [ShowInProductEditor]
        public float footerHeightUnits { get; set; }

        [ShowInProductEditor]
        public float footerHeightMinMM { get; set; }

        [ShowInProductEditor]
        public float footerHeightMaxMM { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 footerBgColor { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 footerButtonBgColor { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 footerButtonFgColor { get; set; }

        /// <summary>
        /// The width of space on each side of the display that is intended to be left free (in permill of th absolute display width).
        /// </summary>
        /// <value>The side per mill.</value>
        [ShowInProductEditor]
        public float borderWidthUnits { get; set; }

        [ShowInProductEditor]
        public float overlayButtonSizeUnits { get; set; }

        [ShowInProductEditor]
        public float overlayButtonSizeMinMM { get; set; }

        [ShowInProductEditor]
        public float overlayButtonSizeMaxMM { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 overlayButtonBgColor { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 overlayButtonFgColor { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 overlayButtonFgDisabledColor { get; set; }

        [ShowInProductEditor(StartSection = "Menu:"), JsonProperty]
        private bool showEmptyMenuEntries { get; set; }
        [JsonIgnore]
        public bool ShowEmptyMenuEntries
        {
            get
            {
                if (Author.LoggedIn)
                {
                    return Author.ShowEmptyMenuEntries;
                }
                else
                {
                    return showEmptyMenuEntries;
                }
            }
        }

        [ShowInProductEditor]
        public float menuEntryHeightUnits { get; set; }

        [ShowInProductEditor]
        public float menuEntryHeightMinMM { get; set; }

        [ShowInProductEditor]
        public float menuEntryHeightMaxMM { get; set; }

        [ShowInProductEditor]
        public float menuEntryWidthUnits { get; set; }

        [ShowInProductEditor]
        public float menuEntryWidthMinMM { get; set; }

        [ShowInProductEditor]
        public float menuEntryWidthMaxMM { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 menuFrameColor { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 menuBGColor { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 menuFGColor { get; set; }

        [ShowInProductEditor]
        public bool menuInhibitsInteraction { get; set; }

        [ShowInProductEditor]
        public bool menu2ShownInQuests { get; set; }

        [ShowInProductEditor, JsonProperty]
        internal bool offerLeaveQuestOnEachPage { get; set; }
        [JsonIgnore]
        public bool OfferLeaveQuests
        {
            get
            {
                return offerLeaveQuestOnEachPage || Author.LoggedIn;
            }
            set
            {
                offerLeaveQuestOnEachPage = value;
            }
        }


        [ShowInProductEditor]
        public bool offerFeedback { get; set; }

        [ShowInProductEditor]
        public bool offerAuthorLogin { get; set; }

        [ShowInProductEditor]
        public bool defineAuthorBackDoor { get; set; }

        [ShowInProductEditor]
        public string acceptedAuthorEmail { get; set; }

        [ShowInProductEditor]
        public string acceptedAuthorPassword { get; set; }

        [ShowInProductEditor(StartSection = "List Entries:")]
        public float listEntryHeightUnits { get; set; }

        [ShowInProductEditor]
        public float listEntryHeightMinMM { get; set; }

        [ShowInProductEditor]
        public float listEntryHeightMaxMM { get; set; }

        [ShowInProductEditor]
        public bool listEntryUseTwoLines { get; set; }


        ListEntryDividingMode _listEntryDividingMode;

        [ShowInProductEditor]
        [JsonConverter(typeof(StringEnumConverter))]
        public ListEntryDividingMode listEntryDividingMode
        {
            get
            {
                return _listEntryDividingMode;
            }
            set
            {
                _listEntryDividingMode = value;
            }
        }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 listLineColor { get; set; }

        [ShowInProductEditor]
        public int listStartLineWidth { get; set; }

        [ShowInProductEditor]
        public int dividingLineWidth { get; set; }

        [ShowInProductEditor]
        public int listEndLineWidth { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 listEntryFgColor { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 listEntryBgColor { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 listEntrySecondFgColor { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 listEntrySecondBgColor { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 listBgColor { get; set; }

        [ShowInProductEditor(StartSection = "Internal:")]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 emulationColor { get; set; }

        #endregion


        #region Defaults

        /// <summary>
        /// Initializes a new instance of the <see cref="GQ.Client.Conf.Config"/> class and intializes it with generic default values.
        /// 
        /// This constructor is used by the ProductManager (explicit) as well as the JSON.Net deserialize method (via reflection).
        /// </summary>
        public Config()
        {
            // set default values:
            idExtension = "";
            assetAddOns = new string[] { };
            autoStartQuestID = 0;
            autostartIsPredeployed = false;
            keepAutoStarting = true;
            questInfoViews = new string[] { QuestInfoView.List.ToString(), QuestInfoView.Map.ToString() };
            mapStartPositionType = MapStartPositionType.CenterOfMarkers;
            cloudQuestsVisible = true;
            showCloudQuestsImmediately = false;
            downloadAllCloudQuestOnStart = false;
            localQuestsDeletable = true;
            showHiddenQuests = false;
            downloadStrategy = DownloadStrategy.UPFRONT;

            timeoutMS = 60000L;
            maxIdleTimeMS = 9000L;
            maxParallelDownloads = 15;

            autoSynchQuestInfos = true;
            autoUpdateSubquests = false;
            offerManualUpdate4QuestInfos = !autoSynchQuestInfos;

            acceptedPageTypes = new string[0];
            sceneMappings = new List<SceneMapping>();
            scenePaths = new string[0];
            sceneExtensions = new List<SceneExtension>();
            offerLeaveQuestOnEachPage = true;
            stopAudioWhenLeavingPage = true;

            // Map:
            mapProvider = MapProvider.OpenStreetMap;
            mapBaseUrl = "https://b.tile.openstreetmap.org";
            mapKey = "";
            mapID = "";
            mapTileImageExtension = ".png";
            //			mapKey = "pk.eyJ1IjoiaG9sZ2VybXVlZ2dlIiwiYSI6Im1MLW9rN2MifQ.6KebeI6zZ3QNe18n2AQyaw";
            //			mapID = "mapbox.streets";
            //			mapTileImageExtension = "@2x.png?access_token=" + mapKey;
            useMapOffline = false;
            mapMinimalZoom = 7.0f;
            mapDeltaZoom = 0.5f;
            markerHeightUnits = 55f;
            markerSymbolFGColor = Color.black;
            mapButtonHeightUnits = 55f;
            mapButtonHeightMinMM = 7f;
            mapButtonHeightMaxMM = 12f;

            // UI Strategies:
            hideFooterIfPossible = false;
            autoScrollNewText = true;

            // Layout:
            mainBgColor = Color.white;
            mainFgColor = Color.black;
            mainFontSize = 60;
            maxFontSize = 100;
            showShadows = true;
            headerMiddleButtonPolicy = HeaderMiddleButtonPolicy.TopLogo;
            headerHeightUnits = 60f;
            contentHeightUnits = 750f;
            imageAreaHeightMinUnits = 150f;
            imageAreaHeightMaxUnits = 350f;
            footerHeightUnits = 75f;
            contentDividerUnits = 0;
            borderWidthUnits = 0;
            headerBgColor = Color.white;
            headerButtonBgColor = GQColor.transparent;
            headerButtonFgColor = Color.black;
            contentBackgroundColor = Color.white;
            footerBgColor = Color.white;
            footerButtonBgColor = GQColor.transparent;
            footerButtonFgColor = Color.black;
            overlayButtonSizeUnits = 50;
            overlayButtonSizeMinMM = 6;
            overlayButtonSizeMaxMM = 16;
            overlayButtonBgColor = GQColor.transparent;
            overlayButtonFgColor = Color.black;
            overlayButtonFgDisabledColor = new Color(159f, 159f, 159f, 187f);

            // Foyer List:
            listEntryUseTwoLines = false;
            listEntryHeightUnits = 45f;
            listEntryFgColor = new Color(159f, 159f, 159f, 255f);
            listEntryBgColor = mainFgColor;

            listLineColor = mainFgColor;
            listStartLineWidth = 5;
            dividingLineWidth = 5;
            listEndLineWidth = 5;

            // Menu:
            showEmptyMenuEntries = false;
            categoryDict = new Dictionary<string, Category>();
            foldableCategoryFilters = true;
            categoryFiltersStartFolded = true;
            categoryFoldersStartFolded = true;
            menuEntryHeightUnits = 35f;
            menuEntryWidthUnits = 400f;
            menuInhibitsInteraction = false;
            menu2ShownInQuests = true;
            menuFrameColor = Color.grey;
            menuBGColor = Color.white;
            menuFGColor = Color.black;
            offerFeedback = false;
            offerAuthorLogin = false;
            defineAuthorBackDoor = false;
            acceptedAuthorEmail = "author@email.com";
            acceptedAuthorPassword = "secret";

            // Internal:
            emulationColor = new Color(255f, 182f, 182f, 255f);
        }
        #endregion

    }

    public enum HeaderMiddleButtonPolicy
    {
        TopLogo,
        QuestTitle
    }


    public enum DownloadStrategy
    {
        UPFRONT,
        LAZY,
        BACKGROUND
    }

    public enum QuestInfoView
    {
        List,
        Map
    }

    public enum MapStartPositionType
    {
        CenterOfMarkers,
        PlayerPosition,
        FixedPosition
    }

    public enum PageType
    {
        StartAndExitScreen,
        NPCTalk,
        ImageWithText,
        Menu,
        MultipleChoiceQuestion,
        TextQuestion,
        AudioRecord,
        ImageCapture,
        VideoPlay,
        WebPage,
        MapOSM,
        Navigation,
        TagScanner,
        ReadNFC,
        Custom,
        MetaData
    }

    public enum MapProvider
    {
        OpenStreetMap,
        MapBox
    }

    public enum ListEntryDividingMode
    {
        SeparationLines,
        AlternatingColors
    }

    public enum AssetAddOn
    {
        UniWebView
    }

    public class Color32Converter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color32);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            reader.Read();
            byte r = (byte)reader.ReadAsInt32();
            reader.Read();
            byte g = (byte)reader.ReadAsInt32();
            reader.Read();
            byte b = (byte)reader.ReadAsInt32();
            reader.Read();
            byte a = (byte)reader.ReadAsInt32();
            reader.Read();

            Color32 c = new Color32(r, g, b, a);
            return c;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Color32 c = (Color32)value;
            writer.WriteStartObject();
            writer.WritePropertyName("r");
            writer.WriteValue(c.r);
            writer.WritePropertyName("g");
            writer.WriteValue(c.g);
            writer.WritePropertyName("b");
            writer.WriteValue(c.b);
            writer.WritePropertyName("a");
            writer.WriteValue(c.a);
            writer.WriteEndObject();
        }
    }

    public static class GQColor
    {

        public static readonly Color32 transparent = new Color32(255, 255, 255, 0);
    }

    public class SceneMapping
    {

        public const string ProjectScenesRootPath = "Assets/Scenes/Pages/";
        public static readonly string ProductScenesRootPath = Files.CombinePath(ConfigurationManager.RUNTIME_PRODUCT_DIR, "Scenes/Pages/");

        public SceneMapping(string pageType, string scenePath)
        {
            this.pageTypeName = pageType;
            this.scenePath = scenePath;
        }

        /// <summary>
        /// Pages of this type will use the scene given by the scenePath.
        /// </summary>
        public string pageTypeName;

        /// <summary>
        /// Path of the scene that is used for the given page type.
        /// </summary>
        public string scenePath;
    }

    public class SceneExtension
    {
        /// <summary>
        /// The scene path.
        /// </summary>
        public string scene;

        /// <summary>
        /// The path to gameobject where the prefab should be instantiated.
        /// </summary>
        public string root;

        /// <summary>
        /// The path to the prefab. It is relative to the folder Assets/ConfigAssets/Resources folder.
        /// </summary>
        public string prefab;
    }

    public class ImagePath
    {
        public readonly string path;

        public ImagePath(string path)
        {
            this.path = path;
        }

        public override string ToString()
        {
            return "ImagePath: " + path;
        }

        public override bool Equals(System.Object obj)
        {
            // Other null?
            if (obj == null)
                return path == null || path.Equals("");

            // Compare run-time types.
            if (GetType() != obj.GetType())
                return false;

            return path == ((ImagePath)obj).path;
        }

        public override int GetHashCode()
        {
            return path.GetHashCode();
        }
    }

    public class Category
    {
        public string id;

        /// <summary>
        /// The display name.
        /// </summary>
        public string name;

        public string folderName;

        public ImagePath symbol;

        public Category()
        {
            this.id = "";
            name = "";
            folderName = "";
            symbol = null;
        }

        [JsonConstructor]
        public Category(string id, string name, string folderName, string symbolPath)
        {
            this.id = id;
            this.name = name;
            this.folderName = folderName ?? "";
            this.symbol = new ImagePath(symbolPath);
        }

    }

    public class CategorySet
    {
        public string name;

        public List<Category> categories;

        [JsonConstructor]
        public CategorySet(string name, List<Category> categories)
        {
            this.name = name;
            if (categories == null)
                categories = new List<Category>();
            this.categories = categories;
        }

        public CategorySet()
        {
            name = "";
            categories = new List<Category>();
        }
    }

    public class ShowInProductEditor : Attribute
    {
        public string StartSection { get; set; }
    }
}


