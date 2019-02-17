using GQ.Client.Conf;
using GQ.Client.Model;
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
            toggle.isOn = ConfigurationManager.Current.ShowHiddenQuests;
        }

        public void OnValueChange(bool newValue)
        {
            Author.ShowHiddenQuests = newValue;
        }
    }
}