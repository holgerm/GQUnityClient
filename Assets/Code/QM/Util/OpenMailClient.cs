using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QM.Util
{

    public class OpenMailClient : MonoBehaviour
    {

        public string mailto = "me@example.com";
        public string subject = "My Subject";
        public string body = "";

        public void DoIt()
        {
            Application.OpenURL("mailto:" + mailto + "?subject=" + MyEscapeURL(subject) + "&body=" + MyEscapeURL(body));
        }

        static string MyEscapeURL(string url)
        {
            return WWW.EscapeURL(url).Replace("+", "%20");
        }
    }
}
