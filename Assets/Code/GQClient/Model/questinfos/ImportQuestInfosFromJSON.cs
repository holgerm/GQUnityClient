using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Util;
using GQ.Client.Conf;
using Newtonsoft.Json;
using GQ.Client.Util;
using System;
using GQ.Client.UI;

namespace GQ.Client.Model {

	public class ImportQuestInfosFromJSON : Task {

		public ImportQuestInfosFromJSON() : base() { }

		private string InputJSON { get; set; }

		public override void StartCallback(object sender, TaskEventArgs e) {
			Debug.Log ("StartCallback(): " + e.Content);
			if (e.Content is string) {
				InputJSON = e.Content as string;
			}
			this.Start(e.Step + 1);
		}

		public override void Start(int step = 0) 
		{
			base.Start(step);

			Debug.Log ("Before Import: " + InputJSON);
			QuestInfo[] quests = JsonConvert.DeserializeObject<QuestInfo[]>(InputJSON);
			QuestInfoManager.Instance.Import (quests);
			Debug.Log ("Import done: " + QuestInfoManager.Instance.Count);
			RaiseTaskCompleted ();
		}

		public override object Result {
			get {
				return null;
				}
		}
	}
}
