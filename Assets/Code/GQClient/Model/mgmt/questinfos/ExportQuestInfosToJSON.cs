using System;
using System.Collections;
using System.Collections.Generic;
using Code.GQClient.Err;
using Code.GQClient.FileIO;
using Code.GQClient.Util.tasks;
using Newtonsoft.Json;

namespace Code.GQClient.Model.mgmt.questinfos
{

    public class ExportQuestInfosToJSON : Task {

		public ExportQuestInfosToJSON() : base() { }

		protected override IEnumerator DoTheWork() 
		{
			List<QuestInfo> questInfoList = QuestInfoManager.Instance.GetListOfQuestInfos();

			try {
				string questInfoJSON = 
					(questInfoList.Count == 0) 
					? "[]"
					: JsonConvert.SerializeObject(questInfoList, Newtonsoft.Json.Formatting.Indented);
				Files.WriteAllText(QuestInfoManager.LocalQuestInfoJSONPath, questInfoJSON);
			}
			catch (Exception e) {
				Log.SignalErrorToDeveloper ("Error while trying to export quest info json file: " + e.Message);
                RaiseTaskFailed();
                yield break;
            }

			RaiseTaskCompleted();
		}

		public override object Result {
			get {
				return null;
			}
			set { }
		}
	}
}
