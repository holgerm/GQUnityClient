using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using System.Text;

namespace GQ.Client.Conf {

	/// <summary>
	/// The Configuration manager is the runtime access point to information about the build configuration, 
	/// e.g. build time, version, contained markers and logos, links to server resources etc. 
	/// I.e. everything the app needs for the specific product it is branded to.
	/// 
	/// This class will completely replace the currently still used class Configuration 
	/// and additionally offer any information which is currently entered manuallly in the Unity Inspector View.
	/// </summary>
	public class ConfigurationManager : MonoBehaviour {

		public const string RUNTIME_PRODUCT_DIR = "Assets/ConfigAssets/Resources";
		public const string RUNTIME_PRODUCT_FILE = RUNTIME_PRODUCT_DIR + "product";
		public const string CONFIG_FILE = "Product.json";
		public const string BUILD_TIME_FILE_NAME = "buildtime";
		public const string BUILD_TIME_FILE_PATH = RUNTIME_PRODUCT_DIR + BUILD_TIME_FILE_NAME + ".txt";

		#region Initialize

		void Awake () {
			deserialize();
		}

		#endregion

		#region RETRIEVING THE CURRENT PRODUCT

		private static Config _current = null;

		public static Config Current {
			get {
				if ( _current == null ) {
					deserialize();
				}
			
				return _current;
			}
			set {
				_current = value;
			}
		}

		private static Sprite _topLogo;

		public static Sprite TopLogo {
			get {
				return _topLogo;
			}
			set {
				_topLogo = value;
			}
		}

		private static Sprite _defaultMarker;

		public static Sprite DefaultMarker {
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

		public static void deserialize () {

			TextAsset configAsset = Resources.Load("Product") as TextAsset;

			if ( configAsset == null ) {
				throw new ArgumentException("Something went wrong with the Config JSON File. Check it. It should be at " + RUNTIME_PRODUCT_DIR);
			}

			try {
				_current = JsonMapper.ToObject<Config>(configAsset.text);
			} catch ( Exception e ) {
				Debug.LogWarning("Product Configuration: Exception thrown when parsing Product.json: " + e.Message);
			}

		}

		#endregion

	}


	public enum QuestVisualizationMethod {
		list,
		map
	}

}


