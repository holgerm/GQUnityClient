using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Code.GQClient.Err;
using Code.GQClient.FileIO;
using Code.GQClient.Util.tasks;
using Newtonsoft.Json;

namespace Code.GQClient.Model.mgmt.quests {

	/// <summary>
	/// This task performs step 4 of 4 during quest media sync. 
	/// It saves the local media info into a json file media.json within the quest folder.
	/// </summary>
	public class ExportMediaInfoList : Task {

		public ExportMediaInfoList() : base() { 
		}
			
		protected override IEnumerator DoTheWork() 
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
                yield break;
			}
		}
	}
}
