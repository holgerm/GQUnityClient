using GQ.Client.Err;
using System.Xml;
using GQ.Client.Util;

namespace GQ.Client.Model
{

    public class ActionSetVariable : ActionAbstractWithVar
    {
        #region Structure
        public ActionSetVariable(XmlReader reader) : base(reader) { }

        protected IExpression valueExpression = null;

        /// <summary>
        /// Called with the reader at the value element.
        /// </summary>
        /// <param name="reader">Reader.</param>
        protected override void ReadContent(XmlReader reader)
        {
            GQML.AssertReaderAtStart(reader, GQML.ACTION_SETVARIABLE_VALUE);

            string containingElementName = reader.LocalName;
            switch (reader.LocalName)
            {
                case GQML.ACTION_SETVARIABLE_VALUE:
                    // go into the content to the next element which is the expression:
                    while (!GQML.IsExpressionType(reader.LocalName))
                    {
                        reader.Read();
                    }
                    valueExpression = ExpressionHelper.ParseSingleExpressionFromXML(reader);
                    break;
                default:
                    base.ReadContent(reader);
                    break;
            }

            GQML.AssertReaderAtEnd(reader, GQML.ACTION_SETVARIABLE_VALUE);
            // comsume the EndElement </value>
            reader.Read();
        }
        #endregion


        #region Functions
        public override void Execute()
        {
            if (VarName == null)
            {
                Log.SignalErrorToDeveloper("SetVariable Action without varname can not be executed. (Ignored)");
                return;
            }

            if (valueExpression == null)
            {
                Log.SignalErrorToDeveloper("SetVariable Action without value can not be executed. (Ignored)");
                return;
            }

            Variables.SetVariableValue(VarName.MakeReplacements(), valueExpression.Evaluate());
        }
        #endregion
    }
}
