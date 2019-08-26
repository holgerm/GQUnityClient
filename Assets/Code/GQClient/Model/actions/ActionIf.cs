using System.Collections.Generic;
using System.Xml;
using GQ.Client.Err;
using System;
using System.Reflection;

namespace GQ.Client.Model
{
	public class ActionIf : Action
	{

        #region Structure
        protected Condition condition;
		protected ActionList thenActions = new ActionList ();
		protected ActionList elseActions = new ActionList ();

        /// <summary>
        /// Is called with the reader positioned at the action (if) start element. It will consume the whole action.
        /// </summary>
        public ActionIf(XmlReader reader) : base(reader) {
        }

        protected override void CheckStart(XmlReader reader)			
        {
            base.CheckStart(reader);

			if (reader.IsEmptyElement) {
				Log.SignalErrorToDeveloper ("If Action found with no content.");
				reader.Read ();
				return;
			}
        }

        protected override void ReadContent(XmlReader reader)
        {
            ReadConditionElement (reader);

			if (reader.NodeType == XmlNodeType.Element && reader.LocalName.Equals (GQML.THEN)) {
				ReadThenOrElseElement (reader);
 			}

            if (reader.NodeType == XmlNodeType.Element && reader.LocalName.Equals (GQML.ELSE)) {
				ReadThenOrElseElement (reader);
			} 

			if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals (GQML.ACTION)) {
				return;
			}

            Log.SignalErrorToDeveloper(
                "Unexpected xml {0} {1} found in condition element in line {2} at position {3}",
                reader.NodeType,
                reader.LocalName,
                ((IXmlLineInfo)reader).LineNumber,
                ((IXmlLineInfo)reader).LinePosition);
            reader.Skip ();

            // for further content we call super method:
            base.ReadContent(reader);
		}


		public void ReadConditionElement (System.Xml.XmlReader reader)
		{
			if (!GQML.IsReaderAtStart (reader, GQML.CONDITION)) {
				condition = new Condition (reader);
				return;
			}

			// normal case when we found a condition:
			condition = new Condition(reader);
			condition.Parent = this;
        }

        public void ReadThenOrElseElement (System.Xml.XmlReader reader)
		{
			string branchName = reader.LocalName;

			// consume starting branch element (then or else) if it is NOT EMPTY:	
            if (!reader.IsEmptyElement)
			    reader.Read ();

			while (!GQML.IsReaderAtEnd (reader, branchName)) {

				if (reader.NodeType == XmlNodeType.Element && reader.LocalName.Equals (GQML.ACTION)) {
					string actionName = reader.GetAttribute (GQML.ACTION_TYPE);
					if (actionName == null) {
						Log.SignalErrorToDeveloper ("Action without type attribute found.");
						reader.Skip ();
						continue;
					}

					// Determine the full name of the according action sub type (e.g. GQ.Client.Model.XML.SetVariableAction) 
					//		where SetVariable is taken form ath type attribute of the xml action element.
					string ruleTypeFullName = this.GetType ().FullName;
					int lastDotIndex = ruleTypeFullName.LastIndexOf (".");
					string modelNamespace = ruleTypeFullName.Substring (0, lastDotIndex);
					Type actionType = Type.GetType (string.Format ("{0}.Action{1}", modelNamespace, actionName));

					if (actionType == null) {
						Log.SignalErrorToDeveloper ("No Implementation for Action Type {0} found.", actionName);
						reader.Skip ();
						continue;
					}

                    ConstructorInfo constructorInfoObj = actionType.GetConstructor(new Type[] { typeof(XmlReader) });
                    if (constructorInfoObj == null)
                    {
                        Log.SignalErrorToDeveloper("Action {0} misses a Constructor for creating the model from XmlReader.", actionName);
                    }
                    Action action = (Action)constructorInfoObj.Invoke(new object[] { reader });

                    if ("then".Equals (branchName)) {
						thenActions.containedActions.Add (action);
						action.Parent = thenActions;
						thenActions.Parent = this;
					}
					else {
						elseActions.containedActions.Add (action);
						action.Parent = elseActions;
						elseActions.Parent = this;
					}
				} else {
					Log.SignalErrorToDeveloper ("Unexcpected xml {0} named {1} inside rule found.", reader.NodeType, reader.LocalName);
					reader.Read ();
				}
			} 

			// consume the end element of the then or else element:
			reader.Read();
		}

		private static List<string> triggerNodeNames = 
			new List<string> (
				new string[] { GQML.ON_START, GQML.ON_SUCCESS, GQML.ON_FAIL, GQML.ON_END, GQML.ON_ENTER, GQML.ON_LEAVE, GQML.ON_TAP });


		internal static bool isTriggerType (string xmlTriggerCandidate)
		{
			return triggerNodeNames.Contains (xmlTriggerCandidate);
		}

		#endregion


		#region Functions

		public override void Execute ()
		{
			if (condition.IsFulfilled ()) {
				thenActions.Apply ();
			} else {
				elseActions.Apply ();
			}
		}

		#endregion


		#region Null

		public static readonly Trigger Null = new NullTrigger ();

		private class NullTrigger : Trigger
		{

			internal NullTrigger ()
			{
			}

			public override void Initiate ()
			{
				
			}
		}

		#endregion

	}
}
