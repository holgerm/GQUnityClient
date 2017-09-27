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
using GQ.Client.Err;

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

			// TODO soll wirklich der ListController hier dem Manager sagen, dass er ein update braucht?
			// sollte doch eher passieren, wenn wieder online, oder user interaktion, oder letztes updates lange her...
			// aber vielleicht eben doch auch hier beim Start des Controllers, d.h. bei neuer Anzeige der Liste.
			qim.UpdateQuestInfos ();

		}


		#region React on Events

		public void OnQuestInfoChanged (object sender, QuestInfoChangedEvent e) {
			QuestInfoController qiCtrl;
			switch (e.ChangeType) {
			case ChangeType.AddedInfo:
				qiCtrl = 
					QuestInfoController.Create (
						root: InfoList.gameObject,
						qInfo: e.NewQuestInfo
					).GetComponent<QuestInfoController> ();
				questInfoControllers.Add (e.NewQuestInfo.Id, qiCtrl);
				qiCtrl.SetContent (e.NewQuestInfo);
				qiCtrl.Show ();
				UpdateView ();
				break;
			case ChangeType.ChangedInfo:
				if (!questInfoControllers.TryGetValue(e.OldQuestInfo.Id, out qiCtrl)) {
					Log.SignalErrorToDeveloper(
						"Quest Info Controller for quest id {0} not found when a Change event occurred.",
						e.OldQuestInfo.Id
					);
					break;
				}
				qiCtrl.SetContent (e.NewQuestInfo);
				qiCtrl.Show ();
				UpdateView ();
				break;
			case ChangeType.RemovedInfo:
				if (!questInfoControllers.TryGetValue (e.OldQuestInfo.Id, out qiCtrl)) {
					Log.SignalErrorToDeveloper (
						"Quest Info Controller for quest id {0} not found when a Remove event occurred.",
						e.OldQuestInfo.Id
					);
					break;
				}
				qiCtrl.Hide ();
				questInfoControllers.Remove (e.OldQuestInfo.Id);
				break;							
			case ChangeType.ListChanged:
				// hide and delete all list elements:
				foreach (KeyValuePair<int, QuestInfoController> kvp in questInfoControllers) {
					kvp.Value.Hide ();
					kvp.Value.Destroy ();
				}
				foreach (QuestInfo info in QuestInfoManager.Instance.GetListOfQuestInfos()) {
					// create new list elements
					if (QuestInfoManager.Instance.Filter.accept (info)) {
						qiCtrl = 
						QuestInfoController.Create (
							root: InfoList.gameObject,
							qInfo: info
						).GetComponent<QuestInfoController> ();
						questInfoControllers.Add (info.Id, qiCtrl);
						qiCtrl.SetContent (info);
						qiCtrl.Show ();
						UpdateView ();
					}
				}
				break;							
			}
		}

		/// <summary>
		/// Updates the view. Takes the current sorter into account to move the gameobjects in the right order.
		/// </summary>
		public void UpdateView() {
			List<QuestInfoController> qcList = new List<QuestInfoController> (questInfoControllers.Values);
			qcList.Sort ();
			for (int i = 0; i < qcList.Count; i++) {
				qcList[i].transform.SetSiblingIndex (i);
			}
		}

		#endregion

	}
}