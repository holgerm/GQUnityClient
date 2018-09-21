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
            body = "Ich möchte euch zu eurer App etwas mitteilen:\n\n";
        }

    }
}
