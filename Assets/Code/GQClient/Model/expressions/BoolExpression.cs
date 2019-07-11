using System.Xml;

namespace GQ.Client.Model
{

    public class BoolExpression : SimpleExpression
	{
        #region Structure
        public BoolExpression(XmlReader reader) : base(reader) { }

        protected override void setValue (string valueAsString)
		{
            valueAsString = valueAsString.Trim();

            value = new Value (valueAsString, Value.Type.Bool);
		}
		#endregion
	}
}
