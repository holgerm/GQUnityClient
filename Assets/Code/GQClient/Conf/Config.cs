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
		public int   	downloadTimeOutSeconds   { get; set; }

		public string 	nameForQuest { get; set; }

		public string 	questVisualization { get; set; }

		public bool 	questVisualizationChangeable  { get; set; }

		public bool 	cloudQuestsVisible  { get; set; }

		public bool 	showCloudQuestsImmediately  { get; set; }

		public bool 	downloadAllCloudQuestOnStart  { get; set; }

		public bool 	localQuestsDeletable  { get; set; }

		public bool 	hideHiddenQuests  { get; set; }

		public bool 	hasMenuWithinQuests  { get; set; }

		[ShowInProductEditor(StartSection="Pages & Scenes:")]
		public string[]	acceptedPageTypes { get; set; }

		public Dictionary<string, string> GetSceneMappingsDict () { 
			Dictionary<string, string> smDict = new Dictionary<string, string> ();
			foreach (SceneMapping sm in sceneMappings) {
				smDict.Add (sm.pageTypeName, sm.scenePath);
			}
			return smDict;
		}

		[ShowInProductEditor]
		public List<SceneMapping> sceneMappings { get; set; }

		[ShowInProductEditor]
		public string[]	scenePaths { get; set; }

		[ShowInProductEditor]
		public List<SceneExtension> sceneExtensions { get; set; }


		#region Map

		[ShowInProductEditor(StartSection="Map & Markers:")]
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

		[JsonIgnore]
		private List<Category> _categories;

		[ShowInProductEditor]
		public List<Category> categories { 
			get {
				return _categories;
			}
			set {
				_categories = value;
				categoryDict = new Dictionary<string, Category> ();
				if (value != null)
					foreach (Category c in value) {
						categoryDict.Add (c.id, c);
					}
			} 
		}

		[JsonIgnore]
		public Dictionary<string, Category> categoryDict;

		#endregion


		#region Layout

		[ShowInProductEditor(StartSection="Layout & Colors:")]
		public int 		headerHeightPermill { get; set; }

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
		public ImagePath topLogo { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	contentBackgroundColor  { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	contentFontColor  { get; set; }

		[ShowInProductEditor]
		public int 		footerHeightPermill { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	footerBgColor  { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	footerButtonBgColor  { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	footerButtonFgColor  { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	overlayButtonBgColor  { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	overlayButtonFgColor  { get; set; }

		[ShowInProductEditor]
		[JsonConverter (typeof(Color32Converter))]		
		public Color32	overlayButtonFgDisabledColor  { get; set; }

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
			questVisualization = "list";
			cloudQuestsVisible = true;
			showCloudQuestsImmediately = false;
			downloadAllCloudQuestOnStart = false;
			localQuestsDeletable = true;
			hideHiddenQuests = false;
			hasMenuWithinQuests = true;
			DownloadStrategy = DownloadStrategy.UPFRONT;
			downloadTimeOutSeconds = 300;

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



			// Layout:
			headerHeightPermill = 50;
			headerBgColor = Color.white;
			headerButtonBgColor = GQColor.transparent;
			headerButtonFgColor = Color.black;
			contentBackgroundColor = Color.white;
			contentFontColor = Color.black;
			footerHeightPermill = 75;
			footerBgColor = Color.white;
			footerButtonBgColor = GQColor.transparent;
			footerButtonFgColor = Color.black;
			overlayButtonBgColor = GQColor.transparent;
			overlayButtonFgColor = Color.black;
			overlayButtonFgDisabledColor = new Color (159f, 159f, 159f, 187f);
		}

		#endregion

	}


	public enum DownloadStrategy
	{
		UPFRONT,
		LAZY,
		BACKGROUND
	}

	public enum PageType {
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

	public struct SceneMapping {

		public const string PageSceneAssetPathRoot = "Assets/Scenes/Pages/";

		public SceneMapping(string pageType, string scenePath) {
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

	public struct SceneExtension
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

		public override string ToString() {
			return "ImagePath: " + path;
		}

		public override bool Equals(System.Object other) 
		{
			// Other null?
			if (other == null)
				return path == null || path.Equals ("");

			// Compare run-time types.
			if (GetType() != other.GetType()) 
				return false;

			return path == ((ImagePath)other).path;
		}
		public override int GetHashCode() 
		{
			return path.GetHashCode();
		}
	}

	public struct Category
	{
		public string id;

		/// <summary>
		/// The display name.
		/// </summary>
		public string name;

		public ImagePath symbol;

	}

	public class ShowInProductEditor : Attribute
	{	
		public string StartSection { get; set; }
	}
}


