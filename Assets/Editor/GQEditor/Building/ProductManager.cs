using System.IO;
using System;
using System.Collections.Generic;
using GQ.Client.Util;
using GQ.Client.UI;
using UnityEngine;
using GQ.Client.Conf;
using System.Text;
using System.Linq;
using System.Collections;
using UnityEditor;
using System.Text.RegularExpressions;
using GQ.Editor.Util;
using GQTests;
using GQ.Editor.UI;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Reflection;
using GQ.Client.Err;

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


        public string _STREAMING_ASSET_PATH = "Assets/StreamingAssets";

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

        public const string START_SCENE = "Assets/Scenes/StartScene.unity";
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

            Debug.Log("Starting to prepare new product. Old product was: " + ConfigurationManager.Current.id);
            unloadAssetAddOns(ConfigurationManager.Current.assetAddOns);

            string productDirPath = Files.CombinePath(ProductsDirPath, productID);

            if (!Directory.Exists(productDirPath))
            {
                throw new ArgumentException("Product can not be build , since its Spec does not exist: " + productID);
            }

            ProductSpec newProduct = new ProductSpec(productDirPath);

            if (!newProduct.IsValid())
            {
                throw new ArgumentException("Invalid product: " + newProduct.Id + "\n" + newProduct.AllErrorsAsString());
            }

            // clear build folder:
            if (!Directory.Exists(BuildExportPath))
            {
                Directory.CreateDirectory(BuildExportPath);
            }

            Files.ClearDir(BuildExportPath);

            DirectoryInfo productDirInfo = new DirectoryInfo(productDirPath);

            foreach (FileInfo file in productDirInfo.GetFiles())
            {
                if (file.Name.StartsWith(".") || file.Name.EndsWith(".meta"))
                    continue;

                Files.CopyFile(
                    Files.CombinePath(productDirPath, file.Name),
                    BuildExportPath
                );
            }

            foreach (DirectoryInfo dir in productDirInfo.GetDirectories())
            {
                if (dir.Name.StartsWith("_") || dir.Name.Equals("StreamingAssets"))
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

            PlayerSettings.productName = newProduct.Config.name;
            string appIdentifier = ProductSpec.GQ_BUNDLE_ID_PREFIX + "." + newProduct.Config.id + newProduct.Config.idExtension;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, appIdentifier);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, appIdentifier);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, appIdentifier);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.WebGL, appIdentifier);

            ProductEditor.BuildIsDirty = false;
            CurrentProduct = newProduct; // remember the new product for the editor time access point.
            ConfigurationManager.Reset(); // tell the runtime access point that the product has changed.

            Scene currentScene = SceneManager.GetActiveScene();
            EditorSceneManager.OpenScene(currentScene.path);

            ProductEditor.IsCurrentlyPreparingProduct = false;
            GQAssetChangePostprocessor.writeBuildDate();

            // Load required AssetAddOns (plugins etc.):
            loadAssetAddOns(CurrentProduct.Config.assetAddOns);

            // update view in editor:
            LayoutConfig.ResetAll();
        }

        #endregion


        #region AssetAddOns

        private void unloadAssetAddOns(string[] assetAddOns)
        {
            foreach (string assetAddOn in assetAddOns)
            {
                unloadAaoRecursively(assetAddOn);

                deleteAaoSectionFromGitignore(assetAddOn);
            }
        }

        private void unloadAaoRecursively(string assetAddOn, string relPath = "")
        {
            Debug.Log("AAO UN-loading: " + relPath);

            // recursively go into every dir in the AssetAddOn tree:
            string aaoPath = Files.CombinePath(ASSET_ADD_ON_DIR_PATH, assetAddOn, relPath);
            foreach (string dir in Directory.GetDirectories(aaoPath))
            {
                unloadAaoRecursively(assetAddOn, Files.CombinePath(relPath, Files.DirName(dir)));
            }

            // set the according dir within Assets:
            string assetDir = Files.CombinePath(Application.dataPath, relPath);
            foreach (string file in Directory.GetFiles(aaoPath))
            {
                Debug.Log("AAO Deleting file: " + file);
                string assetFile = Files.CombinePath(assetDir, Files.FileName(file));
                Files.DeleteFile(assetFile);
            }

            // shall this directory be deleted?:
            if (!Directory.Exists(assetDir))
                // continue recursion if this dir does not exist (e.g. manually deleted)
                return;

            if (!Array.Exists(
                Directory.GetFiles(assetDir),
                element => Files.FileName(element).StartsWith(AAO_MARKERFILE_PREFIX, StringComparison.CurrentCulture)
            ))
            {
                // case 1: no marker file, i.e. this dir is independent of AAOs and is kept. We reduce gitignore.
                Debug.Log(string.Format("Dir {0} does NOT contain AAO Marker and is kept.", assetDir));
                //deleteAaoSectionFromGitignore(assetDir, assetAddOn);
            }
            else
            {
                Debug.Log(string.Format("Dir {0} DOES contain AAO Marker ...", assetDir));
                if (File.Exists(Files.CombinePath(assetDir, AAO_MARKERFILE_PREFIX + assetAddOn)))
                {
                    // this dir depended on the current AAO, hence we delete the according marker file:
                    bool deleted = Files.DeleteFile(Files.CombinePath(assetDir, AAO_MARKERFILE_PREFIX + assetAddOn));
                    Debug.Log(string.Format("Dir {0} contains our AAO Marker {1} and has been deleted: {2}.",
                                            assetDir, AAO_MARKERFILE_PREFIX + assetAddOn, deleted));
                }
                if (Array.Exists(
                    Directory.GetFiles(assetDir),
                    element => Files.FileName(element).StartsWith(AAO_MARKERFILE_PREFIX, StringComparison.CurrentCulture)
                ))
                {
                    // case 2: this dir depends also on other AAOs and is kept. We reduce the gitignore.
                    Debug.Log(string.Format("Dir {0} contains other AAO Markers hence we keep it.", assetDir));
                    //deleteAaoSectionFromGitignore(assetDir, assetAddOn);
                }
                else
                {
                    // case 3: this dir depended only on this AAO, hence we can delete it (including the gitignore)
                    Debug.Log(string.Format("Dir {0} contains NO other AAO Markers hence we DELETE it.", assetDir));
                    Files.DeleteDir(assetDir);
                }
            }

        }

        private void deleteAaoSectionFromGitignore(string assetAddOn)
        {
            if (!File.Exists(Files.GIT_EXCLUDE_FILE))
            {
                File.Create(Files.GIT_EXCLUDE_FILE);
            }

            string gitSectionRegExp =
                @"([\s\S]*)(# BEGIN GQ AAO: " + assetAddOn +
                @"[\s\S]*# END GQ AAO: " + assetAddOn +
                @")([\s\S]*)";

            string gitignoreText = File.ReadAllText(Files.GIT_EXCLUDE_FILE);

            Regex regex = new Regex(gitSectionRegExp);
            Match match = regex.Match(gitignoreText);
            if (match.Success)
            {
                string before = match.Groups[1].Value;
                string after = match.Groups[3].Value;
                gitignoreText = before + "\n" + after;
                Debug.Log("deleteAaoSectionFromGitignore: MATCHES: before: " + before + ", after: " + after);
            }
            else
            {
                Debug.Log("deleteAaoSectionFromGitignore: DID NOT MATCH");
            }

            gitignoreText = gitignoreText.Trim();

            if (gitignoreText.Length > 0)
            {
                File.WriteAllText(Files.GIT_EXCLUDE_FILE, gitignoreText);
            }
        }

        private void loadAssetAddOns(string[] assetAddOns)
        {
            List<string> gitignorePatterns = new List<string>();

            foreach (string assetAddOn in assetAddOns)
            {
                loadAaoRecursively(assetAddOn, gitignorePatterns, true);

                if (gitignorePatterns.Count > 0)
                {
                    // Store additions to git exclude file:
                    string gitExcludeSection = "\n\n# BEGIN GQ AAO: " + assetAddOn + "\n";
                    foreach (string pattern in gitignorePatterns)
                    {
                        gitExcludeSection += pattern + "\n";
                    }
                    gitExcludeSection += "# END GQ AAO: " + assetAddOn;
                    File.AppendAllText(Files.GIT_EXCLUDE_FILE, gitExcludeSection);
                }
            }

        }

        /// <summary>
        /// Loads the aao recursively.
        /// </summary>
        /// <returns><c>true</c>, if the current directory was created by this AAO and 
        /// should therefore be included in gitignore, <c>false</c> otherwise.</returns>
        /// <param name="assetAddOn">Asset add on.</param>
        /// <param name="gitCollectIgnores">Flag controlling wether gitignores are collected (not within created and completely ignored dirs).</param>
        /// <param name="relPath">Rel path.</param>
        private void loadAaoRecursively(string assetAddOn, List<string> gitignorePatterns, bool gitCollectIgnores, string relPath = "")
        {
            bool gitCollectIgnoresInSubdirs = gitCollectIgnores;
            Debug.Log("AAO Loading: " + relPath);
            // set the according dir within Assets:
            string assetDir = Files.CombinePath(Application.dataPath, relPath);
            // if dir does not exist in asstes create and mark it:
            if (!Directory.Exists(assetDir))
            {
                // this dir does not exist yet, we create it, mark it and collect git ignores if we are not in an already ignored subdir:
                Files.CreateDir(assetDir);
                string dirMarkerFile = Files.CombinePath(assetDir, AAO_MARKERFILE_PREFIX + assetAddOn);
                File.Create(dirMarkerFile);
                if (gitCollectIgnores)
                {
                    gitignorePatterns.Add(Assets.RelativeAssetPath(assetDir) + "/");
                    gitignorePatterns.Add(Assets.RelativeAssetPath(assetDir) + ".meta");
                    gitCollectIgnoresInSubdirs = false;
                }
            }
            else
            {
                if (Array.Exists(
                    Directory.GetFiles(assetDir),
                    element => Files.FileName(element).StartsWith(AAO_MARKERFILE_PREFIX, StringComparison.CurrentCulture)
                ))
                {
                    // this dir has been created by another aao so we add our marker file:
                    string newAAOMarkerfile = Files.CombinePath(assetDir, AAO_MARKERFILE_PREFIX + assetAddOn);
                    File.Create(newAAOMarkerfile);
                    if (gitCollectIgnores)
                        gitignorePatterns.Add(Assets.RelativeAssetPath(newAAOMarkerfile));
                }
            }
            // copy all files from corresponding aao dir into this asset dir:
            string aaoPath = Files.CombinePath(ASSET_ADD_ON_DIR_PATH, assetAddOn, relPath);
            foreach (string file in Directory.GetFiles(aaoPath))
            {
                if (file.EndsWith(".DS_Store", StringComparison.CurrentCulture))
                {
                    continue;
                }
                if (file.EndsWith(AAO_MARKERFILE_PREFIX + assetAddOn, StringComparison.CurrentCulture))
                {
                    Log.SignalErrorToDeveloper(
                        "Asset-Add-On {0} is not compatible with our add-on-system: it includes itself an .AssetAddOn file.",
                        AAO_MARKERFILE_PREFIX + assetAddOn);
                }
                if (File.Exists(Files.CombinePath(assetDir, Files.FileName(file))))
                {
                    Log.SignalErrorToDeveloper(
                        "Asset-Add-On {0} is not compatible with another add-on or our add-on-system: the file {1} already exists and will not be overridden.",
                        assetAddOn,
                        Files.CombinePath(assetDir, Files.FileName(file))
                    );
                }
                Debug.Log("AAO File Copy: " + file);
                Files.CopyFile(
                    fromFilePath: file,
                    toDirPath: assetDir,
                    overwrite: false
                );
                if (gitCollectIgnores)
                {
                    gitignorePatterns.Add(
                    Files.CombinePath(Assets.RelativeAssetPath(assetDir), Files.FileName(file)));
                    gitignorePatterns.Add(
                        Files.CombinePath(Assets.RelativeAssetPath(assetDir), Files.FileName(file) + ".meta"));
                }
            }
            // recursively go into every dir in the AssetAddOn tree:
            foreach (string dir in Directory.GetDirectories(aaoPath))
            {
                loadAaoRecursively(assetAddOn, gitignorePatterns, gitCollectIgnoresInSubdirs, Files.CombinePath(relPath, Files.DirName(dir)));
            }
        }

        /// <summary>
        /// In this directory all defined AsssetAddOns are stored.
        /// </summary>
        private static string ASSET_ADD_ON_DIR_PATH = Files.CombinePath(GQAssert.PROJECT_PATH, "Production/AssetAddOns/");

        private static string AAO_MARKERFILE_PREFIX = ".AssetAddOn_";

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