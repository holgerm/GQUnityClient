using Code.GQClient.Conf;
using Code.GQClient.Model.mgmt.questinfos;
using Code.GQClient.UI.author;
using Code.GQClient.Util;
using UnityEngine;

namespace Code.GQClient.UI.menu
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