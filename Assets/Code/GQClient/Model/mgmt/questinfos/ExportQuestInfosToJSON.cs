using System.Collections;
using System.Collections.Generic;
using GQ.Client.Util;
using Newtonsoft.Json;
using System;
using GQ.Client.Err;
using GQ.Client.FileIO;

namespace GQ.Client.Model
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
