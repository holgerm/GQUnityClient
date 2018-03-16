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
using QM.Util;

namespace GQ.Client.Model
{

	/// <summary>
	/// Media Info about the local game media that is persisted in JSON file game-media.json in the quest folder.
	/// </summary>
	public class LocalMediaInfo
	{
		public string url;
		/// <summary>
		/// Only the relative opart of the absolute dir path is persisted, since on iOS the application data folder changes between different app versions.
		/// </summary>
		public string dir;

		[JsonIgnore]
		public string absDir {
			get {
				if (dir == null) {
					return null;
				}

				return Device.GetPersistentDatapath () + dir;
			}
			set {
				if (value == null) {
					dir = null;
					return;
				}

				if (value.StartsWith (Device.GetPersistentDatapath ())) {
					dir = value.Substring (Device.GetPersistentDatapath ().Length);
				} else {
					dir = value;
				}
			}
		}

		public string filename;
		public long size;
		public long time;

		public LocalMediaInfo (string url, string absDir, string filename, long size, long time)
		{
			this.url = url;
			this.absDir = absDir;
			this.filename = filename;
			this.size = size;
			this.time = time;
		}

		[JsonIgnore]
		public string LocalPath {
			get {
				return absDir + "/" + filename;
			}
		}

	}
}
