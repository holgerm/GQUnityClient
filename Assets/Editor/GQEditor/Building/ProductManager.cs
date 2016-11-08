using System.IO;
using System;
using System.Collections.Generic;
using GQ.Util;
using UnityEngine;
using GQ.Client.Conf;
using System.Text;
using LitJson;
using System.Linq;
using System.Collections;
using UnityEditor;
using System.Text.RegularExpressions;
using GQ.Editor.Util;
using GQTests;
using GQ.Editor.UI;

namespace GQ.Editor.Building {
	public class ProductManager {

		#region Paths and Storage

		/// <summary>
		/// In this directory all defined products are stored. This data is NOT included in the app build.
		/// </summary>
		public const string PRODUCTS_DIR_PATH_DEFAULT = "Assets/Editor/products/";

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


		public string _ANDROID_MANIFEST_PATH = "Assets/Plugins/Android/AndroidManifest.xml";

		public string ANDROID_MANIFEST_PATH {
			get {
				return _ANDROID_MANIFEST_PATH;
			}
			private set {
				_ANDROID_MANIFEST_PATH = value;
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
					_testInstance.ANDROID_MANIFEST_PATH = 
						Files.CombinePath(androidPluginDirPath, "AndroidManifest.xml");
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
			_productDict = new Dictionary<string, ProductSpec>();
			_currentProduct = null;

			IEnumerable<string> productDirCandidates = Directory.GetDirectories(ProductsDirPath).Select(d => new DirectoryInfo(d).FullName);

			foreach ( var productCandidatePath in productDirCandidates ) {
				LoadProductSpec(productCandidatePath);
			}
		}

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
			Assets.CreateSubfolder(ProductsDirPath, newProductID);
			AssetDatabase.Refresh();
			Assets.CopyAssetsDir(TEMPLATE_PRODUCT_PATH, newProductDirPath);

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

			Assets.ClearAssetFolder(BuildExportPath); 

			Assets.CopyAssetsDir(productDirPath, BuildExportPath, false);

			DirectoryInfo productDirInfo = new DirectoryInfo(productDirPath);

			foreach ( DirectoryInfo dir in productDirInfo.GetDirectories() ) {
				if ( dir.Name.StartsWith("_") || dir.Name.Equals("StreamingAssets") )
					continue;
				
				Assets.CreateSubfolder(BuildExportPath, dir.Name);

				Assets.CopyAssetsDir(
					Files.CombinePath(productDirPath, dir.Name), 
					Files.CombinePath(BuildExportPath, dir.Name));
			}

			// copy AndroidManifest (additionally) to plugins/android directory:
			AssetDatabase.DeleteAsset(ANDROID_MANIFEST_PATH);
			AssetDatabase.MoveAsset(Files.CombinePath(BuildExportPath, ProductSpec.ANDROID_MANIFEST), ANDROID_MANIFEST_PATH);

			// copy StreamingAssets:
			Assets.ClearAssetFolder(STREAMING_ASSET_PATH, true);
			if ( Directory.Exists(newProduct.StreamingAssetPath) ) {
				Assets.CopyAssetsDir(
					newProduct.StreamingAssetPath, 
					STREAMING_ASSET_PATH);
			}

			PlayerSettings.productName = newProduct.Config.name;
			PlayerSettings.bundleIdentifier = ProductSpec.GQ_BUNDLE_ID_PREFIX + "." + newProduct.Config.id;

			AssetDatabase.Refresh();

			ProductEditor.BuildIsDirty = false;
			CurrentProduct = newProduct; // remember the new product for the editor time access point.
			ConfigurationManager.Reset(); // tell the runtime access point that the product has changed.
		}


		#endregion


		#region Helper Methods

		private void createConfigWithDefaults (string productID) {
			Config config = new Config();

			// set default values:
			config.id = productID;
			config.name = "QuestMill App " + productID;
			config.mapboxKey = "pk.eyJ1IjoiaG9sZ2VybXVlZ2dlIiwiYSI6Im1MLW9rN2MifQ.6KebeI6zZ3QNe18n2AQyaw";
			config.mapboxMapID = "mapbox.streets";

			// serialize into new product folder:
			serializeConfig(config, Files.CombinePath(ProductsDirPath, productID));
		}

		internal void serializeConfig (Config config, string productDirPath) {
			StringBuilder sb = new StringBuilder();
			JsonWriter jsonWriter = new JsonWriter(sb);
			jsonWriter.PrettyPrint = true;
			JsonMapper.ToJson(config, jsonWriter);

			string configFilePath = Files.CombinePath(productDirPath, ConfigurationManager.CONFIG_FILE);
			File.WriteAllText(configFilePath, sb.ToString());
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