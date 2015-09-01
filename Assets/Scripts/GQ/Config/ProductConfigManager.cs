using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEditor;
using System.Text;

namespace GQ.Conf
{
	public class ProductConfigManager
	{

		const string RUNTIME_PRODUCT_DIR = "Assets/ConfigAssets/Resources";
		const string RUNTIME_PRODUCT_FILE = RUNTIME_PRODUCT_DIR + "/product";
		public const string PRODUCTS_DIR = "Assets/Editor/products";
		public const string PRODUCT_FILE = "product.json";

		//////////////////////////////////
		// CHANGING THE CURRENT PRODUCT:
		
		/// <summary>
		/// Changes the currently used product to the given ID. 
		/// 
		/// The contract assumes that for the given ID a folder containing all necessary config data exists and is readable. 
		/// Any such checks need to be made in advance to calling this method.
		/// </summary>
		/// <param name="id">Product Identifier.</param>
		public static void load (string id)
		{
			if (id.Equals (current.id)) {
				return;
			}

			DirectoryInfo configPersistentDir = new DirectoryInfo (PRODUCTS_DIR + "/" + id);
			DirectoryInfo configRuntimeDir = new DirectoryInfo (RUNTIME_PRODUCT_DIR);
				
			if (!configRuntimeDir.Exists) {
				configRuntimeDir.Create ();
			}

			GQ.Util.Files.clearDirectory (configRuntimeDir.FullName);

			foreach (FileInfo file in configPersistentDir.GetFiles()) {
				if (!file.Extension.ToLower ().EndsWith ("meta") && !file.Extension.ToLower ().EndsWith ("ds_store")) {
					File.Copy (file.FullName, RUNTIME_PRODUCT_DIR + "/" + file.Name);
				}
			}
			AssetDatabase.ImportAsset (RUNTIME_PRODUCT_DIR, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);

			current = deserialize ();

//			TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath (RUNTIME_PRODUCT_DIR + "/appIcon");
//			importer.isReadable = true;
//			AssetDatabase.ImportAsset (RUNTIME_PRODUCT_DIR + "/appIcon");
//			// Load Textures:
//			appIconTexture = Resources.Load ("appIcon") as Texture2D;
//			AssetDatabase.Refresh ();
			// TODO fix this bullshit with: http://docs.unity3d.com/ScriptReference/EditorUtility.OpenFilePanel.html
		}

		//////////////////////////////////
		// RETRIEVING THE CURRENT PRODUCT:

		private static Config _current = null;

		public static Config current {
			get {
				if (_current == null) {
					_current = deserialize ();
				}
			
				return _current;
			}
			set {
				_current = value;
			}
		}

		private static Texture2D _appIconTexture;

		public static Texture2D appIconTexture {
			get {
				return _appIconTexture;
			}
			set {
				_appIconTexture = value;
			}
		}
		
		private static Texture2D _splashScreen;
		
		public static Texture2D splashScreen {
			get {
				return _splashScreen;
			}
			set {
				_splashScreen = value;
			}
		}

		private static Sprite _topLogo;

		public static Sprite topLogo {
			get {
				return _topLogo;
			}
			set {
				_topLogo = value;
			}
		}
		
		private static Sprite _defaultMarker;
		
		public static Sprite defaultMarker {
			get {
				return _defaultMarker;
			}
			set {
				_defaultMarker = value;
			}
		}

		public static string GetBundleIdentifier ()
		{
			return "com.questmill.geoquest." + current.id;
		}

		static Config deserialize ()
		{
			if (!File.Exists (RUNTIME_PRODUCT_FILE + ".json")) {
				throw new ArgumentException ("Config JSON File Missing! Please provide one at " + RUNTIME_PRODUCT_FILE + ".json");
			}

			TextAsset configAsset = Resources.Load ("product") as TextAsset;

			if (configAsset == null) {
				throw new ArgumentException ("Config JSON File does not represent a loadable asset. Cf. " + RUNTIME_PRODUCT_FILE + ".json");
			}
			return JsonMapper.ToObject<Config> (configAsset.text);
		}

		public static void save (string productID)
		{
			serialize ();

//			byte[] bytes = _appIconTexture.EncodeToJPG ();
//			File.WriteAllBytes (RUNTIME_PRODUCT_DIR + "/appIcon.png", bytes);

			string configPersistentDirPath = PRODUCTS_DIR + "/" + productID;
			DirectoryInfo configPersistentDir = new DirectoryInfo (configPersistentDirPath);
			DirectoryInfo configRuntimeDir = new DirectoryInfo (RUNTIME_PRODUCT_DIR);
			
			if (!configPersistentDir.Exists) {
				configPersistentDir.Create ();
			}
			
			GQ.Util.Files.clearDirectory (configPersistentDir.FullName);
			
			foreach (FileInfo file in configRuntimeDir.GetFiles()) {
				if (!file.Extension.EndsWith ("meta")) {
					File.Copy (file.FullName, configPersistentDirPath + "/" + file.Name);
				}
			}

			AssetDatabase.ImportAsset (PRODUCTS_DIR + "/" + current.id, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
		}

		static void serialize ()
		{
			StringBuilder sb = new StringBuilder ();
			JsonWriter jsonWriter = new JsonWriter (sb);
			jsonWriter.PrettyPrint = true;
			JsonMapper.ToJson (current, jsonWriter);
			File.WriteAllText (RUNTIME_PRODUCT_FILE + ".json", sb.ToString ());
			AssetDatabase.ImportAsset (RUNTIME_PRODUCT_DIR, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
		}


	}
	
	public enum QuestVisualizationMethod
	{
		list,
		map
	}

}


