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
			
		public override void Start(int step = 0) 
		{
			base.Start(step);

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
			string mediaJSON = JsonConvert.SerializeObject(localInfos, Newtonsoft.Json.Formatting.Indented);
			File.WriteAllText(QuestManager.Instance.CurrentMediaJSONPath, mediaJSON);

			RaiseTaskCompleted();
		}
	}
}
