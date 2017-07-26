using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using System.Collections.Generic;
using System;
using GQ.Util;
using GQ.Client.UI.Dialogs;
using GQ.Client.Util;

namespace GQ.Client.UI.Foyer {

	/// <summary>
	/// Shows all Quest Info objects, e.g. in a scrollable list within the foyer. Drives a dialog while refreshing its content.
	/// </summary>
	public class InfoListController : MonoBehaviour {

		public Transform InfoList;
		public GameObject QuestInfoUIPrefab;

		private QuestInfoManager qm;

		void Reset()
		{
			if (InfoList == null) {
				InfoList = transform;
			}

			if (QuestInfoUIPrefab == null) 
			{
				QuestInfoUIPrefab = (GameObject) Resources.Load ("QuestInfoPanel");
				if (QuestInfoUIPrefab == null) 
				{
					Debug.LogError ("QuestInfoUIPrefab variable must be set to prefab that defines UI element representing a QuestInfo. " +
						"Did not find QuestInfo prefeb in Resources Folder.");
				}
			}
		}	

		// Use this for initialization
		void Start () 
		{
			qm = QuestInfoManager.Instance;

			ServerQuestInfoLoader loader1 = new ServerQuestInfoLoader ();
			UIBehaviour behaviour1 = new UpdateQuestInfoDialogBehaviour (loader1);

			ServerQuestInfoLoader loader2 = new ServerQuestInfoLoader ();
			UIBehaviour behaviour2 = new UpdateQuestInfoDialogBehaviour (loader2);

			TaskSequence t = new TaskSequence(loader1, loader2);
			t.Start ();
		}
			
	}
}