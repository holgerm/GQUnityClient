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
		
		public string   id     { get; set; }

		public string   name   { get; set; }

		public int   	portal   { get; set; }

		public int   	autoStartQuestID   { get; set; }

		public bool 	autostartIsPredeployed  { get; set; }

		public bool 	keepAutoStarting  { get; set; }

		[JsonConverter (typeof(StringEnumConverter))]
		public DownloadStrategy DownloadStrategy { get; set; }

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

		public bool 	showTextInLoadingLogo  { get; set; }

		public bool 	showNetConnectionWarning  { get; set; }

		public List<SceneExtension> sceneExtensions { get; set; }


		#region Map

		[JsonConverter (typeof(StringEnumConverter))]
		public MapProvider 	mapProvider { get; set; }

		public string 	mapBaseUrl { get; set; }

		public string 	mapKey { get; set; }

		public string 	mapID { get; set; }

		public string 	mapTileImageExtension { get; set; }

		public bool		useMapOffline { get; set; }

		public float	mapMinimalZoom { get; set; }

		public float	mapDeltaZoom { get; set; }

		public ImagePath marker { get; set; }

		[JsonIgnore]
		private List<Category> _categories;

		public List<Category> categories { 
			get {
				return _categories;
			}
			set {
				_categories = value;
				Debug.Log (("Setting Categories: #" + value.Count).Yellow());
				categoryDict = new Dictionary<string, Category> ();
				foreach (Category c in value) {
					categoryDict.Add (c.id, c);
				}
			} 
		}

		[JsonIgnore]
		public Dictionary<string, Category> categoryDict;

		#endregion


		#region Layout

		public int 		headerHeightPermill { get; set; }

		[JsonConverter (typeof(ColorConverter))]		
		public Color	headerBgColor  { get; set; }

		[JsonConverter (typeof(ColorConverter))]		
		public Color	headerButtonBgColor  { get; set; }

		[JsonConverter (typeof(ColorConverter))]		
		public Color	headerButtonFgColor  { get; set; }

		public ImagePath topLogo { get; set; }

		[JsonConverter (typeof(ColorConverter))]		
		public Color	contentBackgroundColor  { get; set; }

		[JsonConverter (typeof(ColorConverter))]		
		public Color	contentFontColor  { get; set; }

		public int 		footerHeightPermill { get; set; }

		[JsonConverter (typeof(ColorConverter))]		
		public Color	footerBgColor  { get; set; }

		[JsonConverter (typeof(ColorConverter))]		
		public Color	footerButtonBgColor  { get; set; }

		[JsonConverter (typeof(ColorConverter))]		
		public Color	footerButtonFgColor  { get; set; }

		[JsonConverter (typeof(ColorConverter))]		
		public Color	overlayButtonBgColor  { get; set; }

		[JsonConverter (typeof(ColorConverter))]		
		public Color	overlayButtonFgColor  { get; set; }

		[JsonConverter (typeof(ColorConverter))]		
		public Color	overlayButtonFgDisabledColor  { get; set; }

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
			showNetConnectionWarning = true;
			showTextInLoadingLogo = true;

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

	public enum MapProvider
	{
		OpenStreetMap,
		MapBox
	}

	public class ColorConverter : JsonConverter
	{
		public override bool CanConvert (Type objectType)
		{
			return objectType == typeof(Color);
		}

		public override object ReadJson (JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			reader.Read ();
			float r = (float)reader.ReadAsDouble ();
			reader.Read ();
			float g = (float)reader.ReadAsDouble ();
			reader.Read ();
			float b = (float)reader.ReadAsDouble ();
			reader.Read ();
			float a = (float)reader.ReadAsDouble ();
			reader.Read ();

			Color c = new Color (r, g, b, a);
			return c;
		}

		public override void WriteJson (JsonWriter writer, object value, JsonSerializer serializer)
		{
			Color c = (Color)value;
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

		public static readonly Color transparent = new Color (1f, 1f, 1f, 0f);
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

	public struct ImagePath
	{
		public string path;

		public ImagePath (string path)
		{
			this.path = path;
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
}


