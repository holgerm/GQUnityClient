using Code.GQClient.Conf;
using Code.GQClient.start;
using Code.GQClient.Util;
using Code.QM.UI;
using UnityEngine;

namespace Code.GQClient.UI.menu
{
    public class Menu2Config : MonoBehaviour
    {
        public GameObject menuRadioGroup;
        public GameObject partnersInfoMenuEntry;
        public GameObject feedbackMenuEntry;
        public GameObject authorLoginMenuEntry;


        // Use this for initialization
        private void Start()
        {
            if (Config.Current.offerPartnersInfo && Base.Instance.partnersCanvas != null)
            {
                partnersInfoMenuEntry = ActivateMenuEntry("Unsere Partner", "icons/partners",
                    Base.Instance.partnersCanvas);
            }

            feedbackMenuEntry.SetActive(Config.Current.offerFeedback);
            authorLoginMenuEntry.SetActive(Config.Current.offerAuthorLogin);
        }

        private GameObject ActivateMenuEntry(string elementName, string elementIconPath,
            GameObject activationGo)
        {
            var menuEntryGo = PrefabController.Create(
                AssetBundles.PREFABS,
                MenuRadioElement.PREFAB,
                menuRadioGroup);
            menuEntryGo.GetComponent<MenuRadioElement>().Initialize(
                elementName,
                elementIconPath,
                activationGo);
            menuEntryGo.SetActive(true);
            return menuEntryGo;
        }
    }
}