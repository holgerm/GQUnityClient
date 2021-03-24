using Code.GQClient.Conf;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.author
{

    public class ShowHiddenQuestsOption : MonoBehaviour
    {

        public Toggle toggle;

        public void Start()
        {
            toggle.isOn = Config.Current.ShowHiddenQuests;
        }

        public void OnValueChange(bool newValue)
        {
            Author.ShowHiddenQuests = newValue;
        }
    }
}