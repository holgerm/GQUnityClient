using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using System.Text;

namespace GQ.Conf {
	public class ProductConfigManager {

		public const string RUNTIME_PRODUCT_DIR = "Assets/ConfigAssets/Resources";
		public const string RUNTIME_PRODUCT_FILE = RUNTIME_PRODUCT_DIR + "/product";
		public const string PRODUCT_FILE = "product.json";
		public const string BUILD_TIME_FILE_NAME = "buildtime";
		public const string BUILD_TIME_FILE_PATH = RUNTIME_PRODUCT_DIR + "/" + BUILD_TIME_FILE_NAME + ".txt";

		#region RETRIEVING THE CURRENT PRODUCT
		private static Config _current = null;

		public static Config current {
			get {
				if ( _current == null ) {
					_current = deserialize();
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

		private static string _buildtime;

		public static string Buildtime {
			get {
				if ( _buildtime == null ) {
					try {
						TextAsset buildTimeAsset = Resources.Load(BUILD_TIME_FILE_NAME) as TextAsset;

						if ( buildTimeAsset == null ) {
							throw new ArgumentException("Buildtime File does not represent a loadable asset. Cf. " + BUILD_TIME_FILE_NAME);
						}
						_buildtime = buildTimeAsset.text;
						Debug.Log("Build time read from file: " + _buildtime);
					} catch ( Exception exc ) {
						Debug.LogWarning("Could not read build time file at " + BUILD_TIME_FILE_PATH + " " + exc.Message);
						_buildtime = "unknown";
					}
				}
				return _buildtime;
			}
			set {
				_buildtime = value;
			}
		}

		public static Config deserialize () {
			if ( !File.Exists(RUNTIME_PRODUCT_FILE + ".json") ) {
				throw new ArgumentException("Config JSON File Missing! Please provide one at " + RUNTIME_PRODUCT_FILE + ".json");
			}

			TextAsset configAsset = Resources.Load("product") as TextAsset;

			if ( configAsset == null ) {
				throw new ArgumentException("Config JSON File does not represent a loadable asset. Cf. " + RUNTIME_PRODUCT_FILE + ".json");
			}
			current = JsonMapper.ToObject<Config>(configAsset.text);
			return current;
		}

		#endregion

	}
	
	public enum QuestVisualizationMethod {
		list,
		map
	}

}


