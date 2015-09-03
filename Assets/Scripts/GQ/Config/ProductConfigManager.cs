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

		public const string RUNTIME_PRODUCT_DIR = "Assets/ConfigAssets/Resources";
		public const string RUNTIME_PRODUCT_FILE = RUNTIME_PRODUCT_DIR + "/product";
		public const string PRODUCT_FILE = "product.json";
		public const string TOP_LOGO_FILE_BASE = "topLogo";
		public const string DEFAULT_MARKER_FILE_BASE = "defaultMarker";


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

		public static Config deserialize ()
		{
			if (!File.Exists (RUNTIME_PRODUCT_FILE + ".json")) {
				throw new ArgumentException ("Config JSON File Missing! Please provide one at " + RUNTIME_PRODUCT_FILE + ".json");
			}

			TextAsset configAsset = Resources.Load ("product") as TextAsset;

			if (configAsset == null) {
				throw new ArgumentException ("Config JSON File does not represent a loadable asset. Cf. " + RUNTIME_PRODUCT_FILE + ".json");
			}
			current = JsonMapper.ToObject<Config> (configAsset.text);
			return current;
		}

		public static void serialize ()
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


