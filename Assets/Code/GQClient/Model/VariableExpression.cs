using UnityEngine;
using System.Collections;

namespace GQ.Client.Model
{

	public class VariableExpression : SimpleExpression
	{

		#region Structure

		protected override void setValue (string valueAsString)
		{
			value = new Value (valueAsString, Value.Type.VariableName);
		}

		#endregion

	}
}
