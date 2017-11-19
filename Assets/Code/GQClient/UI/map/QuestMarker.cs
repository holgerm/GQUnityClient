using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.UI;
using GQ.Client.Err;
using GQ.Client.Util;
using GQ.Client.Model;
using GQ.Client.UI.Dialogs;

namespace GQ.Client.UI
{

	public class QuestMarker : Marker {

		public QuestInfo Data { get; set; }

		public override void UpdateView () {}

		public override void OnTouch() {
			Debug.Log(string.Format("Marker '{0}' has been touched. (id: {1}, file: {2})", Data.Name, Data.Id, QuestManager.GetLocalPath4Quest (Data.Id)));
			Play ();
		}

		protected void Play ()
		{
			if (Data == null) {
				Log.SignalErrorToDeveloper("Tried to play quest for QuestMarker without QuestInfo data.");
				return;
			}
			
			// Load quest data: game.xml
			LocalFileLoader loadGameXML = 
				new LocalFileLoader (
					filePath: QuestManager.GetLocalPath4Quest (Data.Id) + QuestManager.QUEST_FILE_NAME
				);
			new DownloadDialogBehaviour (loadGameXML, "Loading quest");

			QuestStarter questStarter = new QuestStarter ();

			TaskSequence t = 
				new TaskSequence (loadGameXML, questStarter);

			t.Start ();
		}

	}

}
