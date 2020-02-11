using Code.GQClient.Conf;
using UnityEngine;

namespace Code.GQClient.UI.menu
{

    public class Menu2Config : MonoBehaviour
    {
        public GameObject partnersInfoMenuEntry;
        public GameObject feedbackMenuEntry;
        public GameObject authorLoginMenuEntry;


        // Use this for initialization
        void Start()
        {
            partnersInfoMenuEntry.SetActive(ConfigurationManager.Current.offerPartnersInfo);
            feedbackMenuEntry.SetActive(ConfigurationManager.Current.offerFeedback);
            authorLoginMenuEntry.SetActive(ConfigurationManager.Current.offerAuthorLogin);
        }
    }
}