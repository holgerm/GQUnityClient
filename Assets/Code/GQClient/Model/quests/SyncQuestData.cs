using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Util;
using GQ.Client.Conf;
using Newtonsoft.Json;
using GQ.Client.Util;
using System;
using GQ.Client.UI;
using GQ.Client.Err;

namespace GQ.Client.Model {

	public class SyncQuestData : Task {

		public SyncQuestData() : base() { }

		private string gameXML { get; set; }

		public override void StartCallback(object sender, TaskEventArgs e) {
			if (e.Content is string) {
				gameXML = e.Content as string;
			}
			else {
				// TODO End this task somehow (UI?)
				Log.SignalErrorToDeveloper(
					"Improper TaskEventArg received in SyncQuestData Task. Should be of type string but was " + 
					e.Content.GetType().Name);
			}
			this.Start(e.Step + 1);
		}

		public override void Start(int step = 0) 
		{
			base.Start(step);

			// TODO perfrom steps 1 to 4 of synching quest data between client and server.

			// step 1 deserialize game.xml:
			Quest quest = QuestManager.Instance.DeserializeQuest(gameXML);

			// step 2 import local media infos
			quest.ImportLocalMediaInfos();

			// step 3 include remote media infos:
			quest.DownloadOrUpdateMedia();

			// step 4 reexport media infos:
			quest.PersistLocalMediaInfo();

			RaiseTaskCompleted ();
		}

		public override object Result {
			get {
				return null;
			}
			protected set { }
		}
	}
}
