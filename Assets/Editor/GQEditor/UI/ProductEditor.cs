// #define DEBUG_LOG

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Text;
using System.Reflection;
using GQ.Editor.Building;
using System.Collections.Generic;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.UI.layout;
using Code.GQClient.Util;
using Code.QM.Util;
using GQ.Editor.Util;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Object = System.Object;

namespace GQ.Editor.UI
{
    public class ProductEditor : EditorWindow
    {
        public static GUIStyle TextareaGUIStyle { get; private set; }

        private Texture warnIcon;
        Vector2 scrollPos;
        Vector2 scrollPosRT;

        private static string _currentBuildName = null;

        public static string CurrentBuildName
        {
            get
            {
                if (_currentBuildName == null)
                {
                    Config curConfig = updateFromCurrentConfig();
                    _currentBuildName = curConfig == null ? "[null]" : curConfig.id;
                }

                return _currentBuildName;
            }
            set { _currentBuildName = value; }
        }

        internal const string WARN_ICON_PATH = "Assets/Editor/GQEditor/images/warn.png";

        private static bool _buildIsDirty = false;

        public static bool BuildIsDirty
        {
            get { return _buildIsDirty; }
            set { _buildIsDirty = value; }
        }

        private static ProductEditor _instance = null;

        public static ProductEditor Instance
        {
            get { return _instance; }
            private set { _instance = value; }
        }

        private static bool _isCurrentlyPreparingProduct = false;

        public static bool IsCurrentlyPreparingProduct
        {
            get { return _isCurrentlyPreparingProduct; }
            set { _isCurrentlyPreparingProduct = value; }
        }


        [MenuItem("Window/QuestMill Product Editor")]
        public static void Init()
        {
            var editorAsm = typeof(UnityEditor.Editor).Assembly;
            var inspectorWindowType = editorAsm.GetType("UnityEditor.InspectorWindow");
            ProductEditor editor;
            if (inspectorWindowType != null)
                editor = EditorWindow.GetWindow<ProductEditor>("GQ Product", true, inspectorWindowType);
            else
                editor = EditorWindow.GetWindow<ProductEditor>(typeof(ProductEditor));
            editor.Show();
            Instance = editor;
        }

        int selectedProductIndex;
        int mainVersionNumber;
        int yearVersionNumber;
        int monthVersionNumber;
        int buildVersionNumber;

        static ProductManager _pm;

        public static ProductManager Pm
        {
            get
            {
                if (_pm == null)
                    _pm = ProductManager.Instance;
                return _pm;
            }
            set { _pm = value; }
        }

        #region Initialization

        public void OnEnable()
        {
            Instance = this;

            readStateFromEditorPrefs();
            warnIcon = (Texture) AssetDatabase.LoadAssetAtPath(WARN_ICON_PATH, typeof(Texture));
        }

        void readStateFromEditorPrefs()
        {
            selectedProductIndex = EditorPrefs.HasKey("selectedProductIndex")
                ? EditorPrefs.GetInt("selectedProductIndex")
                : 0;
            Pm.ConfigFilesHaveChanges = EditorPrefs.HasKey("configDirty") ? EditorPrefs.GetBool("configDirty") : false;
            mainVersionNumber = EditorPrefs.HasKey("mainVersionNumber") ? EditorPrefs.GetInt("mainVersionNumber") : 0;
            yearVersionNumber = EditorPrefs.HasKey("yearVersionNumber")
                ? EditorPrefs.GetInt("yearVersionNumber")
                : DateTime.Now.Year;
            monthVersionNumber = EditorPrefs.HasKey("monthVersionNumber")
                ? EditorPrefs.GetInt("monthVersionNumber")
                : DateTime.Now.Month;
            buildVersionNumber =
                EditorPrefs.HasKey("buildVersionNumber") ? EditorPrefs.GetInt("buildVersionNumber") : 1;
            updateVersionText();
        }

        #endregion

        #region GUI

        /// <summary>
        /// Creates the Editor GUI consisting of these parts:
        /// 
        /// 1. Product Manager Part: 
        /// 	- Select Current Project
        /// 	- Prepare Build
        /// 2. Error Part (TODO)
        /// 3. Product Details Part:
        /// 	- Show all key avlue pairs of the product spec
        /// 	- Editing option (TODO)
        /// </summary>
        void OnGUI()
        {
            // adjust textarea style:
            TextareaGUIStyle = GUI.skin.textField;
            TextareaGUIStyle.wordWrap = true;

            gui4ProductManager();

            EditorGUILayout.Space();

            // TODO showErrors

            gui4ProductDetails();

            // gui4RTProductDetails();

            gui4ProductEditPart();

            EditorGUILayout.Space();

            gui4Versioning();

            EditorGUILayout.Space();

            GUILayout.FlexibleSpace();
        }

        private string newProductID = "";


        void gui4ProductManager()
        {
            // Heading:
            GUILayout.Label("Product Manager", EditorStyles.boldLabel);

            string shownBuildName;
            EditorGUILayout.BeginHorizontal();
            {
                if (CurrentBuildName == null)
                {
                    shownBuildName = "missing";
                    EditorGUILayout.PrefixLabel(new GUIContent("Current Build:", warnIcon));
                }
                else
                {
                    shownBuildName = CurrentBuildName;
                    if (BuildIsDirty)
                    {
                        EditorGUILayout.PrefixLabel(new GUIContent("Current Build:", warnIcon));
                    }
                    else
                    {
                        EditorGUILayout.PrefixLabel(new GUIContent("Current Build:"));
                    }
                }

                GUILayout.Label(shownBuildName);

                if (GUILayout.Button("Reload"))
                {
                    Config reloadedConfig = updateFromCurrentConfig();
                    if (reloadedConfig != null)
                    {
                        Pm.SetProductConfig(Pm.AllProductIds.ElementAt(selectedProductIndex), reloadedConfig);
                    }
                }


                if (Pm.ConfigFilesHaveChanges)
                {
                    if (GUILayout.Button("Persist"))
                    {
                        persistProduct();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent($"{Pm.AllProductIds.Count} Products found:", warnIcon));
            if (GUILayout.Button("Refresh from Product Directory"))
            {
                Pm.InitProductDictionary();
            }

            EditorGUILayout.EndHorizontal();

            if (selectedProductIndex < 0 || selectedProductIndex >= Pm.AllProductIds.Count)
                selectedProductIndex = 0;
            string selectedProductName = Pm.AllProductIds.ElementAt(selectedProductIndex);

            GUIContent prepareBuildButtonGUIContent,
                availableProductsPopupGUIContent,
                newProductLabelGUIContent,
                createProductButtonGUIContent;

            if (configIsDirty)
            {
                // adding tooltip to explain why these elements are disabled:
                string explanation = "Please Save or Revert your changes first.";
                prepareBuildButtonGUIContent = new GUIContent("Prepare Build", explanation);
                availableProductsPopupGUIContent = new GUIContent("Available Products:", explanation);
                newProductLabelGUIContent = new GUIContent("New product (id):", explanation);
                createProductButtonGUIContent = new GUIContent("Create", explanation);
            }
            else
            {
                prepareBuildButtonGUIContent = new GUIContent("Prepare Build");
                availableProductsPopupGUIContent = new GUIContent("Available Products:");
                newProductLabelGUIContent = new GUIContent("New product (id):");
                createProductButtonGUIContent = new GUIContent("Create");
            }

            using (new EditorGUI.DisabledGroupScope(false))
                // was: configIsDirty)) State of flag was sometimes buggy and hid prepare build button ...
            {
                // Prepare Build Button:
                //               using (new EditorGUI.DisabledGroupScope(foyerIsNOTActiveScene))
                //             {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(prepareBuildButtonGUIContent))
                    {
                        Pm.PrepareProductForBuild(selectedProductName);
                    }
                }
                //               }

                EditorGUILayout.EndHorizontal();

                // Product Selection Popup:
                string[] productIds = Pm.AllProductIds.ToArray<string>();
                // SORRY: This is to fulfill the not-so-flexible overloading scheme of Popup() here:
                List<GUIContent> guiContentListOfProducts = new List<GUIContent>();
                for (int i = 0; i < productIds.Length; i++)
                {
                    guiContentListOfProducts.Add(new GUIContent(productIds[i]));
                }

                int newIndex = EditorGUILayout.Popup(
                    availableProductsPopupGUIContent,
                    selectedProductIndex,
                    guiContentListOfProducts.ToArray()
                );
                selectProduct(newIndex);

                // Create New Product row:
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label(newProductLabelGUIContent);
                    newProductID = EditorGUILayout.TextField(
                        newProductID,
                        GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    bool createButtonshouldBeDisabled =
                        newProductID.Equals("") || Pm.AllProductIds.Contains(newProductID);
                    using (new EditorGUI.DisabledGroupScope(createButtonshouldBeDisabled))
                    {
                        if (GUILayout.Button(createProductButtonGUIContent))
                        {
                            Pm.createNewProduct(newProductID);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            } // Disabled Scope for dirty Config ends, i.e. you must first save or revert the current product's details.
        }

        private void persistProduct()
        {
            string productDir = Files.CombinePath(ProductManager.ProductsDirPath, CurrentBuildName);
            string importedPackageDir = Files.CombinePath(ConfigurationManager.RUNTIME_PRODUCT_DIR, "ImportedPackage");

            foreach (string dir in Directory.GetDirectories(ConfigurationManager.RUNTIME_PRODUCT_DIR))
            {
                if (dir.Equals(importedPackageDir))
                {
                    // do not copy the ImportedPackage dir to the product, but export it as unity package:
                    string productPackageFile =
                        Files.CombinePath(ProductManager.ProductsDirPath, CurrentBuildName,
                            CurrentBuildName + ".unitypackage");
                    AssetDatabase.ExportPackage(
                        importedPackageDir,
                        productPackageFile,
                        ExportPackageOptions.Recurse
                    );
                }
                else
                {
                    Files.CopyDir(dir, productDir, replace: true);
                }
            }

            foreach (string file in Directory.GetFiles(ConfigurationManager.RUNTIME_PRODUCT_DIR))
            {
                Files.CopyFile(file, productDir, overwrite: true);
            }

            SaveMapConfigAsJSON();

            Pm.ConfigFilesHaveChanges = false;
        }

        private void SaveMapConfigAsJSON()
        {
            GameObject mapGo = GameObject.Find("Map");
            if (mapGo == null) return;
            OnlineMaps map = mapGo.GetComponent<OnlineMaps>();
            if (map == null) return;

            string json = EditorJsonUtility.ToJson(map, true);
            string path = Files.CombinePath(ProductManager.ProductsDirPath, CurrentBuildName,
                ProductSpec.ONLINEMAPS_CONFIG);
            File.WriteAllText(path, json);
        }

        internal static Config updateFromCurrentConfig()
        {
            try
            {
                string configFile = Files.CombinePath(ProductManager.Instance.BuildExportPath,
                    ConfigurationManager.CONFIG_FILE);
                if (!File.Exists(configFile))
                    return null;
                string configText = File.ReadAllText(configFile);
                Config buildConfig = Config._doDeserializeConfig(configText);
                updateEditorBuildSceneSettings(buildConfig);
                return buildConfig;
            }
            catch (Exception exc)
            {
                Debug.LogWarning("ProductEditor.currentBuild() threw exception:\n" + exc.Message);
                return null;
            }
        }

        bool configIsDirty = false;

        static private void updateEditorBuildSceneSettings(Config config)
        {
            EditorBuildSettingsScene[] sceneSettings = new EditorBuildSettingsScene[config.scenePaths.Length];
            for (int i = 0; i < config.scenePaths.Length; i++)
            {
                sceneSettings[i] = new EditorBuildSettingsScene(config.scenePaths[i], true);
            }

            EditorBuildSettings.scenes = sceneSettings;
        }

        static public Config SelectedConfig { get; private set; }
        // static public RTConfig SelectedRTConfig { get; private set; }

        static public float WidthForValues { get; private set; }

        static public float WidthForNames { get; private set; }

        void gui4ProductDetails()
        {
            GUILayout.Label("Product Details", EditorStyles.boldLabel);
            ProductSpec p = Pm.AllProducts.ElementAt(selectedProductIndex);
            SelectedConfig = p.Config;

            // Begin ScrollView:
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos))
            {
                scrollPos = scrollView.scrollPosition;

                using (new EditorGUI.DisabledGroupScope((!allowChanges)))
                {
                    // ScrollView begins (optionally disabled):

                    // StartScene Layout:
                    gui4StartScene();

                    PropertyInfo[] propertyInfos =
                        typeof(Config).GetProperties(BindingFlags.Public | BindingFlags.NonPublic |
                                                     BindingFlags.Instance);

                    // get max widths for names and values:
                    float allNamesMax = 0f, allValuesMax = 0f;

                    foreach (PropertyInfo curPropInfo in propertyInfos)
                    {
                        if (!Attribute.IsDefined(curPropInfo, typeof(ShowInProductEditor)))
                            continue;

                        string propName = curPropInfo.Name + ":";
                        string value = Objects.ToString(curPropInfo.GetValue(SelectedConfig, null));

                        float nameMin, nameMax;
                        float valueMin, valueMax;
                        GUIStyle guiStyle = new GUIStyle();

                        guiStyle.CalcMinMaxWidth(new GUIContent(propName + ":"), out nameMin, out nameMax);
                        allNamesMax = Math.Max(allNamesMax, nameMax);
                        guiStyle.CalcMinMaxWidth(new GUIContent(value), out valueMin, out valueMax);
                        allValuesMax = Math.Max(allValuesMax, valueMax);
                    }

                    // calculate widths for names and values finally: we allow no more than 40% of the editor width for names.
                    // add left, middle and right borders as given:
                    float borders = new GUIStyle(GUI.skin.textField).margin.left +
                                    new GUIStyle(GUI.skin.textField).margin.horizontal +
                                    new GUIStyle(GUI.skin.textField).margin.right;
                    // calculate widths for names and vlaues finally: we allow no more than 40% of the editor width for names, but do not take more than we need.
                    WidthForNames = Math.Min((position.width - borders) * 0.4f, allNamesMax);
                    WidthForValues = position.width - (borders + WidthForNames);

                    EditorGUIUtility.labelWidth = WidthForNames;

                    // show all properties as textfields or textareas in fitting width:
                    foreach (PropertyInfo curPropInfo in propertyInfos)
                    {
                        if (ProductEditorPart.entryHidden(curPropInfo))
                            continue;
                        configIsDirty |= ProductEditorPart.CreateGui(curPropInfo, SelectedConfig);
                    }
                } // End Scope Disabled Group 
            } // End Scope ScrollView 
        }


        // void gui4RTProductDetails()
        // {
        //     GUILayout.Label("RT Product Details", EditorStyles.boldLabel);
        //     ProductSpec p = Pm.AllProducts.ElementAt(selectedProductIndex);
        //     // SelectedRTConfig = p.RTConfig;
        //
        //     // Begin ScrollView:
        //     using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosRT))
        //     {
        //         scrollPosRT = scrollView.scrollPosition;
        //
        //         using (new EditorGUI.DisabledGroupScope((!allowChanges)))
        //         {
        //             // ScrollView begins (optionally disabled):
        //
        //             // StartScene Layout:
        //             gui4StartScene();
        //
        //             PropertyInfo[] propertyInfos =
        //                 typeof(RTConfig).GetProperties(BindingFlags.Public | BindingFlags.NonPublic |
        //                                                BindingFlags.Instance);
        //
        //             // get max widths for names and values:
        //             float allNamesMax = 0f, allValuesMax = 0f;
        //
        //             foreach (PropertyInfo curPropInfo in propertyInfos)
        //             {
        //                 if (!Attribute.IsDefined(curPropInfo, typeof(ShowInProductEditor)))
        //                     continue;
        //
        //                 string propName = curPropInfo.Name + ":";
        //                 string value = Objects.ToString(curPropInfo.GetValue(SelectedRTConfig, null));
        //
        //                 float nameMin, nameMax;
        //                 float valueMin, valueMax;
        //                 GUIStyle guiStyle = new GUIStyle();
        //
        //                 guiStyle.CalcMinMaxWidth(new GUIContent(propName + ":"), out nameMin, out nameMax);
        //                 allNamesMax = Math.Max(allNamesMax, nameMax);
        //                 guiStyle.CalcMinMaxWidth(new GUIContent(value), out valueMin, out valueMax);
        //                 allValuesMax = Math.Max(allValuesMax, valueMax);
        //             }
        //
        //             // calculate widths for names and values finally: we allow no more than 40% of the editor width for names.
        //             // add left, middle and right borders as given:
        //             float borders = new GUIStyle(GUI.skin.textField).margin.left +
        //                             new GUIStyle(GUI.skin.textField).margin.horizontal +
        //                             new GUIStyle(GUI.skin.textField).margin.right;
        //             // calculate widths for names and vlaues finally: we allow no more than 40% of the editor width for names, but do not take more than we need.
        //             WidthForNames = Math.Min((position.width - borders) * 0.4f, allNamesMax);
        //             WidthForValues = position.width - (borders + WidthForNames);
        //
        //             EditorGUIUtility.labelWidth = WidthForNames;
        //
        //             // show all properties as textfields or textareas in fitting width:
        //             foreach (PropertyInfo curPropInfo in propertyInfos)
        //             {
        //                 if (ProductEditorPart.entryHidden(curPropInfo))
        //                     continue;
        //                 configIsDirty |= ProductEditorPart.CreateGui(curPropInfo, SelectedRTConfig);
        //             }
        //         } // End Scope Disabled Group 
        //     } // End Scope ScrollView 
        // }

        private bool allowChanges = false;

        void gui4ProductEditPart()
        {
            GUILayout.Label("Editing Options", EditorStyles.boldLabel);

            // Create New Product row:
            EditorGUILayout.BeginHorizontal();
            {
                bool oldAllowChanges = allowChanges;
                allowChanges = EditorGUILayout.Toggle("Allow Editing ...", allowChanges);
                if (!oldAllowChanges && allowChanges)
                {
                    // siwtching allowChanges ON:
                }

                if (oldAllowChanges && !allowChanges)
                {
                    // siwtching allowChanges OFF:
                }

                EditorGUI.BeginDisabledGroup(!allowChanges || !configIsDirty);
                {
                    if (GUILayout.Button("Save"))
                    {
                        Pm.serializeConfigs(SelectedConfig, ConfigurationManager.RUNTIME_PRODUCT_DIR);
                        configIsDirty = false;
                        LayoutConfig.ResetAll(); // TODO check and implement update all layout components in editor
                    }

                    if (GUILayout.Button("Revert"))
                    {
                        ProductSpec p = Pm.AllProducts.ElementAt(selectedProductIndex);
                        p.initConfig();
                        GUIUtility.keyboardControl = 0;
                        GUIUtility.hotControl = 0;
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();
        }

        static bool showDetailsStartScene;

        void gui4StartScene()
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("StartScene:", EditorStyles.boldLabel);

                if (GUILayout.Button("Use Layout"))
                {
                    //SelectedConfig.preserveAspectOfSplashFG = 
                }
            }
            EditorGUILayout.EndHorizontal();

            showDetailsStartScene = EditorGUILayout.Foldout(showDetailsStartScene, "Details:");
            if (showDetailsStartScene)
            {
            }
        }

        bool allowVersionChanges = false;

        void gui4Versioning()
        {
            GUILayout.Label("Versioning Options", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Current Version", version);

            // Create New Product row:
            EditorGUILayout.BeginHorizontal();
            {
                bool oldAllowVersionChanges = allowVersionChanges;
                allowVersionChanges = EditorGUILayout.Toggle("Allow Editing ...", allowVersionChanges);
                if (!oldAllowVersionChanges && allowVersionChanges)
                {
                    // siwtching allowChanges ON:
                }

                if (oldAllowVersionChanges && !allowVersionChanges)
                {
                    // siwtching allowChanges OFF:
                }
            }
            EditorGUILayout.EndHorizontal();

            if (allowVersionChanges)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Main +"))
                    {
                        if (EditorUtility.DisplayDialog(
                            "Really increase Main Version Number?",
                            "It will then be " + (mainVersionNumber + 1),
                            "Yes, increase!",
                            "Cancel"))
                        {
                            mainVersionNumber++;
                            EditorPrefs.SetInt("mainVersionNumber", mainVersionNumber);
                            updateVersionText();
                        }
                    }

                    if (GUILayout.Button("Main -"))
                    {
                        if (EditorUtility.DisplayDialog(
                            "Really decrease Main Version Number?",
                            "It will then be " + (mainVersionNumber - 1),
                            "Yes, decrease!",
                            "Cancel"))
                        {
                            mainVersionNumber--;
                            EditorPrefs.SetInt("mainVersionNumber", mainVersionNumber);
                            updateVersionText();
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Reset Build Nr"))
                    {
                        if (EditorUtility.DisplayDialog(
                            "Really reset Build Version Number?",
                            "It will then be 1",
                            "Yes, reset!",
                            "Cancel"))
                        {
                            buildVersionNumber = 1;
                            EditorPrefs.SetInt("buildVersionNumber", buildVersionNumber);
                            updateVersionText();
                        }
                    }

                    GUILayout.Label($"Build Nr: {buildVersionNumber}", EditorStyles.label);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        #endregion

        string version;

        private void updateVersionText()
        {
            // internal version number:
            version = String.Format(
                "{0}.{1}.{2:D2}.{3:D2}",
                mainVersionNumber,
                yearVersionNumber - 2000,
                monthVersionNumber,
                buildVersionNumber
            );

            // bundle version for iOS and Android:
            PlayerSettings.bundleVersion = String.Format(
                "{0}.{1}.{2:D2}",
                mainVersionNumber,
                yearVersionNumber - 2000,
                monthVersionNumber
            );

            // bundle version code for Android:
            int bundleVersionCode;
            string bundleVersionCodeString = String.Format(
                "{0}{1:D3}{2:D2}{3:D2}",
                mainVersionNumber,
                yearVersionNumber - 2000,
                monthVersionNumber,
                buildVersionNumber
            );
            if (Int32.TryParse(bundleVersionCodeString, out bundleVersionCode))
                PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
            else
            {
                Debug.LogError("Bundle Version Code not valid as Int32: " + bundleVersionCodeString);
            }

            // build for iOS:
            PlayerSettings.iOS.buildNumber = buildVersionNumber.ToString();

            allowVersionChanges = false;
        }


        [PostProcessBuild(10)]
        private static void IncreaseBuildNrAfterBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (Instance == null)
            {
                Debug.LogError("ProductEditor Window was not available during build. Build Nr has not been increased!");
            }

            Instance.buildVersionNumber++;
            EditorPrefs.SetInt("buildVersionNumber", Instance.buildVersionNumber);
            Instance.updateVersionText();
        }


        void selectProduct(int index)
        {
            if (index.Equals(selectedProductIndex))
            {
                return;
            }

            try
            {
                selectedProductIndex = index;
                EditorPrefs.SetInt("selectedProductIndex", selectedProductIndex);
            }
            catch (System.IndexOutOfRangeException e)
            {
                Debug.LogWarning(e.Message);
            }

            // TODO create a Product object and store it. So we can access errors and manipulate details.
        }
    }


    abstract public class ProductEditorPart
    {
        private static Dictionary<Type, ProductEditorPart>
            cachedEditorParts = new Dictionary<Type, ProductEditorPart>();

        public static Dictionary<string, int> popupSelections = new Dictionary<string, int>();

        protected static bool configIsDirty;

        static public GUIContent NamePrefixGUIContent { get; private set; }

        protected static GUIStyle STYLE_LABEL_RightAdjusted;
        protected static GUIStyle STYLE_LABEL_Bold;
        protected static GUIStyle STYLE_LABEL_Bold_RightAdjusted;
        protected static GUIStyle STYLE_FOLDOUT_Bold;

        static ProductEditorPart()
        {
            STYLE_LABEL_RightAdjusted = new GUIStyle(EditorStyles.label);
            STYLE_LABEL_RightAdjusted.alignment = TextAnchor.MiddleRight;
            STYLE_LABEL_Bold = new GUIStyle(EditorStyles.label);
            STYLE_LABEL_Bold.fontStyle = FontStyle.Bold;
            STYLE_LABEL_Bold_RightAdjusted = new GUIStyle(EditorStyles.label);
            STYLE_LABEL_Bold_RightAdjusted.alignment = TextAnchor.MiddleRight;
            STYLE_LABEL_Bold_RightAdjusted.fontStyle = FontStyle.Bold;
            STYLE_FOLDOUT_Bold = new GUIStyle(EditorStyles.foldout);
            STYLE_FOLDOUT_Bold.fontStyle = FontStyle.Bold;
        }

        /// <summary>
        /// Creates the GUI.
        /// </summary>
        /// <returns>the dirty state of this property.</returns>
        /// <param name="curPropInfo">Current property info.</param>
        static public bool CreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            Type propertyType = curPropInfo.PropertyType;
            ProductEditorPart accordingEditorPart;
            string className = null;

            if (!cachedEditorParts.TryGetValue(propertyType, out accordingEditorPart))
            {
                // construct the class name of the according editor gui creator class (a subclass of me):
                // the class name scheme is: PEP4<basictype>[Of<typearg1>[And<typearg2...]...]
                // for array types the Scheme is: PEP4<basictype>Array
                StringBuilder classNameBuilder = new StringBuilder(typeof(ProductEditorPart).FullName + "4");
                if (propertyType.Name.Contains("`"))
                {
                    classNameBuilder.Append(propertyType.Name.Substring(0, propertyType.Name.LastIndexOf("`")));
                }
                else
                {
                    classNameBuilder.Append(propertyType.Name);
                }

                Type[] argTypes = propertyType.GetGenericArguments();
                for (int i = 0; i < argTypes.Length; i++)
                {
                    classNameBuilder.Append((i == 0) ? "Of" : "And");
                    classNameBuilder.Append(argTypes[i].Name);
                }

                className = classNameBuilder.Replace("[]", "Array").ToString();

                // create a new instance of the according product editor part class:
                try
                {
                    accordingEditorPart =
                        typeof(ProductEditorPart).Assembly.CreateInstance(className) as ProductEditorPart;
                    if (accordingEditorPart != null)
                        cachedEditorParts.Add(propertyType, accordingEditorPart);
                }
                catch (Exception e)
                {
                    Log.SignalErrorToDeveloper("Unhandled property Type: {0} ({1})\t{2}", curPropInfo.PropertyType.Name,
                        className, e.Message);
                    return false;
                }
            }

            if (accordingEditorPart == null)
            {
                Log.SignalErrorToDeveloper($"Unhandled property Type: {curPropInfo.PropertyType.Name} ({className})");
                return false;
            }

            if (ProductEditorPart.entryHidden(curPropInfo))
                return false;
            else if (Attribute.IsDefined(curPropInfo, typeof(ShowInProductEditor)))
            {
                var attributes = curPropInfo.GetCustomAttributes(typeof(ShowInProductEditor), false);
                ShowInProductEditor attr = (ShowInProductEditor) attributes[0];
                if (attr.StartSection != null && attr.StartSection != "")
                {
                    GUILayout.Label(attr.StartSection, EditorStyles.boldLabel);
                }
            }

            GUIStyle guiStyle = new GUIStyle();

            string name = curPropInfo.Name;
            float nameMin, nameWidthNeed;
            guiStyle.CalcMinMaxWidth(new GUIContent(name + ":"), out nameMin, out nameWidthNeed);
            if (ProductEditor.WidthForNames < nameWidthNeed)
                // Show hover over name because it is too long to be shown:
                NamePrefixGUIContent = new GUIContent(name + ":", name);
            else
                // Show only name without hover:
                NamePrefixGUIContent = new GUIContent(name + ":");

            accordingEditorPart.doCreateGui(curPropInfo, propertyObject);
            return configIsDirty;
        }

        abstract protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject);

        static protected bool entryDisabled(PropertyInfo propInfo)
        {
            var disabled = false;
            // the entry for the given property will be disabled, if one of the following is true
            disabled |= propInfo.Name.Equals("id");
            disabled |= propInfo.Name.Equals("categoryFiltersStartFolded") &&
                        ProductEditor.SelectedConfig.foldableCategoryFilters == false;

            return disabled;
        }

        /// <summary>
        /// Says wether the given entry should be hidden:
        /// </summary>
        static internal bool entryHidden(PropertyInfo propInfo)
        {
            var config = ProductEditor.SelectedConfig;
            var hidden = false;

            // Unreadable properties:
            hidden |= !propInfo.CanRead;

            // Partner canvas:
            if (propInfo.Name.Equals("offerPartnersInfo") || propInfo.Name.Equals("showPartnersInfoAtStart"))
            {
                if (config.offerPartnersInfo == false &&
                    Resources.Load("ImportedPackage/prefabs/PartnersCanvas") == null)
                {
                    // hide it and done.
                    return true;
                }
            }

            // List Entry Dividing Modes:
            hidden |= (
                config.listEntryDividingMode == ListEntryDividingMode.SeparationLines
            ) && (
                propInfo.Name.Equals("listEntrySecondBgColor") ||
                propInfo.Name.Equals("listEntrySecondFgColor")
            );
            hidden |= (
                config.listEntryDividingMode == ListEntryDividingMode.AlternatingColors
            ) && (
                propInfo.Name.Equals("listLineColor") ||
                propInfo.Name.Equals("listStartLineWidth") ||
                propInfo.Name.Equals("dividingLineWidth") ||
                propInfo.Name.Equals("listEndLineWidth")
            );

            // Text for warning dialog when leaving quest:
            hidden |= (
                !config.warnWhenLeavingQuest
            ) && (
                propInfo.Name.Equals("warnDialogTitleWhenLeavingQuest") ||
                propInfo.Name.Equals("warnDialogMessageWhenLeavingQuest") ||
                propInfo.Name.Equals("warnDialogOKWhenLeavingQuest") ||
                propInfo.Name.Equals("warnDialogCancelWhenLeavingQuest")
            );

            // AuthorLogin BackDoor:
            hidden |= (
                !config.offerAuthorLogin
            ) && (
                propInfo.Name.Equals("defineAuthorBackDoor")
            );
            hidden |= (
                !config.defineAuthorBackDoor ||
                !config.offerAuthorLogin
            ) && (
                propInfo.Name.Equals("acceptedAuthorEmail") ||
                propInfo.Name.Equals("acceptedAuthorPassword")
            );

            // Synchronization features:
            hidden |= (
                propInfo.Name.Equals("autoUpdateFrequency")
            ) && (
                !config.autoSyncQuestInfos &&
                config.OfferManualUpdate4QuestInfos
            );

            // Undefined properties:
            hidden |= !Attribute.IsDefined(propInfo, typeof(ShowInProductEditor));

            return hidden;
        }
    }

    public class ProductEditorPart4Boolean : ProductEditorPart
    {
        override protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;

            using (new EditorGUI.DisabledGroupScope(entryDisabled(curPropInfo)))
            {
                // show checkbox:
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel(NamePrefixGUIContent);
                    bool oldBoolVal = (bool) curPropInfo.GetValue(propertyObject, null);
                    bool newBoolVal = EditorGUILayout.Toggle(oldBoolVal);
                    if (newBoolVal != oldBoolVal)
                    {
                        configIsDirty = true;
                        curPropInfo.SetValue(propertyObject, newBoolVal, null);

                        if (curPropInfo.Name == "foldableCategoryFilters" && newBoolVal == false)
                        {
                            // if categories are not foldable they must start unfolded:
                            ProductEditor.SelectedConfig.categoryFiltersStartFolded = false;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            return configIsDirty;
        }
    }


    public class ProductEditorPart4Color32 : ProductEditorPart
    {
        override protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;

            Color32 oldColorVal = (Color32) curPropInfo.GetValue(propertyObject, null);
            Color32 newColorVal = oldColorVal;

            // show Color field if value fits in one line:
            newColorVal = EditorGUILayout.ColorField(NamePrefixGUIContent, oldColorVal);
            if (!newColorVal.Equals(oldColorVal))
            {
                configIsDirty = true;
                curPropInfo.SetValue(propertyObject, newColorVal, null);
            }

            return configIsDirty;
        }
    }


    public class ProductEditorPart4Color32Array : ProductEditorPart
    {
        protected override bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;

            if (ProductEditor.SelectedConfig.colorPaletteSize > 10)
                ProductEditor.SelectedConfig.colorPaletteSize = 10;

            var oldColorValues = (Color32[]) curPropInfo.GetValue(propertyObject, null);
            var newColorValues = new Color32[10];
            for (var i = 0; i < 10; i++)
            {
                newColorValues[i] =
                    i < oldColorValues.Length ? oldColorValues[i] : ProductEditor.SelectedConfig.mainBgColor;
            }

            // show Color fields:
            for (var i = 0; i < ProductEditor.SelectedConfig.colorPaletteSize; i++)
            {
                newColorValues[i] = EditorGUILayout.ColorField(NamePrefixGUIContent, oldColorValues[i]);
            }

            if (!newColorValues.Equals(oldColorValues))
            {
                configIsDirty = true;
                curPropInfo.SetValue(propertyObject, newColorValues, null);
            }

            return configIsDirty;
        }
    }


    public class ProductEditorPart4AlignmentOption : ProductEditorPart
    {
        int? _selected;

        int selected
        {
            get
            {
                if (_selected == null)
                {
                    _selected = (int?) ProductEditor.SelectedConfig.textAlignment;
                }

                return (int) _selected;
            }
            set { _selected = value; }
        }

        string[] values = Enum.GetNames(typeof(AlignmentOption));

        override protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;

            int previouslySelected = selected;
            selected =
                EditorGUILayout.Popup(
                    curPropInfo.Name,
                    selected,
                    values
                );
            if (previouslySelected != selected)
            {
                configIsDirty = true;
                curPropInfo.SetValue(propertyObject, (AlignmentOption) selected, null);
            }

            if (configIsDirty)
                Debug.Log(curPropInfo.Name + " (AlignmentOption) changed");
            return configIsDirty;
        }
    }


    public class ProductEditorPart4HeaderMiddleButtonPolicy : ProductEditorPart
    {
        int? _selected;

        int selected
        {
            get
            {
                if (_selected == null)
                {
                    _selected = (int?) ProductEditor.SelectedConfig.headerMiddleButtonPolicy;
                }

                return (int) _selected;
            }
            set { _selected = value; }
        }

        string[] values = Enum.GetNames(typeof(HeaderMiddleButtonPolicy));

        override protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;

            // TODO implement all three strategies
            int previouslySelected = selected;
            selected =
                EditorGUILayout.Popup(
                    "Header Middle Button Policy",
                    selected,
                    values
                );
            if (previouslySelected != selected)
            {
                configIsDirty = true;
                curPropInfo.SetValue(propertyObject, (HeaderMiddleButtonPolicy) selected, null);
            }

            if (configIsDirty)
                Debug.Log("HeaderMiddleButtonPolicy changed");
            return configIsDirty;
        }
    }


    public class ProductEditorPart4DownloadStrategy : ProductEditorPart
    {
        int? _selected;

        int selected
        {
            get
            {
                if (_selected == null)
                {
                    _selected = (int?) ProductEditor.SelectedConfig.downloadStrategy;
                }

                return (int) _selected;
            }
            set { _selected = value; }
        }

        string[] downloadStrategyNames = Enum.GetNames(typeof(DownloadStrategy));

        protected override bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;

            // TODO implement all three strategies
            int oldDownloadStrategy = selected;
            selected =
                EditorGUILayout.Popup(
                    "Download Strategy",
                    selected,
                    downloadStrategyNames
                );
            if (oldDownloadStrategy != selected)
            {
                configIsDirty = true;
                curPropInfo.SetValue(propertyObject, (DownloadStrategy) selected, null);
            }

            return configIsDirty;
        }
    }

    public class ProductEditorPart4iOSTargetDevice : ProductEditorPart
    {
        int? _selected;

        int selected
        {
            get
            {
                if (_selected == null)
                {
                    _selected = (int?) ProductEditor.SelectedConfig.iOsDeviceTypes;
                }

                return (int) _selected;
            }
            set { _selected = value; }
        }

        readonly string[] iOsDeviceTypeNames = Enum.GetNames(typeof(iOSTargetDevice));

        protected override bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;

            // TODO implement all three strategies
            int oldDownloadStrategy = selected;
            selected =
                EditorGUILayout.Popup(
                    "Supported iOS Device Types",
                    selected,
                    iOsDeviceTypeNames
                );
            if (oldDownloadStrategy != selected)
            {
                configIsDirty = true;
                curPropInfo.SetValue(propertyObject, (iOSTargetDevice) selected, null);
                PlayerSettings.iOS.targetDevice = (iOSTargetDevice) selected;
            }

            return configIsDirty;
        }
    }

    public class ProductEditorPart4ImagePath : ProductEditorPart
    {
        override protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;
            GUIContent myNamePrefixGUIContent = NamePrefixGUIContent;

            using (new EditorGUI.DisabledGroupScope(entryDisabled(curPropInfo)))
            {
                // get currently stored image path from config:
                ImagePath oldVal = (ImagePath) curPropInfo.GetValue(propertyObject, null);
                Sprite oldSprite = null;
                if (oldVal != null)
                    oldSprite = oldVal.GetSprite();

                // show textarea if value does not fit within one line:
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(myNamePrefixGUIContent);
                Sprite newSprite =
                    (Sprite) EditorGUILayout.ObjectField(oldSprite, typeof(Sprite), false);
                string path = AssetDatabase.GetAssetPath(newSprite);
                ImagePath newVal = new ImagePath(Files.GetResourcesRelativePath(path));
                EditorGUILayout.EndHorizontal();

                if (!newVal.Equals(oldVal))
                {
                    configIsDirty = true;
                    curPropInfo.SetValue(propertyObject, newVal, null);
                }
            }

            return configIsDirty;
        }
    }


    public class ProductEditorPart4Int32 : ProductEditorPart
    {
        override protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;
            GUIContent myNamePrefixGUIContent = NamePrefixGUIContent;

            using (new EditorGUI.DisabledGroupScope(entryDisabled(curPropInfo)))
            {
                // id of products may not be altered.
                if (curPropInfo.Name.Equals("id"))
                {
                    myNamePrefixGUIContent =
                        new GUIContent(curPropInfo.Name, "You may not alter the id of a product.");
                }

                int oldIntVal = (int) curPropInfo.GetValue(propertyObject, null);

                // show text field if value fits in one line:
                int newIntVal = EditorGUILayout.IntField(myNamePrefixGUIContent, oldIntVal);
                if (newIntVal != oldIntVal)
                {
                    configIsDirty = true;
                    curPropInfo.SetValue(propertyObject, newIntVal, null);
                }
            }

            return configIsDirty;
        }
    }


    /// <summary>
    /// Product editor part for properties typed as long.
    /// </summary>
    public class ProductEditorPart4Int64 : ProductEditorPart
    {
        override protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;
            GUIContent myNamePrefixGUIContent = NamePrefixGUIContent;

            using (new EditorGUI.DisabledGroupScope(entryDisabled(curPropInfo)))
            {
                long oldIntVal = (long) curPropInfo.GetValue(propertyObject, null);

                // show text field if value fits in one line:
                long newIntVal = EditorGUILayout.LongField(myNamePrefixGUIContent, oldIntVal);
                if (newIntVal != oldIntVal)
                {
                    configIsDirty = true;
                    curPropInfo.SetValue(propertyObject, newIntVal, null);
                }
            }

            return configIsDirty;
        }
    }


    public class ProductEditorPart4Byte : ProductEditorPart
    {
        override protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;
            GUIContent myNamePrefixGUIContent = NamePrefixGUIContent;

            using (new EditorGUI.DisabledGroupScope(entryDisabled(curPropInfo)))
            {
                byte oldVal = (byte) curPropInfo.GetValue(propertyObject, null);

                // show text field if value fits in one line:
                int intVal = EditorGUILayout.IntField(myNamePrefixGUIContent, oldVal);
                byte newVal;
                if (intVal < 0)
                    newVal = 0;
                else if (intVal > 255)
                    newVal = 255;
                else
                    newVal = (byte) intVal;

                if (newVal != oldVal)
                {
                    configIsDirty = true;
                    curPropInfo.SetValue(propertyObject, newVal, null);
                }
            }

            return configIsDirty;
        }
    }


    public class ProductEditorPart4ListOfCategorySet : ProductEditorPart
    {
        bool showList = false;

        override protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            List<CategorySet> allElements =
                (List<CategorySet>) curPropInfo.GetValue(propertyObject, null);
            if (allElements == null)
                allElements = new List<CategorySet>();
            bool valsChanged = false;

            showList = EditorGUILayout.Foldout(
                showList,
                string.Format("Category Sets: ({0})", allElements.Count),
                STYLE_FOLDOUT_Bold);

            if (showList)
            {
                configIsDirty = false;

                // Header with Add Button:
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(
                    new GUIContent("Add Category Set:"),
                    EditorStyles.textField,
                    STYLE_LABEL_Bold
                );
                if (GUILayout.Button("+"))
                {
                    CategorySet catSet = new CategorySet();
                    allElements.Insert(0, catSet);
                    valsChanged = true;
                }

                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < allElements.Count; i++)
                {
                    bool elemChanged = false;
                    CategorySet oldElem = allElements[i];
                    CategorySet newElem = oldElem;

                    // category set name:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(
                        new GUIContent((i + 1).ToString() + ". name:"),
                        EditorStyles.textField,
                        STYLE_LABEL_Bold
                    );
                    string newName = EditorGUILayout.TextField(oldElem.name);

                    elemChanged |= (newName != oldElem.name);

                    if (GUILayout.Button("-"))
                    {
                        if (EditorUtility.DisplayDialog(
                            string.Format("Really delete category set {0}?",
                                (oldElem.name != null && oldElem.name != "") ? oldElem.name : i.ToString()),
                            string.Format(
                                "This can not be undone"
                            ),
                            "Yes, delete it!",
                            "No, keep it"))
                        {
                            allElements.RemoveAll(item => item.name == oldElem.name);
                            valsChanged = true;
                        }
                    }

                    EditorGUILayout.EndHorizontal(); // end row for name and "-".

                    if (elemChanged)
                    {
                        valsChanged = true;
                        newElem.name = newName;
                        // TODO newElem.categories = ....
                        allElements[i] = newElem;
                    }
                }
            }

            if (valsChanged)
            {
                // Update Config property for category sets:
                configIsDirty = true;
                curPropInfo.SetValue(propertyObject, allElements, null);
            }

            return configIsDirty;
        }
    }


    public class ProductEditorPart4ListOfCategory : ProductEditorPart
    {
        bool showList = false;

        override protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            List<Category> allElements = (List<Category>) curPropInfo.GetValue(propertyObject, null);
            if (allElements == null)
                allElements = new List<Category>();
            bool valsChanged = false;
            bool folderNameChanged = false;

            showList = EditorGUILayout.Foldout(showList, string.Format("Categories: ({0})", allElements.Count),
                STYLE_FOLDOUT_Bold);
            if (showList)
            {
                configIsDirty = false;
                // Header with Add Button:
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(
                    new GUIContent("Add Category:"),
                    EditorStyles.textField,
                    STYLE_LABEL_Bold
                );
                if (GUILayout.Button("+"))
                {
                    Category cat = new Category();
                    allElements.Insert(0, cat);
                    valsChanged = true;
                }

                if (GUILayout.Button("Sort"))
                {
                    allElements = getSortedAccordingToFolders(allElements);
                    valsChanged = true;
                }

                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < allElements.Count; i++)
                {
                    bool elemChanged = false;
                    Category oldElem = allElements[i];
                    Category newElem = oldElem;

                    // category name:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(
                        new GUIContent((i + 1).ToString() + ". name:"),
                        EditorStyles.textField,
                        STYLE_LABEL_Bold
                    );
                    string newName = EditorGUILayout.TextField(oldElem.name);
                    EditorGUILayout.EndHorizontal(); // end horizontal line for id.
                    elemChanged |= (newName != oldElem.name);

                    // id as text:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(
                        new GUIContent("id:", "Id must be unqiue within these categories."),
                        EditorStyles.textField,
                        STYLE_LABEL_RightAdjusted
                    );
                    string newId = EditorGUILayout.TextField(oldElem.id);
                    EditorGUILayout.EndHorizontal(); // end horizontal line for id.
                    if (newId != oldElem.id)
                    {
                        // check that the new id is not used among the other categories, else reset to the old id:
                        bool newIdIsUnique = true;
                        for (int j = 0; j < allElements.Count; j++)
                        {
                            if (j == i)
                                continue;
                            if (allElements[j].Equals(allElements[i]))
                            {
                                newIdIsUnique = false;
                                break;
                            }
                        }

                        if (!newIdIsUnique)
                        {
                            // reset if not unique:
                            newId = oldElem.id;
                        }
                        else
                        {
                            elemChanged |= (newId != oldElem.id);
                        }
                    }

                    // folder as text:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(
                        new GUIContent("folder name:", "Folder name is optional."),
                        EditorStyles.textField,
                        STYLE_LABEL_RightAdjusted
                    );
                    string newFolderName = EditorGUILayout.TextField(oldElem.folderName);
                    EditorGUILayout.EndHorizontal(); // end horizontal line for id.
                    folderNameChanged |= (newFolderName != oldElem.folderName);
                    elemChanged |= folderNameChanged;

                    // symbol:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(
                        new GUIContent("symbol:"),
                        EditorStyles.textField,
                        STYLE_LABEL_RightAdjusted
                    );
                    // get currently stored image path from config:
                    RTImagePath oldSymbolPath = oldElem.symbol;
                    RTImagePath newSymbolPath = oldSymbolPath;
                    Sprite oldSymbolSprite = null;
                    if (oldSymbolPath != null)
                        oldSymbolSprite = oldSymbolPath.GetSprite();
                    Sprite newSymbolSprite =
                        (Sprite) EditorGUILayout.ObjectField(oldSymbolSprite, typeof(Sprite), false);
                    if (newSymbolSprite != oldSymbolSprite)
                    {
                        string path = AssetDatabase.GetAssetPath(newSymbolSprite);
                        newSymbolPath = new RTImagePath(Files.GetResourcesRelativePath(path, false));
                        elemChanged |= !newSymbolPath.Equals(oldSymbolPath);
                    }

                    if (elemChanged)
                    {
                        valsChanged = true;
                        newElem.name = newName;
                        newElem.folderName = newFolderName;
                        newElem.id = newId;
                        newElem.symbol = newSymbolPath;
                        allElements[i] = newElem;
                    }

                    if (GUILayout.Button("-"))
                    {
                        if (EditorUtility.DisplayDialog(
                            string.Format("Really delete category {0}?",
                                (oldElem.name != null && oldElem.name != "") ? oldElem.name : i.ToString()),
                            string.Format(
                                "This can not be undone"
                            ),
                            "Yes, delete it!",
                            "No, keep it"))
                        {
                            allElements.Remove(allElements[i]);
                            valsChanged = true;
                        }
                    }

                    EditorGUILayout
                        .EndHorizontal(); // end horizontal line of symbol and delete button for current category.
                }

                if (valsChanged)
                {
                    // Update Config property for scene extensions:
                    configIsDirty = true;
                    curPropInfo.SetValue(propertyObject, allElements, null);
                }
            }

            return configIsDirty;
        }

        public List<Category> getSortedAccordingToFolders(List<Category> allCats)
        {
            // we create for each folder a list of current indices where the elements of that folders stay:
            Dictionary<string, List<int>> catPositionsForFolders = new Dictionary<string, List<int>>();
            // we collect the order in wich the folders start to create the collected folders in that order again later:
            List<string> orderOfFolders = new List<string>();

            for (int i = 0; i < allCats.Count; i++)
            {
                List<int> positionList;
                if (!catPositionsForFolders.TryGetValue(allCats[i].folderName, out positionList))
                {
                    positionList = new List<int>();
                    catPositionsForFolders.Add(allCats[i].folderName, positionList);
                    orderOfFolders.Add(allCats[i].folderName);
                }

                positionList.Add(i);
            }

            List<Category> sortedList = new List<Category>(allCats.Count);
            int newIndex = 0;
            for (int i = 0; i < orderOfFolders.Count; i++)
            {
                List<int> oldIndicesForThisFolder;
                if (!catPositionsForFolders.TryGetValue(orderOfFolders[i], out oldIndicesForThisFolder))
                {
                    Log.SignalErrorToDeveloper(
                        "Sorting Categories for Folders broken: Folder {0} not in catPositionsForFolders list! Fix it!",
                        orderOfFolders[i]);
                }

                foreach (int oldIndex in oldIndicesForThisFolder)
                {
                    sortedList.Insert(newIndex++, allCats[oldIndex]);
                }
            }

            return sortedList;
        }
    }


    public class ProductEditorPart4ListOfSceneExtension : ProductEditorPart
    {
        bool showList = false;

        override protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            List<SceneExtension> allElements =
                (List<SceneExtension>) curPropInfo.GetValue(propertyObject, null);
            if (allElements == null)
                allElements = new List<SceneExtension>();
            bool valsChanged = false;
            string sceneName;

            showList = EditorGUILayout.Foldout(showList, string.Format("Scene Extensions: ({0})", allElements.Count),
                STYLE_FOLDOUT_Bold);
            if (showList)
            {
                configIsDirty = false;
                // Header with Add Button:
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(
                    new GUIContent("Add Extension:"),
                    EditorStyles.textField,
                    STYLE_LABEL_Bold
                );
                if (GUILayout.Button("+"))
                {
                    SceneExtension sce = new SceneExtension();
                    sce.root = "";
                    sce.prefab = "";
                    sce.scene = EditorSceneManager.GetActiveScene().path;
                    allElements.Add(sce);
                    valsChanged = true;
                }

                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < allElements.Count; i++)
                {
                    SceneExtension oldSceneExt = allElements[i];
                    SceneExtension newSceneExt = oldSceneExt;
                    // entry for extension only enabled when in same scene:
                    bool sceneExtensionDisabled = EditorSceneManager.GetActiveScene().path != oldSceneExt.scene;

                    using (new EditorGUI.DisabledGroupScope(sceneExtensionDisabled))
                    {
                        EditorGUILayout.BeginHorizontal();
                        bool sceneExtChanged = false;
                        // scene name:
                        sceneName = Files.FileName(oldSceneExt.scene);
                        if (sceneName.EndsWith(".unity"))
                            sceneName = sceneName.Substring(0, sceneName.Length - ".unity".Length);
                        // prefab:
                        EditorGUILayout.PrefixLabel(new GUIContent("  -> " + sceneName,
                            "The prefab extends this scene."));
                        GameObject oldPrefabGO = Resources.Load<GameObject>(oldSceneExt.prefab);
                        GameObject newPrefabGO =
                            (GameObject) EditorGUILayout.ObjectField(oldPrefabGO, typeof(GameObject), false);
                        if (newPrefabGO != oldPrefabGO && PrefabUtility.GetPrefabType(newPrefabGO) == PrefabType.Prefab)
                        {
                            // if user selects another prefab we store it:
                            newSceneExt.prefab =
                                Files.GetResourcesRelativePath(AssetDatabase.GetAssetPath(newPrefabGO));
                            sceneExtChanged = true;
                        }

                        EditorGUILayout.EndHorizontal();
                        // root:
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(new GUIContent("\t\tat",
                            "The root gameobject where the prefab is injected."));
                        if (sceneExtensionDisabled)
                        {
                            EditorGUILayout.TextField(Files.FileName(oldSceneExt.root));
                        }
                        else
                        {
                            GameObject oldRootGO = GameObject.Find(oldSceneExt.root);
                            GameObject newRootGO =
                                (GameObject) EditorGUILayout.ObjectField(oldRootGO, typeof(GameObject), true);
                            if (newRootGO != oldRootGO && newRootGO.scene == EditorSceneManager.GetActiveScene())
                            {
                                newSceneExt.root = newRootGO.transform.GetPath();
                                sceneExtChanged = true;
                            }
                        }

                        if (sceneExtChanged)
                        {
                            newSceneExt.scene = EditorSceneManager.GetActiveScene().path;
                            allElements[i] = newSceneExt;
                            valsChanged = true;
                        }
                    } // end disabled group for current scene extension

                    if (GUILayout.Button("-"))
                    {
                        if (EditorUtility.DisplayDialog(
                            string.Format("Really delete extension for scene {0}?", sceneName),
                            string.Format(
                                "It adds {0} to {1}.",
                                Files.FileName(oldSceneExt.prefab),
                                Files.FileName(oldSceneExt.root)
                            ),
                            "Yes, delete it!",
                            "No, keep it"))
                        {
                            allElements.Remove(oldSceneExt);
                            valsChanged = true;
                        }
                    }

                    EditorGUILayout
                        .EndHorizontal(); // end horizontal line of prefab and delete button for current scene extension.
                }

                if (valsChanged)
                {
                    // Update Config property for scene extensions:
                    configIsDirty = true;
                    curPropInfo.SetValue(propertyObject, allElements, null);
                }
            }

            return configIsDirty;
        }
    }


    public class ProductEditorPart4AndroidSdkVersions : ProductEditorPart
    {
        int? _selected;

        int selected
        {
            get
            {
                if (_selected == null)
                {
                    _selected = valueIndexByNumber[(int) ProductEditor.SelectedConfig.androidMinSDKVersion];
                }

                return (int) _selected;
            }
            set { _selected = value; }
        }

        static readonly string[] names = Enum.GetNames(typeof(AndroidSdkVersions));
        static readonly Array vals = Enum.GetValues(typeof(AndroidSdkVersions));

        private static Dictionary<int, int> _valueIndexByNumber;

        public static Dictionary<int, int> valueIndexByNumber
        {
            get
            {
                if (_valueIndexByNumber == null)
                {
                    _valueIndexByNumber = new Dictionary<int, int>();
                    for (int i = 0; i < names.Length; i++)
                    {
                        _valueIndexByNumber.Add((int) vals.GetValue(i), i);
                    }
                }

                return _valueIndexByNumber;
            }
        }

        private static Dictionary<int, string> _valueNameByIndex;

        public static Dictionary<int, string> valueNameByIndex
        {
            get
            {
                if (_valueNameByIndex == null)
                {
                    _valueNameByIndex = new Dictionary<int, string>();
                    for (int i = 0; i < names.Length; i++)
                    {
                        _valueNameByIndex.Add(i, names[i]);
                    }
                }

                return _valueNameByIndex;
            }
        }

        override protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;
            int old = selected;
            int curIndex;
            valueIndexByNumber.TryGetValue((int) ProductEditor.SelectedConfig.androidMinSDKVersion, out curIndex);
            selected =
                EditorGUILayout.Popup(
                    "Android Min SDK Version",
                    selected,
                    names
                );
            if (old != selected)
            {
                configIsDirty = true;
                PlayerSettings.Android.minSdkVersion =
                    (AndroidSdkVersions) Enum.Parse(typeof(AndroidSdkVersions), valueNameByIndex[selected]);
                curPropInfo.SetValue(propertyObject, PlayerSettings.Android.minSdkVersion, null);
            }

            return configIsDirty;
        }
    }

    // TODO can't we make these classes generic?
    public class ProductEditorPart4ListEntryDividingMode : ProductEditorPart
    {
        int? _selected;

        int selected
        {
            get
            {
                if (_selected == null)
                {
                    _selected = (int?) ProductEditor.SelectedConfig.listEntryDividingMode;
                }

                return (int) _selected;
            }
            set { _selected = value; }
        }

        string[] names = Enum.GetNames(typeof(ListEntryDividingMode));

        override protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;

            int oldSelection = selected;
            selected =
                EditorGUILayout.Popup(
                    "List Entry Dividing Mode:",
                    selected,
                    names
                );
            if (oldSelection != selected)
            {
                configIsDirty = true;
                curPropInfo.SetValue(propertyObject, (ListEntryDividingMode) selected, null);
            }

            ListEntryDividingMode mode = (ListEntryDividingMode) selected;
            //Debug.Log("Sel: " + selection + "     mode: " + mode.ToString());
            return configIsDirty;
        }
    }

    public class ProductEditorPart4MapStartPositionType : ProductEditorPart
    {
        int? _selected;

        int selected
        {
            get
            {
                if (_selected == null)
                {
                    _selected = (int?) ProductEditor.SelectedConfig.mapStartPositionType;
                }

                return (int) _selected;
            }
            set { _selected = value; }
        }

        string[] mapStartPositionTypeNames = Enum.GetNames(typeof(MapStartPositionType));

        override protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;

            int oldMapSTartPositionType = selected;
            selected =
                EditorGUILayout.Popup(
                    "Map Start Position",
                    selected,
                    mapStartPositionTypeNames
                );
            if (oldMapSTartPositionType != selected)
            {
                configIsDirty = true;
                curPropInfo.SetValue(propertyObject, (MapStartPositionType) selected, null);
            }

            return configIsDirty;
        }
    }


    public class ProductEditorPart4TaskUIMode : ProductEditorPart
    {
        int? _selected;

        int selected
        {
            get
            {
                if (_selected == null)
                {
                    _selected = (int?) ProductEditor.SelectedConfig.taskUI;
                }

                return (int) _selected;
            }
            set { _selected = value; }
        }

        string[] taskUIModeNames = Enum.GetNames(typeof(TaskUIMode));

        override protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;

            int oldTaskUI = selected;
            selected =
                EditorGUILayout.Popup(
                    "Task UI Mode",
                    selected,
                    taskUIModeNames
                );
            if (oldTaskUI != selected)
            {
                configIsDirty = true;
                curPropInfo.SetValue(propertyObject, (TaskUIMode) selected, null);
            }

            return configIsDirty;
        }
    }


    public class ProductEditorPart4Single : ProductEditorPart
    {
        override protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;

            float oldFloatVal = (float) curPropInfo.GetValue(propertyObject, null);
            float newFloatVal = oldFloatVal;

            // show text field if value fits in one line:
            newFloatVal = EditorGUILayout.FloatField(NamePrefixGUIContent, oldFloatVal);
            if (newFloatVal != oldFloatVal)
            {
                configIsDirty = true;
                curPropInfo.SetValue(propertyObject, newFloatVal, null);
            }

            return configIsDirty;
        }
    }


    public class ProductEditorPart4Double : ProductEditorPart
    {
        override protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;

            double oldDoubleVal = (double) curPropInfo.GetValue(propertyObject, null);
            double newDoubleVal = oldDoubleVal;

            // show text field if value fits in one line:
            newDoubleVal = EditorGUILayout.DoubleField(NamePrefixGUIContent, oldDoubleVal);
            if (newDoubleVal != oldDoubleVal)
            {
                configIsDirty = true;
                curPropInfo.SetValue(propertyObject, newDoubleVal, null);
            }

            return configIsDirty;
        }
    }


    public class ProductEditorPart4String : ProductEditorPart
    {
        override protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;
            GUIContent myNamePrefixGUIContent = NamePrefixGUIContent;

            using (new EditorGUI.DisabledGroupScope(entryDisabled(curPropInfo)))
            {
                // id of products may not be altered.
                if (curPropInfo.Name.Equals("id"))
                {
                    myNamePrefixGUIContent = new GUIContent(curPropInfo.Name, "You may not alter the id of a product.");
                }

                // show textfield or if value too long show textarea:
                string oldStringVal = (string) curPropInfo.GetValue(propertyObject, null);
                oldStringVal = Objects.ToString(oldStringVal);
                string newStringVal;
                GUIStyle guiStyle = new GUIStyle();
                float valueMin, valueNeededWidth;
                guiStyle.CalcMinMaxWidth(new GUIContent(oldStringVal), out valueMin, out valueNeededWidth);

                if (ProductEditor.WidthForValues < valueNeededWidth)
                {
                    // show textarea if value does not fit within one line:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(myNamePrefixGUIContent);
                    newStringVal = EditorGUILayout.TextArea(oldStringVal, ProductEditor.TextareaGUIStyle);
                    newStringVal = Objects.ToString(newStringVal);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    // show text field if value fits in one line:
                    newStringVal = EditorGUILayout.TextField(myNamePrefixGUIContent, oldStringVal);
                    newStringVal = Objects.ToString(newStringVal);
                }

                if (!newStringVal.Equals(oldStringVal))
                {
                    configIsDirty = true;
                    curPropInfo.SetValue(propertyObject, newStringVal, null);
                }
            }

            return configIsDirty;
        }
    }

    public class ProductEditorPart4StringArray : ProductEditorPart
    {
        bool showDetails = false;

        override protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            switch (curPropInfo.Name)
            {
                case "scenePaths":
                    configIsDirty = doGui4ScenePaths(curPropInfo);
                    break;
                case "acceptedPageTypes":
                    configIsDirty = doGui4AcceptedPageTypes(curPropInfo, propertyObject);
                    break;
                case "questInfoViews":
                    configIsDirty = doGui4QuestInfoViews(curPropInfo, propertyObject);
                    break;
                case "assetAddOns":
                    configIsDirty = doGui4AssetAddOns(curPropInfo, propertyObject);
                    break;
                default:
                    Log.SignalErrorToDeveloper("Unhandled property Type: {0}", curPropInfo.PropertyType.Name);
                    break;
            }

            return configIsDirty;
        }

        private bool doGui4ScenePaths(PropertyInfo curPropInfo)
        {
            configIsDirty = false;

            showDetails = EditorGUILayout.Foldout(
                showDetails,
                string.Format("Included Scenes: ({0})", ProductEditor.SelectedConfig.scenePaths.Length),
                STYLE_FOLDOUT_Bold
            );

            if (showDetails)
            {
                // Header with Import Button:
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(
                    new GUIContent("Update:"),
                    EditorStyles.textField,
                    STYLE_LABEL_Bold
                );
                if (GUILayout.Button("Import from Editor Settings"))
                {
                    EditorWindow editorBuildSettingsWindow =
                        EditorWindow.GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
                    editorBuildSettingsWindow.Show();

                    List<string> scenePathsFromSettings = new List<string>();
                    for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
                    {
                        if (EditorBuildSettings.scenes[i].enabled)
                        {
                            scenePathsFromSettings.Add(EditorBuildSettings.scenes[i].path);
                        }
                    }

                    ProductEditor.SelectedConfig.scenePaths = scenePathsFromSettings.ToArray();
                    configIsDirty = true;
                }

                EditorGUILayout.EndHorizontal();

                using (new EditorGUI.DisabledGroupScope(true))
                {
                    for (int i = 0; i < ProductEditor.SelectedConfig.scenePaths.Length; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(
                            new GUIContent("" + (i + 1) + ":"),
                            EditorStyles.textField,
                            STYLE_LABEL_RightAdjusted
                        );
                        EditorGUILayout.TextField(ProductEditor.SelectedConfig.scenePaths[i]);
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            return configIsDirty;
        }

        int acceptedPageTypeSelection = 0;

        private bool doGui4AcceptedPageTypes(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;

            if (ProductEditor.SelectedConfig.acceptedPageTypes == null)
            {
                ProductEditor.SelectedConfig.acceptedPageTypes = new string[0];
            }

            List<string> allElements = new List<string>(ProductEditor.SelectedConfig.acceptedPageTypes);

            List<string> pageTypesToAdd = new List<string>();
            foreach (string pageType in Enum.GetNames(typeof(PageType)))
            {
                if (!allElements.Contains(pageType))
                {
                    pageTypesToAdd.Add(pageType);
                }
            }

            showDetails = EditorGUILayout.Foldout(
                showDetails,
                string.Format("Accepted Page Types: ({0})", ProductEditor.SelectedConfig.acceptedPageTypes.Length),
                STYLE_FOLDOUT_Bold
            );

            if (showDetails)
            {
                using (new EditorGUI.DisabledGroupScope(pageTypesToAdd.Count == 0))
                {
                    // Header with Add Button:
                    EditorGUILayout.BeginHorizontal();

                    acceptedPageTypeSelection =
                        EditorGUILayout.Popup(
                            "Add Page Type:",
                            acceptedPageTypeSelection,
                            pageTypesToAdd.ToArray()
                        );

                    if (GUILayout.Button("+"))
                    {
                        allElements.Add(pageTypesToAdd[acceptedPageTypeSelection]);
                        configIsDirty = true;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                for (int i = 0; i < ProductEditor.SelectedConfig.acceptedPageTypes.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(
                        new GUIContent("" + (i + 1) + ":"),
                        EditorStyles.textField,
                        STYLE_LABEL_RightAdjusted
                    );
                    using (new EditorGUI.DisabledGroupScope(true))
                    {
                        string pageType = EditorGUILayout.TextField(ProductEditor.SelectedConfig.acceptedPageTypes[i]);
                        if (!pageType.Equals(ProductEditor.SelectedConfig.acceptedPageTypes[i]))
                        {
                            allElements[i] = pageType;
                            configIsDirty = true;
                        }
                    }

                    if (GUILayout.Button("-"))
                    {
                        if (EditorUtility.DisplayDialog(
                            string.Format("Really Delete Accepted Page Type {0}?",
                                ProductEditor.SelectedConfig.acceptedPageTypes[i]),
                            "Sure?.",
                            "Yes, delete it!",
                            "No, keep it"))
                        {
                            allElements.Remove(ProductEditor.SelectedConfig.acceptedPageTypes[i]);
                            configIsDirty = true;
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            if (configIsDirty)
            {
                curPropInfo.SetValue(propertyObject, allElements.ToArray(), null);
            }

            return configIsDirty;
        }

        // AssetAddOn STARTS
        int assetAddOnSelection = 0;
        bool showDetails_AssetAddOns = false;

        private bool doGui4AssetAddOns(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;

            if (ProductEditor.SelectedConfig.assetAddOns == null)
            {
                int numberOfAssetAddOns = Enum.GetNames(typeof(AssetAddOn)).Length;
                ProductEditor.SelectedConfig.assetAddOns = new string[numberOfAssetAddOns];
            }

            List<string> allElements = new List<string>(ProductEditor.SelectedConfig.assetAddOns);

            List<string> assetAddOnsToAdd = new List<string>();
            foreach (string pageType in Enum.GetNames(typeof(AssetAddOn)))
            {
                if (!allElements.Contains(pageType))
                {
                    assetAddOnsToAdd.Add(pageType);
                }
            }

            showDetails_AssetAddOns = EditorGUILayout.Foldout(
                showDetails_AssetAddOns,
                string.Format("Asset Add Ons: ({0})", ProductEditor.SelectedConfig.assetAddOns.Length),
                STYLE_FOLDOUT_Bold
            );

            if (showDetails_AssetAddOns)
            {
                using (new EditorGUI.DisabledGroupScope(assetAddOnsToAdd.Count == 0))
                {
                    // Header with Add Button:
                    EditorGUILayout.BeginHorizontal();

                    if (assetAddOnSelection < 0)
                        assetAddOnSelection = 0;
                    if (assetAddOnSelection > assetAddOnsToAdd.Count - 1)
                        assetAddOnSelection = assetAddOnsToAdd.Count - 1;
                    assetAddOnSelection =
                        EditorGUILayout.Popup(
                            "Add AssetAddOn:",
                            assetAddOnSelection,
                            assetAddOnsToAdd.ToArray()
                        );

                    if (GUILayout.Button("+"))
                    {
                        allElements.Add(assetAddOnsToAdd[assetAddOnSelection]);
                        configIsDirty = true;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                for (int i = 0; i < ProductEditor.SelectedConfig.assetAddOns.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(
                        new GUIContent("" + (i + 1) + ":"),
                        EditorStyles.textField,
                        STYLE_LABEL_RightAdjusted
                    );
                    using (new EditorGUI.DisabledGroupScope(true))
                    {
                        string pageType = EditorGUILayout.TextField(ProductEditor.SelectedConfig.assetAddOns[i]);
                        if (!pageType.Equals(ProductEditor.SelectedConfig.assetAddOns[i]))
                        {
                            allElements[i] = pageType;
                            configIsDirty = true;
                        }
                    }

                    if (GUILayout.Button("-"))
                    {
                        allElements.Remove(ProductEditor.SelectedConfig.assetAddOns[i]);
                        configIsDirty = true;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            if (configIsDirty)
            {
                curPropInfo.SetValue(propertyObject, allElements.ToArray(), null);
            }

            return configIsDirty;
        }
        // AssetAddOn ENDS

        int questInfoViewSelection = 0;

        private bool doGui4QuestInfoViews(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;

            if (ProductEditor.SelectedConfig.questInfoViews == null)
            {
                ProductEditor.SelectedConfig.questInfoViews = new string[2]
                    {QuestInfoView.Map.ToString(), QuestInfoView.List.ToString()};
            }

            List<string> allElements = new List<string>(ProductEditor.SelectedConfig.questInfoViews);

            List<string> viewsToAdd = new List<string>();
            foreach (string pageType in Enum.GetNames(typeof(QuestInfoView)))
            {
                if (!allElements.Contains(pageType))
                {
                    viewsToAdd.Add(pageType);
                }
            }

            showDetails = EditorGUILayout.Foldout(
                showDetails,
                string.Format("Questinfo View & Select Options: ({0})",
                    ProductEditor.SelectedConfig.questInfoViews.Length),
                STYLE_FOLDOUT_Bold
            );

            if (showDetails)
            {
                using (new EditorGUI.DisabledGroupScope(viewsToAdd.Count == 0))
                {
                    // Header with Add Button:
                    EditorGUILayout.BeginHorizontal();

                    if (questInfoViewSelection < 0)
                        questInfoViewSelection = 0;
                    if (questInfoViewSelection > viewsToAdd.Count - 1)
                        questInfoViewSelection = viewsToAdd.Count - 1;
                    questInfoViewSelection =
                        EditorGUILayout.Popup(
                            "Add Questinfo Viewing Option:",
                            questInfoViewSelection,
                            viewsToAdd.ToArray()
                        );

                    if (GUILayout.Button("+"))
                    {
                        allElements.Add(viewsToAdd[questInfoViewSelection]);
                        configIsDirty = true;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                for (int i = 0; i < ProductEditor.SelectedConfig.questInfoViews.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(
                        new GUIContent("" + (i + 1) + ":"),
                        EditorStyles.textField,
                        STYLE_LABEL_RightAdjusted
                    );
                    using (new EditorGUI.DisabledGroupScope(true))
                    {
                        string pageType = EditorGUILayout.TextField(ProductEditor.SelectedConfig.questInfoViews[i]);
                        if (!pageType.Equals(ProductEditor.SelectedConfig.questInfoViews[i]))
                        {
                            allElements[i] = pageType;
                            configIsDirty = true;
                        }
                    }

                    if (GUILayout.Button("-"))
                    {
                        //						bool delete = EditorUtility.DisplayDialog (
                        //							              string.Format ("Really Delete Questinfo View Type {0}?", ProductEditor.SelectedConfig.questInfoViews [i]), 
                        //							              "Sure?.", 
                        //							              "Yes, delete it!", 
                        //							              "No, keep it");
                        //						if (delete) {
                        allElements.Remove(ProductEditor.SelectedConfig.questInfoViews[i]);
                        configIsDirty = true;
                        //						}
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            if (configIsDirty)
            {
                curPropInfo.SetValue(propertyObject, allElements.ToArray(), null);
            }

            return configIsDirty;
        }
    }


    public class ProductEditorPart4ListOfSceneMapping : ProductEditorPart
    {
        public const string ProjectScenesRootPath = "Assets/Scenes/Pages/";

        public static readonly string ProductScenesRootPath =
            Files.CombinePath(ConfigurationManager.RUNTIME_PRODUCT_DIR, "ImportedPackage/Scenes/Pages/");


        bool showDetails = false;

        int selectedPageTypeToAdd = 0;
        int selectedSceneToAdd = 0;

        override protected bool doCreateGui(PropertyInfo curPropInfo, Object propertyObject)
        {
            configIsDirty = false;
            GUIContent myNamePrefixGUIContent = NamePrefixGUIContent;

            bool valsChanged = false;
            List<SceneMapping> allElements = ProductEditor.SelectedConfig.sceneMappings;

            List<string> availablePageTypesToMap = new List<string>();
            foreach (string pageType in ProductEditor.SelectedConfig.acceptedPageTypes)
            {
                bool alreadyMapped = false;
                foreach (SceneMapping mapping in allElements)
                {
                    if (pageType.Equals(mapping.pageTypeName))
                    {
                        alreadyMapped = true;
                        break; // do not add this page type since it is already mapped
                    }
                }

                if (!alreadyMapped)
                    availablePageTypesToMap.Add(pageType);
            }

            // Collect all available page scenes:
            List<string> pageScenes = new List<string>();
            // general project page scenes:
            foreach (string scenePath in Directory.GetFiles(ProjectScenesRootPath, "*.unity"))
            {
                pageScenes.Add(
                    scenePath.Substring(0, scenePath.Length - ".unity".Length)
                );
            }

            // product specific page scenes:
            if (Directory.Exists(ProductScenesRootPath))
                foreach (string scenePath in Directory.GetFiles(ProductScenesRootPath, "*.unity"))
                {
                    pageScenes.Add(
                        scenePath.Substring(0, scenePath.Length - ".unity".Length)
                    );
                }

            showDetails = EditorGUILayout.Foldout(showDetails,
                string.Format("Scene Mappings: ({0})", allElements.Count), STYLE_FOLDOUT_Bold);
            if (showDetails)
            {
                configIsDirty = false;
                using (new EditorGUI.DisabledGroupScope(
                    entryDisabled(curPropInfo) || availablePageTypesToMap.Count == 0))
                {
                    // Two line header with Add Button:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(
                        new GUIContent("Add Mapping from:"),
                        EditorStyles.textField,
                        STYLE_LABEL_Bold
                    );
                    selectedPageTypeToAdd =
                        EditorGUILayout.Popup(
                            selectedPageTypeToAdd,
                            availablePageTypesToMap.ToArray()
                        );
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(
                        new GUIContent("to:"),
                        EditorStyles.textField,
                        STYLE_LABEL_Bold_RightAdjusted
                    );
                    selectedSceneToAdd =
                        EditorGUILayout.Popup(
                            selectedSceneToAdd,
                            pageScenes.Select(s => s.Substring(
                                s.LastIndexOf("/") + 1)).ToArray());
                    if (GUILayout.Button("+"))
                    {
                        allElements.Add(
                            new SceneMapping(
                                availablePageTypesToMap[selectedPageTypeToAdd],
                                pageScenes[selectedSceneToAdd] + ".unity"
                            )
                        );
                        valsChanged = true;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                for (int i = 0; i < allElements.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    using (new EditorGUI.DisabledGroupScope(true))
                    {
                        EditorGUILayout.PrefixLabel(
                            allElements[i].pageTypeName + " ->",
                            EditorStyles.textField,
                            STYLE_LABEL_RightAdjusted
                        );
                        EditorGUILayout.TextField(
                            allElements[i].scenePath.Substring(
                                ProjectScenesRootPath.Length,
                                allElements[i].scenePath.Length - (ProjectScenesRootPath.Length + ".unity".Length)
                            )
                        );
                    }

                    if (GUILayout.Button("-"))
                    {
                        allElements.RemoveAt(i);
                        valsChanged = true;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (valsChanged)
                {
                    // Update Config property for scene extensions:
                    configIsDirty = true;
                    curPropInfo.SetValue(propertyObject, allElements, null);
                    List<EditorBuildSettingsScene> editorBuildScenes =
                        new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                    foreach (SceneMapping sm in allElements)
                    {
                        bool sceneInBuild = false;
                        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                        {
                            if (scene.path.Equals(sm.scenePath))
                            {
                                sceneInBuild = true;
                                scene.enabled = true;
                                break;
                            }
                        }

                        if (sceneInBuild == false)
                        {
                            // we need to add the target of this scene mapping to the build settings:
                            editorBuildScenes.Add(new EditorBuildSettingsScene(sm.scenePath, true));
                        }
                    }

                    EditorBuildSettings.scenes = editorBuildScenes.ToArray();

#if DEBUG_LOG
                    foreach (EditorBuildSettingsScene sc in EditorBuildSettings.scenes)
                    {
                        Debug.Log(("After Create GUI in Editor: EditorBuildSettings.scenes contains: " + sc.path).Red());
                    }
#endif
                }
            }

            return configIsDirty;
        }
    }
}