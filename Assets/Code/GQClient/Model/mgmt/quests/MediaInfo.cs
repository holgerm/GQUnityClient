using Code.GQClient.Err;
using Code.GQClient.FileIO;
using UnityEngine;

namespace Code.GQClient.Model.mgmt.quests
{

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
				Debug.Log($"MediaInfo localfile set to: {value}");
			}
		}

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
					return Files.CombinePath (LocalDir, LocalFileName);
				}
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

		public MediaInfo(string baseDir, string url)
		{
			this.Url = url;
			this.LocalDir = Files.CombinePath (baseDir, "files");
			this.LocalFileName = null;
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
		/// <param name="url">URL.</param>
		/// <param name="dir">Dir.</param>
		/// <param name="filename">Filename.</param>
		public MediaInfo (int questID, string pseudoVariable, string dir, string filename) {
			this.Url = pseudoVariable;
			this.LocalDir = dir;
			this.LocalFileName = filename;
			this.localTimestamp = 0L; // TODO set to now in milliseconds
			this.RemoteTimestamp = 0L;
			this.RemoteSize = NOT_AVAILABLE;
		}

		public bool IsLocallyAvailable {
			get {
				return !(LocalSize == NOT_AVAILABLE);
			}
		}
	}
	
}
