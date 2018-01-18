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

using System;
using GQ.Client.Util;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

namespace GQ.Client.Model
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
			}
		}

		public string LocalPath {
			get {
				if (LocalDir == null || LocalFileName == null) {
					Log.SignalErrorToAuthor("MediaInfo for url {0} invalid: LocalDir: {1}, LocalFileName: {2}",
						Url,
						(LocalDir == null) ? "null" : LocalDir,
						(LocalFileName == null) ? "null" : LocalFileName);
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
			this.LocalDir = localInfo.absDir;
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
	
}
