using System.Xml;

namespace GQ.Client.Model
{
    public class TextExpression : SimpleExpression
	{
        #region Structure
        public TextExpression(XmlReader reader) : base(reader) { }

        protected override void setValue (string valueAsString)
		{
			value = new Value (valueAsString, Value.Type.Text);
		}


		#endregion


	}
}
