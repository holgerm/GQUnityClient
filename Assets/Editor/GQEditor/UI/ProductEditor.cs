using UnityEditor;
using System.Collections;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Text;
using System.Reflection;
using GQ.Client.Util;
using GQ.Client.UI;
using GQ.Client.Conf;
using GQ.Editor.Building;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using GQ.Editor.Util;
using GQTests;
using UnityEditor.SceneManagement;
using QM.Util;

namespace GQ.Editor.UI
{
	public class ProductEditor : EditorWindow
	{

		public static GUIStyle TextareaGUIStyle { get; private set; }

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
		int mainVersionNumber;
		int yearVersionNumber;
		int monthVersionNumber;
		int buildVersionNumber;

		static ProductManager _pm;

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

		#region Initialization

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
			mainVersionNumber = EditorPrefs.HasKey ("mainVersionNumber") ? EditorPrefs.GetInt ("mainVersionNumber") : 0;
			yearVersionNumber = EditorPrefs.HasKey ("yearVersionNumber") ? EditorPrefs.GetInt ("yearVersionNumber") : DateTime.Now.Year;
			monthVersionNumber = EditorPrefs.HasKey ("monthVersionNumber") ? EditorPrefs.GetInt ("monthVersionNumber") : DateTime.Now.Month;
			buildVersionNumber = EditorPrefs.HasKey ("buildVersionNumber") ? EditorPrefs.GetInt ("buildVersionNumber") : 1;
			updateVersionText ();
		}

		#endregion

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
			TextareaGUIStyle = GUI.skin.textField;
			TextareaGUIStyle.wordWrap = true;

			gui4ProductManager ();

			EditorGUILayout.Space ();

			// TODO showErrors

			gui4ProductDetails ();

			gui4ProductEditPart ();

			EditorGUILayout.Space ();

			gui4Versioning ();

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

		static public Config SelectedConfig { get; private set; }

		static public float WidthForValues { get; private set; }

		static public float WidthForNames { get; private set; }

		void gui4ProductDetails ()
		{
			GUILayout.Label ("Product Details", EditorStyles.boldLabel);
			ProductSpec p = Pm.AllProducts.ElementAt (selectedProductIndex);
			SelectedConfig = p.Config;

			// Begin ScrollView:
			using (var scrollView = new EditorGUILayout.ScrollViewScope (scrollPos)) {
				scrollPos = scrollView.scrollPosition;

				using (new EditorGUI.DisabledGroupScope ((!allowChanges))) {
					// ScrollView begins (optionally disabled):
				
					PropertyInfo[] propertyInfos = typeof(Config).GetProperties ();

					// get max widths for names and values:
					float allNamesMax = 0f, allValuesMax = 0f;

					foreach (PropertyInfo curPropInfo in propertyInfos) {
//		TODO				if (entryHidden (curPropInfo))
//							continue;

						string name = curPropInfo.Name + ":";
						string value = Objects.ToString (curPropInfo.GetValue (SelectedConfig, null));

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
					WidthForNames = Math.Min ((position.width - borders) * 0.4f, allNamesMax);
					WidthForValues = position.width - (borders + WidthForNames);

					EditorGUIUtility.labelWidth = WidthForNames;

					// show all properties as textfields or textareas in fitting width:
					foreach (PropertyInfo curPropInfo in propertyInfos) {
						configIsDirty |= ProductEditorPart.CreateGui (curPropInfo);
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
						Pm.serializeConfig (SelectedConfig, ConfigurationManager.RUNTIME_PRODUCT_DIR);
						configIsDirty = false;
						LayoutConfig.ResetAll (); // TODO check and implement update all laoyut components in editor
					}
					if (GUILayout.Button ("Revert")) {
						ProductSpec p = Pm.AllProducts.ElementAt (selectedProductIndex);
						p.initConfig ();
						GUIUtility.keyboardControl = 0;
						GUIUtility.hotControl = 0;
					}
				}
				EditorGUI.EndDisabledGroup ();

			}
			EditorGUILayout.EndHorizontal ();
		}

		bool allowVersionChanges = false;

		void gui4Versioning ()
		{
			GUILayout.Label ("Versioning Options", EditorStyles.boldLabel);
			EditorGUILayout.LabelField ("Current Version", version);

			// Create New Product row:
			EditorGUILayout.BeginHorizontal ();
			{

				bool oldAllowVersionChanges = allowVersionChanges;
				allowVersionChanges = EditorGUILayout.Toggle ("Allow Editing ...", allowVersionChanges);
				if (!oldAllowVersionChanges && allowVersionChanges) {
					// siwtching allowChanges ON:
				}
				if (oldAllowVersionChanges && !allowVersionChanges) {
					// siwtching allowChanges OFF:
				}
					
			}
			EditorGUILayout.EndHorizontal ();

			if (allowVersionChanges) {
				EditorGUILayout.BeginHorizontal ();
				{
					if (GUILayout.Button ("Build +")) {
						if (EditorUtility.DisplayDialog (
							    "Really increase Build Version Number?", 
							    "It will then be " + (buildVersionNumber + 1), 
							    "Yes, increase!", 
							    "Cancel")) {
							buildVersionNumber++;
							EditorPrefs.SetInt ("buildVersionNumber", buildVersionNumber);
							updateVersionText ();
						}
					}
					if (GUILayout.Button ("Main +")) {
						if (EditorUtility.DisplayDialog (
							    "Really increase Main Version Number?", 
							    "It will then be " + (mainVersionNumber + 1), 
							    "Yes, increase!", 
							    "Cancel")) {
							mainVersionNumber++;
							EditorPrefs.SetInt ("mainVersionNumber", mainVersionNumber);
							updateVersionText ();
						}
					}
				}
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				{
					if (buildVersionNumber > 0 && GUILayout.Button ("Build -")) {
						if (EditorUtility.DisplayDialog (
							    "Really decrease Build Version Number?", 
							    "It will then be " + (buildVersionNumber - 1), 
							    "Yes, decrease!", 
							    "Cancel")) {
							buildVersionNumber--;
							EditorPrefs.SetInt ("buildVersionNumber", buildVersionNumber);
							updateVersionText ();
						}
					}
					if (GUILayout.Button ("Main -")) {
						if (EditorUtility.DisplayDialog (
							    "Really decrease Main Version Number?", 
							    "It will then be " + (mainVersionNumber - 1), 
							    "Yes, decrease!", 
							    "Cancel")) {
							mainVersionNumber--;
							EditorPrefs.SetInt ("mainVersionNumber", mainVersionNumber);
							updateVersionText ();
						}
					}
				}
				EditorGUILayout.EndHorizontal ();
			}

		}

		#endregion

		string version;

		private void updateVersionText ()
		{
			// internal version number:
			version = String.Format (
				"{0}.{1}.{2:D2}.{3:D2}", 
				mainVersionNumber,
				yearVersionNumber - 2000, 
				monthVersionNumber,
				buildVersionNumber
			);

			// bundle version for iOS and Android:
			PlayerSettings.bundleVersion = String.Format (
				"{0}.{1}.{2:D2}", 
				mainVersionNumber,
				yearVersionNumber - 2000, 
				monthVersionNumber
			);

			// bundle version code for Android:
			int bundleVersionCode;
			string bundleVersionCodeString = String.Format (
				                                 "{0}{1:D3}{2:D2}{3:D2}", 
				                                 mainVersionNumber,
				                                 yearVersionNumber - 2000, 
				                                 monthVersionNumber,
				                                 buildVersionNumber
			                                 );
			if (Int32.TryParse (bundleVersionCodeString, out bundleVersionCode))
				PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
			else {
				Debug.LogError ("Bundle Version Code not valid as Int32: " + bundleVersionCodeString);
			}

			// build for iOS:
			PlayerSettings.iOS.buildNumber = buildVersionNumber.ToString ();

			allowVersionChanges = false;
		}



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


	abstract public  class ProductEditorPart
	{
		static private Dictionary<Type, ProductEditorPart> cachedEditorParts = new Dictionary<Type, ProductEditorPart> ();

		static public GUIContent NamePrefixGUIContent { get; private set; }

		static public bool CreateGui (PropertyInfo curPropInfo)
		{
			Type propertyType = curPropInfo.PropertyType;
			ProductEditorPart accordingEditorPart;

			if (!cachedEditorParts.TryGetValue (propertyType, out accordingEditorPart)) {
				// construct the class name of the according editor gui creator class (a subclass of me):
				// the class name scheme is: PEP4<basictype>[Of<typearg1>[And<typearg2...]...]
				StringBuilder classNameBuilder = new StringBuilder (typeof(ProductEditorPart).FullName + "4");
				classNameBuilder.Append (
					(propertyType.Name.Contains ("`") ? 
						propertyType.Name.Substring (0, propertyType.Name.LastIndexOf ("`")) : 
						propertyType.Name));
				Type[] argTypes = propertyType.GetGenericArguments ();
				for (int i = 0; i < argTypes.Length; i++) {
					classNameBuilder.Append ((i == 0) ? "Of" : "And");
					classNameBuilder.Append (argTypes [i].Name);
				}
				string className = classNameBuilder.ToString ();

				// create a new instance of the according product editor part class:
				try {
					accordingEditorPart = typeof(ProductEditorPart).Assembly.CreateInstance (className) as ProductEditorPart;
					if (accordingEditorPart != null)
						cachedEditorParts.Add (propertyType, accordingEditorPart);
				} catch (Exception e) {
					Debug.Log ("Unhandled property Type: " + curPropInfo.PropertyType.Name + "\t" + e.Message);
					return false;
				}
			} 

			if (accordingEditorPart == null) {
				Debug.Log ("Unhandled property Type: " + propertyType.FullName);
				return false;
			}

			if (entryHidden (curPropInfo))
				return false;

			GUIStyle guiStyle = new GUIStyle ();

			string name = curPropInfo.Name;
			float nameMin, nameWidthNeed;
			guiStyle.CalcMinMaxWidth (new GUIContent (name + ":"), out nameMin, out nameWidthNeed);
			if (ProductEditor.WidthForNames < nameWidthNeed)
				// Show hover over name because it is too long to be shown:
				NamePrefixGUIContent = new GUIContent (name + ":", name);
			else
				// Show only name without hover:
				NamePrefixGUIContent = new GUIContent (name + ":");
			
			return accordingEditorPart.doCreateGui (curPropInfo);
		}

		abstract protected bool doCreateGui (PropertyInfo curPropInfo);

		static protected bool entryDisabled (PropertyInfo propInfo)
		{
			bool disabled = false;
			// the entry for the given property will be disabled, if one of the following is true
			disabled |= propInfo.Name.Equals ("id");

			return disabled;
		}

		static protected bool entryHidden (PropertyInfo propInfo)
		{
			bool hidden = false;

			hidden |= !propInfo.CanRead;
			hidden |= (
			    ProductEditor.SelectedConfig.mapProvider == MapProvider.OpenStreetMap
			) && (
			    propInfo.Name.Equals ("mapBaseUrl") ||
			    propInfo.Name.Equals ("mapKey") ||
			    propInfo.Name.Equals ("mapID") ||
			    propInfo.Name.Equals ("mapTileImageExtension")
			);
			hidden |= (
			    ProductEditor.SelectedConfig.mapProvider == MapProvider.MapBox
			) && (
			    propInfo.Name.Equals ("mapBaseUrl") ||
			    propInfo.Name.Equals ("mapTileImageExtension")
			);

			return hidden;
		}

	}

	public class ProductEditorPart4Boolean : ProductEditorPart
	{

		override protected bool doCreateGui (PropertyInfo curPropInfo)
		{
			bool markConfigAsDirty = false;

			// show checkbox:
			EditorGUILayout.BeginHorizontal ();
			{
				EditorGUILayout.PrefixLabel (NamePrefixGUIContent);
				bool oldBoolVal = (bool)curPropInfo.GetValue (ProductEditor.SelectedConfig, null);
				bool newBoolVal = EditorGUILayout.Toggle (oldBoolVal);
				if (newBoolVal != oldBoolVal) {
					markConfigAsDirty = true;
					curPropInfo.SetValue (ProductEditor.SelectedConfig, newBoolVal, null);
				}
			}
			EditorGUILayout.EndHorizontal ();

			return markConfigAsDirty;
		}
	}


	public class ProductEditorPart4Color : ProductEditorPart
	{

		override protected bool doCreateGui (PropertyInfo curPropInfo)
		{
			bool markConfigAsDirty = false;

			Color oldColorVal = (Color)curPropInfo.GetValue (ProductEditor.SelectedConfig, null);
			Color newColorVal = oldColorVal;

			// show Color field if value fits in one line:
			newColorVal = EditorGUILayout.ColorField (NamePrefixGUIContent, oldColorVal);
			if (newColorVal != oldColorVal) {
				markConfigAsDirty = true;
				curPropInfo.SetValue (ProductEditor.SelectedConfig, newColorVal, null);
			}

			return markConfigAsDirty;
		}
	}


	public class ProductEditorPart4DownloadStrategy : ProductEditorPart
	{

		int selectedDownloadStrategy;
		string[] downloadStrategyNames = Enum.GetNames (typeof(DownloadStrategy));

		override protected bool doCreateGui (PropertyInfo curPropInfo)
		{
			bool markConfigAsDirty = false;

			// TODO implement all three strategies
			int oldDownloadStrategy = selectedDownloadStrategy;
			selectedDownloadStrategy = 
				EditorGUILayout.Popup (
				"Download Strategy", 
				selectedDownloadStrategy, 
				downloadStrategyNames
			);
			if (oldDownloadStrategy != selectedDownloadStrategy) {
				markConfigAsDirty = true;
				curPropInfo.SetValue (ProductEditor.SelectedConfig, (DownloadStrategy)selectedDownloadStrategy, null);
			}

			return markConfigAsDirty;
		}
	}


	public class ProductEditorPart4ImagePath : ProductEditorPart
	{
		override protected bool doCreateGui (PropertyInfo curPropInfo)
		{
			bool markConfigAsDirty = false;
			GUIContent myNamePrefixGUIContent = NamePrefixGUIContent;

			using (new EditorGUI.DisabledGroupScope (entryDisabled (curPropInfo))) {
				// get currently stored image path from config:
				ImagePath oldVal = (ImagePath)curPropInfo.GetValue (ProductEditor.SelectedConfig, null);
				Sprite oldSprite = Resources.Load<Sprite> (oldVal.path);

				// show textarea if value does not fit within one line:
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.PrefixLabel (myNamePrefixGUIContent);
				Sprite newSprite = 
					(Sprite)EditorGUILayout.ObjectField (oldSprite, typeof(Sprite), false);
				string path = AssetDatabase.GetAssetPath (newSprite);
				ImagePath newVal = new ImagePath (Files.GetResourcesRelativePath (path));
				EditorGUILayout.EndHorizontal ();
				if (newVal.path != "" && newVal.path != null && !newVal.path.Equals (oldVal.path)) {
					markConfigAsDirty = true;
					curPropInfo.SetValue (ProductEditor.SelectedConfig, newVal, null);
				}
			}

			return markConfigAsDirty;
		}
	}


	public class ProductEditorPart4Int32 : ProductEditorPart
	{

		override protected bool doCreateGui (PropertyInfo curPropInfo)
		{
			bool markConfigAsDirty = false;
			GUIContent myNamePrefixGUIContent = NamePrefixGUIContent;

			using (new EditorGUI.DisabledGroupScope (entryDisabled (curPropInfo))) {
				// id of products may not be altered.
				if (curPropInfo.Name.Equals ("id")) {
					myNamePrefixGUIContent = new GUIContent (curPropInfo.Name, "You may not alter the id of a product.");
				}
				int oldIntVal = (int)curPropInfo.GetValue (ProductEditor.SelectedConfig, null);

				// show text field if value fits in one line:
				int newIntVal = EditorGUILayout.IntField (myNamePrefixGUIContent, oldIntVal);
				if (newIntVal != oldIntVal) {
					markConfigAsDirty = true;
					curPropInfo.SetValue (ProductEditor.SelectedConfig, newIntVal, null);
				}
			}

			return markConfigAsDirty;
		}
	}


	public class ProductEditorPart4ListOfCategory : ProductEditorPart
	{

		override protected bool doCreateGui (PropertyInfo curPropInfo)
		{
			bool markConfigAsDirty = false;

			List<Category> values = (List<Category>)curPropInfo.GetValue (ProductEditor.SelectedConfig, null);
			if (values == null)
				values = new List<Category> ();
			bool valsChanged = false;

			// Header with Add and Clear Button:
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label (
				string.Format ("Categories ({0})", values.Count), 
				EditorStyles.boldLabel
			);
			if (GUILayout.Button ("+")) {
				Category cat = new Category ();
				cat.symbol = new ImagePath ();
				values.Add (cat);
				valsChanged = true;
			}
			EditorGUILayout.EndHorizontal ();

			for (int i = 0; i < values.Count; i++) {
				Category oldCat = values [i];
				Category newCat = oldCat;

				bool valChanged = false;
				// category name:
				string newName = EditorGUILayout.TextField (new GUIContent ("name:"), oldCat.name);
				valChanged |= (newName != oldCat.name);
				// id as text:
				string newId = EditorGUILayout.TextField (new GUIContent ("id:", "Id must be unqiue within these categories."), oldCat.id);
				if (newId != oldCat.id) {
					// check that the new id is not used among the other categories, else reset to the old id:
					bool newIdIsUnique = true;
					for (int j = 0; j < values.Count; j++) {
						if (j == i)
							continue;
						if (values [j] == values [i]) {
							newIdIsUnique = false;
							break;
						}
					}
					if (!newIdIsUnique) {
						// reset if not unique:
						newId = oldCat.id;
					} else {
						valChanged |= (newId != oldCat.id);
					}
				}

				// symbol:
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.PrefixLabel (new GUIContent ("symbol:"));
				// get currently stored image path from config:
				ImagePath oldSymbolPath = oldCat.symbol;
				ImagePath newSymbolPath = oldSymbolPath;
				Sprite oldSymbolSprite = Resources.Load<Sprite> (oldSymbolPath.path);
				Sprite newSymbolSprite = 
					(Sprite)EditorGUILayout.ObjectField (oldSymbolSprite, typeof(Sprite), false);
				if (newSymbolSprite != oldSymbolSprite) {
					string path = AssetDatabase.GetAssetPath (newSymbolSprite);
					newSymbolPath = new ImagePath (Files.GetResourcesRelativePath (path));
					valChanged |= (newSymbolPath.path != oldSymbolPath.path);
				}
				if (valChanged) {
					valsChanged = true;
					newCat.name = newName;
					newCat.id = newId;
					newCat.symbol = newSymbolPath;
					values [i] = newCat;
				}

				if (GUILayout.Button ("-")) {
					if (EditorUtility.DisplayDialog (
						    string.Format ("Really delete category {0}?", (oldCat.name != null && oldCat.name != "") ? oldCat.name : i.ToString ()), 
						    string.Format (
							    "This can not be undone"
						    ), 
						    "Yes, delete it!", 
						    "No, keep it")) {
						values.Remove (values [i]);
						valsChanged = true;
					}
				}
				EditorGUILayout.EndHorizontal (); // end horizontal line of symbol and delete button for current category.
			}
			if (valsChanged) {
				// Update Config property for scene extensions:
				markConfigAsDirty = true;
				curPropInfo.SetValue (ProductEditor.SelectedConfig, values, null);
			}

			return markConfigAsDirty;
		}
	}


	public class ProductEditorPart4ListOfSceneExtension : ProductEditorPart
	{

		override protected bool doCreateGui (PropertyInfo curPropInfo)
		{
			bool markConfigAsDirty = false;

			List<SceneExtension> sceneExtsVal = (List<SceneExtension>)curPropInfo.GetValue (ProductEditor.SelectedConfig, null);
			bool sceneExtsChanged = false;
			string sceneName;

			// Header with Add and Clear Button:
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label (
				string.Format ("Scene Extensions ({0})", sceneExtsVal.Count), 
				EditorStyles.boldLabel
			);
			if (GUILayout.Button ("+")) {
				SceneExtension sce = new SceneExtension ();
				sce.root = "";
				sce.prefab = "";
				sce.scene = EditorSceneManager.GetActiveScene ().path;
				sceneExtsVal.Add (sce);
				sceneExtsChanged = true;
			}
			EditorGUILayout.EndHorizontal ();

			for (int i = 0; i < sceneExtsVal.Count; i++) {
				SceneExtension oldSceneExt = sceneExtsVal [i];
				SceneExtension newSceneExt = oldSceneExt;
				// entry for extension only enabled when in same scene:
				bool sceneExtensionDisabled = EditorSceneManager.GetActiveScene ().path != oldSceneExt.scene;

				using (new EditorGUI.DisabledGroupScope (sceneExtensionDisabled)) {
					EditorGUILayout.BeginHorizontal ();
					bool sceneExtChanged = false;
					// scene name:
					sceneName = Files.FileName (oldSceneExt.scene);
					if (sceneName.EndsWith (".unity"))
						sceneName = sceneName.Substring (0, sceneName.Length - ".unity".Length);
					// prefab:
					EditorGUILayout.PrefixLabel (new GUIContent ("  -> " + sceneName, "The prefab extends this scene."));
					GameObject oldPrefabGO = Resources.Load<GameObject> (oldSceneExt.prefab);
					GameObject newPrefabGO = 
						(GameObject)EditorGUILayout.ObjectField (oldPrefabGO, typeof(GameObject), false);
					if (newPrefabGO != oldPrefabGO && PrefabUtility.GetPrefabType (newPrefabGO) == PrefabType.Prefab) {
						// if user selected another prefab we store it:
						newSceneExt.prefab = Files.GetResourcesRelativePath (AssetDatabase.GetAssetPath (newPrefabGO));
						sceneExtChanged = true;
						Debug.Log ("Old Prefab: " + oldSceneExt.prefab);
						Debug.Log ("New Prefab: " + newSceneExt.prefab);
					}
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.BeginHorizontal ();
					// root:
					EditorGUILayout.PrefixLabel (new GUIContent ("\t\tat", "The root gameobject where the prefab is injected."));
					if (sceneExtensionDisabled) {
						EditorGUILayout.TextField (Files.FileName (oldSceneExt.root));
					} else {
						GameObject oldRootGO = GameObject.Find (oldSceneExt.root);
						GameObject newRootGO = 
							(GameObject)EditorGUILayout.ObjectField (oldRootGO, typeof(GameObject), true);
						if (newRootGO != oldRootGO && newRootGO.scene == EditorSceneManager.GetActiveScene ()) {
							newSceneExt.root = newRootGO.transform.GetPath ();
							sceneExtChanged = true;
							Debug.Log ("New Root: " + newSceneExt.root);
						}
					}
					if (sceneExtChanged) {
						newSceneExt.scene = EditorSceneManager.GetActiveScene ().path;
						sceneExtsVal [i] = newSceneExt;
						sceneExtsChanged = true;
					}
				} // end disabled group for current scene extension
				if (GUILayout.Button ("-")) {
					if (EditorUtility.DisplayDialog (
						    string.Format ("Really delete extension for scene {0}?", sceneName), 
						    string.Format (
							    "It adds {0} to {1}.", 
							    Files.FileName (oldSceneExt.prefab),
							    Files.FileName (oldSceneExt.root)
						    ), 
						    "Yes, delete it!", 
						    "No, keep it")) {
						sceneExtsVal.Remove (oldSceneExt);
						sceneExtsChanged = true;
					}
				}
				EditorGUILayout.EndHorizontal (); // end horizontal line of prefab and delete button for current scene extension.
			}
			if (sceneExtsChanged) {
				// Update Config property for scene extensions:
				markConfigAsDirty = true;
				curPropInfo.SetValue (ProductEditor.SelectedConfig, sceneExtsVal, null);
			}

			return markConfigAsDirty;
		}
	}


	public class ProductEditorPart4MapProvider : ProductEditorPart
	{
		int selectedMapProvider;
		string[] mapProviderNames = Enum.GetNames (typeof(MapProvider));

		override protected bool doCreateGui (PropertyInfo curPropInfo)
		{
			bool markConfigAsDirty = false;

			int oldMapProvider = selectedMapProvider;
			selectedMapProvider = 
				EditorGUILayout.Popup (
				"Map Provider", 
				selectedMapProvider, 
				mapProviderNames
			);
			if (oldMapProvider != selectedMapProvider) {
				markConfigAsDirty = true;
				curPropInfo.SetValue (ProductEditor.SelectedConfig, (MapProvider)selectedMapProvider, null);
			}

			return markConfigAsDirty;
		}
	}


	public class ProductEditorPart4Single : ProductEditorPart
	{

		override protected bool doCreateGui (PropertyInfo curPropInfo)
		{
			bool markConfigAsDirty = false;

			float oldFloatVal = (float)curPropInfo.GetValue (ProductEditor.SelectedConfig, null);
			float newFloatVal = oldFloatVal;

			// show text field if value fits in one line:
			newFloatVal = EditorGUILayout.FloatField (NamePrefixGUIContent, oldFloatVal);
			if (newFloatVal != oldFloatVal) {
				markConfigAsDirty = true;
				curPropInfo.SetValue (ProductEditor.SelectedConfig, newFloatVal, null);
			}

			return markConfigAsDirty;
		}
	}


	public class ProductEditorPart4String : ProductEditorPart
	{
		override protected bool doCreateGui (PropertyInfo curPropInfo)
		{
			bool markConfigAsDirty = false;
			GUIContent myNamePrefixGUIContent = NamePrefixGUIContent;

			using (new EditorGUI.DisabledGroupScope (entryDisabled (curPropInfo))) {
				// id of products may not be altered.
				if (curPropInfo.Name.Equals ("id")) {
					myNamePrefixGUIContent = new GUIContent (curPropInfo.Name, "You may not alter the id of a product.");
				} 
				// show textfield or if value too long show textarea:
				string oldStringVal = (string)curPropInfo.GetValue (ProductEditor.SelectedConfig, null);
				oldStringVal = Objects.ToString (oldStringVal);
				string newStringVal;
				GUIStyle guiStyle = new GUIStyle ();
				float valueMin, valueNeededWidth;
				guiStyle.CalcMinMaxWidth (new GUIContent (oldStringVal), out valueMin, out valueNeededWidth);

				if (ProductEditor.WidthForValues < valueNeededWidth) {
					// show textarea if value does not fit within one line:
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.PrefixLabel (myNamePrefixGUIContent);
					newStringVal = EditorGUILayout.TextArea (oldStringVal, ProductEditor.TextareaGUIStyle);
					newStringVal = Objects.ToString (newStringVal);
					EditorGUILayout.EndHorizontal ();
				} else {
					// show text field if value fits in one line:
					newStringVal = EditorGUILayout.TextField (myNamePrefixGUIContent, oldStringVal);
					newStringVal = Objects.ToString (newStringVal);
				}
				if (!newStringVal.Equals (oldStringVal)) {
					markConfigAsDirty = true;
					curPropInfo.SetValue (ProductEditor.SelectedConfig, newStringVal, null);
				}
			}

			return markConfigAsDirty;
		}
	}


}

