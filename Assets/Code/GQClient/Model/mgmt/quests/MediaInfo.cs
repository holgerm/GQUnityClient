using System;
using Code.GQClient.Err;
using Code.GQClient.FileIO;
using Code.QM.Util;
using GQClient.Model;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.GQClient.Model.mgmt.quests
{

    public class MediaInfo
	{

		[JsonIgnore] private string _url;

		public string Url {
			get => _url;
			private set {
				if (_url == null || _url != value)
				{
					_url = value;
					QuestManager.Instance.MediaStoreIsDirty = true;
				}
			}
		}

		[JsonIgnore] private string _localDir;

		public string LocalDir {
			get => _localDir;
			set {
				if (_localDir == null || _localDir != value)
				{
					_localDir = value;
					QuestManager.Instance.MediaStoreIsDirty = true;
				}
			}
		}

		[JsonIgnore] private string _localFileName;

		public string LocalFileName {
			get => _localFileName;
			set {
				if (_localFileName == null || _localFileName != value)
				{
					_localFileName = value;
					QuestManager.Instance.MediaStoreIsDirty = true;
				}
			}
		}

		[JsonIgnore]
		public string LocalPath {
			get {
				if (LocalDir == null || LocalFileName == null) {
					Log.SignalErrorToAuthor("MediaInfo for url {0} invalid: LocalDir: {1}, LocalFileName: {2}",
						Url,
						LocalDir ?? "null",
						LocalFileName ?? "null");
					return null;
				}
				else {
					return Files.CombinePath (
						Device.GetPersistentDatapath(), 
						LocalDir, 
						LocalFileName);
				}
			}
		}

		[JsonIgnore] private long _localTimestamp;

		public long LocalTimestamp {
			get => _localTimestamp;
			set {
				if (_localTimestamp != value)
				{
					_localTimestamp = value;
					QuestManager.Instance.MediaStoreIsDirty = true;
				}
			}
		}

		[JsonIgnore] private long _remoteTimestamp;

		public long RemoteTimestamp {
			get => _remoteTimestamp;
			set {
				if (_remoteTimestamp != value)
				{
					_remoteTimestamp = value;
					QuestManager.Instance.MediaStoreIsDirty = true;
				}
			}
		}

		[JsonIgnore]
		public const long NOT_AVAILABLE = -1L;
		
		[JsonIgnore]
		public const long UNKNOWN = -2L;

		[JsonIgnore] private long _localSize;

		public long LocalSize {
			get => _localSize;
			set {
				if (_localSize != value)
				{
					_localSize = value;
					QuestManager.Instance.MediaStoreIsDirty = true;
				}
			}
		}

		[JsonIgnore] private long _remoteSize;

		public long RemoteSize {
			get => _remoteSize;
			set {
				if (_remoteSize != value)
				{
					_remoteSize = value;
					QuestManager.Instance.MediaStoreIsDirty = true;
				}
			}
		}

		private int _usageCounter;
		public int UsageCounter
		{
			get => _usageCounter;
			set
			{
				_usageCounter = value;
			}
		}

		[JsonConstructor]
		public MediaInfo(string baseDir, string url)
		{
			this.Url = url;
			this.LocalDir = Files.CombinePath (baseDir, "files");
			this.LocalFileName = Files.FileName(url);
			this.LocalTimestamp = 0L;
			this.LocalSize = NOT_AVAILABLE;
			this.RemoteTimestamp = 0L;
			this.RemoteSize = UNKNOWN;
		}

		public MediaInfo (LocalMediaInfo localInfo)
		{
			this.Url = localInfo.url;
			this.LocalDir = localInfo.absDir;
			this.LocalFileName = localInfo.filename;
			this.LocalTimestamp = localInfo.time;
			this.LocalSize = localInfo.size;
			this.RemoteTimestamp = 0L;
			this.RemoteSize = UNKNOWN;
		}

		/// <summary>
		/// Version for runtime media files:
		/// </summary>
		/// <param name="questID">Quest I.</param>
		/// <param name="pseudoVariable">Given variable name in Editor</param>
		/// <param name="dir">Dir.</param>
		/// <param name="filename">Filename.</param>
		public MediaInfo (int questID, string pseudoVariable, string dir, string filename) {
			this.Url = pseudoVariable;
			this.LocalDir = dir;
			this.LocalFileName = filename;
			this._localTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			this.RemoteTimestamp = 0L;
			this.RemoteSize = NOT_AVAILABLE;
		}

		[JsonIgnore]
		public bool IsLocallyAvailable => LocalSize != NOT_AVAILABLE;

		public override string ToString()
		{
			return $"MediaInfo:\n\turl:{_url}\n\tpath:{LocalPath}";
		}
	}
	
}
