using System.Xml;
using UnityEngine;

namespace GQ.Client.Model
{
    public class ActionVibrate : Action
     {

        public ActionVibrate(XmlReader reader) : base(reader) { }

        public override void Execute ()
		{
			#if UNITY_IOS || UNITY_ANDROID
			Handheld.Vibrate ();
			#endif
		}
	}
}
