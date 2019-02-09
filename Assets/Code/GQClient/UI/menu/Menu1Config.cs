using System.Collections;
using System.Collections.Generic;
using GQ.Client.Conf;
using GQ.Client.Model;
using GQ.Client.Util;
using UnityEngine;

namespace GQ.Client.UI
{

    public class Menu1Config : MonoBehaviour
    {
        public GameObject updateQuestInfos_MenuEntry;


        // Use this for initialization
        void Start()
        {
            updateQuestInfos_MenuEntry.SetActive(
                ConfigurationManager.Current.OfferManualUpdate4QuestInfos
            );
            Author.SettingsChanged += Author_SettingsChanged;
        }

        void Author_SettingsChanged(object sender, System.EventArgs e)
        {
            updateQuestInfos_MenuEntry.SetActive(Author.offerManualUpdate);
            Debug.Log("SEt Entry Update to : " + Author.offerManualUpdate);
        }


        public void UpdateQuestInfos()
        {
            QuestInfoManager.Instance.UpdateQuestInfos();
            Base.Instance.MenuCanvas.SetActive(false);
        }

    }

}