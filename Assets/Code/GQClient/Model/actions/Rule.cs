using System;
using System.Reflection;
using System.Xml;
using Code.GQClient.Err;
using Code.GQClient.Model.gqml;
using Code.GQClient.Util;

namespace Code.GQClient.Model.actions
{
    public class Rule : ActionList
    {

        #region Structure
        /// <summary>
        /// Reads the xml within a given rule element until it finds an action element. 
        /// It then delegates further parsing to the specific action subclass depending on the actions type attribute.
        /// </summary>
        public Rule(System.Xml.XmlReader reader)
        {
            GQML.AssertReaderAtStart(reader, GQML.RULE);

            string ruleName = reader.LocalName;

            if (reader.IsEmptyElement)
            {
                reader.Read();
                return;
            }

            // consume the starting rule element:
            reader.Read();

            while (!GQML.IsReaderAtEnd(reader, GQML.RULE))
            {

                if (GQML.IsReaderAtStart(reader, GQML.ACTION))
                {
                    string actionName = reader.GetAttribute(GQML.ACTION_TYPE);
                    actionName = TextHelper.FirstLetterToUpper(actionName);
                    if (actionName == null)
                    {
                        Log.SignalErrorToDeveloper("Action without type attribute found.");
                        reader.Skip();
                        continue;
                    }

                    // Determine the full name of the according action sub type (e.g. GQ.Client.Model.XML.SetVariableAction) 
                    //		where SetVariable is taken form ath type attribute of the xml action element.
                    string ruleTypeFullName = this.GetType().FullName;
                    int lastDotIndex = ruleTypeFullName.LastIndexOf(".");
                    string modelNamespace = ruleTypeFullName.Substring(0, lastDotIndex);
                    Type actionType = Type.GetType(string.Format("{0}.Action{1}", modelNamespace, actionName));

                    if (actionType == null)
                    {
                        Log.SignalErrorToDeveloper(
                            "No Implementation for Action Type {0} found at line {1} pos {2}",
                            actionName,
                            ((IXmlLineInfo)reader).LineNumber,
                            ((IXmlLineInfo)reader).LinePosition
                            );
                        reader.Skip();
                        continue;
                    }

                    // get right constructor for page type:
                    ConstructorInfo constructorInfoObj = actionType.GetConstructor(new Type[] { typeof(XmlReader) });
                    if (constructorInfoObj == null)
                    {
                        Log.SignalErrorToDeveloper(
                            "Action {0} misses a Constructor for creating the model from XmlReader.",
                            actionName);
                    }
                    Action action = (Action)constructorInfoObj.Invoke(new object[] { reader });
                    action.Parent = this;
                    containedActions.Add(action);
                }
                else
                {
                    Log.SignalErrorToDeveloper(
                        "Unexcpected xml {0} named {1} inside rule found at line {2} pos {3}",
                        reader.NodeType,
                        reader.LocalName,
                        ((IXmlLineInfo)reader).LineNumber,
                        ((IXmlLineInfo)reader).LinePosition);
                    reader.Read();
                }
            }

            GQML.AssertReaderAtEnd(reader, GQML.RULE);
            reader.Read();
        }

        #endregion


    }
}
