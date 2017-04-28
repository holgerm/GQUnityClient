using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using System;
using GQ.Client.Conf;
using GQ.Util;

namespace GQScripts
{

	public class InitQuestManager : MonoBehaviour, QuestInfoImporter_I
	{

		public QuestInfoManager qm;

		// Use this for initialization
		void Start ()
		{
			qm = QuestInfoManager.Instance;
			this.StartImportQuestInfos (new QuestInfoLoaderFromServer ());
		}

		public void ImportQuestInfoDone ()
		{ 
			Debug.Log (String.Format ("Import Done. QM has {0} quests.", qm.Count));
		}
	}
}
