using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml;
using GQ.Client.Err;

namespace GQ.Client.Model
{

	[System.Serializable]
	public class CompoundCondition : ICondition, IConditionContainer, IXmlSerializable
	{
		#region Structure

		public I_GQML Parent { get; set; }

		public Quest Quest {
			get {
				return Parent.Quest;
			}
		}

		public System.Xml.Schema.XmlSchema GetSchema ()
		{
			return null;
		}

		public void WriteXml (System.Xml.XmlWriter writer)
		{
			Debug.LogWarning ("WriteXML not implemented for " + GetType ().Name);
		}

		/// <summary>
		/// The contained conditions. In case of the outer most "condition" tag, it will contain only one subcondition. 
		/// But in general (within And or or conditions) it might contain many subconditions.
		/// </summary>
		protected List<ICondition> containedConditions = new List<ICondition> ();

		/// <summary>
		/// Reader is at the condition element when we call this method. 
		/// Reads the xml within a given condition element until it has consumed all elements witihn the condition including the ending element.
		/// </summary>
		public void ReadXml (System.Xml.XmlReader reader)
		{
			if (reader.IsEmptyElement) {
				reader.Read ();
				return;
			}

			string conditionName = reader.LocalName;

			XmlSerializer serializer;
			XmlRootAttribute xmlRootAttr = new XmlRootAttribute ();
			xmlRootAttr.IsNullable = true;

			// consume the begin of this compound condition
			reader.Read ();

			// and start reading the contained conditions:
			while (!GQML.IsReaderAtEnd (reader, conditionName)) {
				// if we reach the end of this condition element we are ready to leave this method.
				if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals (conditionName)) {
					break;
				}
					
				if (reader.NodeType != XmlNodeType.Element)
					continue;

				xmlRootAttr.ElementName = reader.LocalName;

				switch (reader.LocalName) {
				// COMPOUND CONDITIONS:
				case GQML.AND:
					serializer = new XmlSerializer (typeof(ConditionAnd), xmlRootAttr);
					containedConditions.Add ((ConditionAnd)serializer.Deserialize (reader));
					break;
				case GQML.OR:
					serializer = new XmlSerializer (typeof(ConditionOr), xmlRootAttr);
					containedConditions.Add ((ConditionOr)serializer.Deserialize (reader));
					break;
				case GQML.NOT:
					serializer = new XmlSerializer (typeof(ConditionNot), xmlRootAttr);
					containedConditions.Add ((ConditionNot)serializer.Deserialize (reader));
					break;
				// COMPARING CONDITIONS:
				case GQML.EQUAL:
					serializer = new XmlSerializer (typeof(ConditionEq), xmlRootAttr);
					containedConditions.Add ((ConditionEq)serializer.Deserialize (reader));
					break;
				case GQML.GREATER_THAN:
					serializer = new XmlSerializer (typeof(ConditionGt), xmlRootAttr);
					containedConditions.Add ((ConditionGt)serializer.Deserialize (reader));
					break;
				case GQML.GREATER_EQUAL:
					serializer = new XmlSerializer (typeof(ConditionGeq), xmlRootAttr);
					containedConditions.Add ((ConditionGeq)serializer.Deserialize (reader));
					break;
				case GQML.LESS_THAN:
					serializer = new XmlSerializer (typeof(ConditionLt), xmlRootAttr);
					containedConditions.Add ((ConditionLt)serializer.Deserialize (reader));
					break;
				case GQML.LESS_EQUAL:
					serializer = new XmlSerializer (typeof(ConditionLeq), xmlRootAttr);
					containedConditions.Add ((ConditionLeq)serializer.Deserialize (reader));
					break;
				// UNKOWN CASE:
				default:
					Log.SignalErrorToDeveloper ("Unknown condition type found: " + reader.LocalName);
					break;
				}
			}

			// consume end element of this compound condition:
			reader.Read ();
		}

		#endregion


		#region Function

		public virtual bool IsFulfilled ()
		{
			bool allFulfilled = true;
			foreach (ICondition condition in containedConditions) {
				allFulfilled &= condition.IsFulfilled ();
				if (!allFulfilled)
					break;
			}
			return allFulfilled;
		}

		#endregion
	}
}