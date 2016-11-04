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
using GQ.Editor.Building;
using GQ.Client.Util;

namespace GQ.Editor.UI {
	public class ProductEditor : EditorWindow {

		private GUIStyle textareaGUIStyle;
		private Texture warnIcon;
		Vector2 scrollPos;

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



		[MenuItem("Window/QuestMill Product Editor")]
		public static void  Init () {
			Assembly editorAsm = typeof(UnityEditor.Editor).Assembly;
			Type inspectorWindowType = editorAsm.GetType("UnityEditor.InspectorWindow");
			ProductEditor editor;
			if ( inspectorWindowType != null )
				editor = EditorWindow.GetWindow<ProductEditor>("GQ Product", true, inspectorWindowType);
			else
				editor = EditorWindow.GetWindow<ProductEditor>(typeof(ProductEditor));
			editor.Show();
			Instance = editor;
		}

		int selectedProductIndex;

		ProductManager pm;

		public void OnEnable () {
			Instance = this;

			readStateFromEditorPrefs();
			warnIcon = (Texture)AssetDatabase.LoadAssetAtPath(WARN_ICON_PATH, typeof(Texture));

			pm = ProductManager.Instance;
		}

		void readStateFromEditorPrefs () {
			selectedProductIndex = EditorPrefs.HasKey("selectedProductIndex") ? EditorPrefs.GetInt("selectedProductIndex") : 0;
		}

		//		public void OnDisable () {
		//			// make some saves?
		////			Debug.Log("EDITOR.OnDisable() " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
		//		}
		//
		//		public void OnFocus () {
		//			// make some saves?
		//			Debug.Log("EDITOR.OnFocus() " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
		//		}
		//
		//		public void OnLostFocus () {
		//			// make some saves?
		//			Debug.Log("EDITOR.OnLostFocus() " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
		//		}
		//
		//		public void OnProjectChange () {
		//			// TODO: rescan products folder and build folder
		//			Debug.Log("EDITOR.OnProjectChange() " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
		//		}
		//

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
		void OnGUI () {
			// adjust textarea style:
			textareaGUIStyle = GUI.skin.textField;
			textareaGUIStyle.wordWrap = true;

			gui4ProductManager();

			EditorGUILayout.Space();

			// TODO showErrors

			gui4ProductDetails();

			gui4ProductEditPart();

			GUILayout.FlexibleSpace();

		}

		private string newProductID = "";


		void gui4ProductManager () {
			// Heading:
			GUILayout.Label("Product Manager", EditorStyles.boldLabel);

			// Current Build:
			string buildName = currentBuild();
			EditorGUILayout.BeginHorizontal();
			{
				if ( buildName == null ) {
					buildName = "missing";
					EditorGUILayout.PrefixLabel(new GUIContent("Current Build:", warnIcon));
				}
				else {
					if ( BuildIsDirty ) {
						EditorGUILayout.PrefixLabel(new GUIContent("Current Build:", warnIcon));
					}
					else {
						EditorGUILayout.PrefixLabel(new GUIContent("Current Build:"));
					}
				}
				GUILayout.Label(buildName);
			}
			EditorGUILayout.EndHorizontal();

			// Prepare Build Button:
			EditorGUILayout.BeginHorizontal();
			{
				string selectedProductName = pm.AllProductIds.ElementAt(selectedProductIndex);
				if ( GUILayout.Button("Prepare Build") ) {
					pm.PrepareProductForBuild(selectedProductName);
				}
			}
			EditorGUILayout.EndHorizontal();

			// Product Selection Popup:
			string[] productIds = pm.AllProductIds.ToArray<string>();
			int newIndex = EditorGUILayout.Popup("Available Products:", selectedProductIndex, productIds);
			selectProduct(newIndex);

			// Create New Product row:
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Label("New product (id):");
				newProductID = EditorGUILayout.TextField(
					newProductID, 
					GUILayout.Height(EditorGUIUtility.singleLineHeight));
				bool createButtonshouldBeDisabled = newProductID.Equals("") || pm.AllProductIds.Contains(newProductID);
				EditorGUI.BeginDisabledGroup(createButtonshouldBeDisabled);
				{
					if ( GUILayout.Button("Create") ) {
						pm.createNewProduct(newProductID);
					}
				}
				EditorGUI.EndDisabledGroup();
			}
			EditorGUILayout.EndHorizontal();
		}

		internal string currentBuild () {
			string build = null;

			try {
				string configFile = Files.CombinePath(pm.BuildExportPath, ConfigurationManager.CONFIG_FILE);
				if ( !File.Exists(configFile) )
					return build;
				string configText = File.ReadAllText(configFile);
				Config buildConfig = JsonMapper.ToObject<Config>(configText);
				return buildConfig.id;
			} catch ( Exception exc ) {
				Debug.LogWarning("ProductEditor.currentBuild() threw exception:\n" + exc.Message);
				return build;
			}
		}

		void gui4ProductDetails () {
			GUILayout.Label("Product Details", EditorStyles.boldLabel);
			ProductSpec p = pm.AllProducts.ElementAt(selectedProductIndex);
			 
			// Begin ScrollView:
			using ( var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos /* , GUILayout.Width(100), GUILayout.Height(100) */) ) {
				scrollPos = scrollView.scrollPosition;

				using ( new EditorGUI.DisabledGroupScope((!allowChanges)) ) {
					// ScrollView begins (optionally disabled):
				
					PropertyInfo[] propertyInfos = typeof(Config).GetProperties();

					// get max widths for names and values:
					float allNamesMax = 0f, allValuesMax = 0f;

					foreach ( PropertyInfo curPropInfo in propertyInfos ) {
						if ( !curPropInfo.CanRead )
							continue;

						string name = curPropInfo.Name + ":";
						string value = Objects.ToString(curPropInfo.GetValue(p.Config, null));

						float nameMin, nameMax;
						float valueMin, valueMax;
						GUIStyle guiStyle = new GUIStyle();

						guiStyle.CalcMinMaxWidth(new GUIContent(name + ":"), out nameMin, out nameMax);
						allNamesMax = Math.Max(allNamesMax, nameMax);
						guiStyle.CalcMinMaxWidth(new GUIContent(value), out valueMin, out valueMax);
						allValuesMax = Math.Max(allValuesMax, valueMax);
					}

					// calculate widths for names and values finally: we allow no more than 40% of the editor width for names.
					// add left, middle and right borders as given:
					float borders = new GUIStyle(GUI.skin.textField).margin.left + new GUIStyle(GUI.skin.textField).margin.horizontal + new GUIStyle(GUI.skin.textField).margin.right; 
					// calculate widths for names and vlaues finally: we allow no more than 40% of the editor width for names, but do not take more than we need.
					float widthForNames = Math.Min((position.width - borders) * 0.4f, allNamesMax);
					float widthForValues = position.width - (borders + widthForNames);

					EditorGUIUtility.labelWidth = widthForNames;

					// show all properties as textfields or textareas in fitting width:
					foreach ( PropertyInfo curPropInfo in propertyInfos ) {
						if ( !curPropInfo.CanRead )
							continue;

						GUIStyle guiStyle = new GUIStyle();

						string name = curPropInfo.Name;
						float nameMin, nameWidthNeed;
						guiStyle.CalcMinMaxWidth(new GUIContent(name + ":"), out nameMin, out nameWidthNeed);
						GUIContent namePrefixGUIContent;
						if ( widthForNames < nameWidthNeed )
							// Show hover over name because it is too long to be shown:
							namePrefixGUIContent = new GUIContent(name + ":", name);
						else
							// Show only name without hover:
							namePrefixGUIContent = new GUIContent(name + ":");

						Debug.Log("Property found with proprtyType: " + curPropInfo.PropertyType.Name);

						switch ( curPropInfo.PropertyType.Name ) {
							case "Boolean":
								// show checkbox:
								EditorGUILayout.BeginHorizontal();
								EditorGUILayout.PrefixLabel(namePrefixGUIContent);
								EditorGUILayout.Toggle((bool)curPropInfo.GetValue(p.Config, null));
								EditorGUILayout.EndHorizontal();
								break;
							default:
								string value = Objects.ToString(curPropInfo.GetValue(p.Config, null));
								float valueMin, valueNeededWidth;
								guiStyle.CalcMinMaxWidth(new GUIContent(value), out valueMin, out valueNeededWidth);

								if ( widthForValues < valueNeededWidth ) {
									// show textarea if value does not fit within one line:
									EditorGUILayout.BeginHorizontal();
									EditorGUILayout.PrefixLabel(namePrefixGUIContent);
									EditorGUILayout.TextArea(value, textareaGUIStyle);
									EditorGUILayout.EndHorizontal();
								}
								else {
									// show text field if value fits in one line:
									EditorGUILayout.TextField(namePrefixGUIContent, value);
								}
								break;
						}
					}
				} // End Scope Disabled Group 
			} // End Scope ScrollView 
		}

		private void createGUIElement4Value (PropertyInfo configProperty) {
//			switch (configProperty.GetType()) {
//				case String:
//					
//			}
		}

		private bool allowChanges = false;

		void gui4ProductEditPart () {
			GUILayout.Label("Editing Options", EditorStyles.boldLabel);

			// Create New Product row:
			EditorGUILayout.BeginHorizontal();
			{

				bool oldAllowChanges = allowChanges;
				allowChanges = EditorGUILayout.Toggle("Allow To Edit ...", allowChanges);
				if ( !oldAllowChanges && allowChanges ) {
					// siwtching allowChanges ON:
				}
				if ( oldAllowChanges && !allowChanges ) {
					// siwtching allowChanges OFF:
				}

				EditorGUI.BeginDisabledGroup(!allowChanges);
				{
					if ( GUILayout.Button("Save") ) {
						// TODO
						ProductSpec p = pm.AllProducts.ElementAt(selectedProductIndex);
						pm.serializeConfig(p.Config, Files.CombinePath(ProductManager.ProductsDirPath, p.Id));
					}
				}
				EditorGUI.EndDisabledGroup();

			}
			EditorGUILayout.EndHorizontal();
		}


		#endregion

		void selectProduct (int index) {
			if ( index.Equals(selectedProductIndex) ) {
				return;
			}

			try {
				selectedProductIndex = index;
				EditorPrefs.SetInt("selectedProductIndex", selectedProductIndex);
			} catch ( System.IndexOutOfRangeException e ) {
				Debug.LogWarning(e.Message);
			}

			// TODO create a Product object and store it. So we can access errors and manipulate details.
		}
	}


	class MyAllPostprocessor : AssetPostprocessor {

		static bool productDictionaryDirty;
		static bool buildDirty;

		static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
	
			productDictionaryDirty = false;
			buildDirty = false;

			foreach ( string str in importedAssets ) {

				check4ExternalChanges(str);
			}
	

			foreach ( string str in deletedAssets ) {

				check4ExternalChanges(str);
			}
	

			foreach ( string str in movedAssets ) {

				check4ExternalChanges(str);
			}


			foreach ( string str in movedFromAssetPaths ) {

				check4ExternalChanges(str);
			}

			if ( productDictionaryDirty ) {
				ConfigurationManager.Reset();
			}

			ProductEditor.BuildIsDirty = buildDirty; 
			if ( ProductEditor.Instance != null )
				ProductEditor.Instance.Repaint();
		}

		private static void check4ExternalChanges (string str) {

			if ( productDictionaryDirty == false && str.StartsWith(ProductManager.PRODUCTS_DIR_PATH_DEFAULT) ) {
				// a product might have changed: refresh product list:
				productDictionaryDirty = true;
			}
			if ( str.StartsWith(ProductManager.Instance.BuildExportPath) && !str.Equals(ConfigurationManager.BUILD_TIME_FILE_PATH) ) {
				// the build might be changed externally: signal that to the Product Editor:
				Debug.Log("Set PE Dirty Flag due to change of " + str);

				buildDirty = true;
			}
		}
	}


	//	class MyAssetModificationProcessor : UnityEditor.AssetModificationProcessor {
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
	//	}
}

