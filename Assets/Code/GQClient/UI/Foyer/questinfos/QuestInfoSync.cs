using Code.GQClient.Conf;
using GQClient.Model;
using UnityEngine;

namespace Code.GQClient.UI.Foyer.questinfos
{
    public class QuestInfoSync : MonoBehaviour
    {

        private static bool AlreadySynched = false;
        
        private void Start ()
        {
            float startTime = Time.realtimeSinceStartup;
            if (AlreadySynched) return;
            
            if (Config.Current.OfferManualUpdate4QuestInfos)
            {
                QuestInfoManager.UpdateLocalQuestInfosOnly();
            }
            else
            {
                if (Config.Current.autoSyncQuestInfos)
                {
                    QuestInfoManager.UpdateQuestInfos();
                }
                else
                {
                    QuestInfoManager.UpdateLocalQuestInfosOnly();
                }
            }

            AlreadySynched = true;
        }
    }
}
