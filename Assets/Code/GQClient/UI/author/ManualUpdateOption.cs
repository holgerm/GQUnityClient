using System.Collections;
using System.Collections.Generic;
using GQ.Client.Conf;
using GQ.Client.Model;
using GQ.Client.Util;
using UnityEngine;
using UnityEngine.UI;

namespace GQ.Client.UI
{

    public class ManualUpdateOption : MonoBehaviour
    {

        public Toggle toggle;

        public void Start()
        {
            toggle.isOn = ConfigurationManager.Current.OfferManualUpdate4QuestInfos;
        }

        public void OnValueChange(bool newValue)
        {
            Author.OfferManualUpdate = newValue;
        }
    }
}