using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using GQ.Client.Err;

namespace GQ.Client.Model
{

	public class ActionAddToScore : ActionAbstract {

		#region Structure

		protected int scoreToAdd;

		protected override void ReadAttributes (XmlReader reader)
		{
			scoreToAdd = GQML.GetIntAttribute (GQML.ACTION_SETVARIABLE_VALUE, reader);
		}

		#endregion


		#region Functions

		public override void Execute ()
		{
			int oldScore = Variables.GetValue (GQML.VAR_SCORE).AsInt ();

			Variables.SetVariableValue (GQML.VAR_SCORE, new Value (oldScore + scoreToAdd));
			Debug.Log (string.Format("ADD {0} to {1} SCORE: {2}", scoreToAdd, oldScore, Variables.GetValue(GQML.VAR_SCORE).AsInt()).Yellow());
		}

		#endregion
		}
}
