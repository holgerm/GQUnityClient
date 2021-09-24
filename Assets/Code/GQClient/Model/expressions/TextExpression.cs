using System.Xml;
using Code.GQClient.Util;
using UnityEngine;

namespace Code.GQClient.Model.expressions
{
    public class TextExpression : SimpleExpression
    {
        #region Structure

        public TextExpression(XmlReader reader) : base(reader)
        {
        }

        protected override void setValue(string valueAsString)
        {
            value = new Value(valueAsString, Value.Type.Text);
        }

        #endregion


        #region Runtime

        public override Value Evaluate()
        {
            string textVal = value.AsString();
            string replacedTextVal = textVal.MakeReplacements();
            if (textVal.Equals(replacedTextVal))
                return value;
            else
                return new Value(replacedTextVal, Value.Type.Text);
        }

        #endregion
    }
}