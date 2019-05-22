using System;
using GQ.Client.Conf;
using GQ.Client.Model;
using GQ.Client.Util;
using QM.Util;
using UnityEngine;
using UnityEngine.UI;

public class ShowDeleteOption : MonoBehaviour {

    public Toggle toggle;

    public void Start()
    {
        toggle.isOn = Author.ShowDeleteOptionForLocalQuests;
    }

    public void OnValueChange(bool newValue)
    {
        if (Author.ShowDeleteOptionForLocalQuests == newValue)
            return;

        Author.ShowDeleteOptionForLocalQuests = newValue;
        if (DeleteOptionVisibilityChanged != null)
        {
            DeleteOptionVisibilityChanged();
        }
    }

    public static event VoidToVoid DeleteOptionVisibilityChanged;
}
