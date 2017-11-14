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
	/// Shows all Quest Info objects, on a map within the foyer. Refreshing its content silently (no dialogs shown etc.).
	/// </summary>
	public class QuestMapController : QuestContainerController
	{

		#region React on Events

		public override void OnQuestInfoChanged (object sender, QuestInfoChangedEvent e)
		{
			QuestInfoController qiCtrl;
			switch (e.ChangeType) {
			case ChangeType.AddedInfo:
//				qiCtrl = 
//					QuestMapMarkerController.Create (
//					root: InfoList.gameObject,
//					qInfo: e.NewQuestInfo,
//					containerController: this
//				).GetComponent<QuestMapMarkerController> ();
//				QuestInfoControllers.Add (e.NewQuestInfo.Id, qiCtrl);
//				qiCtrl.Show ();
				break;
			case ChangeType.ChangedInfo:
				if (!QuestInfoControllers.TryGetValue (e.OldQuestInfo.Id, out qiCtrl)) {
					Log.SignalErrorToDeveloper (
						"Quest Info Controller for quest id {0} not found when a Change event occurred.",
						e.OldQuestInfo.Id
					);
					break;
				}
				qiCtrl.UpdateView ();
				qiCtrl.Show ();
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
			}
		}

		public override void UpdateView ()
		{
			if (this == null) {
				Debug.Log ("QuestMapController is null".Red ());
				return;
			}
//			if (InfoList == null) {
//				Debug.Log ("QuestMapMarkerController.InfoList is null".Red ());
//				return;
//			}

			// hide and delete all list elements:
			foreach (KeyValuePair<int, QuestInfoController> kvp in QuestInfoControllers) {
				kvp.Value.Hide ();
				kvp.Value.Destroy ();
			}
			foreach (QuestInfo info in QuestInfoManager.Instance.GetListOfQuestInfos()) {
				// create new list elements
				if (QuestInfoManager.Instance.Filter.accept (info)) {
//					QuestMapMarkerController qiCtrl = 
//						QuestMapMarkerController.Create (
//							root: InfoList.gameObject,
//							qInfo: info,
//							containerController: this
//						).GetComponent<QuestMapMarkerController> ();
//					QuestInfoControllers.Add (info.Id, qiCtrl);
//					qiCtrl.Show ();
				}
			}
		}

		#endregion

	}
}