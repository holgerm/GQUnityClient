using System;
using System.Collections;
using Code.GQClient.Err;
using Code.GQClient.FileIO;
using Code.GQClient.Util.tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace GQClient.Model
{

    public class ExportQuestInfosToJson : Task {

		public ExportQuestInfosToJson() : base() { }

		protected override IEnumerator DoTheWork() 
		{
			var questInfoList = QuestInfoManager.Instance.GetListOfQuestInfos();

			try {
				var questInfoJson = 
					(questInfoList.Count == 0) 
					? "[]"
					: JsonConvert.SerializeObject(questInfoList, Formatting.Indented);
				Files.WriteAllText(QuestInfoManager.LocalQuestInfoJsonPath, questInfoJson);

			}
			catch (Exception e) {
				Log.SignalErrorToDeveloper ("Error while trying to export quest info json file: " + e.Message);
                RaiseTaskFailed();
                yield break;
            }

			RaiseTaskCompleted();
		}

		public override object Result {
			get => null;
			set { }
		}
	}
}
