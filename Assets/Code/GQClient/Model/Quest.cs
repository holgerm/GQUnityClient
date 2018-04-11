using UnityEngine;
using Newtonsoft.Json;
using GQ.Client.FileIO;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System;
using GQ.Client.Conf;
using System.Collections.Generic;
using GQ.Client.Err;
using System.IO;
using System.Xml;
using GQ.Client.Util;

namespace GQ.Client.Model
{

	/// <summary>
	/// The root object of a quests model at runtime. It represents all details of the quest at runtime.
	/// </summary>
	[System.Serializable]
	[XmlRoot (GQML.QUEST)]
	public class Quest  : IComparable<Quest>, IXmlSerializable
	{

		#region Attributes
		public string Name { get; set; }
		public int Id { get; set; }
		public long LastUpdate { get; set; }
		public string XmlFormat { get; set; }
		public bool IndividualReturnDefinitions { get; set; }

		public bool IsHidden {
			get {
				// TODO change the latter two checks to test a flag stored in game.xml base element as an attribute and move to QuestInfo
				return (ConfigurationManager.Current.hideHiddenQuests && Name != null && Name.StartsWith ("---"));
			}
		}
		#endregion

		#region State Pages
		protected Dictionary<int, IPage> pageDict = new Dictionary<int, IPage> ();

		public IPage GetPageWithID (int id)
		{
			IPage page;
			if (pageDict.TryGetValue (id, out page)) {
				return page;
			} else {
				return Page.Null;
			}
		}

		protected IPage startPage;

		public IPage StartPage {
			get {
				if (startPage == null) {
					Log.SignalErrorToDeveloper ("Quest {0}({1}) can not be started, since start page is not set.", Name, Id);
					return null;
				} else
					return startPage;
			}
			set {
				startPage = value;
			}
		}

		protected IPage currentPage;

		public IPage CurrentPage {
			get {
				return currentPage;
			}
			internal set {
				currentPage = value;
			}
		}
		#endregion

		#region Hotspots
		protected Dictionary<int, Hotspot> hotspotDict = new Dictionary<int, Hotspot> ();

		public Hotspot GetHotspotWithID (int id)
		{
			Hotspot hotspot;
			hotspotDict.TryGetValue (id, out hotspot);
			return hotspot;
		}

		public Dictionary<int, Hotspot>.ValueCollection AllHotspots {
			get {
				return hotspotDict.Values;
			}
		}
		#endregion


		#region Metadata
		public Dictionary<string, string> metadata = new Dictionary<string, string> ();
		#endregion


		#region Media
		public Dictionary<string, MediaInfo> MediaStore = new Dictionary<string, MediaInfo> ();

		private void initMediaStore ()
		{
			MediaStore = new Dictionary<string, MediaInfo> ();

			string mediaJSON = "";
			try {
				mediaJSON = File.ReadAllText (MediaJsonPath);
			} catch (FileNotFoundException) {
				mediaJSON = @"[]"; // we use an empty list then
			} catch (Exception e) {
				Log.SignalErrorToDeveloper ("Error reading media.json for quest " + Id + ": " + e.Message);
				mediaJSON = @"[]"; // we use an empty list then
			}

			List<LocalMediaInfo> localInfos = JsonConvert.DeserializeObject<List<LocalMediaInfo>> (mediaJSON);

			foreach (LocalMediaInfo localInfo in localInfos) {
				MediaInfo info = new MediaInfo (localInfo);
				MediaStore.Add (info.Url, info);
			}
		}

		public void AddMedia (string url)
		{
			if (!MediaStore.ContainsKey (url)) {
				MediaInfo info = new MediaInfo (Id, url);
				MediaStore.Add (url, info);
			}
		}

		public string MediaJsonPath {
			get {
				return Files.CombinePath (QuestManager.GetLocalPath4Quest (Id), "media.json");
			}
		}
		#endregion


		#region XML Reading
		public System.Xml.Schema.XmlSchema GetSchema ()
		{
			return null;
		}

		/// <summary>
		/// This method should only be called from the XML Serialization Framework. 
		/// It will be indirectly used by the QuestManager method DeserializeQuest().
		/// </summary>
		/// <returns>The xml.</returns>
		/// <param name="reader">Reader.</param>
		public void ReadXml (System.Xml.XmlReader reader)
		{
			QuestManager.CurrentlyParsingQuest = this; // TODO use event system instead

			// proceed to quest start element:
			while (!GQML.IsReaderAtStart (reader, GQML.QUEST)) {
				reader.Read ();
			}

			// we need the id first, because it is used in createing the media store...
			Id = GQML.GetIntAttribute (GQML.QUEST_ID, reader);

			// set up the media store, depends on the id of the quest for paths:
			initMediaStore ();

			// read all further attributes
			ReadFurtherAttributes (reader);

			// Start the xml content: consume the begin quest element:
			reader.Read ();

			// Content:
			XmlRootAttribute xmlRootAttr = new XmlRootAttribute ();
			xmlRootAttr.IsNullable = true;

			while (!GQML.IsReaderAtEnd (reader, GQML.QUEST)) {

				if (reader.NodeType != XmlNodeType.Element && !reader.Read ()) {
					return;
				}

				// now we are at an element:
				switch (reader.LocalName) {
				case GQML.PAGE:
					ReadPage (reader);
					break;
				case GQML.HOTSPOT:
					ReadHotspot (reader);
					break;
				}
			}

			// we are done with this quest:
			QuestManager.CurrentlyParsingQuest = Null;
		}

		private void ReadFurtherAttributes (XmlReader reader)
		{
			Name = GQML.GetStringAttribute (GQML.QUEST_NAME, reader);
			XmlFormat = GQML.GetStringAttribute (GQML.QUEST_XMLFORMAT, reader);
			LastUpdate = GQML.GetLongAttribute (GQML.QUEST_LASTUPDATE, reader);
			IndividualReturnDefinitions = GQML.GetOptionalBoolAttribute (GQML.QUEST_INDIVIDUAL_RETURN_DEFINITIONS, reader);
		}

		private void ReadPage (XmlReader reader)
		{
			// now the reader is at a page element:
			string pageTypeName = reader.GetAttribute (GQML.PAGE_TYPE);
			if (pageTypeName == null) {
				Log.SignalErrorToDeveloper ("Page without type attribute found.");
				reader.Skip ();
				return;
			}

			// Determine the full name of the according page type (e.g. GQ.Client.Model.XML.PageNPCTalk) 
			//		where SetVariable is taken form ath type attribute of the xml action element.
			string __myTypeName = this.GetType ().FullName;
			int lastDotIndex = __myTypeName.LastIndexOf (".");
			string modelNamespace = __myTypeName.Substring (0, lastDotIndex);
			string targetScenePath = null;
			// TODO: Implement page2scene mapping here:
			Dictionary<string, string> sceneMappings = ConfigurationManager.Current.GetSceneMappingsDict ();
			if (sceneMappings.TryGetValue (pageTypeName, out targetScenePath)) {
				pageTypeName = targetScenePath.Substring (
					targetScenePath.LastIndexOf ("/") + 1, 
					targetScenePath.Length - (targetScenePath.LastIndexOf ("/") + 1 + ".unity".Length)
				);
			} 
			pageTypeName = string.Format ("{0}.Page{1}", modelNamespace, pageTypeName);
			Type pageType = Type.GetType (pageTypeName);

			if (pageType == null) {
				Log.SignalErrorToDeveloper ("No Implementation for Page Type {0}.", pageTypeName);
				reader.Skip ();
				return;
			}

			XmlSerializer serializer = new XmlSerializer (pageType);
			IPage page = (IPage)serializer.Deserialize (reader);
			page.Parent = this;
			if (pageDict.Count == 0)
				StartPage = page;
			if (pageDict.ContainsKey (page.Id)) {
				pageDict.Remove (page.Id);
			}
			try {
				pageDict.Add (page.Id, page);
			} catch (Exception e) {
				Debug.LogWarning ((e.Message + " id: " + page.Id).Yellow ());
			}
		}

		private void ReadHotspot (XmlReader reader)
		{
			XmlSerializer serializer = new XmlSerializer (typeof(Hotspot));
			Hotspot hotspot = (Hotspot)serializer.Deserialize (reader);
			hotspotDict.Add (hotspot.Id, hotspot);
		}


		public void WriteXml (System.Xml.XmlWriter writer)
		{
			Debug.LogWarning ("WriteXML not implemented for " + GetType ().Name);
		}
		#endregion


		#region Runtime API
		public virtual void Start ()
		{
			if (StartPage == null) {
				Log.SignalErrorToDeveloper (
					"Quest {0} can not be started, since the StartPage is null",
					Id
				);
				return;
			}

			Variables.SetVariableValue ("quest.name", new Value (Name));

			QuestManager.Instance.CurrentPage = null;

			StartPage.Start ();
		}

		public void End ()
		{
			Audio.Clear ();
			Variables.ClearAll (); // persistente variablen nicht löschen
			SceneManager.UnloadSceneAsync (QuestManager.Instance.CurrentScene);
			Base.Instance.ShowFoyerCanvases ();
			Resources.UnloadUnusedAssets ();
		}

		public void GoBackOnePage ()
		{
			// TODO was is an OLD implementation!

//			Page show = previouspages [previouspages.Count - 1];
//			previouspages.Remove (previouspages [previouspages.Count - 1]);
//
//			if (_allowReturn > 0)
//				_allowReturn--;
//
//			questdatabase questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
//			questdb.changePage (show.Id);
//
		}

		[SerializeField]
		//		private int _allowReturn = 0;

		//		public bool AllowReturn {
		//			get {
		//				if (IndividualReturnDefinitions) {
		//					return (
		//					    _allowReturn > 0
		//					    && previouspages.Count > 0
		//					    && previouspages [previouspages.Count - 1] != null
		//					);
		//				} else {
		//					return (
		//					    previouspages.Count > 0
		//					    && previouspages [previouspages.Count - 1] != null
		//					    && !previouspages [previouspages.Count - 1].type.Equals (GQML.PAGE_TYPE_TEXT_QUESTION)
		//					    && !previouspages [previouspages.Count - 1].type.Equals (GQML.PAGE_TYPE_MULTIPLE_CHOICE_QUESTION)
		//					);
		//				}
		//			}
		//			set {
		//				if (value == false)
		//					_allowReturn = 0;
		//				else
		//					_allowReturn++;
		//			}
		//		}

		public bool UsesLocation {
			get {
				return hotspotDict.Count > 0;
				// TODO check whether this quest contains map page or uses location system variables
			}
		}

		public bool SendsDataToServer {
			get {
				return false;
				// TODO check whether this quest contains map page or uses location system variables
			}
		}

		#endregion


		#region Null Object

		public static readonly Quest Null = new NullQuest ();

		private class NullQuest : Quest
		{

			public NullQuest ()
				: base ()
			{
				Name = "Null Quest";
				Id = 0;
				LastUpdate = 0;
				XmlFormat = "0";
				IndividualReturnDefinitions = false;
			}

			public override void Start ()
			{
				Log.WarnDeveloper ("Null Quest started.");
			}
		}

		#endregion


		public int CompareTo (Quest q)
		{
			// TODO this is old. Should we change it?

			if (q == null) {
				return 1;
			} else {

				return this.Name.ToUpper ().CompareTo (q.Name.ToUpper ());
			}

		}

	}

}

