using System;
using System.Xml;
using Code.GQClient.Err;

namespace Code.GQClient.Model.expressions
{

    public class NumberExpression : SimpleExpression
	{
        #region Structure
        public NumberExpression(XmlReader reader) : base(reader) { }

        protected override void setValue (string valueAsString)
		{
            valueAsString = valueAsString.Trim();

			int valueAsInt;
			if (Int32.TryParse (valueAsString, out valueAsInt)) {
				value = new Value (valueAsString, Value.Type.Integer);
				return;
			}

			double valueAsDouble;
			if (Double.TryParse (valueAsString, out valueAsDouble)) {
				value = new Value (valueAsString, Value.Type.Float);
			} else {
				value = new Value ("0", Value.Type.Integer);
				Log.WarnAuthor ("Tried to store {0} to a num typed value, but that does not work. We store 0 as Integer instead.", valueAsString);
			}
		}
		#endregion
	}
}
