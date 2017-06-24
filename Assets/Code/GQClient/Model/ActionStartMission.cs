using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using System.Xml.Serialization;
using GQ.Client.Err;
using System.Xml;

namespace GQ.Client.Model
{

	public class ActionStartMission : ActionAbstract
	{
		#region Structure

		protected int id;

		public int Id {
			get {
				return id;
			}
			protected set {
				id = value;
			}
		}


		protected bool allowReturn;

		public bool AllowReturn {
			get {
				return allowReturn;
			}
			protected set {
				allowReturn = value;
			}
		}

		#endregion


		#region Functions

		public override void Execute ()
		{
		}

		#endregion
	}
}
