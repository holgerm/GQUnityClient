using System.Collections;
using System.Collections.Generic;
using GQ.Client.Conf;
using GQ.Client.Model;
using UnityEngine;

public class QuestInfoSync : MonoBehaviour {

    bool updateStarted;
	// Use this for initialization
	void Start () {
        if (!ConfigurationManager.Current.manualUpdateQuestInfos)
        {
            QuestInfoManager.Instance.UpdateQuestInfos();
        }
        else
        {
            QuestInfoManager.Instance.UpdateLocalQuestInfosOnly();
        }

        updateStarted = true;
    }

    void OnApplicationPause(bool paused) {
        if (!ConfigurationManager.Current.manualUpdateQuestInfos && updateStarted && !paused) {
            QuestInfoManager.Instance.UpdateQuestInfos();
        }
    }

}
