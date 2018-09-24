using System.Collections;
using System.Collections.Generic;
using GQ.Client.Conf;
using UnityEngine;

namespace GQ.Client.Util
{

    public class OpenMailClient : QM.Util.OpenMailClient
    {

        // Use this for initialization
        void Start()
        {
            mailto = ConfigurationManager.Current.id + "@quest-mill.com";
            subject = "Feedback zur App " + ConfigurationManager.Current.name;
            string version = Resources.Load<TextAsset>("buildtime").text;
            body = string.Format("Ich benutze Version {0} eurer App {1} und möchte euch dazu etwas mitteilen:", version, ConfigurationManager.Current.name);
        }

    }
}
