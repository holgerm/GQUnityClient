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

		#region Fields

		public Transform InfoList;
		private string INFOLIST_PATH = "Viewport/InfoList";

		protected QuestInfoManager qim;

		protected Dictionary<int, QuestInfoController> questInfoControllers;

		#endregion


		#region Editor Setup

		void Reset()
		{
			InfoList = EnsurePrefabVariableIsSet<Transform> (InfoList, "InfoList", INFOLIST_PATH);
		}	

		#endregion

		// Use this for initialization
		void Start () 
		{
			qim = QuestInfoManager.Instance;

			qim.OnChange += OnQuestInfoChanged;

			if (questInfoControllers == null) {
				questInfoControllers = new Dictionary<int, QuestInfoController> ();
			}

			qim.UpdateQuestInfos ();

		}

		public void OnQuestInfoChanged (object sender, QuestInfoChangedEvent e) {
			switch (e.ChangeType) {
			case ChangeType.Added:
				QuestInfoController qiCtrl = 
					QuestInfoController.Create (
						root: InfoList.gameObject
					).GetComponent<QuestInfoController> ();
				questInfoControllers.Add (e.NewQuestInfo.Id, qiCtrl);
				qiCtrl.SetContent(e.NewQuestInfo);
				qiCtrl.Show();
				break;
			case ChangeType.Changed:
				// TODO
				break;
			case ChangeType.Removed:
				// TODO
				break;							
			}
		}

	}
}