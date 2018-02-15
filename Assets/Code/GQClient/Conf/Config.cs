using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using GQ.Client.Err;
using Newtonsoft.Json.Converters;

namespace GQ.Client.Conf
{
	/// <summary>
	/// Config class specifies textual parameters of a product. It is used both at runtime to initilize the app's branding details from and 
	/// at editor time to back the product editor view and store the parameters while we use the editor.
	/// </summary>
	public class Config
	{
		//////////////////////////////////
		// THE ACTUAL PRODUCT CONFIG DATA:	
		
		[ShowInProductEditor]
		public string   id     { get; set; }

		[ShowInProductEditor]
		public string   name   { get; set; }

		[ShowInProductEditor]
		public int   	portal   { get; set; }

		[ShowInProductEditor]
		public int   	autoStartQuestID   { get; set; }

		[ShowInProductEditor]
		public bool 	autostartIsPredeployed  { get; set; }

		[ShowInProductEditor]
		public bool 	keepAutoStarting  { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(StringEnumConverter))]
		public DownloadStrategy DownloadStrategy { get; set; }

		[ShowInProductEditor]
		public long   	timeoutMS   { get; set; }

		[ShowInProductEditor]
		public string 	nameForQuestSg { get; set; }

		[ShowInProductEditor]
		public string 	nameForQuestsPl { get; set; }

		[ShowInProductEditor]
		public string[] 	questInfoViews { get; set; }

		public bool 	cloudQuestsVisible  { get; set; }

		public bool 	showCloudQuestsImmediately  { get; set; }

		public bool 	downloadAllCloudQuestOnStart  { get; set; }

		public bool 	localQuestsDeletable  { get; set; }

		public bool 	hideHiddenQuests  { get; set; }

		[ShowInProductEditor (StartSection = "Pages & Scenes:")]
		public string[]	acceptedPageTypes { get; set; }

		public Dictionary<string, string> GetSceneMappingsDict ()
		{ 
			Dictionary<string, string> smDict = new Dictionary<string, string> ();
			foreach (SceneMapping sm in sceneMappings) {
				smDict.Add (sm.pageTypeName, sm.scenePath);
			}
			return smDict;
		}

		[ShowInProductEditor]
		public List<SceneMapping> sceneMappings { get; set; }

		private string[] _scenePaths;

		[ShowInProductEditor]
		public string[]	scenePaths { 
			get {
				if (_scenePaths == null) {
					_scenePaths = new string[0];
				}
				return _scenePaths;
			} 
			set {
				if (value != null)
					_scenePaths = value;
			}
		}

		[ShowInProductEditor]
		public List<SceneExtension> sceneExtensions { get; set; }


		#region Map

		[ShowInProductEditor (StartSection = "Map & Markers:")]
		[JsonConverter (typeof(StringEnumConverter))]
		public MapProvider 	mapProvider { get; set; }

		[ShowInProductEditor]
		public string 	mapBaseUrl { get; set; }

		[ShowInProductEditor]
		public string 	mapKey { get; set; }

		[ShowInProductEditor]
		public string 	mapID { get; set; }

		[ShowInProductEditor]
		public string 	mapTileImageExtension { get; set; }

		public bool		useMapOffline { get; set; }

		[ShowInProductEditor]
		public float	mapMinimalZoom { get; set; }

		[ShowInProductEditor]
		public float	mapDeltaZoom { get; set; }


		[ShowInProductEditor]
		public bool mapStartAtLocation { get; set; }

		[ShowInProductEditor]
		public double mapStartAtLongitude { get; set; }

		[ShowInProductEditor]
		public double mapStartAtLatitude { get; set; }

		[JsonIgnore]
		private ImagePath _marker;

		[ShowInProductEditor]
		public ImagePath marker { 
			get {
				if (_marker == null) {
					return new ImagePath ("defaults/readable/defaultMarker");
				}
				return _marker;
			}
			set {
				_marker = value;
			}
		}

		[ShowInProductEditor]
		public int markerHeightUnits { get; set; }

		[ShowInProductEditor]
		public int markerHeightMinMM { get; set; }

		[ShowInProductEditor]
		public int markerHeightMaxMM { get; set; }

		/// <summary>
		/// This should not be shown in the Product Editor neither persistetd in Product.json but calculated in the background instead. 
		/// It will rely on markerHeightUnits, markerHeightMinMM and markerHeightMaxMM.
		/// </summary>
		/// <value>The marker scale.</value>
		[ShowInProductEditor]
		public float markerScale { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	markerColor  { get; set; }

		[JsonIgnore]
		private byte _markerBGAlpha = 168;

		[ShowInProductEditor]
		public byte markerBGAlpha {
			get {
				return _markerBGAlpha;
			}
			set {
				if (value < 0) {
					_markerBGAlpha = 0;
					return;
				}
				if (value > 255) {
					_markerBGAlpha = 255;
					return;
				}
				_markerBGAlpha = value;
			}
		}

		[ShowInProductEditor]
		public int mapButtonHeightUnits { get; set; }

		[ShowInProductEditor]
		public int mapButtonHeightMinMM { get; set; }

		[ShowInProductEditor]
		public int mapButtonHeightMaxMM { get; set; }


		[ShowInProductEditor (StartSection = "Categories & Filters:")]
		public bool foldableCategoryFilters { get; set; }

		[ShowInProductEditor]
		public bool categoryFiltersStartFolded { get; set; }

		[ShowInProductEditor]
		public bool categoryFoldersStartFolded { get; set; }

		/// <summary>
		/// Used as characterization of the quest infos, e.g. to determine the shown symbols in the foyer list.
		/// </summary>
		/// <value>The main category set.</value>
		[ShowInProductEditor]
		public string mainCategorySet { get; set; }

		public CategorySet GetMainCategorySet ()
		{
			return categorySets.Find (cat => cat.name == mainCategorySet);
		}

		[ShowInProductEditor]
		public List<CategorySet> categorySets {
			get {
				if (_categorySets == null) {
					_categorySets = new List<CategorySet> ();
				} 
				categoryDict = new Dictionary<string, Category> ();
				foreach (CategorySet cs in _categorySets) {
					foreach (Category c in cs.categories) {
						categoryDict [c.id] = c;
					}
				}
				return _categorySets;
			}
			set {
				_categorySets = value;
			}
		}

		[JsonIgnore] 
		private List<CategorySet> _categorySets;


		[JsonIgnore]
		public Dictionary<string, Category> categoryDict;

		#endregion


		#region Layout

		[ShowInProductEditor]
		public ImagePath topLogo { get; set; }

		[ShowInProductEditor (StartSection = "Layout & Colors:")]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	mainColor  { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	headerBgColor  { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	headerButtonBgColor  { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	headerButtonFgColor  { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	contentBackgroundColor  { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	contentFontColor  { get; set; }

		[ShowInProductEditor]
		public float 		headerHeightUnits { get; set; }

		[ShowInProductEditor]
		public float headerHeightMinMM { get; set; }

		[ShowInProductEditor]
		public float headerHeightMaxMM { get; set; }

		[ShowInProductEditor]
		public float 		contentHeightUnits { get; set; }

		[ShowInProductEditor]
		public float contentTopMarginUnits  { get; set; }

		[ShowInProductEditor]
		public float contentDividerUnits  { get; set; }

		[ShowInProductEditor]
		public float contentBottomMarginUnits  { get; set; }

		[ShowInProductEditor]
		public float imageAreaHeightMinUnits { get; set; }

		[ShowInProductEditor]
		public float imageAreaHeightMaxUnits { get; set; }

		[ShowInProductEditor]
		public bool fitExceedingImagesIntoArea { get; set; }

		[ShowInProductEditor]
		public float 		footerHeightUnits { get; set; }

		[ShowInProductEditor]
		public float footerHeightMinMM { get; set; }

		[ShowInProductEditor]
		public float footerHeightMaxMM { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	footerBgColor  { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	footerButtonBgColor  { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	footerButtonFgColor  { get; set; }

		/// <summary>
		/// The width of space on each side of the display that is intended to be left free (in permill of th absolute display width).
		/// </summary>
		/// <value>The side per mill.</value>
		[ShowInProductEditor]
		public float borderWidthUnits  { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	overlayButtonBgColor  { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	overlayButtonFgColor  { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	overlayButtonFgDisabledColor  { get; set; }

		[ShowInProductEditor (StartSection = "Menu:")]
		public bool showEmptyMenuEntries { get; set; }

		[ShowInProductEditor]
		public float menuEntryHeightUnits { get; set; }

		[ShowInProductEditor]
		public float menuEntryHeightMinMM { get; set; }

		[ShowInProductEditor]
		public float menuEntryHeightMaxMM { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	menuBGColor  { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	menuPartBGColor  { get; set; }

		[JsonIgnore]
		private float _disabledAlpha = 0.5f;

		[ShowInProductEditor]
		public float disabledAlpha {
			get {
				return _disabledAlpha;
			}
			set {
				if (value < 0f) {
					_disabledAlpha = 0f;
					return;
				}
				if (value > 1f) {
					_disabledAlpha = 1f;
					return;
				}
				_disabledAlpha = value;
			}
		}

		[ShowInProductEditor]
		public float listEntryHeightUnits { get; set; }

		[ShowInProductEditor]
		public float listEntryHeightMinMM { get; set; }

		[ShowInProductEditor]
		public float listEntryHeightMaxMM { get; set; }

		#endregion


		#region Defaults

		/// <summary>
		/// Initializes a new instance of the <see cref="GQ.Client.Conf.Config"/> class and intializes it with generic default values.
		/// 
		/// This constructor is used by the ProductManager (explicit) as well as the JSON.Net deserialize method (via reflection).
		/// </summary>
		public Config ()
		{			
			// set default values:
			autoStartQuestID = 0;
			autostartIsPredeployed = false;
			keepAutoStarting = true;
			questInfoViews = new string[] { QuestInfoView.List.ToString(), QuestInfoView.Map.ToString() };
			cloudQuestsVisible = true;
			showCloudQuestsImmediately = false;
			downloadAllCloudQuestOnStart = false;
			localQuestsDeletable = true;
			hideHiddenQuests = false;
			DownloadStrategy = DownloadStrategy.UPFRONT;
			timeoutMS = 10000L;

			acceptedPageTypes = new string[0];
			sceneMappings = new List<SceneMapping> ();
			scenePaths = new string[0];
			sceneExtensions = new List<SceneExtension> ();

			// Map:
			mapProvider = MapProvider.OpenStreetMap;
			mapBaseUrl = "http://a.tile.openstreetmap.org";
			mapKey = "";
			mapID = "";
			mapTileImageExtension = ".png";
//			mapKey = "pk.eyJ1IjoiaG9sZ2VybXVlZ2dlIiwiYSI6Im1MLW9rN2MifQ.6KebeI6zZ3QNe18n2AQyaw";
//			mapID = "mapbox.streets";
//			mapTileImageExtension = "@2x.png?access_token=" + mapKey;
			useMapOffline = false;
			mapMinimalZoom = 7.0f;
			mapDeltaZoom = 0.5f;
			markerScale = 1.0f;


			// Layout:
			headerHeightUnits = 60f;
			contentHeightUnits = 750f;
			imageAreaHeightMinUnits = 150f;
			imageAreaHeightMaxUnits = 350f;
			footerHeightUnits = 75f;
			contentDividerUnits = 0;
			borderWidthUnits = 0;
			headerBgColor = Color.white;
			headerButtonBgColor = GQColor.transparent;
			headerButtonFgColor = Color.black;
			contentBackgroundColor = Color.white;
			contentFontColor = Color.black;
			footerBgColor = Color.white;
			footerButtonBgColor = GQColor.transparent;
			footerButtonFgColor = Color.black;
			overlayButtonBgColor = GQColor.transparent;
			overlayButtonFgColor = Color.black;
			overlayButtonFgDisabledColor = new Color (159f, 159f, 159f, 187f);


			// Menu:
			showEmptyMenuEntries = false;
			categoryDict = new Dictionary<string, Category> ();
			foldableCategoryFilters = true;
			categoryFiltersStartFolded = true;
			categoryFoldersStartFolded = true;
		}

		#endregion

	}


	public enum DownloadStrategy
	{
		UPFRONT,
		LAZY,
		BACKGROUND
	}

	public enum QuestInfoView {
		List,
		Map
	}

	public enum PageType
	{
		StartAndExitScreen,
		NPCTalk,
		ImageWithText,
		Menu,
		MultipleChoiceQuestion,
		TextQuestion,
		AudioRecord,
		ImageCapture,
		VideoPlay,
		WebPage,
		MapOSM,
		Navigation,
		TagScanner,
		ReadNFC,
		Custom,
		MetaData
	}

	public enum MapProvider
	{
		OpenStreetMap,
		MapBox
	}

	public class Color32Converter : JsonConverter
	{
		public override bool CanConvert (Type objectType)
		{
			return objectType == typeof(Color32);
		}

		public override object ReadJson (JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			reader.Read ();
			byte r = (byte)reader.ReadAsInt32 ();
			reader.Read ();
			byte g = (byte)reader.ReadAsInt32 ();
			reader.Read ();
			byte b = (byte)reader.ReadAsInt32 ();
			reader.Read ();
			byte a = (byte)reader.ReadAsInt32 ();
			reader.Read ();

			Color32 c = new Color32 (r, g, b, a);
			return c;
		}

		public override void WriteJson (JsonWriter writer, object value, JsonSerializer serializer)
		{
			Color32 c = (Color32)value;
			writer.WriteStartObject ();
			writer.WritePropertyName ("r");
			writer.WriteValue (c.r);
			writer.WritePropertyName ("g");
			writer.WriteValue (c.g);
			writer.WritePropertyName ("b");
			writer.WriteValue (c.b);
			writer.WritePropertyName ("a");
			writer.WriteValue (c.a);
			writer.WriteEndObject ();
		}
	}

	public static class GQColor
	{

		public static readonly Color32 transparent = new Color32 (255, 255, 255, 0);
	}

	public struct SceneMapping
	{

		public const string PageSceneAssetPathRoot = "Assets/Scenes/Pages/";

		public SceneMapping (string pageType, string scenePath)
		{
			this.pageTypeName = pageType;
			this.scenePath = scenePath;
		}

		/// <summary>
		/// Pages of this type will use the scene given by the scenePath.
		/// </summary>
		public string pageTypeName;

		/// <summary>
		/// Path of the scene that is used for the given page type.
		/// </summary>
		public string scenePath;
	}

	public class SceneExtension
	{
		/// <summary>
		/// The scene path.
		/// </summary>
		public string scene;

		/// <summary>
		/// The path to gameobject where the prefab should be instantiated.
		/// </summary>
		public string root;

		/// <summary>
		/// The path to the prefab. It is relative to the folder Assets/ConfigAssets/Resources folder.
		/// </summary>
		public string prefab;
	}

	public class ImagePath
	{
		public string path;

		public ImagePath (string path)
		{
			this.path = path;
		}

		public override string ToString ()
		{
			return "ImagePath: " + path;
		}

		public override bool Equals (System.Object other)
		{
			// Other null?
			if (other == null)
				return path == null || path.Equals ("");

			// Compare run-time types.
			if (GetType () != other.GetType ())
				return false;

			return path == ((ImagePath)other).path;
		}

		public override int GetHashCode ()
		{
			return path.GetHashCode ();
		}
	}

	public class Category
	{
		public string id;

		/// <summary>
		/// The display name.
		/// </summary>
		public string name;

		public string folderName;

		public ImagePath symbol;

		public Category ()
		{
			this.id = "";
			name = "";
			folderName = "";
			symbol = null;
		}

		[JsonConstructor]
		public Category (string id, string name, string folderName, string symbolPath)
		{
			this.id = id;
			this.name = name;
			this.folderName = folderName == null ? "" : folderName;
			this.symbol = new ImagePath (symbolPath);
		}

	}

	public class CategorySet
	{
		public string name;

		public List<Category> categories;

		[JsonConstructor]
		public CategorySet (string name, List<Category> categories)
		{
			this.name = name;
			if (categories == null)
				categories = new List<Category> ();
			this.categories = categories;
		}

		public CategorySet ()
		{
			name = "";
			categories = new List<Category> ();
		}
	}

	public class ShowInProductEditor : Attribute
	{
		public string StartSection { get; set; }
	}
}


