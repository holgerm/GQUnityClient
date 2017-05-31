using UnityEngine;
using System.Collections;

namespace GQ.Client.Model.XML
{

	public class VariableExpression : Expression
	{

		#region Structure

		protected override void setValue (string valueAsString)
		{
			value = new Value (valueAsString, Value.Type.VariableName);
		}

		#endregion

	}
}
