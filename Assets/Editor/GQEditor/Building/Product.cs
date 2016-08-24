using System.IO;
using System;
using System.Collections.Generic;
using GQ.Util;
using GQ.Client.Conf;
using UnityEngine;
using LitJson;

namespace GQ.Editor.Building {

	/// <summary>
	/// The Product class represents a product specifiation of our app. Each Product instance refers to image files and resources directly 
	/// and to all textual parameters via a Config object. Product instances are used by the ProductManager and can be edited in the ProductEditor view.
	/// 
	/// A product is backed on file by diverse graphic files and a configuration file (Product.json). 
	/// These files reside in one folder (the product folder) which can have an arbitrary name. 
	/// You create a Product instance by calling the Constructor with the product folder path as argument.
	/// </summary>
	public class Product {

		#region Product Configuration Properties

		public string Id {
			get {
				return Config.id;
			}
			private set {
				Config.id = value; // TODO
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

		internal const string APP_ICON_PATH = "AppIcon.png";

		public string AppIconPath {
			get {
				return Files.CombinePath(Dir, APP_ICON_PATH);
			}
		}

		internal const string SPLASH_SCREEN_PATH = "SplashScreen.jpg";

		public string SplashScreenPath {
			get {
				return Files.CombinePath(Dir, SPLASH_SCREEN_PATH);
			}
		}

		internal const string TOP_LOGO_PATH = "TopLogo.jpg";

		public string TopLogoPath {
			get {
				return Files.CombinePath(Dir, TOP_LOGO_PATH);
			}	
		}

		public string ConfigPath {
			get {
				return Files.CombinePath(Dir, ConfigurationManager.CONFIG_FILE);
			}	
		}

		private Config _config;

		public Config Config {
			get {
				return _config;
			}
			set {
				_config = value;
			}
		}


		#endregion


		#region Creating Product Intances

		/// <summary>
		/// Initializes a new Product instance by the given id and directory. 
		/// It expects that all product files are contained in the directory. 
		/// Among referring to image files etc. it also deserializes a Config object internally to read the Product.json specification of all textual parameters.
		/// 
		/// Throws ArgumentException when the folder does not contain all necessary stuff correctly, i.e. branding files and a matching config file.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="dir">Dir.</param>
		internal Product (string dirPath) {
			// Check path:
			if ( !Directory.Exists(dirPath) )
				throw new ArgumentException("Invalid path: Product directory not found: " + dirPath);

			// Init Dir:
			if ( dirPath.EndsWith("/") )
				dirPath = dirPath.Substring(0, dirPath.Length);
			_dir = dirPath;

			// init and check Config:
			try {
				string configJSON = File.ReadAllText(ConfigPath);
				Config = JsonMapper.ToObject<Config>(configJSON);
			} catch ( Exception exc ) {
				throw new ArgumentException("Invalid product definition. Config file could not be read.", exc);
			}
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