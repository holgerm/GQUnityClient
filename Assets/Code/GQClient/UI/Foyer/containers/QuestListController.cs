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
using QM.Util;

namespace GQ.Client.UI.Foyer
{

	/// <summary>
	/// Shows all Quest Info objects, e.g. in a scrollable list within the foyer. Drives a dialog while refreshing its content.
	/// </summary>
	public class QuestListController : QuestContainerController
	{
		public Transform InfoList;


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
				QuestInfoControllers.Add (e.NewQuestInfo.Id, qiCtrl);
				qiCtrl.Show ();
				sortView ();
				break;
			case ChangeType.ChangedInfo:
				if (e.OldQuestInfo == null || !QuestInfoControllers.TryGetValue (e.OldQuestInfo.Id, out qiCtrl)) {
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
				if (!QuestInfoControllers.TryGetValue (e.OldQuestInfo.Id, out qiCtrl)) {
					Log.SignalErrorToDeveloper (
						"Quest Info Controller for quest id {0} not found when a Remove event occurred.",
						e.OldQuestInfo.Id
					);
					break;
				}
				qiCtrl.Hide ();
				QuestInfoControllers.Remove (e.OldQuestInfo.Id);
				break;							
			case ChangeType.ListChanged:
				UpdateView ();
				break;							
			case ChangeType.FilterChanged:
				UpdateViewAfterFilterChanged ();
				break;							
			}
		}

		/// <summary>
		/// Sorts the list. Takes the current sorter into account to move the gameobjects in the right order.
		/// </summary>
		private void sortView ()
		{
			List<QuestInfoController> qcList = new List<QuestInfoController> (QuestInfoControllers.Values);
			qcList.Sort ();
			for (int i = 0; i < qcList.Count; i++) {
				qcList [i].transform.SetSiblingIndex (i);
			}
		}

		public override void UpdateView ()
		{
			if (this == null) {
				return;
			}
			if (InfoList == null) {
				return;
			}

			// hide and delete all list elements:
			foreach (KeyValuePair<int, QuestInfoController> kvp in QuestInfoControllers) {
				kvp.Value.Hide ();
				kvp.Value.Destroy ();
			}

			QuestInfoControllers.Clear();

			foreach (QuestInfo info in QuestInfoManager.Instance.GetFilteredQuestInfos()) {
				// create new list elements
				QuestListElementController qiCtrl = 
					QuestListElementController.Create (
						root: InfoList.gameObject,
						qInfo: info,
						containerController: this
					).GetComponent<QuestListElementController> ();
				QuestInfoControllers[info.Id] = qiCtrl;
				qiCtrl.Show ();
			}

			sortView ();
		}

		public void UpdateViewAfterFilterChanged ()
		{
			WATCH.Lap ("1");
			if (this == null) {
				return;
			}
			if (InfoList == null) {
				return;
			}

			// we make a separate list of ids of all old quest infos:
			List<int> rememberedOldIDs = new List<int>(QuestInfoControllers.Keys);
			int created = 0;
			int kept = 0;

			// we create new qi elements and keep those we can reuse. We remove those from our helper list.
			foreach (QuestInfo info in QuestInfoManager.Instance.GetFilteredQuestInfos()) {
				QuestInfoController qiCtrl;
				if (QuestInfoControllers.TryGetValue(info.Id, out qiCtrl)) {
					// this new element was already there, hence we keep it:
					rememberedOldIDs.Remove(info.Id);
					kept++;
				}
				else {
					WATCH.Start("2");
					// this new element was not there before, hence we create it:
					QuestListElementController qleCtrl = 
						QuestListElementController.Create (
							root: InfoList.gameObject,
							qInfo: info,
							containerController: this
						).GetComponent<QuestListElementController> ();
					WATCH.Show ("2", string.Format("created {0}", info.Name));
					QuestInfoControllers[info.Id] = qleCtrl;
					qleCtrl.Show ();
					WATCH.StopAndShow ("2");
					created++;
				}
			}

			WATCH.Show ("1", string.Format("kept {0} existing and created {1} new.", kept, created));

			int removed = 0;
			// now in the helper list only the old unused elements are left. Hence we delete them:
			foreach (int oldID in rememberedOldIDs) {
				QuestInfoControllers [oldID].Hide ();
				QuestInfoControllers [oldID].Destroy ();
				QuestInfoControllers.Remove (oldID);
				removed++;
			}

			WATCH.Show ("1", string.Format("did remove {0} unused old.", removed));

			sortView ();
		}

		#endregion

	}
}