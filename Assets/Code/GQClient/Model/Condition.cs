using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using GQ.Client.Err;
using System;
using GQ.Client.Util;

namespace GQ.Client.Model
{
	/// <summary>
	/// Condition element may only contain exactly one conrete condition element, 
	/// like a compund condition (OR, AND, NOT) or a comparing condition (e.g. LT, EQ, etc.).
	/// </summary>
	public class Condition : I_GQML, IConditionContainer, ICondition, IXmlSerializable {

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
		public void ReadXml (System.Xml.XmlReader reader)
		{
			GQML.AssertReaderAtStart (reader, GQML.CONDITION);

			// consume the starting condition element:
			reader.Read ();

			if (reader.NodeType == XmlNodeType.Element && GQML.IsConditionType (reader.LocalName)) {
				ReadConcreteCondition (reader);
			} else {
				Log.SignalErrorToDeveloper (
					"Unexpected xml {0} {1} found in condition element",
					reader.NodeType,
					reader.LocalName);
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

			XmlRootAttribute xmlRootAttr = new XmlRootAttribute ();
			xmlRootAttr.IsNullable = true;
			xmlRootAttr.ElementName = reader.LocalName;

			XmlSerializer serializer = new XmlSerializer (conditionType, xmlRootAttr);
			condition = (ICondition)serializer.Deserialize (reader);
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
