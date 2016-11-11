using System.Collections.Generic;

namespace GQ.Client.Conf {
	/// <summary>
	/// Config class specifies textual parameters of a product. It is used both at runtime to initilize the app's branding details from and 
	/// at editor time to back the product editor view and store the parameters while we use the editor.
	/// </summary>
	public class Config {
		//////////////////////////////////
		// THE ACTUAL PRODUCT CONFIG DATA:	
		
		public string   id     { get; set; }

		public string   name   { get; set; }

		public int   	portal   { get; set; }

		public int   	autoStartQuestID   { get; set; }

		public bool 	autostartIsPredeployed  { get; set; }

		public int   	downloadTimeOutSeconds   { get; set; }

		public string 	nameForQuest { get; set; }

		public string 	questVisualization { get; set; }

		public bool 	questVisualizationChangeable  { get; set; }

		public bool 	showCloudQuestsImmediately  { get; set; }

		public bool 	showTextInLoadingLogo  { get; set; }

		public bool 	showNetConnectionWarning  { get; set; }

		public string 	colorProfile { get; set; }

		public string 	mapboxKey { get; set; }

		public string 	mapboxMapID { get; set; }

		public bool		useMapOffline { get; set; }

		public string 	imprint  { get; set; }

		public List<CategoryInfo> markers { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GQ.Client.Conf.Config"/> class and intializes it with generic default values.
		/// 
		/// This constructor is used by the ProductManager (explicit) as well as the JSON.Net deserialize method (via reflection).
		/// </summary>
		public Config () {			
			// set default values:
			mapboxKey = "pk.eyJ1IjoiaG9sZ2VybXVlZ2dlIiwiYSI6Im1MLW9rN2MifQ.6KebeI6zZ3QNe18n2AQyaw";
			mapboxMapID = "mapbox.streets";
			questVisualization = "list";
			// prepare markers as an empty list:
			markers = new List<CategoryInfo>();
			useMapOffline = false;
			questVisualization = "list";
		}

	}


}


