using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GQ.Client.Model
{

	public class ActionVibrate : ActionAbstract {

		public override void Execute ()
		{
			#if UNITY_IOS || UNITY_ANDROID
			Handheld.Vibrate ();
			#endif
		}


	}
}
