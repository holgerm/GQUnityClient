using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Util;
using GQ.Client.Conf;
using Newtonsoft.Json;
using GQ.Client.Util;
using System;
using GQ.Client.UI;
using System.IO;
using GQ.Client.Err;

namespace GQ.Client.Model {

	public class ExportQuestInfosToJSON : Task {

		public ExportQuestInfosToJSON() : base() { }

		public override void Start(int step = 0) 
		{
			base.Start(step);

			List<QuestInfo> questInfoList = new List<QuestInfo> (QuestInfoManager.Instance.QuestDict.Values);

			try {
				string questInfoJSON = 
					(questInfoList.Count == 0) 
					? "[]"
					: JsonConvert.SerializeObject(questInfoList, Newtonsoft.Json.Formatting.Indented);
				File.WriteAllText(QuestInfoManager.LocalQuestInfoJSONPath, questInfoJSON);
			}
			catch (Exception e) {
				Log.SignalErrorToDeveloper ("Error while trying to export quest info json file: " + e.Message);
				RaiseTaskFailed ();
				return;
			}

			RaiseTaskCompleted();
		}

		public override object Result {
			get {
				return null;
			}
			protected set { }
		}
	}
}
