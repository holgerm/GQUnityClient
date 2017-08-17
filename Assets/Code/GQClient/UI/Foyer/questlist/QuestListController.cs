using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using System.Collections.Generic;
using System;
using GQ.Util;
using GQ.Client.UI.Dialogs;
using GQ.Client.Util;
using GQ.Client.Conf;
using UnityEngine.UI;

namespace GQ.Client.UI.Foyer {

	/// <summary>
	/// Shows all Quest Info objects, e.g. in a scrollable list within the foyer. Drives a dialog while refreshing its content.
	/// </summary>
	public class QuestListController : PrefabController {

		public Transform InfoList;
		private string INFOLIST_PATH = "Viewport/InfoList";

		protected List<GameObject> questInfoElements; 

		private QuestInfoManager qm;

		void Reset()
		{
			InfoList = EnsurePrefabVariableIsSet<Transform> (InfoList, "InfoList", INFOLIST_PATH);
		}	

		// Use this for initialization
		void Start () 
		{
			questInfoElements = new List<GameObject> ();
			qm = QuestInfoManager.Instance;

			qm.OnChange += 
				(object sender, QuestInfoChangedEvent e) => 
			{
				switch (e.ChangeType) {
				case ChangeType.Added:
					QuestInfoController qiui = QuestInfoController.Create (root: InfoList.gameObject).GetComponent<QuestInfoController>();
					qiui.SetContent(e.NewQuestInfo);
					qiui.Show();
					break;
				case ChangeType.Changed:
					// TODO
					break;
				case ChangeType.Removed:
					// TODO
					break;							
				}
			};

			Download downloader = 
				new Download (
					url: ConfigurationManager.UrlPublicQuestsJSON, 
					timeout: 120000);
			new DownloadDialogBehaviour (downloader, "Updating quests");

			ImportQuestInfosFromJSON importer = 
				new ImportQuestInfosFromJSON ();
			new SimpleDialogBehaviour (
				importer,
				"Importing Quest Data",
				"Reading all found quests into the local data store."
			);

			TaskSequence t = new TaskSequence(downloader, importer);
			t.Start ();
		}

	}
}