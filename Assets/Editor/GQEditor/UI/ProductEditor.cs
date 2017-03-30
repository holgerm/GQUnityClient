using UnityEditor;
using System.Collections;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Text;
using System.Reflection;
using GQ.Util;
using GQ.Client.Conf;
using GQ.Editor.Building;
using GQ.Client.Util;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using GQ.Editor.Util;
using GQTests;

namespace GQ.Editor.UI
{
	public class ProductEditor : EditorWindow
	{

		private GUIStyle textareaGUIStyle;
		private Texture warnIcon;
		Vector2 scrollPos;

		private static string _currentBuildName = null;

		public static string CurrentBuildName {
			get {
				if (_currentBuildName == null) {
					_currentBuildName = currentBuild ();
				}
				return _currentBuildName;
			}
			set {
				_currentBuildName = value;
			}
		}

		internal const string WARN_ICON_PATH = "Assets/Editor/GQEditor/images/warn.png";

		private static bool _buildIsDirty = false;

		public static bool BuildIsDirty {
			get {
				return _buildIsDirty;
			}
			set {
				_buildIsDirty = value;
			}
		}

		private static ProductEditor _instance = null;

		public static ProductEditor Instance {
			get {
				return _instance;
			}
			private set {
				_instance = value;
			}
		}

		private static bool _isCurrentlyPreparingProduct = false;

		public static bool IsCurrentlyPreparingProduct {
			get {
				return _isCurrentlyPreparingProduct;
			}
			set {
				_isCurrentlyPreparingProduct = value;
			}
		}


		[MenuItem ("Window/QuestMill Product Editor")]
		public static void  Init ()
		{
			Assembly editorAsm = typeof(UnityEditor.Editor).Assembly;
			Type inspectorWindowType = editorAsm.GetType ("UnityEditor.InspectorWindow");
			ProductEditor editor;
			if (inspectorWindowType != null)
				editor = EditorWindow.GetWindow<ProductEditor> ("GQ Product", true, inspectorWindowType);
			else
				editor = EditorWindow.GetWindow<ProductEditor> (typeof(ProductEditor));
			editor.Show ();
			Instance = editor;
		}

		int selectedProductIndex;

		ProductManager _pm;

		public ProductManager Pm {
			get {
				if (_pm == null)
					_pm = ProductManager.Instance;
				return _pm;
			}
			set {
				_pm = value;
			}
		}

		public void OnEnable ()
		{
			Instance = this;

			readStateFromEditorPrefs ();
			warnIcon = (Texture)AssetDatabase.LoadAssetAtPath (WARN_ICON_PATH, typeof(Texture));
		}

		void readStateFromEditorPrefs ()
		{
			selectedProductIndex = EditorPrefs.HasKey ("selectedProductIndex") ? EditorPrefs.GetInt ("selectedProductIndex") : 0;
			Pm.ConfigFilesHaveChanges = EditorPrefs.HasKey ("configDirty") ? EditorPrefs.GetBool ("configDirty") : false;
		}

		#region GUI

		/// <summary>
		/// Creates the Editor GUI consisting of these parts:
		/// 
		/// 1. Product Manager Part: 
		/// 	- Select Current Project
		/// 	- Prepare Build
		/// 2. Error Part (TODO)
		/// 3. Product Details Part:
		/// 	- Show all key avlue pairs of the product spec
		/// 	- Editing option (TODO)
		/// </summary>
		void OnGUI ()
		{
			// adjust textarea style:
			textareaGUIStyle = GUI.skin.textField;
			textareaGUIStyle.wordWrap = true;

			gui4ProductManager ();

			EditorGUILayout.Space ();

			// TODO showErrors

			gui4ProductDetails ();

			gui4ProductEditPart ();

			EditorGUILayout.Space ();

			GUILayout.FlexibleSpace ();

		}

		private string newProductID = "";


		void gui4ProductManager ()
		{
			// Heading:
			GUILayout.Label ("Product Manager", EditorStyles.boldLabel);

			// Current Build:
			CurrentBuildName = currentBuild ();

			string shownBuildName; 
			EditorGUILayout.BeginHorizontal ();
			{
				if (CurrentBuildName == null) {
					shownBuildName = "missing";
					EditorGUILayout.PrefixLabel (new GUIContent ("Current Build:", warnIcon));
				} else {
					shownBuildName = CurrentBuildName;
					if (BuildIsDirty) {
						EditorGUILayout.PrefixLabel (new GUIContent ("Current Build:", warnIcon));
					} else {
						EditorGUILayout.PrefixLabel (new GUIContent ("Current Build:"));
					}
				}
				GUILayout.Label (shownBuildName);

				if (Pm.ConfigFilesHaveChanges) {
					if (GUILayout.Button ("Persist Changes")) {
						Files.CopyDirContents (
							ConfigurationManager.RUNTIME_PRODUCT_DIR, 
							Files.CombinePath (ProductManager.ProductsDirPath, CurrentBuildName),
							copyContentsOnly: true
						);
						Files.CopyDir (
							Pm.STREAMING_ASSET_PATH,
							Files.CombinePath (ProductManager.ProductsDirPath, CurrentBuildName),
							replace: false
						);
						Pm.ConfigFilesHaveChanges = false;
					}
				}
			}
			EditorGUILayout.EndHorizontal ();

			if (selectedProductIndex < 0 || selectedProductIndex >= Pm.AllProductIds.Count)
				selectedProductIndex = 0;
			string selectedProductName = Pm.AllProductIds.ElementAt (selectedProductIndex);

			GUIContent prepareBuildButtonGUIContent, availableProductsPopupGUIContent, newProductLabelGUIContent, createProductButtonGUIContent;

			if (configIsDirty) {
				// adding tooltip to explain why these elements are disabled:
				string explanation = "You must Save or Revert your changes first.";
				prepareBuildButtonGUIContent = new GUIContent ("Prepare Build", explanation);
				availableProductsPopupGUIContent = new GUIContent ("Available Products:", explanation);
				newProductLabelGUIContent = new GUIContent ("New product (id):", explanation);
				createProductButtonGUIContent = new GUIContent ("Create", explanation);
			} else {
				prepareBuildButtonGUIContent = new GUIContent ("Prepare Build");
				availableProductsPopupGUIContent = new GUIContent ("Available Products:");
				newProductLabelGUIContent = new GUIContent ("New product (id):");
				createProductButtonGUIContent = new GUIContent ("Create");
			}

			using (new EditorGUI.DisabledGroupScope ((configIsDirty))) {
				// Prepare Build Button:
				EditorGUILayout.BeginHorizontal ();
				{
					if (GUILayout.Button (prepareBuildButtonGUIContent)) {
						Pm.PrepareProductForBuild (selectedProductName);
					}
				}
				EditorGUILayout.EndHorizontal ();

				// Product Selection Popup:
				string[] productIds = Pm.AllProductIds.ToArray<string> ();
				// SORRY: This is to fulfill the not-so-flexible overloading scheme of Popup() here:
				List<GUIContent> guiContentListOfProducts = new List<GUIContent> ();
				for (int i = 0; i < productIds.Length; i++) {
					guiContentListOfProducts.Add (new GUIContent (productIds [i]));
				}

				int newIndex = EditorGUILayout.Popup (availableProductsPopupGUIContent, selectedProductIndex, guiContentListOfProducts.ToArray ());
				selectProduct (newIndex);

				// Create New Product row:
				EditorGUILayout.BeginHorizontal ();
				{
					GUILayout.Label (newProductLabelGUIContent);
					newProductID = EditorGUILayout.TextField (
						newProductID, 
						GUILayout.Height (EditorGUIUtility.singleLineHeight));
					bool createButtonshouldBeDisabled = newProductID.Equals ("") || Pm.AllProductIds.Contains (newProductID);
					using (new EditorGUI.DisabledGroupScope ((createButtonshouldBeDisabled))) {
						if (GUILayout.Button (createProductButtonGUIContent)) {
							Pm.createNewProduct (newProductID);
						}
					}
				}
				EditorGUILayout.EndHorizontal ();

			} // Disabled Scope for dirty Config ends, i.e. you must first save or revert the current product's details.
		}

		internal static string currentBuild ()
		{
			string build = null;

			try {
				string configFile = Files.CombinePath (ProductManager.Instance.BuildExportPath, ConfigurationManager.CONFIG_FILE);
				if (!File.Exists (configFile))
					return build;
				string configText = File.ReadAllText (configFile);
				Config buildConfig = JsonConvert.DeserializeObject<Config> (configText);
				return buildConfig.id;
			} catch (Exception exc) {
				Debug.LogWarning ("ProductEditor.currentBuild() threw exception:\n" + exc.Message);
				return build;
			}
		}

		bool configIsDirty = false;

		void gui4ProductDetails ()
		{
			GUILayout.Label ("Product Details", EditorStyles.boldLabel);
			ProductSpec p = Pm.AllProducts.ElementAt (selectedProductIndex);

			// Begin ScrollView:
			using (var scrollView = new EditorGUILayout.ScrollViewScope (scrollPos /* , GUILayout.Width(100), GUILayout.Height(100) */)) {
				scrollPos = scrollView.scrollPosition;

				using (new EditorGUI.DisabledGroupScope ((!allowChanges))) {
					// ScrollView begins (optionally disabled):
				
					PropertyInfo[] propertyInfos = typeof(Config).GetProperties ();

					// get max widths for names and values:
					float allNamesMax = 0f, allValuesMax = 0f;

					foreach (PropertyInfo curPropInfo in propertyInfos) {
						if (!curPropInfo.CanRead)
							continue;

						string name = curPropInfo.Name + ":";
						string value = Objects.ToString (curPropInfo.GetValue (p.Config, null));

						float nameMin, nameMax;
						float valueMin, valueMax;
						GUIStyle guiStyle = new GUIStyle ();

						guiStyle.CalcMinMaxWidth (new GUIContent (name + ":"), out nameMin, out nameMax);
						allNamesMax = Math.Max (allNamesMax, nameMax);
						guiStyle.CalcMinMaxWidth (new GUIContent (value), out valueMin, out valueMax);
						allValuesMax = Math.Max (allValuesMax, valueMax);
					}

					// calculate widths for names and values finally: we allow no more than 40% of the editor width for names.
					// add left, middle and right borders as given:
					float borders = new GUIStyle (GUI.skin.textField).margin.left + new GUIStyle (GUI.skin.textField).margin.horizontal + new GUIStyle (GUI.skin.textField).margin.right; 
					// calculate widths for names and vlaues finally: we allow no more than 40% of the editor width for names, but do not take more than we need.
					float widthForNames = Math.Min ((position.width - borders) * 0.4f, allNamesMax);
					float widthForValues = position.width - (borders + widthForNames);

					EditorGUIUtility.labelWidth = widthForNames;

					// show all properties as textfields or textareas in fitting width:
					foreach (PropertyInfo curPropInfo in propertyInfos) {
						if (!curPropInfo.CanRead)
							continue;

						GUIStyle guiStyle = new GUIStyle ();

						string name = curPropInfo.Name;
						float nameMin, nameWidthNeed;
						guiStyle.CalcMinMaxWidth (new GUIContent (name + ":"), out nameMin, out nameWidthNeed);
						GUIContent namePrefixGUIContent;
						if (widthForNames < nameWidthNeed)
							// Show hover over name because it is too long to be shown:
							namePrefixGUIContent = new GUIContent (name + ":", name);
						else
							// Show only name without hover:
							namePrefixGUIContent = new GUIContent (name + ":");

						float valueMin, valueNeededWidth;

						switch (curPropInfo.PropertyType.Name) {
						case "Boolean":
								// show checkbox:
							EditorGUILayout.BeginHorizontal ();
							{
								EditorGUILayout.PrefixLabel (namePrefixGUIContent);
								bool oldBoolVal = (bool)curPropInfo.GetValue (p.Config, null);
								bool newBoolVal = EditorGUILayout.Toggle (oldBoolVal);
								if (newBoolVal != oldBoolVal) {
									configIsDirty = true;
								}
								curPropInfo.SetValue (p.Config, newBoolVal, null);
							}
							EditorGUILayout.EndHorizontal ();
							break;
						case "String":
							using (new EditorGUI.DisabledGroupScope (curPropInfo.Name.Equals ("id"))) {
								// id of products may not be altered.
								if (curPropInfo.Name.Equals ("id")) {
									namePrefixGUIContent = new GUIContent (curPropInfo.Name, "You may not alter the id of a product.");
								}
								// show textfield or if value too long show textarea:
								string oldStringVal = (string)curPropInfo.GetValue (p.Config, null);
								oldStringVal = Objects.ToString (oldStringVal);
								string newStringVal;
								guiStyle.CalcMinMaxWidth (new GUIContent (oldStringVal), out valueMin, out valueNeededWidth);

								if (widthForValues < valueNeededWidth) {
									// show textarea if value does not fit within one line:
									EditorGUILayout.BeginHorizontal ();
									EditorGUILayout.PrefixLabel (namePrefixGUIContent);
									newStringVal = EditorGUILayout.TextArea (oldStringVal, textareaGUIStyle);
									newStringVal = Objects.ToString (newStringVal);
									EditorGUILayout.EndHorizontal ();
								} else {
									// show text field if value fits in one line:
									newStringVal = EditorGUILayout.TextField (namePrefixGUIContent, oldStringVal);
									newStringVal = Objects.ToString (newStringVal);
								}
								if (!newStringVal.Equals (oldStringVal)) {
									configIsDirty = true;
								}
								curPropInfo.SetValue (p.Config, newStringVal, null);
							}
							break;
						case "Int32":
							using (new EditorGUI.DisabledGroupScope (curPropInfo.Name.Equals ("id"))) {
								// id of products may not be altered.
								if (curPropInfo.Name.Equals ("id")) {
									namePrefixGUIContent = new GUIContent (curPropInfo.Name, "You may not alter the id of a product.");
								}
								int oldIntVal = (int)curPropInfo.GetValue (p.Config, null);
								int newIntVal = oldIntVal;

								// show text field if value fits in one line:
								newIntVal = EditorGUILayout.IntField (namePrefixGUIContent, oldIntVal);
								if (newIntVal != oldIntVal) {
									configIsDirty = true;
								}
								curPropInfo.SetValue (p.Config, newIntVal, null);
							}
							break;
						case "List`1":
							{
								// We currently do not offer edit option for Lists! Sorry.
								// show textfield or if value too long show textarea:
								string oldStringVal = curPropInfo.GetValue (p.Config, null).ToString ();
								guiStyle.CalcMinMaxWidth (new GUIContent (oldStringVal), out valueMin, out valueNeededWidth);

								if (widthForValues < valueNeededWidth) {
									// show textarea if value does not fit within one line:
									EditorGUILayout.BeginHorizontal ();
									EditorGUILayout.PrefixLabel (namePrefixGUIContent);
									EditorGUILayout.TextArea (oldStringVal, textareaGUIStyle);
									EditorGUILayout.EndHorizontal ();
								} else {
									// show text field if value fits in one line:
									EditorGUILayout.TextField (namePrefixGUIContent, oldStringVal);
								}
							}
							break;
						default:
							Debug.Log ("Unhandled property Type: " + curPropInfo.PropertyType.Name);
							break;
						}

					}
				} // End Scope Disabled Group 
			} // End Scope ScrollView 
		}

		private bool allowChanges = false;

		void gui4ProductEditPart ()
		{
			GUILayout.Label ("Editing Options", EditorStyles.boldLabel);

			// Create New Product row:
			EditorGUILayout.BeginHorizontal ();
			{

				bool oldAllowChanges = allowChanges;
				allowChanges = EditorGUILayout.Toggle ("Allow Editing ...", allowChanges);
				if (!oldAllowChanges && allowChanges) {
					// siwtching allowChanges ON:
				}
				if (oldAllowChanges && !allowChanges) {
					// siwtching allowChanges OFF:
				}

				EditorGUI.BeginDisabledGroup (!allowChanges || !configIsDirty);
				{
					if (GUILayout.Button ("Save")) {
						ProductSpec p = Pm.AllProducts.ElementAt (selectedProductIndex);
						Pm.serializeConfig (p.Config, ConfigurationManager.RUNTIME_PRODUCT_DIR);
						configIsDirty = false;
					}
					if (GUILayout.Button ("Revert")) {
						ProductSpec p = Pm.AllProducts.ElementAt (selectedProductIndex);
						p.initConfig ();
						GUIUtility.keyboardControl = 0;
						GUIUtility.hotControl = 0;
						configIsDirty = false;
					}
				}
				EditorGUI.EndDisabledGroup ();

			}
			EditorGUILayout.EndHorizontal ();
		}


		#endregion

		void selectProduct (int index)
		{
			if (index.Equals (selectedProductIndex)) {
				return;
			}

			try {
				selectedProductIndex = index;
				EditorPrefs.SetInt ("selectedProductIndex", selectedProductIndex);
			} catch (System.IndexOutOfRangeException e) {
				Debug.LogWarning (e.Message);
			}

			// TODO create a Product object and store it. So we can access errors and manipulate details.
		}
	}


	class GQAssetChangePostprocessor : AssetPostprocessor
	{

		static bool productDirHasChanges;
		static bool configHasChanged;
		static bool buildTimeChanged;

		static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
	
			productDirHasChanges = false;
			configHasChanged = false;
			buildTimeChanged = false;
	
			// if the only change is buildtime.txt we ignore it:
			if (importedAssets.Length == 1 && importedAssets [0].Equals (ConfigurationManager.BUILD_TIME_FILE_PATH))
				return;

			foreach (string str in importedAssets) {

				setChangeFlags (str);
			}
	

			foreach (string str in deletedAssets) {

				setChangeFlags (str);
			}
	

			foreach (string str in movedAssets) {

				setChangeFlags (str);
			}


			foreach (string str in movedFromAssetPaths) {

				setChangeFlags (str);
			}

			if (buildTimeChanged && !ProductEditor.IsCurrentlyPreparingProduct) {
				writeBuildDate ();
			}

			if (configHasChanged) {
				ProductManager.Instance.ConfigFilesHaveChanges = true;
				ConfigurationManager.Reset ();
				configHasChanged = false;
			}

			ProductEditor.BuildIsDirty = productDirHasChanges; 
			if (ProductEditor.Instance != null)
				ProductEditor.Instance.Repaint ();
		}

		private static bool isRealAsset (string assetPath)
		{
			if (assetPath.StartsWith (GQAssert.TEST_DATA_BASE_DIR))
				// test assets are NOT REAL assets:
				return false;

			if (assetPath.Equals (ConfigurationManager.BUILD_TIME_FILE_PATH))
				// buildtime.txt is NOT a REAL asset:
				return false;

			return true;
		}

		private static void setChangeFlags (string str)
		{

			if (productDirHasChanges == false && str.StartsWith (ProductManager.ProductsDirPath)) {
				// a product might have changed: refresh product list:
				productDirHasChanges = true;
			}

			if (!buildTimeChanged && isRealAsset (str)) {
				buildTimeChanged = true;
			}

			if (!configHasChanged &&
			    !ProductEditor.IsCurrentlyPreparingProduct &&
			    isRealAsset (str) &&
			    str.StartsWith (ProductManager.Instance.BuildExportPath)) {
				configHasChanged = true;
			}
		}

		private static bool isWritingBuildTime = false;

		/// <summary>
		/// Writes the current build date into a tiny file in the ConfigAssets. 
		/// It will be read by the application on start and used as additional version number.
		/// </summary>
		static internal void writeBuildDate ()
		{
//			if ( isWritingBuildTime ) {
//				// prevent loops of writing buildtime due to buildtime.txt file asset changes
//				Debug.Log("buildtime left because of monitor is active");
//				return;
//			}
//
//			isWritingBuildTime = true;
			try {
				CultureInfo culture = new CultureInfo ("de-DE"); 
				if (File.Exists (ConfigurationManager.BUILD_TIME_FILE_PATH)) {
					AssetDatabase.DeleteAsset (ConfigurationManager.BUILD_TIME_FILE_PATH);
				} 
				string buildtime = DateTime.Now.ToString ("G", culture);
				File.WriteAllText (ConfigurationManager.BUILD_TIME_FILE_PATH, buildtime);
				AssetDatabase.Refresh ();
				Debug.Log ("Wrote buldtime to " + buildtime);
			} catch (Exception exc) {
				Debug.LogWarning ("Could not write build time file at " + ConfigurationManager.BUILD_TIME_FILE_PATH + "\n" + exc.Message);
				return;
			} 
//			Debug.Log("Resetting buildtime monitor flag.");
//			isWritingBuildTime = false;

		}

	}


	class MyAssetModificationProcessor : UnityEditor.AssetModificationProcessor
	{
		//
		//		static void OnWillCreateAsset (string assetPath) {
		//
		//			Debug.Log("AssetModificationProcessor will CREATE asset: " + assetPath);
		//		}
		//
		//
		//		static void OnWillDeleteAsset (string assetPath, RemoveAssetOptions options) {
		//
		//			Debug.Log("AssetModificationProcessor will DELETE asset: " + assetPath + " with options: " + options.ToString());
		//		}
		//
		//
		//		static void OnWillMoveAsset (string fromPath, string toPath) {
		//
		//			Debug.Log("AssetModificationProcessor will MOVE asset from: " + fromPath + " to: " + toPath);
		//		}
		//
		//
		//		static void OnWillSaveAssets (string[] assetPaths) {
		//
		//			foreach ( string str in assetPaths ) {
		//				Debug.Log("AssetModificationProcessor: Will SAVE asset: " + str);
		//			}
		//
		//		}
		//
		//
		//		static void IsOpenForEdit (string s1, string s2) {
		//
		//			Debug.Log("AssetModificationProcessor IsOpenForEdit(" + s1 + ", " + s2 + ")");
		//		}
	}
}

