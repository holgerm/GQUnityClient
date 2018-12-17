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
            updateQuestInfos_MenuEntry.SetActive(!ConfigurationManager.Current.autoSynchQuestInfos);
        }

        public void UpdateQuestInfos()
        {
            QuestInfoManager.Instance.UpdateQuestInfos();
            Base.Instance.MenuCanvas.SetActive(false);
        }

    }

}