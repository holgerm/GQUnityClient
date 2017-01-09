using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using System;
using GQ.Client.Conf;
using GQ.Util;

public class InitQuestManager : MonoBehaviour, QuestImporter_I {

	public QuestManager qm;

	// Use this for initialization
	void Start () {
		qm = QuestManager.Instance;
		this.StartGetQuestInfosFromServer();
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log(String.Format("QM has {0} quests.", qm.Count));
	}

	public void ImportDone (QuestInfo[] questsFromServer) { 
		qm.Import(questsFromServer);
	}

}
