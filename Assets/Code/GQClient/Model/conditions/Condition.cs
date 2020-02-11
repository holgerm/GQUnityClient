using System;
using System.Reflection;
using System.Xml;
using Code.GQClient.Err;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Util;
using UnityEngine;

namespace Code.GQClient.Model.conditions
{
    /// <summary>
    /// Condition element may only contain exactly one conrete condition element, 
    /// like a compund condition (OR, AND, NOT) or a comparing condition (e.g. LT, EQ, etc.).
    /// </summary>
    public class Condition : I_GQML, IConditionContainer, ICondition {

		#region Structure

		public I_GQML Parent { set; get; }

		public Quest Quest {
			get {
				return Parent.Quest;
			}
		}	

		public System.Xml.Schema.XmlSchema GetSchema ()
		{
			return null;
		}

		protected ICondition condition;

		/// <summary>
		/// Reader must be at the start element of condition.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public Condition(XmlReader reader)
		{
			GQML.AssertReaderAtStart (reader, GQML.CONDITION);

            if (reader.IsEmptyElement)
            {
                reader.Read();
                condition = null;
                return;
            }

            // consume the starting condition element:
            reader.Read ();

			if (reader.NodeType == XmlNodeType.Element && GQML.IsConditionType (reader.LocalName)) {
				ReadConcreteCondition (reader);
			} else {
                Log.SignalErrorToDeveloper (
					"Unexpected xml {0} {1} found in condition element in line {2} at position {3}",
					reader.NodeType,
					reader.LocalName,
                    ((IXmlLineInfo)reader).LineNumber,
                    ((IXmlLineInfo)reader).LinePosition);
				while (
					!GQML.IsReaderAtEnd (reader, GQML.CONDITION)
					&& !(reader.NodeType == XmlNodeType.None)) {
					reader.Read ();
				}
			}

			// consume condition end element:
			reader.Read ();
		}

		private void ReadConcreteCondition(XmlReader reader) {
			// Determine the full name of the contained condition type (e.g. GQ.Client.Model.XML.ConditionOr) 
			string ruleTypeFullName = this.GetType ().FullName;
			int lastDotIndex = ruleTypeFullName.LastIndexOf (".");
			string modelNamespace = ruleTypeFullName.Substring (0, lastDotIndex);
			string conditionTypeName = TextHelper.FirstLetterToUpper (reader.LocalName);
			Type conditionType = 
				Type.GetType (
					string.Format ("{0}.Condition{1}", modelNamespace, conditionTypeName));

			if (conditionType == null) {
				Log.SignalErrorToDeveloper ("No Implementation for Condition Type {0} found.", conditionTypeName);
				reader.Skip ();
				return;
			}

            ConstructorInfo constructorInfoObj = conditionType.GetConstructor(new Type[] { typeof(XmlReader) });
            if (constructorInfoObj == null)
            {
                Log.SignalErrorToDeveloper("Condition {0} misses a Constructor for creating the model from XmlReader.", conditionTypeName);
            }
            condition = (ICondition)constructorInfoObj.Invoke(new object[] { reader });

            condition.Parent = this;
		}

		public void WriteXml (System.Xml.XmlWriter writer)
		{
			Debug.LogWarning ("WriteXML not implemented for " + GetType ().Name);
		}

		#endregion


		#region Functions

		public bool IsFulfilled() {
			return (condition == null) ? true : condition.IsFulfilled ();
		}

		#endregion
	}
}
