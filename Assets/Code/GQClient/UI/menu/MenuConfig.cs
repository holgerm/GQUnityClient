using System.Collections;
using System.Collections.Generic;
using GQ.Client.Conf;
using UnityEngine;

namespace GQ.Client.Util
{

    public class MenuConfig : MonoBehaviour
    {
        public GameObject feedbackMenuEntry;
        public GameObject authorLoginMenuEntry;


        // Use this for initialization
        void Start()
        {
            feedbackMenuEntry.SetActive(ConfigurationManager.Current.offerFeedback);
            authorLoginMenuEntry.SetActive(ConfigurationManager.Current.offerAuthorLogin);
        }

    }
}