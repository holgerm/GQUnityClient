using Code.GQClient.Conf;
using GQClient.Model;
using UnityEngine;

namespace Code.GQClient.UI.Foyer.questinfos
{
    public class QuestInfoSync : MonoBehaviour {
        private void Start () {
            if (ConfigurationManager.Current.OfferManualUpdate4QuestInfos)
            {
                QuestInfoManager.Instance.UpdateLocalQuestInfosOnly();
            }
            else
            {
                if (ConfigurationManager.Current.autoSyncQuestInfos)
                    QuestInfoManager.Instance.UpdateQuestInfos();
            }
        }
    }
}
