using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using System.Collections.Generic;
using System;
using GQ.Util;
using GQ.Client.UI.Controller;

namespace GQ.Client.UI.Foyer {

	/// <summary>
	/// Shows all Quest Info objects, e.g. in a scrollable list within the foyer.
	/// </summary>
	public class QuestInfoList : MonoBehaviour {

		public Transform Content;
		public GameObject QuestInfoUIPrefab;

		private QuestInfoManager qm;

		void Reset()
		{
			if (Content == null) {
				Content = transform;
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

			LoadQuestInfos.Show ();
		}
			
		public void ShowLoadingScreen()
		{
			GameObject rootCanvas = GameObject.FindGameObjectWithTag (Tags.ROOT_CANVAS);
			Instantiate(
				Resources.Load(Res.DIALOG_SCREEN),
				rootCanvas.transform,
				false
			);
		}

		private void RefreshList()
		{
			IEnumerator<QuestInfo> infos = qm.GetEnumerator ();
			while (infos.MoveNext ()) {
				Debug.Log ("in while ... " + infos.Current.Name);

				// create a new instance of the quest info prefab:
				GameObject currentQuestInfoUI = (GameObject)Instantiate (QuestInfoUIPrefab);
				// make it a child of this object (the content)
				currentQuestInfoUI.transform.SetParent (transform);
				// get the controller for the new UI element:
				QuestInfoPanel qiController = currentQuestInfoUI.GetComponent<QuestInfoPanel>();
				// initialize it with the data form the current quest info model object:
				qiController.SetUp(infos.Current);
			}
		}

			
	}
}