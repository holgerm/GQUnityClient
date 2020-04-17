#define DEBUG_LOG

using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Text.RegularExpressions;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.UI.layout;
using GQ.Editor.Util;
using GQTests;
using GQ.Editor.UI;
using Newtonsoft.Json;
using UnityEditor.SceneManagement;

namespace GQ.Editor.Building
{
    public class ProductManager
    {

        #region Names, Paths and Storage

        /// <summary>
        /// In this directory all defined products are stored. This data is NOT included in the app build.
        /// </summary>
        private static string PRODUCTS_DIR_PATH_DEFAULT = Files.CombinePath(GQAssert.PROJECT_PATH, "Production/products/");

        /// <summary>
        /// This is the template for new products which is copied when we create a new product. It should contain a complete product definition.
        /// </summary>
        public const string TEMPLATE_PRODUCT_PATH = "Assets/Editor/productsTemplate/templateProduct";

        static private string _productsDirPath = PRODUCTS_DIR_PATH_DEFAULT;

        /// <summary>
        /// Setting the product dir creates a completely fresh instance for this singleton and reinitializes all products. 
        /// The formerly known products are "forgotten".
        /// </summary>
        /// <value>The products dir path.</value>
        public static string ProductsDirPath
        {
            get
            {
                return _productsDirPath;
            }
            set
            {
                _productsDirPath = value;
                _instance = new ProductManager();
            }
        }

        public static string PRODUCT_ADDON_PATH
        {
            get
            {
                return GQAssert.PROJECT_PATH + "/" + "Production/products addon/";
            }
        }

        private string _buildExportPath = ConfigurationManager.RUNTIME_PRODUCT_DIR;

        public string BuildExportPath
        {
            get
            {
                return _buildExportPath;
            }
            set
            {
                _buildExportPath = value;
            }
        }


        public string _ANDROID_MANIFEST_DIR = "Assets/Plugins/Android";

        public string ANDROID_MANIFEST_DIR
        {
            get
            {
                return _ANDROID_MANIFEST_DIR;
            }
            private set
            {
                _ANDROID_MANIFEST_DIR = value;
            }
        }

        public string ANDROID_MANIFEST_FILE
        {
            get
            {
                return Files.CombinePath(ANDROID_MANIFEST_DIR, ProductSpec.ANDROID_MANIFEST);
            }
        }


        public string _STREAMING_ASSET_PATH = "Assets/StreamingAssets/prod";

        public string STREAMING_ASSET_PATH
        {
            get
            {
                return _STREAMING_ASSET_PATH;
            }
            private set
            {
                _STREAMING_ASSET_PATH = value;
            }
        }

        public const string DEFAULT_START_SCENE = "DefaultAssets/DefaultStartScene.unity";
        public const string FOYER_SCENE = "Assets/Scenes/Foyer.unity";
        public const string LOADING_CANVAS_NAME = "LoadingCanvas";
        public const string LOADING_CANVAS_PREFAB = "loadingCanvas/LoadingCanvas";
        public const string LOADING_CANVAS_CONTAINER_TAG = "LoadingCanvasContainer";

        #endregion

        #region State

        private bool _configFilesHaveChanges;

        /// <summary>
        /// True if current configuration has changes that are not persistantly stored in the product specifications. 
        /// Any change of files within the ConfigAssets/Resources folder will set this flag to true. 
        /// Pressing the persist button in the GQ Product Editor will set it to false.
        /// </summary>
        public bool ConfigFilesHaveChanges
        {
            get
            {
                return _configFilesHaveChanges;
            }
            set
            {
                if (_configFilesHaveChanges != value)
                {
                    _configFilesHaveChanges = value;
                    EditorPrefs.SetBool("configDirty", _configFilesHaveChanges);
                }
            }
        }

        #endregion


        #region Access to Products

        internal Dictionary<string, ProductSpec> _productDict;

        public ICollection<ProductSpec> AllProducts
        {
            get
            {
                return Instance._productDict.Values;
            }
        }

        public ICollection<string> AllProductIds
        {
            get
            {
                return Instance._productDict.Keys;
            }
        }

        public ProductSpec GetProduct(string productID)
        {
            ProductSpec found = null;

            if (Instance._productDict.TryGetValue(productID, out found))
                return found;
            else
                return null;
        }

        public void SetProductConfig(string id, Config config)
        {
            //			if (_productDict.ContainsKey(id)) {
            //				_productDict.Remove (id);
            //			}
            _productDict[id].Config = config;
        }

        #endregion


        #region Singleton

        static private ProductManager _instance;

        public static ProductManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ProductManager();
                }
                return _instance;
            }
        }

        // TODO move test instance stuff into a testable subclass?
        static private ProductManager _testInstance;

        public static ProductManager TestInstance
        {
            get
            {
                if (_testInstance == null)
                {
                    _testInstance = new ProductManager();

                    _testInstance._buildExportPath =
                        Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "Output", "ConfigAssets", "Resources");
                    if (!Directory.Exists(_testInstance.BuildExportPath))
                        Directory.CreateDirectory(_testInstance.BuildExportPath);

                    string androidPluginDirPath = Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "Output", "Plugins", "Android");
                    _testInstance.ANDROID_MANIFEST_DIR =
                        Files.CombinePath(androidPluginDirPath);
                    if (!Directory.Exists(androidPluginDirPath))
                        Directory.CreateDirectory(androidPluginDirPath);

                    _testInstance.STREAMING_ASSET_PATH =
                        Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "Output", "StreamingAssets");
                    if (!Directory.Exists(_testInstance.STREAMING_ASSET_PATH))
                        Directory.CreateDirectory(_testInstance.STREAMING_ASSET_PATH);
                }
                return _testInstance;
            }
        }

        private ProductManager()
        {
            _errors = new List<string>();
            InitProductDictionary();
            IsImportingPackage = false;
        }

        internal void InitProductDictionary()
        {
            string oldSelectedProductID = null;
            if (_currentProduct != null)
                oldSelectedProductID = _currentProduct.Id;

            _productDict = new Dictionary<string, ProductSpec>();

            IEnumerable<string> productDirCandidates = Directory.GetDirectories(ProductsDirPath).Select(d => new DirectoryInfo(d).FullName);

            foreach (var productCandidatePath in productDirCandidates)
            {
                LoadProductSpec(productCandidatePath);
            }

            if (oldSelectedProductID != null)
            {
                _productDict.TryGetValue(oldSelectedProductID, out _currentProduct);
            }
            else
                _currentProduct = null;
        }

        /// <summary>
        /// Loads the product spec from the given driectory. Any errors are stored in Errors.
        /// </summary>
        /// <returns>The product spec or null if an error occurred.</returns>
        /// <param name="productCandidatePath">Product candidate path.</param>
        internal ProductSpec LoadProductSpec(string productCandidatePath)
        {
            ProductSpec product;
            try
            {
                product = new ProductSpec(productCandidatePath);
                if (_productDict.ContainsKey(product.Id))
                    _productDict.Remove(product.Id);
                _productDict.Add(product.Id, product);
                return product;
            }
            catch (ArgumentException exc)
            {
                Errors.Add("Product Manager found invalid product directory: " + productCandidatePath + "\n" + exc.Message + "\n\n");
                return null;
            }
        }

        internal static void _dispose()
        {
            _productsDirPath = PRODUCTS_DIR_PATH_DEFAULT;
            if (_instance == null)
                return;
            _instance._productDict.Clear();
            _instance._productDict = null;
            _instance = null;
        }

        #endregion


        #region Interaction API

        public ProductSpec createNewProduct(string newProductID)
        {
            if (!ProductSpec.IsValidProductName(newProductID))
            {
                throw new ArgumentException("Invalid product id: " + newProductID);
            }

            string newProductDirPath = Files.CombinePath(ProductsDirPath, newProductID);

            if (Directory.Exists(newProductDirPath))
            {
                throw new ArgumentException("Product name already used: " + newProductID + " in: " + newProductDirPath);
            }

            // copy default template files to a new product folder:
            Files.CreateDir(newProductDirPath);
            Files.CopyDirContents(TEMPLATE_PRODUCT_PATH, newProductDirPath);

            // create Config, populate it with defaults and serialize it into the new product folder:
            createConfigWithDefaults(newProductID);

            ProductSpec newProduct = new ProductSpec(newProductDirPath);
            // append a watermark to the blank AndroidManifest file:
            string watermark = MakeXMLWatermark(newProduct.Id);
            using (StreamWriter sw = File.AppendText(newProduct.AndroidManifestPath))
            {
                sw.WriteLine(watermark);
                sw.Close();
            }



            Instance._productDict.Add(newProduct.Id, newProduct);
            return newProduct;
        }

        private IList<string> _errors;

        /// <summary>
        /// A list of current errors that could be used to show the users (developers) in the Product Editor View which product definitions are invalid. TODO
        /// </summary>
        /// <value>The errors.</value>
        public IList<string> Errors
        {
            get
            {
                return _errors;
            }
        }

        private ProductSpec _currentProduct;

        public ProductSpec CurrentProduct
        {
            get
            {
                return _currentProduct;
            }
            internal set
            {
                _currentProduct = value;
            }
        }

        /// <summary>
        /// Sets the product for build, i.e. files are copied from the product dir to the client configuration dir. 
        /// E.g. for 'wcc' the product dir is in 'Assets/Editor/products/wcc'. 
        /// The client configuration dir is always at 'Assets/ConfigAssets/Ressources'.
        /// 
        /// The following file are copied:
        /// 
        /// 1. All files directly stored in the product dir into the config dir.
        /// 2. AndroidManifest (in 'productDir') to 'Assets/Plugins/Android/'
        /// 3. TODO: Player Preferences?
        /// 
        /// </summary>
        /// <param name="productID">Product I.</param>
        public void PrepareProductForBuild(string productID)
        {

            ProductEditor.IsCurrentlyPreparingProduct = true;

            string productDirPath = Files.CombinePath(ProductsDirPath, productID);

            if (!Directory.Exists(productDirPath))
            {
                throw new ArgumentException("Product can not be build , since its Spec does not exist: " + productID);
            }

            ProductSpec newProduct = new ProductSpec(productDirPath);

            // Un-/Load required AssetAddOns (plugins etc.):
            AssetAddOnManager.switchAssetAddOns(ConfigurationManager.Current, newProduct.Config);

            if (!newProduct.IsValid())
            {
                throw new ArgumentException("Invalid product: " + newProduct.Id + "\n" + newProduct.AllErrorsAsString());
            }

            // load Foyer scene (which exists for all products):
            EditorSceneManager.OpenScene(FOYER_SCENE);

            // clear build folder:
            if (!Directory.Exists(BuildExportPath))
            {
                Directory.CreateDirectory(BuildExportPath);
            }

            Files.ClearDir(BuildExportPath);

            string packageFile = Files.CombinePath(productDirPath, productID + ".unitypackage");
            if (File.Exists(packageFile))
            {
                // import unity packages instead of just copying the files:
                // TODO might have a problem when multiple package files are present! 
                // (cf. https://answers.unity.com/questions/135233/running-assetdatabaseimportpackage-on-multiple-pac.html)
                AssetDatabase.importPackageStarted += (string packageName) =>
                {
                    Debug.Log(("Importing package " + packageName + " started!").Yellow());
                    Instance.IsImportingPackage = true;
                };
                AssetDatabase.importPackageFailed += (string packageName, string errorMessage) =>
                {
                    Instance.IsImportingPackage = false;
                    Log.SignalErrorToDeveloper("Importing package " + packageName + " failed! " + errorMessage);
                };
                AssetDatabase.importPackageCancelled += (string packageName) =>
                {
                    Debug.Log(("Importing package " + packageName + " cancelled! ").Yellow());
                    Instance.IsImportingPackage = false;
                    Log.SignalErrorToDeveloper("Importing package " + packageName + " cancelled! Why?");
                };
                AssetDatabase.importPackageCompleted += (string packageName) =>
                {
                    Debug.Log(("Importing package " + packageName + " completed! ").Yellow());
                    Instance.IsImportingPackage = false;
                    prepareProductTheRestAfterPackageIsImported(newProduct, productDirPath);
                };
                AssetDatabase.ImportPackage(packageFile, false);
            }
            else
            {
                // if we have no package file:
                prepareProductTheRestAfterPackageIsImported(newProduct, productDirPath);
            }
        }

        public bool IsImportingPackage
        {
            set;
            get;
        }

        private void prepareProductTheRestAfterPackageIsImported(ProductSpec newProduct, string productDirPath)
        {
            // Do the rest to activate the new product:
            DirectoryInfo productDirInfo = new DirectoryInfo(productDirPath);

            foreach (FileInfo file in productDirInfo.GetFiles())
            {
                if (file.Name.StartsWith(".", StringComparison.CurrentCulture) || file.Name.EndsWith(".meta", StringComparison.CurrentCulture))
                    // ignore hidden files and unity meta files:
                    continue;

                if (file.Name.EndsWith(".unitypackage", StringComparison.CurrentCulture))
                {
                    continue;
                }

                Files.CopyFile(
                    Files.CombinePath(productDirPath, file.Name),
                    BuildExportPath
                );
            }

            foreach (DirectoryInfo dir in productDirInfo.GetDirectories())
            {
                if (dir.Name.StartsWith("_", StringComparison.CurrentCulture) || dir.Name.Equals("StreamingAssets"))
                    continue;

                Files.CopyDir(
                    Files.CombinePath(productDirPath, dir.Name),
                    BuildExportPath
                );
            }

            // copy AndroidManifest (additionally) to plugins/android directory:
            Files.CopyFile(
                Files.CombinePath(
                    BuildExportPath,
                    ProductSpec.ANDROID_MANIFEST
                ),
                ANDROID_MANIFEST_DIR
            );

            // copy StreamingAssets:
            if (Files.ExistsDir(STREAMING_ASSET_PATH))
                Files.ClearDir(STREAMING_ASSET_PATH);
            else
                Files.CreateDir(STREAMING_ASSET_PATH);

            if (Directory.Exists(newProduct.StreamingAssetPath))
            {
                Files.CopyDirContents(
                    newProduct.StreamingAssetPath,
                    STREAMING_ASSET_PATH);
            }

            // gather scenes ans set them in EditorBuidlSettings:
            var scenes =
                 gatherScenesFromPackage(
                     new List<string>(),
                     Files.CombinePath(ConfigurationManager.RUNTIME_PRODUCT_DIR, "ImportedPackage")
                 );

            // add all config scenes:
            scenes.AddRange(newProduct.Config.scenePaths);

            // detect if we have start scene:
            var startSceneIncluded = false;
            for (var i = 0; i < scenes.Count && !startSceneIncluded; i++)
            {
                if (scenes[i].EndsWith("StartScene.unity", StringComparison.Ordinal))
                {
                    startSceneIncluded = true;
                    
                    if (i > 0)
                        // make start scene the first scene in the list:
                    {
                        var tmp = scenes[0];
                        scenes.Insert(0, scenes[i]);
                        scenes.Insert(i, tmp);
#if DEBUG_LOG
                        Debug.LogFormat("Start Scene found at index {0}: {1} - replaced it as first scene.", i, DEFAULT_START_SCENE);
#endif
                    }
                }
            }

            var ebsScenes = new List<EditorBuildSettingsScene>();

            if (!startSceneIncluded)
            {
                ebsScenes.Add(new EditorBuildSettingsScene(DEFAULT_START_SCENE, true));
#if DEBUG_LOG
                Debug.LogFormat("No Start Scene found - using default start scene: {0}", DEFAULT_START_SCENE);
#endif
            }
#if DEBUG_LOG
            Debug.LogFormat("Further scenes adding:  #{0}", scenes.Count);
#endif
            foreach (var scenePath in scenes)
            {
                var curScenePath = scenePath.Substring("Assets/".Length);
                ebsScenes.Add(new EditorBuildSettingsScene(curScenePath, true));
#if DEBUG_LOG
                Debug.LogFormat("Scene added: {0}", curScenePath);
#endif
            }
            EditorBuildSettings.scenes = ebsScenes.ToArray();

            PlayerSettings.productName = newProduct.Config.name;
            var appIdentifier = ProductSpec.GQ_BUNDLE_ID_PREFIX + "." + newProduct.Config.id + newProduct.Config.idExtension;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, appIdentifier);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, appIdentifier);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, appIdentifier);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.WebGL, appIdentifier);

            PlayerSettings.iOS.cameraUsageDescription = "We use camera to enable interactive experiences.";
            PlayerSettings.iOS.locationUsageDescription = "We use gps to show your location on the map.";
            PlayerSettings.iOS.microphoneUsageDescription = "We use microphone to enable interactive experiences.";

            ProductEditor.BuildIsDirty = false;
            CurrentProduct = newProduct; // remember the new product for the editor time access point.
            ConfigurationManager.Reset(); // tell the runtime access point that the product has changed.

            //if (ebsScenes.Count > 0) 
            //    EditorSceneManager.OpenScene(ebsScenes[0].path);

            ProductEditor.IsCurrentlyPreparingProduct = false;
            GQAssetChangePostprocessor.writeBuildDate();

            // update view in editor:
            LayoutConfig.ResetAll();

            var completedAt = DateTime.Now;
            Debug.LogWarning("COMPLETED Preparing product " + newProduct.Id + " at " + completedAt.Hour + ":" + completedAt.Minute + ":" + completedAt.Second + "." + completedAt.Millisecond);
        }

        List<string> gatherScenesFromPackage(List<string> gatheredScenes, string dir)
        {
            if (Directory.Exists(dir))
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    if (file.EndsWith(".unity", StringComparison.CurrentCulture))
                    {
                        gatheredScenes.Add(Assets.RelativeAssetPath(file));
                    }
                }

                foreach (string subdir in Directory.GetDirectories(dir))
                {
                    gatheredScenes = gatherScenesFromPackage(gatheredScenes, subdir);
                }
            }

            return gatheredScenes;
        }

        #endregion


        #region Helper Methods

        private void createConfigWithDefaults(string productID)
        {
            Config config = new Config();

            // set product specific default values:
            config.id = productID;
            config.name = "QuestMill App " + productID;

            // serialize into new product folder:
            serializeConfig(config, Files.CombinePath(ProductsDirPath, productID));
        }

        internal void serializeConfig(Config config, string productDirPath)
        {
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            string configFilePath = Files.CombinePath(productDirPath, ConfigurationManager.CONFIG_FILE);
            File.WriteAllText(configFilePath, json);
            if (Assets.IsAssetPath(configFilePath))
                AssetDatabase.Refresh();
        }

        /// <summary>
        /// The watermark that is included in each products android manifest file to associate it with the product.
        /// </summary>
        /// <returns>The product manifest watermark.</returns>
        /// <param name="productId">Product identifier.</param>
        public static string MakeXMLWatermark(string id)
        {
            return String.Format("<!-- product id: {0} -->", id);
        }

        public static string Extract_ID_FromXML_Watermark(string filepath)
        {
            if (!File.Exists(filepath))
                return null;
            string xmlText = File.ReadAllText(filepath);
            Match match = Regex.Match(xmlText, @"<!-- product id: ([-a-zA-Z0-9_]+) -->");
            if (match.Success)
                return match.Groups[1].Value;
            else
                return null;
        }

        #endregion
    }

}