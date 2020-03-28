using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.GQClient.Err;
using Code.GQClient.FileIO;
using Code.GQClient.Util.tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace GQClient.Model
{

    public class ExportQuestInfosToJSON : Task {

		public ExportQuestInfosToJSON() : base() { }

		protected override IEnumerator DoTheWork() 
		{
			var questInfoList = QuestInfoManager.Instance.GetListOfQuestInfos();

			try {
				var questInfoJSON = 
					(questInfoList.Count == 0) 
					? "[]"
					: JsonConvert.SerializeObject(questInfoList, Newtonsoft.Json.Formatting.Indented);
				Files.WriteAllText(QuestInfoManager.LocalQuestInfoJSONPath, questInfoJSON);

				foreach (var qi in questInfoList.Where(qi => qi.Id == 12902))
				{
					Debug.Log($"ExportQIToJSON. Test QI: {qi}");
				}
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
