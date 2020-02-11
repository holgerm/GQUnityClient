using GQ.Client.Conf;
using GQ.Client.Util;
using UnityEngine;
using UnityEngine.UI;

public class ShowOnlyLocalQuestsOption : MonoBehaviour {


    public Toggle toggle;

    public void Start()
    {
        toggle.isOn = ConfigurationManager.Current.ShowOnlyLocalQuests;
    }

    public void OnValueChange(bool newValue)
    {
        Author.ShowOnlyLocalQuests = newValue;
    }
}
