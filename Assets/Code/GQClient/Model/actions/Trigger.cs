using System.Collections.Generic;
using System.Xml;
using Code.GQClient.Err;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;

namespace Code.GQClient.Model.actions
{
    public class Trigger : IActionListContainer
    {

        #region Structure
        public ITriggerContainer Parent { get; internal set; }

        public Quest Quest
        {
            get
            {
                if (Parent == null) return null;
                return Parent.Quest;
            }
        }

        /// <summary>
        /// The contained actions.
        /// </summary>
        protected List<Rule> containedRules = new List<Rule>();

        public bool IsEmptyOrEndGameOnly()
        {
            foreach (Rule curRule in containedRules)
            {
                foreach (Action curAction in curRule.containedActions)
                {
                    if (!(curAction is ActionEndGame))
                        // some other action found than EndGame:
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Reads the xml within a given rule element until it finds an action element. 
        /// It then delegates further parsing to the specific action subclass depending on the actions type attribute.
        /// </summary>
        public Trigger(System.Xml.XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                reader.Read();
                return;
            }

            while (reader.NodeType != XmlNodeType.Element || !isTriggerType(reader.LocalName))
            {
                // skip this unexpected inner node
                Log.SignalErrorToDeveloper(
                    "Unexpected xml {0} {1} found in condition element in line {2} at position {3}",
                    reader.NodeType,
                    reader.LocalName,
                    ((IXmlLineInfo)reader).LineNumber,
                    ((IXmlLineInfo)reader).LinePosition);
                reader.Read();
            }

            string triggerName = reader.LocalName;

            // consume starting Trigger element:				
            reader.Read();

            while (!GQML.IsReaderAtEnd(reader, triggerName))
            {
                if (reader.NodeType != XmlNodeType.Element)
                    continue;

                if (GQML.IsReaderAtStart(reader, GQML.RULE))
                {
                    Rule rule = new Rule(reader);
                    rule.Parent = this;
                    containedRules.Add(rule);
                }
                else if (reader.NodeType == XmlNodeType.None)
                {
                    Log.WarnDeveloper("Trigger {0} has incorrect xml.", triggerName);
                    return;
                }
            }

            GQML.AssertReaderAtEnd(reader, triggerName);
            reader.Read();
        }

        protected Trigger() { }

        private static List<string> triggerNodeNames =
            new List<string>(
                new string[] {
                    GQML.ON_START,
                    GQML.ON_SUCCESS, GQML.ON_FAIL,
                    GQML.ON_END,
                    GQML.ON_ENTER, GQML.ON_LEAVE, GQML.ON_TAP,
                    GQML.ON_READ,
                    GQML.ON_FOCUS
                });


        internal static bool isTriggerType(string xmlTriggerCandidate)
        {
            return triggerNodeNames.Contains(xmlTriggerCandidate);
        }
        #endregion

        #region Functions
        public virtual void Initiate()
        {
            foreach (Rule rule in containedRules)
            {
                rule.Apply();
            }
        }
        #endregion

        #region Null
        public static readonly Trigger Null = new NullTrigger();

        private class NullTrigger : Trigger
        {

            internal NullTrigger()
            {
            }

            public override void Initiate()
            {
            }
        }
        #endregion
    }
}
