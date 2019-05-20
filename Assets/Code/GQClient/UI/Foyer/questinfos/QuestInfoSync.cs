using System.Collections;
using System.Collections.Generic;
using GQ.Client.Conf;
using GQ.Client.Model;
using UnityEngine;

public class QuestInfoSync : MonoBehaviour {

    bool updateStarted;
	
    void Start () {
        if (!ConfigurationManager.Current.OfferManualUpdate4QuestInfos)
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
        if (!ConfigurationManager.Current.OfferManualUpdate4QuestInfos && updateStarted && !paused) {
            QuestInfoManager.Instance.UpdateQuestInfos();
        }
    }

}
