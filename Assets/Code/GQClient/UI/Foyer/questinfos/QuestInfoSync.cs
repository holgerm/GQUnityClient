using GQ.Client.Conf;
using GQ.Client.Model;
using UnityEngine;

public class QuestInfoSync : MonoBehaviour {

    void Start () {
        if (ConfigurationManager.Current.OfferManualUpdate4QuestInfos)
        {
            QuestInfoManager.Instance.UpdateLocalQuestInfosOnly();
        }
        else
        {
            QuestInfoManager.Instance.UpdateQuestInfos();
        }
    }
}
