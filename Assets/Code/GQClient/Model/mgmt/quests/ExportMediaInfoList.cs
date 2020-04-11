using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Code.GQClient.Err;
using Code.GQClient.FileIO;
using Code.GQClient.Util.tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.GQClient.Model.mgmt.quests {

	/// <summary>
	/// This task performs step 4 of 4 during quest media sync. 
	/// It saves the local media info into a json file media.json within the quest folder.
	/// </summary>
	public class ExportMediaInfoList : Task {

		public ExportMediaInfoList() : base() { 
		}

		private Quest _quest;
			
		protected override void ReadInput(object input = null)
		{
			if (input is PrepareMediaInfoList.QuestWithMediaList questWithMediaList)
			{
					_quest = questWithMediaList.Quest;
			}
			else
			{
				Log.SignalErrorToDeveloper("ExportMediaInfoList task did not receive valid MediaInfo List from Input.");
			}

			if (_quest.MediaStore == null || _quest.MediaStore.Count == 0)
			{
				RaiseTaskCompleted();
			}
		}
		
		protected override IEnumerator DoTheWork() 
		{
			// step 4 persist the updated local media info:
			var localInfos = new List<LocalMediaInfo> ();
			foreach (var info in _quest.MediaStore.Values) {
				localInfos.Add (
					new LocalMediaInfo (
						info.Url,
						info.LocalDir,
						info.LocalFileName,
						info.LocalSize,
						info.LocalTimestamp)
				);
			}

			try {
				var mediaJson = 
					(localInfos.Count == 0) 
					? "[]"
					: JsonConvert.SerializeObject(localInfos, Newtonsoft.Json.Formatting.Indented);

				// write local media json for quest:
				var dir4MediaJson = Files.ParentDir(QuestManager.MediaJsonPath4Quest(_quest.Id));
				if (!Directory.Exists(dir4MediaJson)) {
					Directory.CreateDirectory(dir4MediaJson);
				}
				File.WriteAllText(QuestManager.MediaJsonPath4Quest(_quest.Id), mediaJson);
			}
			catch (Exception e) {
				Log.SignalErrorToDeveloper ("Error while trying to export media info json file: " + e.Message);
                yield break;
			}
		}
	}
}
