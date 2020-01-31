using GQ.Client.Conf;
using UnityEngine;

namespace GQ.Client.UI
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