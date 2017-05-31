using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

namespace GQ.Client.Model.XML
{

	public class BoolExpression : Expression
	{
		#region Structure

		protected override void setValue (string valueAsString)
		{
			value = new Value (valueAsString, Value.Type.Bool);
		}

		#endregion

	}
}
