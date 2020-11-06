using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Code.GQClient.UI.author;
using Code.GQClient.UI.map;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

#if UNITY_EDITOR

#endif

namespace Code.GQClient.Conf
{
    /// <summary>
    /// Config class specifies textual parameters of a product. It is used both at runtime to initilize the app's branding details from and 
    /// at editor time to back the product editor view and store the parameters while we use the editor.
    /// </summary>
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
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

        #region StartScene Layout

        // Content is shown in special area of editor
        public bool preserveAspectOfSplashFG { get; set; }

        #endregion


        #region General

        [ShowInProductEditor(StartSection = "General:")]
        public string id { get; set; }

        [ShowInProductEditor] public string idExtension { get; set; }

        [ShowInProductEditor] public string name { get; set; }

        [ShowInProductEditor] public int portal { get; set; }

        [ShowInProductEditor] public string[] assetAddOns { get; set; }

#if UNITY_EDITOR
        [ShowInProductEditor]
        [JsonConverter(typeof(StringEnumConverter))]
        public UnityEditor.AndroidSdkVersions androidMinSDKVersion { get; set; }


        [ShowInProductEditor]
        [JsonConverter(typeof(StringEnumConverter))]
        public UnityEditor.iOSTargetDevice iOsDeviceTypes { get; set; }
#endif

        [ShowInProductEditor] public int autoStartQuestID { get; set; }

        [ShowInProductEditor] public bool autostartIsPredeployed { get; set; }

        [ShowInProductEditor] public bool keepAutoStarting { get; set; }

        [ShowInProductEditor] public long timeoutMS { get; set; }

        [ShowInProductEditor] public long maxIdleTimeMS { get; set; }

        [ShowInProductEditor] public int maxParallelDownloads { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(StringEnumConverter))]
        public DownloadStrategy downloadStrategy { get; set; }

        [ShowInProductEditor] public string nameForQuestSg { get; set; }

        [ShowInProductEditor] public string nameForQuestsPl { get; set; }

        [ShowInProductEditor] public string[] questInfoViews { get; set; }

        [ShowInProductEditor] public float topicButtonAspectRatio { get; set; }

        public bool cloudQuestsVisible { get; set; }

        public bool showCloudQuestsImmediately { get; set; }

        public bool downloadAllCloudQuestOnStart { get; set; }

        [ShowInProductEditor] public bool localQuestsDeletable { get; set; }

        [ShowInProductEditor, JsonProperty] private bool showHiddenQuests { get; set; }

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

        [ShowInProductEditor, JsonProperty] public bool showOnlyLocalQuests { get; set; }

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

        [ShowInProductEditor] public List<SceneMapping> sceneMappings { get; set; }

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

        [ShowInProductEditor] public List<SceneExtension> sceneExtensions { get; set; }

        [ShowInProductEditor] public bool stopAudioWhenLeavingPage { get; set; }

        #endregion

        #region Synching

        [ShowInProductEditor(StartSection = "Synchronization:")]
        public bool autoSyncQuestInfos { get; set; }

        [ShowInProductEditor] public bool autoLoadQuests { get; set; }

        [ShowInProductEditor] public bool autoUpdateQuests { get; set; }

        /// <summary>
        /// If set, quests called by StartQuest actions will update before being started if possible and load if needed.
        /// </summary>
        [ShowInProductEditor]
        public bool autoUpdateSubquests { get; set; }

        [ShowInProductEditor, JsonProperty] private bool offerManualUpdate4QuestInfos { get; set; }

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
        public MapStartPositionType mapStartPositionType { get; set; }

        [ShowInProductEditor] public double mapStartAtLongitude { get; set; }

        [ShowInProductEditor] public double mapStartAtLatitude { get; set; }

        [ShowInProductEditor] public float mapStartZoom { get; set; }

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

        [JsonIgnore] private ImagePath _hotspotMarker;

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

        [JsonIgnore] private ImagePath _marker;

        [ShowInProductEditor] public float markerHeightUnits { get; set; }

        [ShowInProductEditor] public float markerHeightMinMM { get; set; }

        [ShowInProductEditor] public float markerHeightMaxMM { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 markerColor { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 markerSymbolFGColor { get; set; }

        [JsonIgnore] private byte _markerBGAlpha = 168;

        [ShowInProductEditor]
        public byte markerBGAlpha
        {
            get { return _markerBGAlpha; }
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

        [ShowInProductEditor] public float mapButtonHeightUnits { get; set; }

        [ShowInProductEditor] public float mapButtonHeightMinMM { get; set; }

        [ShowInProductEditor] public float mapButtonHeightMaxMM { get; set; }

        [JsonIgnore] private float _disabledAlpha = 0.5f;

        [ShowInProductEditor]
        public float disabledAlpha
        {
            get { return _disabledAlpha; }
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
        [JsonConverter(typeof(StringEnumConverter))]
        public TaskUIMode taskUI { get; set; }

        #endregion


        #region Layout

        [ShowInProductEditor(StartSection = "Layout & Colors:")]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 mainFgColor { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 mainBgColor { get; set; }

        [ShowInProductEditor] public int colorPaletteSize { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32ArrayConverter))]
        public Color32[] colorPalette { get; set; }


        [JsonIgnore]
        public Color32 NextPaletteColor
        {
            get
            {
                var color = colorPalette[_paletteColorCounter];
                _paletteColorCounter = (_paletteColorCounter + 1) % colorPaletteSize;
                return color;
            }
        }

        public static void ResetColorPalette()
        {
            _paletteColorCounter = 0;
        }

        public static int _paletteColorCounter = 0;


        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 paletteFGColor { get; set; }

        [ShowInProductEditor] public int mainFontSize { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(StringEnumConverter))]
        public AlignmentOption textAlignment { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 textLinkColor { get; set; }

        [ShowInProductEditor] public float lineSpacing { get; set; }

        [ShowInProductEditor] public bool showShadows { get; set; }

        [ShowInProductEditor] public ImagePath topLogo { get; set; }

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

        [ShowInProductEditor] public float headerHeightUnits { get; set; }

        [ShowInProductEditor] public float headerHeightMinMM { get; set; }

        [ShowInProductEditor] public float headerHeightMaxMM { get; set; }

        [ShowInProductEditor(StartSection = "Content:")]
        public float contentHeightUnits { get; set; }

        [ShowInProductEditor] public float contentTopMarginUnits { get; set; }

        [ShowInProductEditor] public float contentDividerUnits { get; set; }

        [ShowInProductEditor] public float contentBottomMarginUnits { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 contentBackgroundColor { get; set; }

        [ShowInProductEditor] public float imageAreaHeightMinUnits { get; set; }

        [ShowInProductEditor] public float imageAreaHeightMaxUnits { get; set; }

        [ShowInProductEditor] public bool fitExceedingImagesIntoArea { get; set; }

        [ShowInProductEditor] public bool autoScrollNewText { get; set; }

        [ShowInProductEditor(StartSection = "Footer:")]
        public float footerHeightUnits { get; set; }

        [ShowInProductEditor] public float footerHeightMinMM { get; set; }

        [ShowInProductEditor] public float footerHeightMaxMM { get; set; }

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

        [ShowInProductEditor(StartSection = "Overlays:")]
        public float overlayButtonSizeUnits { get; set; }

        [ShowInProductEditor] public float overlayButtonSizeMinMM { get; set; }

        [ShowInProductEditor] public float overlayButtonSizeMaxMM { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 overlayButtonBgColor { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 overlayButtonFgColor { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 overlayButtonFgDisabledColor { get; set; }

        // ----------------------------------------
        // MENU:

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

        [ShowInProductEditor] public float menuEntryHeightUnits { get; set; }

        [ShowInProductEditor] public float menuEntryHeightMinMM { get; set; }

        [ShowInProductEditor] public float menuEntryHeightMaxMM { get; set; }

        [ShowInProductEditor] public float menuEntryWidthUnits { get; set; }

        [ShowInProductEditor] public float menuEntryWidthMinMM { get; set; }

        [ShowInProductEditor] public float menuEntryWidthMaxMM { get; set; }

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
        [JsonConverter(typeof(Color32Converter))]
        public Color32 categoryFolderBGColor { get; set; }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 categoryEntryBGColor { get; set; }

        [ShowInProductEditor] public bool menuInhibitsInteraction { get; set; }

        [ShowInProductEditor] public bool menu2ShownInQuests { get; set; }

        [ShowInProductEditor, JsonProperty] internal bool offerLeaveQuestOnEachPage { get; set; }

        [JsonIgnore]
        public bool OfferLeaveQuests
        {
            get { return offerLeaveQuestOnEachPage || Author.LoggedIn; }
            set { offerLeaveQuestOnEachPage = value; }
        }

        [ShowInProductEditor] public bool warnWhenLeavingQuest { get; set; }

        [ShowInProductEditor] public string warnDialogTitleWhenLeavingQuest { get; set; }

        [ShowInProductEditor] public string warnDialogMessageWhenLeavingQuest { get; set; }

        [ShowInProductEditor] public string warnDialogOKWhenLeavingQuest { get; set; }

        [ShowInProductEditor] public string warnDialogCancelWhenLeavingQuest { get; set; }


        [ShowInProductEditor(StartSection = "Offered Canvases by 2nd Menu:")]
        public bool defineAuthorBackDoor { get; set; }

        [ShowInProductEditor] public string acceptedAuthorEmail { get; set; }

        [ShowInProductEditor] public string acceptedAuthorPassword { get; set; }

        [ShowInProductEditor] public bool offerFeedback { get; set; }

        [ShowInProductEditor] public bool offerAuthorLogin { get; set; }

        [ShowInProductEditor] public bool offerPartnersInfo { get; set; }

        [ShowInProductEditor] public bool showPartnersInfoAtStart { get; set; }


        [ShowInProductEditor(StartSection = "List Entries:")]
        public float listEntryHeightUnits { get; set; }

        [ShowInProductEditor] public float listEntryHeightMinMM { get; set; }

        [ShowInProductEditor] public float listEntryHeightMaxMM { get; set; }

        [ShowInProductEditor] public bool listEntryUseTwoLines { get; set; }


        ListEntryDividingMode _listEntryDividingMode;

        [ShowInProductEditor]
        [JsonConverter(typeof(StringEnumConverter))]
        public ListEntryDividingMode listEntryDividingMode
        {
            get => _listEntryDividingMode;
            set => _listEntryDividingMode = value;
        }

        [ShowInProductEditor]
        [JsonConverter(typeof(Color32Converter))]
        public Color32 listLineColor { get; set; }

        [ShowInProductEditor] public int dividingLineWidth { get; set; }

        [ShowInProductEditor] public int listEndLineWidth { get; set; }

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
        /// Initializes a new instance of the <see cref="Config"/> class and initializes it with generic default values.
        /// 
        /// This constructor is used by the ProductManager (explicit) as well as the JSON.Net deserialize method (via reflection).
        /// </summary>
        public Config()
        {
            // set default values:

            // start scene layout:
            preserveAspectOfSplashFG = true;

            // general:
            idExtension = "";
            assetAddOns = new string[] { };
            autoStartQuestID = 0;
            autostartIsPredeployed = false;
            keepAutoStarting = true;
            questInfoViews = new string[]
            {
                QuestInfoView.List.ToString(),
                QuestInfoView.TopicTree.ToString(),
                QuestInfoView.Map.ToString()
            };
#if UNITY_EDITOR
            iOsDeviceTypes = UnityEditor.iOSTargetDevice.iPhoneAndiPad;
#endif
            topicButtonAspectRatio = 1.0f;
            mapStartPositionType = MapStartPositionType.CenterOfMarkers;
            cloudQuestsVisible = true;
            showCloudQuestsImmediately = false;
            downloadAllCloudQuestOnStart = false;
            localQuestsDeletable = true;
            showHiddenQuests = false;
            downloadStrategy = DownloadStrategy.UPFRONT;

            nameForQuestSg = "Quest";
            nameForQuestsPl = "Quests";

            timeoutMS = 60000L;
            maxIdleTimeMS = 9000L;
            maxParallelDownloads = 15;

            autoSyncQuestInfos = true;
            autoLoadQuests = false;
            autoUpdateQuests = false;
            autoUpdateSubquests = false;
            offerManualUpdate4QuestInfos = !autoSyncQuestInfos;

            acceptedPageTypes = new string[0];
            sceneMappings = new List<SceneMapping>();
            scenePaths = new string[0];
            sceneExtensions = new List<SceneExtension>();
            offerLeaveQuestOnEachPage = true;
            warnWhenLeavingQuest = true;
            warnDialogTitleWhenLeavingQuest = "Aktuelles Quest verlassen?";
            warnDialogMessageWhenLeavingQuest =
                "Sie müssten sie dann evtl. wieder ganz von vorne beginnen. Wollen Sie das Quest ...";
            warnDialogOKWhenLeavingQuest = "Verlassen";
            warnDialogCancelWhenLeavingQuest = "Fortsetzen";
            stopAudioWhenLeavingPage = true;

            // Map:
            mapStartZoom = 14.0f;
            markerHeightUnits = 55f;
            markerSymbolFGColor = Color.black;
            mapButtonHeightUnits = 55f;
            mapButtonHeightMinMM = 7f;
            mapButtonHeightMaxMM = 12f;

            // UI Strategies:
            hideFooterIfPossible = false;
            taskUI = TaskUIMode.ProgressAtBottom;
            autoScrollNewText = true;

            // Layout:
            mainBgColor = Color.white;
            mainFgColor = Color.black;
            colorPaletteSize = 4;
            colorPalette = new Color32[] {Color.blue, Color.green, Color.yellow, Color.red};
            paletteFGColor = Color.white;
            mainFontSize = 60;
            textAlignment = AlignmentOption.Left;
            textLinkColor = Color.blue;
            lineSpacing = 10f;
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
            dividingLineWidth = 5;
            listEndLineWidth = 5;

            // Menu:
            showEmptyMenuEntries = false;
            menuEntryHeightUnits = 35f;
            menuEntryWidthUnits = 400f;
            menuInhibitsInteraction = false;
            menu2ShownInQuests = true;
            menuFrameColor = Color.grey;
            menuBGColor = Color.white;
            menuFGColor = Color.black;
            categoryFolderBGColor = menuBGColor;
            categoryEntryBGColor = menuBGColor;
            offerPartnersInfo = false;
            showPartnersInfoAtStart = false;
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

    public enum AlignmentOption
    {
        Left,
        Center,
        Right,
        Justified,
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
        Map,
        TopicTree
    }

    public enum MapStartPositionType
    {
        CenterOfMarkers,
        PlayerPosition,
        FixedPosition
    }

    public enum TaskUIMode
    {
        Dialog,
        ProgressAtBottom
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

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            reader.Read();
            var r = (byte) reader.ReadAsInt32();
            reader.Read();
            var g = (byte) reader.ReadAsInt32();
            reader.Read();
            var b = (byte) reader.ReadAsInt32();
            reader.Read();
            var a = (byte) reader.ReadAsInt32();
            reader.Read();

            var c = new Color32(r, g, b, a);
            return c;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var c = (Color32) value;
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

    public class Color32ArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color32[]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var colors = new Color32[10];
            reader.Read();
            for (var i = 0; i < 10; i++)
            {
                reader.Read();
                var r = (byte) reader.ReadAsInt32();
                reader.Read();
                var g = (byte) reader.ReadAsInt32();
                reader.Read();
                var b = (byte) reader.ReadAsInt32();
                reader.Read();
                var a = (byte) reader.ReadAsInt32();
                reader.Read();
                reader.Read();
                colors[i] = new Color32(r, g, b, a);
            }

            return colors;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var colors = (Color32[]) value;
            writer.WriteStartArray();

            foreach (var c in colors)
            {
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

            writer.WriteEndArray();
        }
    }

    public static class GQColor
    {
        public static readonly Color32 transparent = new Color32(255, 255, 255, 0);
    }

    public class SceneMapping
    {
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

            return path == ((ImagePath) obj).path;
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