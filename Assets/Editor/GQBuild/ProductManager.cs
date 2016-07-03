using System.IO;
using System;
using System.Collections.Generic;
using GQ.Util;

namespace GQ.Build {
	public class ProductManager {

		#region Path and Storage

		/// <summary>
		/// In this directory all defined producst are stored. This data is NOT included in the app build.
		/// </summary>
		public const string PRODUCTS_DIR_PATH_DEFAULT = "Assets/Editor/products/";

		/// <summary>
		/// This is the template for new products which is copied when we create a new product. It should contain a complete product definition.
		/// </summary>
		public const string TEMPLATE_PRODUCT_PATH = "Assets/Editor/productsDefault/";

		static private string _productsDirPath = PRODUCTS_DIR_PATH_DEFAULT;

		/// <summary>
		/// Gets or sets the products dir path. You can set the path only before you instantiate the singleton ProductManager.
		/// </summary>
		/// <value>The products dir path.</value>
		public static string ProductsDirPath {
			get {
				return _productsDirPath;
			}
			set {
				if ( _instance == null ) {
					_productsDirPath = value;
				}
			}
		}

		#endregion


		#region Access to Products

		private Dictionary<string, Product> _productDict;

		public List<Product> AllProducts {
			get {
				return new List<Product>(_productDict.Values);
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
			_productDict = new Dictionary<string, Product>();
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

			Product newProduct = Product.createFromDirectory(newProductDirPath);
			_productDict.Add(newProductID, newProduct);
			return newProduct;
		}
	}


}