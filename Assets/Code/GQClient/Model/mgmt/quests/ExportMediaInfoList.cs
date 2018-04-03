using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Util;
using GQ.Client.Conf;
using Newtonsoft.Json;
using System;
using GQ.Client.UI;
using GQ.Client.Err;
using System.IO;
using GQ.Client.FileIO;

namespace GQ.Client.Model {

	/// <summary>
	/// This task performs step 4 of 4 during quest media sync. 
	/// It saves the local media info into a json file media.json within the quest folder.
	/// </summary>
	public class ExportMediaInfoList : Task {

		public ExportMediaInfoList() : base() { 
		}
			
		public override bool Run() 
		{
			// step 4 persist the updated local media info:
			List<LocalMediaInfo> localInfos = new List<LocalMediaInfo> ();
			foreach (KeyValuePair<string,MediaInfo> kvpEntry in QuestManager.Instance.CurrentQuest.MediaStore) {
				localInfos.Add (
					new LocalMediaInfo (
						kvpEntry.Value.Url,
						kvpEntry.Value.LocalDir,
						kvpEntry.Value.LocalFileName,
						kvpEntry.Value.LocalSize,
						kvpEntry.Value.LocalTimestamp)
				);
			}

			try {
				string mediaJSON = 
					(localInfos.Count == 0) 
					? "[]"
					: JsonConvert.SerializeObject(localInfos, Newtonsoft.Json.Formatting.Indented);
				string dir4MediaJSON = Files.ParentDir(QuestManager.Instance.CurrentMediaJSONPath);
				if (!Directory.Exists(dir4MediaJSON)) {
					Directory.CreateDirectory(dir4MediaJSON);
				}
				File.WriteAllText(QuestManager.Instance.CurrentMediaJSONPath, mediaJSON);
			}
			catch (Exception e) {
				Log.SignalErrorToDeveloper ("Error while trying to export media info json file: " + e.Message);
				return false;
			}

			return true;
		}
	}
}
