using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

namespace GQ.Client.Model
{

	public class TextExpression : SimpleExpression
	{

		#region Structure

		protected override void setValue (string valueAsString)
		{
			value = new Value (valueAsString, Value.Type.Text);
		}


		#endregion


	}
}
