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

namespace GQ.Editor.Building {
	public class ProductManager {

		#region Path and Storage

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

		#endregion


		#region Access to Products

		private Dictionary<string, Product> _productDict;

		public ICollection<Product> AllProducts {
			get {
				return _productDict.Values;
			}
		}

		public ICollection<string> AllProductIds {
			get {
				return _productDict.Keys;
			}
		}

		public Product getProduct (string productID) {
			Product found = null;

			if ( _productDict.TryGetValue(productID, out found) )
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

		private ProductManager () {
			_errors = new List<string>();
			initProductDictionary();
		}

		private void initProductDictionary () {
			_productDict = new Dictionary<string, Product>();

			IEnumerable<string> productDirCandidates = Directory.GetDirectories(ProductsDirPath).Select(d => new DirectoryInfo(d).FullName);

			foreach ( var productCandidatePath in productDirCandidates ) {
				Product product;
				try {
					product = new Product(productCandidatePath);
					_productDict.Add(product.Id, product);
				} catch ( ArgumentException exc ) {
					Errors.Add("Product Manager found invalid product directory: " + productCandidatePath + "\n" + exc.Message + "\n\n");
				}
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

		public Product createNewProduct (string newProductID) {
			if ( !Product.IsValid(newProductID) ) {
				throw new ArgumentException("Invalid product id: " + newProductID);
			}

			string newProductDirPath = Files.CombinePath(ProductsDirPath, newProductID);

			if ( Directory.Exists(newProductDirPath) ) {
				throw new ArgumentException("Product name already used: " + newProductID + " in: " + newProductDirPath);
			}

			// copy default template files to a new product folder:
			Files.CopyDirectory(TEMPLATE_PRODUCT_PATH, newProductDirPath);

			// create Config, populate it with defaults and serialize it into the new product folder:
			createConfigWithDefaults(newProductID);

			Product newProduct = new Product(newProductDirPath);
			_productDict.Add(newProduct.Id, newProduct);
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

		/// <summary>
		/// Sets the product for build, i.e. files are copied from the product dir to the client configuration dir. 
		/// E.g. for 'wcc' the product dir is in 'Assets/Editor/products/wcc'. 
		/// The client configuration dir is always at 'Assets/ConfigAssets/Ressources'.
		/// 
		/// The following file are copied:
		/// 
		/// 1. All files directly stored in the product dir into the config dir.
		/// 2. AndroidManifest (in 'productDir/Android') to 'Assets/Plugins/Android/'
		/// 3. TODO: Player Preferences?
		/// 
		/// </summary>
		/// <param name="productID">Product I.</param>
		public void SetProductForBuild (string productID) {
			if ( !Product.IsValid(productID) ) {
				throw new ArgumentException("Invalid product id: " + productID);
			}

			string productDirPath = Files.CombinePath(ProductsDirPath, productID);

			if ( !Directory.Exists(productDirPath) ) {
				throw new ArgumentException("Product can not be build , since it does not exist: " + productID);
			}

			// clear build folder:
			if ( !Directory.Exists(BuildExportPath) ) {
				Directory.CreateDirectory(BuildExportPath);
			}

			Assets.ClearAssetFolder(BuildExportPath); 

			Assets.CopyAssetsDir(productDirPath, BuildExportPath);

			DirectoryInfo productDirInfo = new DirectoryInfo(productDirPath);

			foreach ( DirectoryInfo dir in productDirInfo.GetDirectories() ) {
				if ( dir.Name.StartsWith("_") )
					continue;
				
				Assets.CreateSubfolder(BuildExportPath, dir.Name);

				string originPath = Files.CombinePath(productDirPath, dir.Name);
				string targetPath = Files.CombinePath(BuildExportPath, dir.Name);
				Assets.CopyAssetsDir(originPath, targetPath);
			}

			AssetDatabase.Refresh();
		}


		#endregion


		#region Helper Methods

		private void createConfigWithDefaults (string productID) {
			Config config = new Config();

			// set default values:
			config.id = productID;
			config.name = "QuestLab App " + productID;

			// serialize into new product folder:
			serializeConfig(config, Files.CombinePath(ProductsDirPath, productID));
		}

		private void serializeConfig (Config config, string productDirPath) {
			StringBuilder sb = new StringBuilder();
			JsonWriter jsonWriter = new JsonWriter(sb);
			jsonWriter.PrettyPrint = true;
			JsonMapper.ToJson(config, jsonWriter);

			string configFilePath = Files.CombinePath(productDirPath, ConfigurationManager.CONFIG_FILE);
			File.WriteAllText(configFilePath, sb.ToString());
		}

		#endregion
	}



}