using Code.GQClient.Conf;
using Code.GQClient.Model.mgmt.questinfos;
using UnityEngine;

namespace Code.GQClient.UI.Foyer.questinfos
{
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
}
