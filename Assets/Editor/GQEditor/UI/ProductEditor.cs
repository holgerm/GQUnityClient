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
		}

		int selectedProductIndex;

		ProductManager pm;

		public void OnEnable () {
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

			// TODO Editing opions for Product Details
		}

		private string newProductID = "";


		void gui4ProductManager () {
			// Heading:
			GUILayout.Label("Product Manager", EditorStyles.boldLabel);

			// Current Build:
			string buildName = currentBuild();
			EditorGUILayout.BeginHorizontal();
			if ( buildName == null ) {
				buildName = "missing";
				EditorGUILayout.PrefixLabel(new GUIContent("Current Build:", warnIcon));
			}
			else {
				EditorGUILayout.PrefixLabel(new GUIContent("Current Build:"));
			}
			GUILayout.Label(buildName);
			EditorGUILayout.EndHorizontal();

			// Prepare Build Button:
			EditorGUILayout.BeginHorizontal();
			// Disable build button if current build is the same as selectedProduct:
			string selectedProductName = pm.AllProductIds.ElementAt(selectedProductIndex);
			EditorGUI.BeginDisabledGroup(selectedProductName.Equals(buildName));
			if ( GUILayout.Button("Prepare Build") ) {
				if ( !buildName.Equals(selectedProductName) )
					pm.SetProductForBuild(selectedProductName);
			}
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();

			// Build Selection Popup:
			string[] productIds = pm.AllProductIds.ToArray<string>();
			int newIndex = EditorGUILayout.Popup("Available Products:", selectedProductIndex, productIds);
			selectProduct(newIndex);

			// Create New Product row:
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("New product (id):");
			newProductID = EditorGUILayout.TextField(
				newProductID, 
				GUILayout.Height(EditorGUIUtility.singleLineHeight));
			bool createButtonshouldBeDisabled = newProductID.Equals("") || pm.AllProductIds.Contains(newProductID);
			EditorGUI.BeginDisabledGroup(createButtonshouldBeDisabled);
			if ( GUILayout.Button("Create") ) {
				pm.createNewProduct(newProductID);
			}
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
		}

		private string currentBuild () {
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
		}

		void gui4ProductDetails () {
			GUILayout.Label("Product Details", EditorStyles.boldLabel);
			Product p = pm.AllProducts.ElementAt(selectedProductIndex);

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

			GUI.enabled = false;

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

			// calculate widths for names and vlaues finally: we allow no more than 40% of the editor width for names.
			// add left and right borders as given plus 3 for the middle:
			float borders = new GUIStyle(GUI.skin.textField).margin.left + new GUIStyle(GUI.skin.textField).margin.horizontal + new GUIStyle(GUI.skin.textField).margin.right; 
			// calculate widths for names and vlaues finally: we allow no more than 40% of the editor width for names, but do not take more than we need.
			float widthForNames = Math.Min((position.width - borders) * 0.4f, allNamesMax);
			float widthForValues = position.width - (borders + widthForNames);

			EditorGUIUtility.labelWidth = widthForNames;

			// show all properties as textfields or textareas in fitting width:
			foreach ( PropertyInfo curPropInfo in propertyInfos ) {
				if ( !curPropInfo.CanRead )
					continue;

				string name = curPropInfo.Name;
				string value = Objects.ToString(curPropInfo.GetValue(p.Config, null));

				float nameMin, nameWidthNeed;
				float valueMin, valueNeededWidth;
				GUIStyle guiStyle = new GUIStyle();

				guiStyle.CalcMinMaxWidth(new GUIContent(name + ":"), out nameMin, out nameWidthNeed);
				guiStyle.CalcMinMaxWidth(new GUIContent(value), out valueMin, out valueNeededWidth);

				if ( widthForValues < valueNeededWidth ) {
					// show textarea if value does not fit within one line:
					if ( widthForNames < nameWidthNeed ) {
						// show tooltip if not enough space for name:
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.PrefixLabel(new GUIContent(name + ":", name));
						EditorGUILayout.TextArea(value, textareaGUIStyle);
						EditorGUILayout.EndHorizontal();
					}
					else {
						// show simply the name if it fits:
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.PrefixLabel(new GUIContent(name + ":"));
						EditorGUILayout.TextArea(value, textareaGUIStyle);
						EditorGUILayout.EndHorizontal();
					}
				}
				else {
					// show text field if value fits in one line:
					if ( widthForNames < nameWidthNeed ) {
						// show tooltip if not enough space for name:
						EditorGUILayout.TextField(new GUIContent(name + ":", name), value);
					}
					else {
						// show simply the name if it fits:
						EditorGUILayout.TextField(new GUIContent(name + ":"), value);
					}
				}
			}
				
			GUI.enabled = true;
			EditorGUILayout.EndScrollView();
		}

		#endregion

	}


	//	class MyAllPostprocessor : AssetPostprocessor {
	//
	//		static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
	//
	//			foreach ( string str in importedAssets ) {
	//				Debug.Log("AssetPostprocessor: Imported Asset: " + str);
	//			}
	//
	//			foreach ( string str in deletedAssets ) {
	//				Debug.Log("AssetPostprocessor: Deleted Asset: " + str);
	//			}
	//
	//			for ( int i = 0; i < movedAssets.Length; i++ ) {
	//				Debug.Log("AssetPostprocessor: Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
	//			}
	//		}
	//	}
	//
	//
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

