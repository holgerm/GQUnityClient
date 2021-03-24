using Code.GQClient.Conf;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.author
{

    public class ManualUpdateOption : MonoBehaviour
    {

        public Toggle toggle;

        public void Start()
        {
            toggle.isOn = Config.Current.OfferManualUpdate4QuestInfos;
        }

        public void OnValueChange(bool newValue)
        {
            Author.OfferManualUpdate = newValue;
        }
    }
}