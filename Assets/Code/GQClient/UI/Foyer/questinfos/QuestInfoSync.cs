using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.QM.Util;
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
            
            if (ConfigurationManager.Current.OfferManualUpdate4QuestInfos)
            {
                QuestInfoManager.UpdateLocalQuestInfosOnly();
            }
            else
            {
                if (ConfigurationManager.Current.autoSyncQuestInfos)
                {
                    QuestInfoManager.UpdateQuestInfos();
                }
                else
                {
                    QuestInfoManager.UpdateLocalQuestInfosOnly();
                }
            }

            AlreadySynched = true;
            
            Debug.Log($"Synch took {Time.realtimeSinceStartup - startTime} seconds".Yellow());

        }
    }
}
