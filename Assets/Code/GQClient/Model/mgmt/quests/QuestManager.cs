using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using GQ.Client.Model;
using System.IO;
using GQ.Client.Err;
using System.Collections.Generic;
using GQ.Client.Conf;
using GQ.Client.FileIO;
using Newtonsoft.Json;
using System;
using GQ.Client.Util;
using UnityEngine.SceneManagement;

namespace GQ.Client.Model
{

	public class QuestManager
	{

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


		#region quest management functions

		public Quest CurrentQuest { get; set; }

		public void StartQuest (int id)
		{
			// TODO
		}

		public Page CurrentPage { get; set; }

		public Scene CurrentScene { 
			get { 
				Scene curScene = SceneManager.GetSceneByName (CurrentPage.GetType ().Name.Substring (4)); 
				return curScene;
			} 
		}

		#endregion


		#region Quest Access

		public static string GetQuestURI (int questID)
		{
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
		public static string GetLocalPath4Quest (int questID)
		{
			return QuestInfoManager.LocalQuestsPath + questID + "/";
		}

		public const string QUEST_FILE_NAME = "game.xml";

		/// <summary>
		/// Makes the local file name from the given URL, 
		/// so that the file name is unique and reflects the filename within the url.
		/// </summary>
		/// <returns>The local file name from URL.</returns>
		/// <param name="url">URL.</param>
		public static string MakeLocalFileNameFromUrl (string url)
		{
			string filename = Files.FileName (url);


			return filename; // TODO
		}

		public string CurrentMediaJSONPath {
			get {
				return GetLocalPath4Quest (CurrentQuest.Id) + "/media.json";
			}
		}

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
		/// store its root the quest object as CurrentQuest.
		/// 
		/// This is step 1 of 4 in media sync (download or update of a quest).
		///
		/// </summary>
		/// <param name="xml">Xml.</param>
		public void DeserializeQuest (string xml)
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

			using (TextReader reader = new StringReader (xml)) {
				CurrentQuest = (Quest)serializer.Deserialize (reader);
			}
		}


		/// <summary>
		/// Imports the local media infos fomr the game-media.json file and updates the existing media store. 
		/// This is step 2 of 4 in media sync (download or update of a quest).
		/// </summary>
		public void ImportLocalMediaInfo ()
		{
			string mediaJSON = "";
			try {
				mediaJSON = File.ReadAllText (CurrentQuest.MediaJsonPath);
			} catch (FileNotFoundException e) {
				mediaJSON = @"[]"; // we use an empty list then
			} catch (Exception e) {
				Log.SignalErrorToDeveloper ("Error reading media.json for quest " + CurrentQuest.Id + ": " + e.Message);
				mediaJSON = @"[]"; // we use an empty list then
			}

			List<LocalMediaInfo> localInfos = JsonConvert.DeserializeObject<List<LocalMediaInfo>> (mediaJSON);

			List<string> occupiedFileNames = new List<string> ();

			foreach (LocalMediaInfo localInfo in localInfos) {
				MediaInfo info;
				if (CurrentQuest.MediaStore.TryGetValue (localInfo.url, out info)) {
					// add local information to media store:
					info.LocalDir = localInfo.dir;
					info.LocalFileName = localInfo.filename;
					info.LocalSize = localInfo.size;
					info.LocalTimestamp = localInfo.time;
					// remember filenames as occupied for later creation of new unique filenames
					occupiedFileNames.Add (info.LocalFileName);
				} else {
					// this media file is not useful anymore, we delete it:
					string filePath = localInfo.LocalPath;
					try {
						File.Delete (filePath);
					} catch (Exception e) {
						Log.SignalErrorToDeveloper (
							"Error while deleting media file " + filePath +
							" : " + e.Message);
					}
				}
			}

			// Step 2b determine missing local filenames for new urls:
			foreach (KeyValuePair<string,MediaInfo> kvpEntry in CurrentQuest.MediaStore) {
				if (kvpEntry.Value.LocalFileName == null || kvpEntry.Value.LocalFileName == "") {
					string fileName = Files.FileName (kvpEntry.Value.Url);
					string fileNameCandidate = fileName;
					int discriminationNr = 1;
					string discriminiationAppendix = "";
					while (occupiedFileNames.Contains (fileNameCandidate)) {
						fileNameCandidate = fileName + discriminiationAppendix;
						discriminiationAppendix = "-" + discriminationNr++;
					}
					kvpEntry.Value.LocalFileName = fileNameCandidate;
				}
			}
		}


		/// <summary>
		/// This is step 3 of 4 during quest media sync. Downloads or updates the media files needed for this quest.
		/// </summary>
		public List<MediaInfo> GetListOfFilesNeedDownload ()
		{
			// 1. we create a list of files to be downloaded / updated (as Dictionary with all neeeded data for multi downloader:
			List<MediaInfo> filesToDownload = new List<MediaInfo> ();

			MediaInfo info;
			foreach (KeyValuePair<string,MediaInfo> kvpEntry in CurrentQuest.MediaStore) {
				info = kvpEntry.Value;

				// Request file header
				// TODO WHAT IF OFFLINE?
				Dictionary<string, string> headers = HTTP.GetRequestHeaders (info.Url);

				string headerValue;
				if (!headers.TryGetValue (HTTP.CONTENT_LENGTH, out headerValue)) {
					Log.SignalErrorToDeveloper ("{0} header missing for url {1}", HTTP.CONTENT_LENGTH, info.Url);
					info.RemoteSize = MediaInfo.UNKNOWN;
				} else {
					info.RemoteSize = long.Parse (headerValue);
				}

				if (!headers.TryGetValue (HTTP.LAST_MODIFIED, out headerValue)) {
					Log.SignalErrorToDeveloper ("{0} header missing for url {1}", HTTP.LAST_MODIFIED, info.Url);
					info.RemoteTimestamp = MediaInfo.UNKNOWN;
					// Since we do not know the timestamp of this file we load it:
					filesToDownload.Add (info);
				} else {
					info.RemoteTimestamp = long.Parse (headerValue);

					// if the remote file is newer we update: 
					// or if media is not locally available we load it:
					if (info.RemoteTimestamp > info.LocalTimestamp || !info.IsLocallyAvailable) {
						filesToDownload.Add (info);
					}								
				}
			}

			return filesToDownload;
		}

		#endregion
	}

	public class MediaInfo
	{

		string url;

		public string Url {
			get {
				return url;
			}
			private set {
				url = value;
			}
		}

		string localDir;

		public string LocalDir {
			get {
				return localDir;
			}
			set {
				localDir = value;
			}
		}

		string localFileName;

		public string LocalFileName {
			get {
				return localFileName;
			}
			set {
				localFileName = value;
			}
		}

		public string LocalPath {
			get {
				if (LocalDir == null || LocalFileName == null)
					return null;
				else
					return Files.CombinePath (LocalDir, LocalFileName);
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

		public MediaInfo (int questID, string url)
		{
			this.Url = url;
			this.LocalDir = Files.CombinePath (QuestManager.GetLocalPath4Quest (questID), "files");
			this.LocalFileName = null;
			this.LocalTimestamp = 0L;
			this.LocalSize = NOT_AVAILABLE;
			this.RemoteTimestamp = 0L;
			this.RemoteSize = UNKNOWN;
		}

		public MediaInfo (LocalMediaInfo localInfo)
		{
			this.Url = localInfo.url;
			this.LocalDir = localInfo.dir;
			this.LocalFileName = localInfo.filename;
			this.LocalTimestamp = localInfo.time;
			this.LocalSize = localInfo.size;
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
	public struct LocalMediaInfo
	{
		public string url;
		public string dir;
		public string filename;
		public long size;
		public long time;

		public LocalMediaInfo (string url, string dir, string filename, long size, long time)
		{
			this.url = url;
			this.dir = dir;
			this.filename = filename;
			this.size = size;
			this.time = time;
		}

		[JsonIgnore]
		public string LocalPath {
			get {
				return dir + "/" + filename;
			}
		}

	}
}
