using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using GQ.Client.Model;
using System.IO;
using GQ.Client.Err;
using System.Collections.Generic;
using GQ.Client.Conf;

namespace GQ.Client.Model
{

	public class QuestManager
	{

		#region quest management functions

		public Quest CurrentQuest { get; set; }

		public Page CurrentPage { get; set; }


		private static Quest currentlyParsingQuest;

		public static Quest CurrentlyParsingQuest {
			get {
				if (currentlyParsingQuest == null)
					currentlyParsingQuest = Quest.Null;
				return currentlyParsingQuest;
			}
			set {
				currentlyParsingQuest = value;
			}
		}
			
		/// <summary>
		/// Reads the quest from its game.xml file and creates a complete model hierarchy in memory and 
		/// returns its root the quest object.
		/// 
		/// TODO move this to Quest as a static function?
		/// </summary>
		/// <returns>The quest model object.</returns>
		/// <param name="xml">Xml.</param>
		public Quest DeserializeQuest (string xml)
		{
			// Creates an instance of the XmlSerializer class;
			// specifies the type of object to be deserialized.
			XmlSerializer serializer = new XmlSerializer (typeof(Quest));

			// If the XML document has been altered with unknown 
			// nodes or attributes, handles them with the 
			// UnknownNode and UnknownAttribute events.
			serializer.UnknownNode += new 
				XmlNodeEventHandler (serializer_UnknownNode);
			serializer.UnknownAttribute += new 
				XmlAttributeEventHandler (serializer_UnknownAttribute);

			Quest quest;

			using (TextReader reader = new StringReader (xml)) {
				quest = (Quest)serializer.Deserialize (reader);
			}

			return quest;
		}

		#endregion


		#region singleton

		public static QuestManager Instance {
			get {
				if (_instance == null) {
					_instance = new QuestManager ();
				} 
				return _instance;
			}
			set {
				_instance = value;
			}
		}

		public static void Reset ()
		{
			_instance = null;
		}

		private static QuestManager _instance = null;

		private QuestManager ()
		{
			CurrentQuest = Quest.Null;
			CurrentPage = Page.Null;
		}

		#endregion


		#region Quest Access

		public static string GetQuestURI(int questID) {
			string uri = string.Format ("{0}/editor/{1}/clientxml",
				             ConfigurationManager.GQ_SERVER_BASE_URL,
				             questID
			             );
			return uri;
		}

		/// <summary>
		/// Gets the local quest dir path.
		/// </summary>
		/// <returns>The local quest dir path.</returns>
		/// <param name="questID">Quest I.</param>
		public static string GetLocalQuestDirPath(int questID) {
			return Application.persistentDataPath + "/quests/" + questID + "/";
		}

		public const string QUEST_FILE_NAME = "game.xml";

		#endregion


		#region Parsing

		protected void serializer_UnknownNode
		(object sender, XmlNodeEventArgs e)
		{
			Log.SignalErrorToDeveloper ("Unknown XML Node found in Quest XML:" + e.Name + "\t" + e.Text);
		}

		protected void serializer_UnknownAttribute
		(object sender, XmlAttributeEventArgs e)
		{
			System.Xml.XmlAttribute attr = e.Attr;
			Log.SignalErrorToDeveloper ("Unknown XML Attribute found in Quest XML:" +
			attr.Name + "='" + attr.Value + "'");
		}

		#endregion
	}

	public class MediaInfo {

		string url;
		public string Url {
			get {
				return url;
			}
			private set {
				url = value;
			}
		}


		string localPath;
		public string LocalPath {
			get {
				return localPath;
			}
			set {
				localPath = value;
			}
		}


		long localTimestamp;
		public long LocalTimestamp {
			get {
				return localTimestamp;
			}
			set {
				localTimestamp = value;
			}
		}

		long remoteTimestamp;
		public long RemoteTimestamp {
			get {
				return remoteTimestamp;
			}
			set {
				remoteTimestamp = value;
			}
		}

		public const long NOT_AVAILABLE = -1L;
		public const long UNKNOWN = -2L;

		long localSize;
		public long LocalSize {
			get {
				return localSize;
			}
			set {
				localSize = value;
			}
		}

		long remoteSize;
		public long RemoteSize {
			get {
				return remoteSize;
			}
			set {
				remoteSize = value;
			}
		}


		public MediaInfo(string url) {
			this.Url = url;
			this.LocalTimestamp = 0L;
			this.LocalSize = NOT_AVAILABLE;
			this.RemoteTimestamp = 0L;
			this.RemoteSize = UNKNOWN;
		}

		public bool IsLocallyAvailable {
			get {
				return !(LocalSize == NOT_AVAILABLE);
			}
		}
	}

	/// <summary>
	/// Media Info about the local game media that is persisted in JSON file game-media.json in the quest folder.
	/// </summary>
	public struct LocalMediaInfo {
		public string url;
		public string path;
		public long size;
		public long time;

		public LocalMediaInfo(string url, string path, long size, long time) {
			this.url = url;
			this.path = path;
			this.size = size;
			this.time = time;
		}
	}
}
