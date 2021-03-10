using Code.GQClient.Conf;
using GQClient.Model;
using Code.GQClient.UI.author;
using Code.GQClient.Util;
using UnityEngine;

namespace Code.GQClient.UI.menu
{

    public class Menu1Config : MonoBehaviour
    {
        public GameObject updateQuestInfos_MenuEntry;


        // Use this for initialization
        private void Start()
        {
            var cf = ConfigurationManager.Current;
            updateQuestInfos_MenuEntry.SetActive(
                cf.OfferManualUpdate4QuestInfos
            );
        }

        private void OnEnable()
        {
            Author.SettingsChanged += Author_SettingsChanged;
        }

        private void OnDisable()
        {
            Author.SettingsChanged -= Author_SettingsChanged;
        }

        private void Author_SettingsChanged(object sender, System.EventArgs e)
        {
            var cf = ConfigurationManager.Current;
            updateQuestInfos_MenuEntry.SetActive(cf.OfferManualUpdate4QuestInfos);
        }
    }

}