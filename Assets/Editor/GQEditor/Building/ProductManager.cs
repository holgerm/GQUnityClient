using System.IO;
using System;
using System.Collections.Generic;
using GQ.Util;
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

namespace GQ.Editor.Building {
	public class ProductManager {

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
		public static string ProductsDirPath {
			get {
				return _productsDirPath;
			}
			set {
				_productsDirPath = value;
				_instance = new ProductManager();
			}
		}

		private string _buildExportPath = ConfigurationManager.RUNTIME_PRODUCT_DIR;

		public string BuildExportPath {
			get {
				return _buildExportPath;
			}
			set {
				_buildExportPath = value;
			}
		}


		public string _ANDROID_MANIFEST_DIR = "Assets/Plugins/Android";

		public string ANDROID_MANIFEST_DIR {
			get {
				return _ANDROID_MANIFEST_DIR;
			}
			private set {
				_ANDROID_MANIFEST_DIR = value;
			}
		}

		public string ANDROID_MANIFEST_FILE {
			get {
				return Files.CombinePath(ANDROID_MANIFEST_DIR, ProductSpec.ANDROID_MANIFEST);
			}
		}


		public string _STREAMING_ASSET_PATH = "Assets/StreamingAssets";

		public string STREAMING_ASSET_PATH {
			get {
				return _STREAMING_ASSET_PATH;
			}
			private set {
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
		public bool ConfigFilesHaveChanges {
			get {
				return _configFilesHaveChanges;
			}
			set {
				if ( _configFilesHaveChanges != value ) {
					_configFilesHaveChanges = value;
					EditorPrefs.SetBool("configDirty", _configFilesHaveChanges);
				}
			}
		}

		#endregion


		#region Access to Products

		private Dictionary<string, ProductSpec> _productDict;

		public ICollection<ProductSpec> AllProducts {
			get {
				return Instance._productDict.Values;
			}
		}

		public ICollection<string> AllProductIds {
			get {
				return Instance._productDict.Keys;
			}
		}

		public ProductSpec GetProduct (string productID) {
			ProductSpec found = null;

			if ( Instance._productDict.TryGetValue(productID, out found) )
				return found;
			else
				return null;
		}

		#endregion


		#region Singleton

		static private ProductManager _instance;

		public static ProductManager Instance {
			get {
				if ( _instance == null ) {
					_instance = new ProductManager();
				}
				return _instance;
			}
		}

		// TODO move test instance stuff into a testable subclass?
		static private ProductManager _testInstance;

		public static ProductManager TestInstance {
			get {
				if ( _testInstance == null ) {
					_testInstance = new ProductManager();

					_testInstance._buildExportPath = 
						Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "Output", "ConfigAssets", "Resources");
					if ( !Directory.Exists(_testInstance.BuildExportPath) )
						Directory.CreateDirectory(_testInstance.BuildExportPath);

					string androidPluginDirPath = Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "Output", "Plugins", "Android");
					_testInstance.ANDROID_MANIFEST_DIR = 
						Files.CombinePath(androidPluginDirPath);
					if ( !Directory.Exists(androidPluginDirPath) )
						Directory.CreateDirectory(androidPluginDirPath);

					_testInstance.STREAMING_ASSET_PATH = 
						Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "Output", "StreamingAssets");
					if ( !Directory.Exists(_testInstance.STREAMING_ASSET_PATH) )
						Directory.CreateDirectory(_testInstance.STREAMING_ASSET_PATH);
				}
				return _testInstance;
			}
		}

		private ProductManager () {
			_errors = new List<string>();
			InitProductDictionary();
		}

		internal void InitProductDictionary () {
			string oldSelectedProductID = null;
			if ( _currentProduct != null )
				oldSelectedProductID = _currentProduct.Id;
			
			_productDict = new Dictionary<string, ProductSpec>();

			IEnumerable<string> productDirCandidates = Directory.GetDirectories(ProductsDirPath).Select(d => new DirectoryInfo(d).FullName);

			foreach ( var productCandidatePath in productDirCandidates ) {
				LoadProductSpec(productCandidatePath);
			}

			if ( oldSelectedProductID != null ) {
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
		internal ProductSpec LoadProductSpec (string productCandidatePath) {
			ProductSpec product;
			try {
				product = new ProductSpec(productCandidatePath);
				if ( _productDict.ContainsKey(product.Id) )
					_productDict.Remove(product.Id);
				_productDict.Add(product.Id, product);
				return product;
			} catch ( ArgumentException exc ) {
				Errors.Add("Product Manager found invalid product directory: " + productCandidatePath + "\n" + exc.Message + "\n\n");
				return null;
			}
		}

		internal static void _dispose () {
			_productsDirPath = PRODUCTS_DIR_PATH_DEFAULT;
			if ( _instance == null )
				return;
			_instance._productDict.Clear();
			_instance._productDict = null;
			_instance = null;
		}

		#endregion


		#region Interaction API

		public ProductSpec createNewProduct (string newProductID) {
			if ( !ProductSpec.IsValidProductName(newProductID) ) {
				throw new ArgumentException("Invalid product id: " + newProductID);
			}

			string newProductDirPath = Files.CombinePath(ProductsDirPath, newProductID);

			if ( Directory.Exists(newProductDirPath) ) {
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
			using ( StreamWriter sw = File.AppendText(newProduct.AndroidManifestPath) ) {
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
		public IList<string> Errors {
			get {
				return _errors;
			}
		}

		private ProductSpec _currentProduct;

		public ProductSpec CurrentProduct {
			get {
				return _currentProduct;
			}
			private set {
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
		public void PrepareProductForBuild (string productID) {
			
			ProductEditor.IsCurrentlyPreparingProduct = true;

			string productDirPath = Files.CombinePath(ProductsDirPath, productID);

			if ( !Directory.Exists(productDirPath) ) {
				throw new ArgumentException("Product can not be build , since its Spec does not exist: " + productID);
			}

			ProductSpec newProduct = new ProductSpec(productDirPath);

			if ( !newProduct.IsValid() ) {
				throw new ArgumentException("Invalid product: " + newProduct.Id + "\n" + newProduct.AllErrorsAsString());
			}

			// clear build folder:
			if ( !Directory.Exists(BuildExportPath) ) {
				Directory.CreateDirectory(BuildExportPath);
			}

			Files.ClearDir(BuildExportPath); 

			DirectoryInfo productDirInfo = new DirectoryInfo(productDirPath);

			foreach ( FileInfo file in productDirInfo.GetFiles() ) {
				if ( file.Name.StartsWith(".") || file.Name.EndsWith(".meta") )
					continue;

				Files.CopyFile(
					Files.CombinePath(productDirPath, file.Name), 
					BuildExportPath
				);
			}

			foreach ( DirectoryInfo dir in productDirInfo.GetDirectories() ) {
				if ( dir.Name.StartsWith("_") || dir.Name.Equals("StreamingAssets") )
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
			if ( Files.ExistsDir(STREAMING_ASSET_PATH) )
				Files.ClearDir(STREAMING_ASSET_PATH);
			else
				Files.CreateDir(STREAMING_ASSET_PATH);

			if ( Directory.Exists(newProduct.StreamingAssetPath) ) {
				Files.CopyDirContents(
					newProduct.StreamingAssetPath, 
					STREAMING_ASSET_PATH);
			}

			PlayerSettings.productName = newProduct.Config.name;
			PlayerSettings.bundleIdentifier = ProductSpec.GQ_BUNDLE_ID_PREFIX + "." + newProduct.Config.id;

			replaceLoadingLogoInScene(START_SCENE);

			ProductEditor.BuildIsDirty = false;
			CurrentProduct = newProduct; // remember the new product for the editor time access point.
			ConfigurationManager.Reset(); // tell the runtime access point that the product has changed.

			ProductEditor.IsCurrentlyPreparingProduct = false;
			GQAssetChangePostprocessor.writeBuildDate();
		}

		private void replaceLoadingLogoInScene (string scenePath) {
			// save currently open scenes and open start scene:
			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
			EditorSceneManager.OpenScene(scenePath); 
			Scene startScene = SceneManager.GetSceneByPath(scenePath);

			if ( !startScene.IsValid() ) {
				Errors.Add("Start scene is not valid or not found.");
				return;
			}

			// destroy old loading canvas if exists:
			foreach ( GameObject lcc in GameObject.FindGameObjectsWithTag(LOADING_CANVAS_CONTAINER_TAG) ) {
				foreach ( Transform child in lcc.transform ) {
					UnityEngine.Object.DestroyImmediate(child.gameObject);
				}
			}


			// load prefab for loading canvas:
			GameObject loadingCanvasPrefab = Resources.Load<GameObject>(LOADING_CANVAS_PREFAB);
			if ( loadingCanvasPrefab == null ) {
				Errors.Add("Product misses LoadingCanvas prefab.");
				return;
			}

			// instantiate new loading canvas(es) from prefab into all LCCs:
			foreach ( GameObject lcc in GameObject.FindGameObjectsWithTag(LOADING_CANVAS_CONTAINER_TAG) ) {
				foreach ( Transform child in lcc.transform ) {
					UnityEngine.Object.DestroyImmediate(child.gameObject);
				}

				GameObject loadingCanvas = (GameObject)PrefabUtility.InstantiatePrefab(loadingCanvasPrefab, startScene);
				if ( loadingCanvas == null ) {
					Errors.Add("Unable to create LoadingCanvas.");
					return;
				}
				loadingCanvas.transform.parent = lcc.transform;
				loadingCanvas.name = LOADING_CANVAS_NAME;

				// if product has initializer call it:
				Type loadingCanvasType = null;
				try {
					loadingCanvasType = Type.GetType("GQ.Client.Conf.LoadingCanvas");
				} catch ( Exception e ) {
					Debug.Log("Exception: " + e.Message);
					loadingCanvasType = null;
				}
				if ( loadingCanvasType != null ) {
					Debug.Log("found type: " + loadingCanvasType.FullName);
					MethodInfo initMethod = 
						loadingCanvasType.GetMethod(
							"Init", 
							new Type[] {
								typeof(UnityEngine.GameObject) 
							}
						);
					if ( initMethod != null )
						initMethod.Invoke(null, new GameObject[] {
							loadingCanvas
						});
				}
			}
		}

		#endregion


		#region Helper Methods

		private void createConfigWithDefaults (string productID) {
			Config config = new Config();

			// set product specific default values:
			config.id = productID;
			config.name = "QuestMill App " + productID;

			// serialize into new product folder:
			serializeConfig(config, Files.CombinePath(ProductsDirPath, productID));
		}

		internal void serializeConfig (Config config, string productDirPath) {
			string json = JsonConvert.SerializeObject(config, Formatting.Indented);
			string configFilePath = Files.CombinePath(productDirPath, ConfigurationManager.CONFIG_FILE);
			File.WriteAllText(configFilePath, json);
			if ( Assets.IsAssetPath(configFilePath) )
				AssetDatabase.Refresh();
		}

		/// <summary>
		/// The watermark that is included in each products android manifest file to associate it with the product.
		/// </summary>
		/// <returns>The product manifest watermark.</returns>
		/// <param name="productId">Product identifier.</param>
		public static string MakeXMLWatermark (string id) {
			return String.Format("<!-- product id: {0} -->", id);
		}

		public static string Extract_ID_FromXML_Watermark (string filepath) {
			if ( !File.Exists(filepath) )
				return null;
			string xmlText = File.ReadAllText(filepath);
			Match match = Regex.Match(xmlText, @"<!-- product id: ([-a-zA-Z0-9_]+) -->");
			if ( match.Success )
				return match.Groups[1].Value;
			else
				return null;
		}

		#endregion
	}



}