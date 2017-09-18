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

	public class PrepareMediaInfoList : Task {

		public PrepareMediaInfoList() : base() { }

		private string gameXML { get; set; } 

		public override void InitAfterPreviousTask(object sender, TaskEventArgs e) {
			if (e == null || e.Content == null) {
				RaiseTaskFailed ();
				return;
			}

			if (e.Content is string) {
				gameXML = e.Content as string;
			}
			else {
				// TODO End this task somehow (UI?)
				Log.SignalErrorToDeveloper(
					"Improper TaskEventArg received in SyncQuestData Task. Should be of type string but was " + 
					e.Content.GetType().Name);
			}
		}
			
		public override bool Run() 
		{
			// step 1 deserialize game.xml:
			QuestManager.Instance.DeserializeQuest(gameXML);

			// step 2 import local media info:
			QuestManager.Instance.ImportLocalMediaInfo();

			// step 3 include remote media info:
			Result = QuestManager.Instance.GetListOfFilesNeedDownload();

			return true;
		}
	}
}
