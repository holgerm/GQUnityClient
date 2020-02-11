using Code.GQClient.Conf;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.author
{

    public class ShowEmptyMenuEntriesOption : MonoBehaviour
    {

        public Toggle toggle;

        public void Start()
        {
            toggle.isOn = ConfigurationManager.Current.ShowEmptyMenuEntries;
        }

        public void OnValueChange(bool newValue)
        {
            Author.ShowEmptyMenuEntries = newValue;
        }
    }
}