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
            Config cf = ConfigurationManager.Current;
            updateQuestInfos_MenuEntry.SetActive(
                cf.OfferManualUpdate4QuestInfos
            );
        }

        void OnEnable()
        {
            Author.SettingsChanged += Author_SettingsChanged;
        }

        private void OnDisable()
        {
            Author.SettingsChanged -= Author_SettingsChanged;
        }

        void Author_SettingsChanged(object sender, System.EventArgs e)
        {
            Config cf = ConfigurationManager.Current;
            updateQuestInfos_MenuEntry.SetActive(cf.OfferManualUpdate4QuestInfos);
        }

        public void UpdateQuestInfos()
        {
            QuestInfoManager.Instance.UpdateQuestInfos();
            Base.Instance.MenuCanvas.SetActive(false);
        }

    }

}