using UnityEditor;
using System.Collections;
using UnityEngine;
using System.IO;
using System.Linq;
using LitJson;
using System;
using System.Text;
using System.Reflection;
using GQ.Util;
using GQ.Client.Conf;

namespace GQ.Editor.Building {
	public class ProductEditorOld : EditorWindow {
		static private int selectedProductIndex;
		static private string[] productIDs;
		static private bool initialized = false;
		public const string PRODUCTS_DIR = "Assets/Editor/products/";
		const string RT_PROD_DIR = ConfigurationManager.RUNTIME_PRODUCT_DIR;
		const string RT_PROD_FILE = ConfigurationManager.RUNTIME_PRODUCT_FILE;
		const string APP_ICON_FILE_BASE = "appIcon";
		const string SPLASH_SCREEN_FILE_BASE = "splashScreen";
		const string TOP_LOGO_FILE_BASE = "topLogo";
		const string DEFAULT_MARKER_FILE_BASE = "defaultMarker";
		const string PLACEHOLDERS_SPLASHSCREEN_FILE = "Assets/Editor/productPlaceholders/splashScreen";
		private static Texture2D _appIconTexture;

		public static Texture2D appIcon {
			get {
				return _appIconTexture;
			}
			set {
				if ( !value.Equals(_appIconTexture) ) {
					_appIconTexture = value;
					PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[] {
						_appIconTexture
					});
				}
			}
		}

		private static Texture2D _splashScreen;

		public static Texture2D splashScreen {
			get {
				return _splashScreen;
			}
			set {
				if ( value != null ) {
					_splashScreen = value;
					PlayerSettings.resolutionDialogBanner = _splashScreen;
					try {
						#if !UNITY_WEBPLAYER
						Files.CopyImage(ConfigurationManager.RUNTIME_PRODUCT_DIR + SPLASH_SCREEN_FILE_BASE, 
							PLACEHOLDERS_SPLASHSCREEN_FILE);
						#endif
					} catch ( Exception exc ) {
						Debug.LogWarning("ProductEditorOld set splashScreen property threw exception:\n" + exc.Message);
					}
					AssetDatabase.Refresh();
				}
			}
		}

		[MenuItem("Window/GQ Product Editor (old)")]
		public static void  ShowWindow () {
			Assembly editorAsm = typeof(UnityEditor.Editor).Assembly;
			Type insWndType = editorAsm.GetType("UnityEditor.InspectorWindow");

			EditorWindow.GetWindow <ProductEditorOld>("GQ Product (old)", true, insWndType);
		}

		void initialize () {
			Debug.Log("initialize()");
			this.titleContent = new GUIContent("GQ Product (old)");
			productIDs = retrieveProductNames();
			if ( productIDs.Length < 1 ) {
				Debug.LogWarning("No product definitions found!");
				// TODO display error message in editor
			}
			if ( EditorPrefs.HasKey("ProductIndex") ) {
				selectedProductIndex = EditorPrefs.GetInt("ProductIndex");
				GUI.enabled = false;
				load(productIDs[selectedProductIndex]);
				GUI.enabled = true;
			}
			if ( !Directory.Exists(PRODUCTS_DIR) ) {    
				Directory.CreateDirectory(PRODUCTS_DIR);
				Debug.LogWarning("No product directory found. I created an empty one. No products defined!");
			}
			initialized = true;
		}

		#region GUI

		void OnGUI () {
//			Debug.Log("GQEditor.OnGui() " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

			if ( !initialized ) {
				initialize();
			}

			createGUI();
		}

		Vector2 scrollPos;

		void createGUI () {
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
			createGUIProductSelection();
			EditorGUILayout.Space();
			createGUIShowDetails();
			EditorGUILayout.Space();
			createGUIEditSpec();
			EditorGUILayout.EndScrollView();
		}

		void createGUIProductSelection () {
			GUILayout.Label("Select Product to Build", EditorStyles.boldLabel);
			int newSelProdIndex = EditorGUILayout.Popup("Product ID", selectedProductIndex, productIDs);
			if ( newSelProdIndex != selectedProductIndex ) {
				changeProduct(newSelProdIndex);
			}
		}

		bool allowChanges = false;

		void createGUIShowDetails () {
			GUILayout.Label("Details of Selected Product", EditorStyles.boldLabel);

			GUI.enabled = allowChanges;

			if ( !initialized ) {
				// TODO if not initialized show warnings
				return;
			} 
			GUI.enabled = allowChanges;

			ConfigurationManager.Current.name = 
				EditorGUILayout.TextField(
				"Name", 
				ConfigurationManager.Current.name, 
				GUILayout.Height(EditorGUIUtility.singleLineHeight));

			appIcon = 
				(Texture2D)EditorGUILayout.ObjectField(
				"App Icon", 
				appIcon,
				typeof(Texture),
				false);

			ConfigurationManager.Current.portal = 
				EditorGUILayout.IntField(
				"Portal", 
				ConfigurationManager.Current.portal, 
				GUILayout.Height(EditorGUIUtility.singleLineHeight));
			// TODO check and offer selection from server

			ConfigurationManager.Current.autoStartQuestID = 
				EditorGUILayout.IntField(
				"Autostart Quest ID", 
				ConfigurationManager.Current.autoStartQuestID, 
				GUILayout.Height(EditorGUIUtility.singleLineHeight));
			// TODO check at server and offer browser to select driectly from server

			if ( ConfigurationManager.Current.autoStartQuestID != 0 ) {
				ConfigurationManager.Current.autostartIsPredeployed =
				EditorGUILayout.Toggle("Autostart Predeployed?", ConfigurationManager.Current.autostartIsPredeployed);
			}
			else {
				ConfigurationManager.Current.autostartIsPredeployed = false;
			}
			
			ConfigurationManager.Current.downloadTimeOutSeconds = 
				EditorGUILayout.IntField(
				"Download Timeout (s)", 
				ConfigurationManager.Current.downloadTimeOutSeconds);
			// TODO limit to a value bigger than something (5s?)

			ConfigurationManager.Current.nameForQuest = 
				EditorGUILayout.TextField(
				"Name for 'Quest'", 
				ConfigurationManager.Current.nameForQuest, 
				GUILayout.Height(EditorGUIUtility.singleLineHeight));
			if ( ConfigurationManager.Current.nameForQuest == null || ConfigurationManager.Current.nameForQuest.Equals("") ) {
				ConfigurationManager.Current.nameForQuest = "Quest";
			}
			
			QuestVisualizationMethod mIn;
			if ( ConfigurationManager.Current.questVisualization == null ) {
				mIn = QuestVisualizationMethod.list;
			}
			else {
				mIn = (QuestVisualizationMethod)Enum.Parse(typeof(QuestVisualizationMethod), ConfigurationManager.Current.questVisualization.ToLower());
			}
			string questVisLabel = "Quest Visualization";
			if ( ConfigurationManager.Current.questVisualizationChangeable ) {
				questVisLabel = "Initial " + questVisLabel;
			}
			QuestVisualizationMethod m =
				(QuestVisualizationMethod)EditorGUILayout.EnumPopup(questVisLabel, mIn);
			ConfigurationManager.Current.questVisualization = m.ToString().ToLower();

			ConfigurationManager.Current.questVisualizationChangeable =
				EditorGUILayout.Toggle("Visualization Changeable?", ConfigurationManager.Current.questVisualizationChangeable);
			
			ConfigurationManager.Current.showCloudQuestsImmediately =
				EditorGUILayout.Toggle("Load cloud quests asap?", ConfigurationManager.Current.showCloudQuestsImmediately);

			EditorGUILayout.BeginHorizontal(GUILayout.Width(300));
			EditorGUILayout.PrefixLabel("Imprint");
			Vector2 scrollPos = new Vector2();
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
			ConfigurationManager.Current.imprint = 
				EditorGUILayout.TextArea(
				ConfigurationManager.Current.imprint);
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndHorizontal();

			// TODO splash screen
			Texture2D newSplashScreen = 
				(Texture2D)EditorGUILayout.ObjectField(
					"Splash Screen", 
					splashScreen,
					typeof(Texture),
					false);
			if ( newSplashScreen != null ) {
				splashScreen = newSplashScreen;
			}

			ConfigurationManager.Current.colorProfile = 
				EditorGUILayout.TextField(
				"Color Profile", 
				ConfigurationManager.Current.colorProfile, 
				GUILayout.Height(EditorGUIUtility.singleLineHeight));
			// TODO change to better representation of Color Profile
			
			ConfigurationManager.Current.showTextInLoadingLogo =
				EditorGUILayout.Toggle("Show Loading Text?", ConfigurationManager.Current.showTextInLoadingLogo);
			
			// TODO Animation Loading Logo
			
			ConfigurationManager.Current.showNetConnectionWarning =
				EditorGUILayout.Toggle("Show Connection Warning?", ConfigurationManager.Current.showNetConnectionWarning);

			ConfigurationManager.TopLogo = 
				(Sprite)EditorGUILayout.ObjectField(
				"Top Bar Logo", 
				ConfigurationManager.TopLogo,
				typeof(Sprite),
				false);
			// TODO resize visualization in editor to correct 

			ConfigurationManager.Current.mapboxMapID = 
				EditorGUILayout.TextField(
				"Mapbox Map ID", 
				ConfigurationManager.Current.mapboxMapID, 
				GUILayout.Height(EditorGUIUtility.singleLineHeight));
			
			ConfigurationManager.Current.mapboxKey = 
				EditorGUILayout.TextField(
				"Mapbox User Key", 
				ConfigurationManager.Current.mapboxKey, 
				GUILayout.Height(EditorGUIUtility.singleLineHeight));
			// TODO make generic representation for map types (google, OSM, Mapbox)
			
			// TODO default marker
			ConfigurationManager.DefaultMarker = 
				(Sprite)EditorGUILayout.ObjectField(
				"Default Marker", 
				ConfigurationManager.DefaultMarker,
				typeof(Sprite),
				false);
			// TODO resize visualization in editor to correct 

			// TODO marker categories ...

			GUI.enabled = true;
			return;
		}

		string editedProdID = "not set";

		enum Save {
			Overwrite,
			AsNew
		}

		void createGUIEditSpec () {
			// TODO rework the complete editing UI in the editor to buttons
			GUILayout.Label("Edit Product Configuration", EditorStyles.boldLabel);

			bool oldAllowChanges = allowChanges;
			allowChanges = EditorGUILayout.Toggle("Allow To Edit ...", allowChanges);
			if ( !oldAllowChanges && allowChanges ) {
				// siwtching allowChanges ON:
				editedProdID = productIDs[selectedProductIndex];
			}
			if ( oldAllowChanges && !allowChanges ) {
			}

			if ( allowChanges ) {
				editedProdID = EditorGUILayout.TextField("ID of Edited Product", editedProdID);

				Save saveType;
				string[] buttonText = {
					"Overwrite Existing Product",
					"Store New Product"
				};
				string[] dialogTitle = {
					"Overwrite Existing Product",
					"Save as New Product"
				};
				string[] dialogMessagePrefix = {
					"You current settings for product '",
					"This will create a new product specification named '"
				};
				string[] dialogMessagePostfix = {
					"' will be lost.",
					"'."
				};
				string[] okText = {
					"Overwrite",
					"Create"
				};

				if ( productIDs.Contains(editedProdID) ) {
					saveType = Save.Overwrite;
					// TODO add warning icon 
				}
				else {
					saveType = Save.AsNew;
				}

				if ( GUILayout.Button(buttonText[(int)saveType]) ) {
					bool okPressed = EditorUtility.DisplayDialog(
						                 dialogTitle[(int)saveType], 
						                 dialogMessagePrefix[(int)saveType] + editedProdID +
						                 dialogMessagePostfix[(int)saveType], 
						                 okText[(int)saveType], 
						                 "Cancel");
					if ( okPressed ) {
						performSaveConfig(editedProdID);
					}
				}
			}

		}

		#endregion

		public void OnProjectChange () {
			// TODO make some updates?
			Debug.Log("GQEditor.OnProjectChange() " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
		}

		public void OnDestroy () {
			// make some saves?
			Debug.Log("GQEditor.OnDestroy() " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
		}


		//////////////////////////////////
		// CHANGING THE CURRENT PRODUCT:
		
		/// <summary>
		/// Changes the currently used product to the given ID. 
		/// 
		/// The contract assumes that for the given ID a folder containing all necessary config data exists and is readable. 
		/// Any such checks need to be made in advance to calling this method.
		/// </summary>
		/// <param name="id">Product Identifier.</param>
		public static void load (string id) {
			DirectoryInfo configPersistentDir = new DirectoryInfo(PRODUCTS_DIR + id);
			DirectoryInfo configRuntimeDir = new DirectoryInfo(RT_PROD_DIR);

			if ( !configRuntimeDir.Exists ) {
				configRuntimeDir.Create();
			}
			
			#if !UNITY_WEBPLAYER
			Files.ClearDirectory(configRuntimeDir.FullName);

			foreach ( FileInfo file in configPersistentDir.GetFiles() ) {
				if ( !file.Extension.ToLower().EndsWith("meta") && !file.Extension.ToLower().EndsWith("ds_store") ) {
					File.Copy(file.FullName, RT_PROD_DIR + file.Name);
				}
			}
			#endif

			AssetDatabase.Refresh();

			ConfigurationManager.deserialize();
			
			// adjust Player Settings to newly loaded product:
			PlayerSettings.bundleIdentifier = "com.questmill.geoquest." + ConfigurationManager.Current.id;
			
			// load images:
			if ( File.Exists(RT_PROD_DIR + APP_ICON_FILE_BASE + ".png") ) {
				appIcon = 
					AssetDatabase.LoadMainAssetAtPath(RT_PROD_DIR + APP_ICON_FILE_BASE + ".png") as Texture2D;
			}
			else
			if ( File.Exists(RT_PROD_DIR + APP_ICON_FILE_BASE + ".jpg") ) {
				appIcon = 
					AssetDatabase.LoadMainAssetAtPath(RT_PROD_DIR + APP_ICON_FILE_BASE + ".jpg") as Texture2D;
			}
			else {
				appIcon = null;
			} // TODO replace null with default

			if ( File.Exists(RT_PROD_DIR + SPLASH_SCREEN_FILE_BASE + ".png") ) {
				splashScreen = 
					AssetDatabase.LoadMainAssetAtPath(RT_PROD_DIR + SPLASH_SCREEN_FILE_BASE + ".png") as Texture2D;
			}
			else
			if ( File.Exists(RT_PROD_DIR + SPLASH_SCREEN_FILE_BASE + ".jpg") ) {
				splashScreen = 
					AssetDatabase.LoadMainAssetAtPath(RT_PROD_DIR + SPLASH_SCREEN_FILE_BASE + ".jpg") as Texture2D;
			}
			else {
				splashScreen = null;
			} // TODO replace null with default
			
			if ( File.Exists(RT_PROD_DIR + TOP_LOGO_FILE_BASE + ".psd") ) {
				ConfigurationManager.TopLogo = 
					AssetDatabase.LoadAssetAtPath(RT_PROD_DIR + TOP_LOGO_FILE_BASE + ".psd", typeof(Sprite)) as Sprite;
			}
			else
			if ( File.Exists(RT_PROD_DIR + TOP_LOGO_FILE_BASE + ".png") ) {
				ConfigurationManager.TopLogo = 
					AssetDatabase.LoadAssetAtPath(RT_PROD_DIR + TOP_LOGO_FILE_BASE + ".png", typeof(Sprite)) as Sprite;
			}
			else
			if ( File.Exists(RT_PROD_DIR + TOP_LOGO_FILE_BASE + ".jpg") ) {
				ConfigurationManager.TopLogo = 
					AssetDatabase.LoadAssetAtPath(RT_PROD_DIR + TOP_LOGO_FILE_BASE + ".jpg", typeof(Sprite)) as Sprite;
			}
			else {
				ConfigurationManager.TopLogo = null;
			} // TODO replace null with default
			
			if ( File.Exists(RT_PROD_DIR + DEFAULT_MARKER_FILE_BASE + ".png") ) {
				ConfigurationManager.DefaultMarker = 
					AssetDatabase.LoadAssetAtPath(RT_PROD_DIR + DEFAULT_MARKER_FILE_BASE + ".png", typeof(Sprite)) as Sprite;
			}
			else {
				ConfigurationManager.DefaultMarker = null;
			} // TODO replace null with default
			
			//			TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath (RUNTIME_PRODUCT_DIR + "/appIcon");
			//			importer.isReadable = true;
			//			AssetDatabase.ImportAsset (RUNTIME_PRODUCT_DIR + "/appIcon");
			//			// Load Textures:
			//			appIconTexture = Resources.Load ("appIcon") as Texture2D;
			//			AssetDatabase.Refresh ();
			// TODO fix this bullshit with: http://docs.unity3d.com/ScriptReference/EditorUtility.OpenFilePanel.html
		}

		void performSaveConfig (string productID) {
			ConfigurationManager.Current.id = productID;

			serialize();
			
			string configPersistentDirPath = PRODUCTS_DIR + productID;
			DirectoryInfo configPersistentDir = new DirectoryInfo(configPersistentDirPath);
			DirectoryInfo configRuntimeDir = new DirectoryInfo(RT_PROD_DIR);
			
			if ( !configPersistentDir.Exists ) {
				configPersistentDir.Create();
			}
			
			#if !UNITY_WEBPLAYER
			Files.ClearDirectory(configPersistentDir.FullName);
			#endif			

			foreach ( FileInfo file in configRuntimeDir.GetFiles() ) {
				if ( !file.Extension.EndsWith("meta") ) {
					File.Copy(file.FullName, configPersistentDirPath + "/" + file.Name);
				}
			}

			// TODO should we store the images, too?
			Debug.Log("Import assets in save(" + productID + ")");
			AssetDatabase.ImportAsset(PRODUCTS_DIR + ConfigurationManager.Current.id, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
		}

		public static void serialize () {
			StringBuilder sb = new StringBuilder();
			JsonWriter jsonWriter = new JsonWriter(sb);
			jsonWriter.PrettyPrint = true;
			JsonMapper.ToJson(ConfigurationManager.Current, jsonWriter);
			File.WriteAllText(RT_PROD_FILE + ".json", sb.ToString());
			AssetDatabase.Refresh();
			Debug.Log("CHECKED");
//			AssetDatabase.ImportAsset(RT_PROD_DIR, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
		}

		void changeProduct (int index) {
			Debug.Log("changeProduct (" + selectedProductIndex + " --> " + index + ")");
			if ( index.Equals(selectedProductIndex) ) {
				return;
			}

			try {
				GUI.enabled = false;
				load(productIDs[index]);
				GUI.enabled = true;
				selectedProductIndex = index;
				EditorPrefs.SetInt("ProductIndex", index);
				allowChanges = false;
			} catch ( System.IndexOutOfRangeException e ) {
				Debug.LogWarning(e.Message);
				initialized = false;
			}
		}

		static string[] retrieveProductNames () {
			return Directory.GetDirectories(PRODUCTS_DIR).Select(d => new DirectoryInfo(d).Name).ToArray();
		}
	}
}

