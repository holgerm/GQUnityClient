using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GQ.Client.Model
{

	public class ActionVibrate : ActionAbstract {

		public override void Execute ()
		{
			Handheld.Vibrate ();
		}


	}
}
