using System.Collections;
using System.Collections.Generic;
using GQ.Client.Util;
using UnityEngine;
using UnityEngine.UI;

namespace GQ.Client.UI
{

    public class ShowHiddenQuestsOption : MonoBehaviour
    {

        public Toggle toggle;

        public void Start()
        {
            toggle.isOn = Base.Instance.ShowHiddenQuests;
        }

        public void OnValueChange(bool newValue)
        {
            Base.Instance.ShowHiddenQuests = newValue;
        }
    }
}