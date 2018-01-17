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
				Debug.Log ("LocalMediaInfo.dir get: dir: " + (dir == null ? null : dir));
				if (dir == null) {
					return null;
				}

				return PersistentDataPath() + dir;
			}
			set {
				if (value == null) {
					Debug.Log ("LocalMediaInfo.dir SET NULL: ");
					dir = null;
					return;
				}

				if (value.StartsWith (PersistentDataPath())) {
					Debug.Log ("LocalMediaInfo.dir SET MIT APPL_PATH: " + value);
					dir = value.Substring (PersistentDataPath().Length);
				} else {
					Debug.Log ("LocalMediaInfo.dir SET OHNE APPL_PATH: " + value);
					dir = value;
				}
				Debug.Log ("LocalMediaInfo.dir set: value: " + value + " dir: " + dir + "  \n    ApplPath: " + PersistentDataPath());
			}
		}
		public string filename;
		public long size;
		public long time;

		public delegate string ConstantStringReturningMethod();
		public static ConstantStringReturningMethod PersistentDataPath = () => {
			return Application.persistentDataPath;
		};

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
