using System.Collections.Generic;
using System.Xml;
using GQ.Client.Err;

namespace GQ.Client.Model
{

    public class ExpressionHelper
    {
        /// <summary>
        /// Reader is at the surrounding element that contains a list of expressions.
        /// 
        /// Reads one xml element surrounding a list of expressions, for example a comparative condition, like equal, greaterthan or lessorequal. 
        /// It consumes the whole surrounding element with all contents including the closing end_element.
        /// </summary>
        /// <param name="reader">Reader.</param>
        public static List<IExpression> ParseExpressionListFromXML(System.Xml.XmlReader reader)
        {
            string surroundingElementName = reader.LocalName;
            GQML.AssertReaderAtStart(reader, surroundingElementName);

            // consume start tag of surrounding element:
            reader.Read();

            List<IExpression> containedExpressions = new List<IExpression>();

            while (!GQML.IsReaderAtEnd(reader, surroundingElementName))
            {
                
                if (reader.LocalName == "help")
                {
                    reader.Read();
                    continue;
                }

                if (reader.NodeType == XmlNodeType.Element && GQML.IsExpressionType(reader.LocalName))
                {
                    IExpression expr = ParseSingleExpressionFromXML(reader);
                    if (expr != null)
                    {
                        containedExpressions.Add(expr);
                    }
                }
                else
                {
                    // skip this unexpected inner node
                    Log.SignalErrorToDeveloper(
                        "Unexpected xml {0} {1} found in condition element in line {2} at position {3}",
                        reader.NodeType,
                        reader.LocalName,
                        ((IXmlLineInfo)reader).LineNumber,
                        ((IXmlLineInfo)reader).LinePosition);
                    reader.Read();
                    continue;
                }
            }

            return containedExpressions;
        }

        /// Is called with the reader at the expression element.
        /// This method consumes the complete expression xml subtree and puts the reader directly after that.
        public static IExpression ParseSingleExpressionFromXML(XmlReader reader)
        {
            string expressionName = reader.LocalName;

            if (reader.NodeType != XmlNodeType.Element || !GQML.IsExpressionType(reader.LocalName))
            {
                Log.SignalErrorToDeveloper(
                    "Instead of an xml element of an expression we got an {0} with name {1}",
                    reader.NodeType.ToString(),
                    reader.LocalName
                );
                reader.Skip();
                return null;
            }

            IExpression resultExpression = null;

            switch (reader.LocalName)
            {
                // COMPOUND CONDITIONS:
                case GQML.VARIABLE:
                    resultExpression = new VarExpression(reader);
                    break;
                case GQML.BOOL:
                    resultExpression = new BoolExpression(reader);
                    break;
                case GQML.NUMBER:
                    resultExpression = new NumberExpression(reader);
                    break;
                case GQML.STRING:
                    resultExpression = new TextExpression(reader);
                    break;
                default:
                    Log.SignalErrorToDeveloper("Expression found with unknown type {0}", reader.LocalName);
                    break;
            }

            return resultExpression;
        }
    }
}