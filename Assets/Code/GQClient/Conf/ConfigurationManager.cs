using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace GQ.Client.Conf
{

	/// <summary>
	/// The Configuration manager is the runtime access point to information about the build configuration, 
	/// e.g. build time, version, contained markers and logos, links to server resources etc. 
	/// I.e. everything the app needs for the specific product it is branded to.
	/// 
	/// This class will completely replace the currently still used class Configuration 
	/// and additionally offer any information which is currently entered manually in the Unity Inspector View.
	/// </summary>
	public class ConfigurationManager : MonoBehaviour
	{

		#region Initialize

		void Awake ()
		{
			deserializeConfig ();
		}

		private static string _version = null;

		public static string Version {
			get {
				if (_version == null) {
					// get the string part before the first dot:
					_version = Buildtime.Substring (0, Buildtime.IndexOf ('.'));
				}
				return _version;
			}
		}

		#endregion

		#region Paths and general Resources

		public const string RUNTIME_PRODUCT_DIR = "Assets/ConfigAssets/Resources";
		public const string CONFIG_FILE = "Product.json";
		public const string BUILD_TIME_FILE_NAME = "buildtime";
		public const string BUILD_TIME_FILE_PATH = RUNTIME_PRODUCT_DIR + "/" + BUILD_TIME_FILE_NAME + ".txt";
		public const string TOPLOGO_FILE_NAME = "TopLogo";
		public const string TOPLOGO_FILE_PATH = RUNTIME_PRODUCT_DIR + "/" + TOPLOGO_FILE_NAME;

		public const string GQ_SERVER_BASE_URL = "http://qeevee.org:9091";

		public static string UrlPublicQuestsJSON 
		{
			get {
				return 
					String.Format (
					"{0}/json/{1}/publicgamesinfo", 
					GQ_SERVER_BASE_URL,
					Current.portal
				);
			}
		}

		#endregion

		#region RETRIEVING THE CURRENT PRODUCT

		private static Config _current = null;

		public static Config Current {
			get {
				if (_current == null) {
					deserializeConfig ();
				}
			
				return _current;
			}
			set {
				_current = value;
			}
		}

		public static void Reset ()
		{
			_current = null;
		}

		private static Sprite _topLogo;

		public static Sprite TopLogo {
			get {
				_topLogo = Resources.Load <Sprite> (TOPLOGO_FILE_NAME);
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

		private static string _buildtime = null;

		public static string Buildtime {
			get {
				if (_buildtime == null) {
					try {
						TextAsset buildTimeAsset = Resources.Load (BUILD_TIME_FILE_NAME) as TextAsset;

						if (buildTimeAsset == null) {
							throw new ArgumentException ("Buildtime File does not represent a loadable asset. Cf. " + BUILD_TIME_FILE_NAME);
						}
						_buildtime = buildTimeAsset.text;
					} catch (Exception exc) {
						Debug.LogWarning ("Could not read build time file at " + BUILD_TIME_FILE_PATH + " " + exc.Message);
						_buildtime = "unknown";
					}
				}
				return _buildtime;
			}
			set {
				_buildtime = value;
			}
		}

		private static string _imprint = null;

		public static string Imprint {
			get {
				if (_imprint == null) {
					TextAsset ta = Resources.Load ("imprint") as TextAsset;
					if (ta == null)
						_imprint = "";
					else
						_imprint = ta.text;
				}
				return _imprint;
				
			}
			set {
				_imprint = value;
			}
		}


		private static string _terms = null;

		public static string Terms {
			get {
				if (_terms == null) {
					TextAsset ta = Resources.Load ("terms") as TextAsset;
					if (ta == null)
						_terms = "";
					else
						_terms = ta.text;
				}
				return _terms;

			}
			set {
				_terms = value;
			}
		}


		private static string _privacyStatement = null;

		public static string PrivacyStatement {
			get {
				if (_privacyStatement == null) {
					TextAsset ta = Resources.Load ("privacy") as TextAsset;
					if (ta == null)
						_privacyStatement = "";
					else
						_privacyStatement = ta.text;
				}
				return _privacyStatement;

			}
			set {
				_privacyStatement = value;
			}
		}


		/// <summary>
		/// Deserialize the Product.json file to the current config object that is used throughout the client.
		/// </summary>
		public static void deserializeConfig ()
		{

			TextAsset configAsset = Resources.Load ("Product") as TextAsset;

			if (configAsset == null) {
				throw new ArgumentException ("Something went wrong with the Config JSON File. Check it. It should be at " + RUNTIME_PRODUCT_DIR);
			}

			try {
				_current = JsonConvert.DeserializeObject<Config> (configAsset.text);
			} catch (Exception e) {
				Debug.LogWarning ("Product Configuration: Exception thrown when parsing Product.json: " + e.Message);
			}

		}

		#endregion

	}


	public enum QuestVisualizationMethod
	{
		list,
		map
	}

}


