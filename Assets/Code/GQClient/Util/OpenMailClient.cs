﻿using Code.GQClient.Conf;
using UnityEngine;

namespace Code.GQClient.Util
{

    public class OpenMailClient : Code.QM.Util.OpenMailClient
    {

        // Use this for initialization
        void Start()
        {
            mailto = "gq_" + Config.Current.id + "@quest-mill.com";
            subject = "Feedback zur App " + Config.Current.name;
            string version = Resources.Load<TextAsset>("buildtime").text;
            body = string.Format("Ich benutze Version {0} der App {1} und möchte dazu etwas mitteilen:", version, Config.Current.name);
        }

    }
}
