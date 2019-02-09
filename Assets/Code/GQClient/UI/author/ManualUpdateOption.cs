using System.Collections;
using System.Collections.Generic;
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
            toggle.isOn = Author.offerManualUpdate;
        }

        public void OnValueChange(bool newValue)
        {
            Author.offerManualUpdate = newValue;
            Debug.Log("ManualUpdateOption NEWVALUE: " + newValue);
        }
    }
}