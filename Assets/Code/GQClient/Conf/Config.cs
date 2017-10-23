using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using GQ.Client.Err;

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

		// TODO change to string
		public DownloadStrategy downloadStrategy { get; set; }

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

		public string 	colorProfile { get; set; }

		public string 	mapboxKey { get; set; }

		public string 	mapboxMapID { get; set; }

		public bool		useMapOffline { get; set; }

		public float	mapMinimalZoom { get; set; }

		public List<CategoryInfo> markers { get; set; }


		#region Layout

		public int 		headerHeightPermill { get; set; }

		[JsonConverter(typeof(ColorConverter))]		
		public Color	headerBgColor  { get; set; }

		[JsonConverter(typeof(ColorConverter))]		
		public Color	headerButtonBgColor  { get; set; }

		[JsonConverter(typeof(ColorConverter))]		
		public Color	headerButtonFgColor  { get; set; }

		[JsonConverter(typeof(ColorConverter))]		
		public Color	contentBackgroundColor  { get; set; }

		[JsonConverter(typeof(ColorConverter))]		
		public Color	contentFontColor  { get; set; }

		public int 		footerHeightPermill { get; set; }

		[JsonConverter(typeof(ColorConverter))]		
		public Color	footerBgColor  { get; set; }

		[JsonConverter(typeof(ColorConverter))]		
		public Color	footerButtonBgColor  { get; set; }

		[JsonConverter(typeof(ColorConverter))]		
		public Color	footerButtonFgColor  { get; set; }

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
			mapboxKey = "pk.eyJ1IjoiaG9sZ2VybXVlZ2dlIiwiYSI6Im1MLW9rN2MifQ.6KebeI6zZ3QNe18n2AQyaw";
			mapboxMapID = "mapbox.streets";
			questVisualization = "list";
			markers = new List<CategoryInfo> ();
			useMapOffline = false;
			mapMinimalZoom = 7.0f;
			questVisualization = "list";
			cloudQuestsVisible = true;
			showCloudQuestsImmediately = false;
			downloadAllCloudQuestOnStart = false;
			localQuestsDeletable = true;
			hideHiddenQuests = false;
			hasMenuWithinQuests = true;
			downloadStrategy = DownloadStrategy.UPFRONT;
			downloadTimeOutSeconds = 300;
			showNetConnectionWarning = true;
			showTextInLoadingLogo = true;
			colorProfile = "default";
			headerButtonBgColor = Color.white;
			contentBackgroundColor = Color.white;
			contentFontColor = Color.black;

			// Layout:
			headerHeightPermill = 50;
			headerBgColor = Color.white;
			footerHeightPermill = 75;
			footerBgColor = Color.white;
			headerButtonBgColor = GQColor.transparent;
			headerButtonFgColor = Color.black;
		}

		#endregion

	}


	public enum DownloadStrategy {
		UPFRONT,
		LAZY,
		BACKGROUND
	}


	public class ColorConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Color);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			reader.Read ();
			float r = (float)reader.ReadAsDouble();
			reader.Read ();
			float g = (float) reader.ReadAsDouble ();
			reader.Read ();
			float b = (float) reader.ReadAsDouble ();
			reader.Read ();
			float a = (float) reader.ReadAsDouble ();
			reader.Read ();

			Color c = new Color (r, g, b, a);
			return c;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
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

	public static class GQColor {

		public static readonly Color transparent = new Color(1f, 1f, 1f, 0f);
	}
}


