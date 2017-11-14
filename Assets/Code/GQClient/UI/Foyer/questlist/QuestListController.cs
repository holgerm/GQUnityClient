using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using System.Collections.Generic;
using System;
using GQ.Client.UI.Dialogs;
using GQ.Client.Util;
using GQ.Client.Conf;
using UnityEngine.UI;
using GQ.Client.Err;

namespace GQ.Client.UI.Foyer
{

	/// <summary>
	/// Shows all Quest Info objects, e.g. in a scrollable list within the foyer. Drives a dialog while refreshing its content.
	/// </summary>
	public class QuestListController : QuestInfoContainerController
	{

		#region React on Events

		public override void OnQuestInfoChanged (object sender, QuestInfoChangedEvent e)
		{
			QuestInfoController qiCtrl;
			switch (e.ChangeType) {
			case ChangeType.AddedInfo:
				qiCtrl = 
					QuestListElementController.Create (
					root: InfoList.gameObject,
					qInfo: e.NewQuestInfo,
					containerController: this
				).GetComponent<QuestListElementController> ();
				questInfoControllers.Add (e.NewQuestInfo.Id, qiCtrl);
				qiCtrl.Show ();
				sortView ();
				break;
			case ChangeType.ChangedInfo:
				if (!questInfoControllers.TryGetValue (e.OldQuestInfo.Id, out qiCtrl)) {
					Log.SignalErrorToDeveloper (
						"Quest Info Controller for quest id {0} not found when a Change event occurred.",
						e.OldQuestInfo.Id
					);
					break;
				}
				qiCtrl.UpdateView ();
				qiCtrl.Show ();
				sortView ();
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
				UpdateView ();
				break;							
			}
		}

		/// <summary>
		/// Sorts the list. Takes the current sorter into account to move the gameobjects in the right order.
		/// </summary>
		private void sortView ()
		{
			List<QuestInfoController> qcList = new List<QuestInfoController> (questInfoControllers.Values);
			qcList.Sort ();
			for (int i = 0; i < qcList.Count; i++) {
				qcList [i].transform.SetSiblingIndex (i);
			}
		}

		public override void UpdateView ()
		{
			if (this == null) {
				Debug.Log ("QuestListController is null".Red ());
				return;
			}
			if (InfoList == null) {
				Debug.Log ("QuestListController.InfoList is null".Red ());
				return;
			}

			// hide and delete all list elements:
			foreach (KeyValuePair<int, QuestInfoController> kvp in questInfoControllers) {
				kvp.Value.Hide ();
				kvp.Value.Destroy ();
			}
			foreach (QuestInfo info in QuestInfoManager.Instance.GetListOfQuestInfos()) {
				// create new list elements
				if (QuestInfoManager.Instance.Filter.accept (info)) {
					QuestListElementController qiCtrl = 
						QuestListElementController.Create (
							root: InfoList.gameObject,
							qInfo: info,
							containerController: this
						).GetComponent<QuestListElementController> ();
					questInfoControllers.Add (info.Id, qiCtrl);
					qiCtrl.Show ();
				}
			}
			sortView ();

		}

		#endregion

	}
}