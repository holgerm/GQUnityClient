using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using System.Collections.Generic;
using System;
using GQ.Client.Util;
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

			ImportQuestInfosFromJSON importLocal = 
				new ImportQuestInfosFromJSON (false);
			new SimpleDialogBehaviour (
				importLocal,
				"Updating quests",
				"Reading local quests."
			);

			Downloader downloader = 
				new Downloader (
					url: ConfigurationManager.UrlPublicQuestsJSON, 
					timeout: ConfigurationManager.Current.downloadTimeOutSeconds * 1000);
			new DownloadDialogBehaviour (downloader, "Updating quests");

			ImportQuestInfosFromJSON importFromServer = 
				new ImportQuestInfosFromJSON (true);
			new SimpleDialogBehaviour (
				importFromServer,
				"Updating quests",
				"Reading all found quests into the local data store."
			);

			ExportQuestInfosToJSON exporter = 
				new ExportQuestInfosToJSON ();
			new SimpleDialogBehaviour (
				exporter,
				"Updating quests",
				"Saving Quest Data"
			);

			TaskSequence t = new TaskSequence(importLocal, downloader, importFromServer, exporter);
			t.Start ();
		}

	}
}