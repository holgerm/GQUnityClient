using Code.GQClient.Conf;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.author
{
    public class ShowOnlyLocalQuestsOption : MonoBehaviour {


        public Toggle toggle;

        public void Start()
        {
            toggle.isOn = Config.Current.ShowOnlyLocalQuests;
        }

        public void OnValueChange(bool newValue)
        {
            Author.ShowOnlyLocalQuests = newValue;
        }
    }
}
