using UnityEngine;
using System.Collections;
using System.Text;
using System;
using GQ.Client.Err;
using Newtonsoft.Json;
using System.Collections.Generic;
using GQ.Client.Util;
using GQ.Client.Conf;
using GQ.Client.UI.Dialogs;
using GQ.Client.FileIO;
using GQ.AppSpecific;

namespace GQ.Client.Model
{

	/// <summary>
	/// Stores meta data about a quest, i.e. name, id, and some limited details about its content as well as usage data.
	/// 
	/// A questInfo object has the following live cycle / states:
	/// 
	/// - The Quest exists only on Server and has not been downloaded yet or has just been deleted. 
	///   (Initially if not predeployed)
	/// 	- Can be downloaded
	/// 	- Can NOT be started
	/// 	- Can NOT be updated
	/// 	- Can NOT be deleted
	/// - The Quest has been downloaded and exists locally as well as on server with same version. (After download)
	/// 	- Can NOT be downloaded
	/// 	- Can be started
	/// 	- Can NOT be updated
	/// 	- Can be deleted
	/// - The quest exists locally but has been updated on Server:
	/// 	- Can NOT be downloaded
	/// 	- Can be started
	/// 	- Can be updated
	/// 	- Can be deleted
	/// - The quest exists locally but has been removed from Server:
	/// 	- Can NOT be downloaded
	/// 	- Can be started
	/// 	- Can NOT be updated
	/// 	- Can be deleted but a warning should be shown
	/// The life cycle for quest loaded from server can be seen here: @ref QuestsFromServerLifeCycle
	/// 
	/// With predeployed quest:
	/// - The quest has been predeployed locally and there is no newer version on server:
	/// 	- Can NOT be downloaded
	/// 	- Can be started
	/// 	- Can NOT be updated
	/// 	- Can NOT be deleted
	/// - The quest has been predeployed locally but has been updated on Server:
	/// 	- Can NOT be downloaded
	/// 	- Can be started
	/// 	- Can be updated
	/// 	- Can NOT be deleted
	/// - The quest has been predeployed locally but updated locally to the newest server version:
	/// 	- Can NOT be downloaded
	/// 	- Can be started
	/// 	- Can be downgraded (set back to the older predeployed version)
	/// 	- Can NOT be deleted
	/// The life cycle for predeployed quest can be seen here: @ref QuestsPredeployedLifeCycle
	/// 
	/// We represent these states by four features with two or three values each:
	/// 
	/// - Downloadable (true, false)
	/// - Startable (true, false)
	/// - Updatable (true, false)
	/// - Deletable (Yes, YesWithWarning, No, Downgrade)


	/// </summary>
	[JsonObject (MemberSerialization.OptIn)]
	public class QuestInfo : IComparable<QuestInfo>
	{
		#region Serialized Features

		[JsonProperty]
		private 	int id;

		public 		int  	Id { 
			get {
				return id;
			} 
		}

		[JsonProperty]
		private 	string	name;

		public 		string	Name { 
			get {
				return name;
			} 
		}

		[JsonProperty]
		private 	string	featuredImagePath;

		public 		string 	FeaturedImagePath { 
			get {
				return featuredImagePath;
			} 
		}

		[JsonProperty]
		private 	int? typeID;

		public 		int?	TypeID { 
			get {
				return typeID;
			}
		}

		[JsonProperty]
		private 	string	iconPath;

		public 		string 	IconPath { 
			get {
				return iconPath;
			} 
		}

		/// <summary>
		/// Server-side update timestamp.
		/// </summary>
		/// <value>The last update.</value>
		[JsonProperty]
		private 	long? lastUpdate;

		public 		long? 	LastUpdateOnServer {
			get {
				return lastUpdate;
			}
		}

		[JsonProperty]
		private 	HotspotInfo[]	hotspots;

		public 		HotspotInfo[] 	Hotspots { 
			get {
				return hotspots;
			}
		}

		public HotspotInfo MarkerHotspot {
			get {
				double sumLong = 0f;
				double sumLat = 0f;
				foreach (HotspotInfo h in Hotspots) {
					sumLong += h.Longitude;
					sumLat += h.Latitude;
				}
				if (Hotspots.Length == 0)
					return HotspotInfo.NULL;
				else
					return new HotspotInfo (sumLat / Hotspots.Length, sumLong / Hotspots.Length);
			}
		}

		[JsonProperty]
		private 	MetaDataInfo[] metadata;

		public 		MetaDataInfo[] 	Metadata { 
			get {
				return metadata;
			}
		}

		[JsonProperty]
		private 	long?	_lastUpdateOnDevice = null;

		public 		long? 	LastUpdateOnDevice {
			get {
				return _lastUpdateOnDevice;
			}
			set {
				if (value != _lastUpdateOnDevice) {
					_lastUpdateOnDevice = value;
					if (OnChanged != null)
						OnChanged ();
				}
			}
		}

		[JsonProperty]
		private 	long?	_timestampOfPredeployedVersion = null;

		public 		long? 	TimestampOfPredeployedVersion {
			get {
				return _timestampOfPredeployedVersion;
			}
			set {
				// TODO how will we set this value?
				_timestampOfPredeployedVersion = value;
			}
		}

		[JsonProperty]
		private 	int _playedTimes = 0;

		public 		int 	PlayedTimes {
			get {
				return _playedTimes;
			}
			set {
				if (value != _playedTimes) {
					QuestInfo oldInfo = (QuestInfo)this.MemberwiseClone ();
					_playedTimes = value;
					QuestInfoManager.Instance.raiseDataChange (
						new QuestInfoChangedEvent (
							String.Format ("Info for quest {0} changed.", Name),
							ChangeType.ChangedInfo,
							newQuestInfo: this,
							oldQuestInfo: oldInfo
						)
					);

				}
			}
		}

		#endregion


		#region Derived features

		[JsonIgnore]
		public bool IsOnServer {
			get {
				return (LastUpdateOnServer != null);
			}
		}

		[JsonIgnore]
		public bool IsOnDevice {
			get {
				return (LastUpdateOnDevice != null);
			}
		}

		[JsonIgnore]
		public bool IsPredeployed {
			get {
				return (TimestampOfPredeployedVersion != null);
			}
		}

		[JsonIgnore]
		public bool HasUpdate {
			get {
				return (
				    // exists on both device and server:
				    IsOnDevice && IsOnServer
					// server update is newer (bigger number):
				    && LastUpdateOnServer > LastUpdateOnDevice
				);
			}
		}

		/// <summary>
		/// Determines whether this quest is new. This feature will be used in the UI in future versions.
		/// </summary>
		/// <returns><c>true</c> if this instance is new; otherwise, <c>false</c>.</returns>
		[JsonIgnore]
		public bool IsNew {
			get {
				return PlayedTimes == 0;
			}
		}

		[JsonIgnore]
		private List<string> _categories;

		[JsonIgnore]
		public List<string> Categories {
			get {
				if (_categories == null) {
					_categories = CategoryReader.ReadCategoriesFromMetadata (Metadata);
				}
				return _categories;
			}
		}

		public const string WITHOUT_CATEGORY_ID = "withoutcategory";

		public string CurrentCategoryId {
			get {
				return QuestInfoManager.Instance.Filter.CategoryToShow (this);
			}
		}


		#endregion


		#region Runtime API

		public delegate void ChangeHandler ();

		public event ChangeHandler OnChanged;

		public int HowManyListerners ()
		{
			return OnChanged.GetInvocationList ().Length;
		}

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder ();

			sb.AppendFormat ("{0} (id: {1})\n", Name, Id);
			sb.AppendFormat ("\t last server update: {0}", LastUpdateOnServer);
			sb.AppendFormat ("\t type id: {0}", TypeID);
			sb.AppendFormat ("\t icon path: {0}", IconPath);
			sb.AppendFormat ("\t featured image path: {0}", FeaturedImagePath);
			sb.AppendFormat ("\t with {0} hotspots.", Hotspots == null ? 0 : Hotspots.Length);
			sb.AppendFormat ("\t and {0} metadata entries.", Metadata == null ? 0 : Metadata.Length);

			return sb.ToString ();
		}

		public string GetMetadata (string key)
		{

			foreach (MetaDataInfo md in Metadata) {
				if (md.Key.Equals (key))
					return md.Value;
			}

			return null;
		}

		public void Dispose ()
		{
			OnChanged = null;
		}

		#endregion


		#region Sorting Comparison

		/// <summary>
		/// Returns a value greater than zero in case this object is considered greater than the given other. 
		/// A return value of 0 signals that both objects are equal and 
		/// a value less than zero means that this object is less than the given other one.
		/// </summary>
		/// <param name="otherInfo">Other info.</param>
		public int CompareTo (QuestInfo otherInfo)
		{
			if (SortAscending)
				return Compare (this, otherInfo);
			else
				return -Compare (this, otherInfo);
		}

		public delegate int CompareMethod (QuestInfo one,QuestInfo other);

		static public bool SortAscending = true;

		private static CompareMethod _compare;

		static public CompareMethod Compare {
			get {
				if (_compare == null) {
					_compare = DEFAULT_COMPARE;
				}
				return _compare;
			}
			set {
				_compare = value;
			}
		}

		static public CompareMethod DEFAULT_COMPARE = ByName;

		static public CompareMethod ByName {
			get {
				return (QuestInfo one, QuestInfo other) => {
					return one.Name.CompareTo (other.Name);
				};
			}
		}

		#endregion


		#region Runtime Functions

		public Task Download ()
		{
			// Load quest data: game.xml
			Downloader downloadGameXML = 
				new Downloader (
					url: QuestManager.GetQuestURI (Id), 
					timeout: ConfigurationManager.Current.timeoutMS,
					targetPath: QuestManager.GetLocalPath4Quest (Id) + QuestManager.QUEST_FILE_NAME
				);
			new DownloadDialogBehaviour (
				downloadGameXML, 
				string.Format("Lade {0}", ConfigurationManager.Current.nameForQuestSg)
			);

			// analyze game.xml, gather all media info compare to local media info and detect missing media
			PrepareMediaInfoList prepareMediaInfosToDownload = 
				new PrepareMediaInfoList ();
			new SimpleDialogBehaviour (
				prepareMediaInfosToDownload,
				string.Format("Synchronisiere {0}-Daten", ConfigurationManager.Current.nameForQuestSg),
				"Medien werden vorbereitet"
			);

			// download all missing media info
			MultiDownloader downloadMediaFiles =
				new MultiDownloader (
					maxParallelDownloads: 1,
					timeout: ConfigurationManager.Current.timeoutMS
				);
			new SimpleDialogBehaviour (
				downloadMediaFiles,
				string.Format("Synchronisiere {0}-Daten", ConfigurationManager.Current.nameForQuestSg),
				"Mediendateien werden geladen"
			);
			downloadMediaFiles.OnTaskCompleted += (object sender, TaskEventArgs e) => {
				LastUpdateOnDevice = LastUpdateOnServer;
			};

			// store current media info locally
			ExportMediaInfoList exportLocalMediaInfo =
				new ExportMediaInfoList ();
			new SimpleDialogBehaviour (
				exportLocalMediaInfo,
				string.Format("Synchronisiere {0}-Daten", ConfigurationManager.Current.nameForQuestSg),
				"Medieninformationen werden lokal gespeichert"
			);

			ExportQuestInfosToJSON exportQuestsInfoJSON = 
				new ExportQuestInfosToJSON ();
			new SimpleDialogBehaviour (
				exportQuestsInfoJSON,
				string.Format("Aktualisiere {0}", ConfigurationManager.Current.nameForQuestsPl),
				string.Format("{0}-Daten werden gespeichert", ConfigurationManager.Current.nameForQuestSg)
			);

			TaskSequence t = 
				new TaskSequence (downloadGameXML);
			t.AppendIfCompleted (prepareMediaInfosToDownload);
			t.Append (downloadMediaFiles);
			t.AppendIfCompleted (exportLocalMediaInfo);
			t.Append (exportQuestsInfoJSON);

			return t;
		}

		public void Delete ()
		{
			Files.DeleteDirCompletely (QuestManager.GetLocalPath4Quest (Id));
			LastUpdateOnDevice = null;

			ExportQuestInfosToJSON exportQuestsInfoJSON = 
				new ExportQuestInfosToJSON ();
			new SimpleDialogBehaviour (
				exportQuestsInfoJSON,
				string.Format("Aktualisiere {0}", ConfigurationManager.Current.nameForQuestsPl),
				string.Format("{0}-Daten werden gespeichert", ConfigurationManager.Current.nameForQuestSg)
			);

			exportQuestsInfoJSON.Start ();
		}

		public Task Play ()
		{
			// Load quest data: game.xml
			LocalFileLoader loadGameXML = 
				new LocalFileLoader (
					filePath: QuestManager.GetLocalPath4Quest (Id) + QuestManager.QUEST_FILE_NAME
				);
			new DownloadDialogBehaviour (
				loadGameXML, 
				string.Format("Lade {0}", ConfigurationManager.Current.nameForQuestsPl)
			);

			QuestStarter questStarter = new QuestStarter ();

			TaskSequence t = 
				new TaskSequence (loadGameXML, questStarter);

			return t;
		}


		/// <summary>
		/// Updates the qust info represented by this object.
		/// </summary>
		public void Update ()
		{
			// TODO
			Debug.Log ("TODO: Implement update method! Trying to update quest " + Name);
		}

		#endregion
	}


	public class HotspotInfo
	{

		public HotspotInfo(double lat, double lon) {
			Latitude = lat;
			Longitude = lon;
		} 

		public double Latitude { get; set; }

		public double Longitude { get; set; }

		public static HotspotInfo NULL = new HotspotInfo (0f, 0f);
	}


	public class MetaDataInfo
	{

		public MetaDataInfo(string key, string val) {
			Key = key;
			Value = val;
		}

		public string Key { get; set; }

		public string Value { get; set; }
	}


}