using System.IO;
using System;
using System.Collections.Generic;
using GQ.Util;
using GQ.Client.Conf;
using UnityEngine;

namespace GQ.Build {

	/// <summary>
	/// The Product class represents a product specifiation of our app. Each Product instance refers to image files and resources directly 
	/// and to all textual parameters via a Config object. Product instances are used by the ProductManager and can be edited in the ProductEditor view.
	/// </summary>
	public class Product {

		#region Product Configuration Properties

		protected string _id;

		public string Id {
			get {
				return _id;
			}
			private set {
				_id = value;
			}
		}

		private string _dir;

		/// <summary>
		/// Directory path for this product.
		/// </summary>
		/// <value>The dir.</value>
		public string Dir {
			get {
				return (_dir);
			}
		}

		public string AppIconPath {
			get {
				return Files.CombinePath(Dir, "AppIcon.png");
			}
		}

		public string SplashScreenPath {
			get {
				return Files.CombinePath(Dir, "SplashScreen.jpg");
			}
		}

		public string TopLogoPath {
			get {
				return Files.CombinePath(Dir, "TopLogo.jpg");
			}	
		}

		public string ConfigPath {
			get {
				return Files.CombinePath(Dir, ProductConfigManager.PRODUCT_FILE);
			}	
		}

		#endregion


		#region Creating Product Intances

		/// <summary>
		/// Initializes a new Product instance by the given id and directory. 
		/// It expects that all product files are contained in the directory. 
		/// Among referring to image files etc. it also deserializes a Config object internally to read the Product.json specification of all textual parameters.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="dir">Dir.</param>
		internal Product (string dirPath) {
			if ( !Directory.Exists(dirPath) )
				throw new ArgumentException("Invalid path: Product directory not found: " + dirPath);

			if ( dirPath.EndsWith("/") )
				dirPath = dirPath.Substring(0, dirPath.Length);

			string name = dirPath.Substring(dirPath.LastIndexOf('/') + 1);
			if ( !IsValid(name) )
				throw new ArgumentException("Invalid product name: " + name);
			_id = name;

			string productDir = dirPath.Substring(0, dirPath.LastIndexOf('/'));

			_dir = Files.CombinePath(productDir, name);
		}

		static internal bool IsValid (string name) {
			// TODO do we need to restrict the product names somehow?
			return true;
		}

		#endregion

		public override string ToString () {
			return string.Format("product {0}", Id);
		}

	}
}