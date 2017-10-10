using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using System.Xml.Serialization;
using GQ.Client.Err;
using System.Xml;

namespace GQ.Client.Model
{

	public class ActionEndGame : ActionAbstract
	{

		#region Functions

		public override void Execute ()
		{
			Quest.End ();
		}

		#endregion
	}
}
