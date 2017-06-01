using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml;

namespace GQ.Client.Model.XML
{

	[System.Serializable]
	public class CompoundCondition : ICondition, IXmlSerializable
	{
		#region Structure

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
		/// Reads the xml within a given condition element until it has consumed all elements witihn the condition including the ending element.
		/// </summary>
		public void ReadXml (System.Xml.XmlReader reader)
		{
			XmlSerializer serializer;

			string conditionName = reader.LocalName;

			reader.MoveToContent ();

			XmlRootAttribute xmlRootAttr = new XmlRootAttribute ();
			xmlRootAttr.IsNullable = true;

			bool currentNodeStillToBeConsumed = false;

			while (currentNodeStillToBeConsumed || reader.Read ()) {
				Debug.Log ("COMP_COND: node type: " + reader.NodeType + " name: " + reader.LocalName);

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
					serializer = new XmlSerializer (typeof(AndCondition), xmlRootAttr);
					containedConditions.Add ((AndCondition)serializer.Deserialize (reader));
					currentNodeStillToBeConsumed = true;
					break;
				case GQML.OR:
					serializer = new XmlSerializer (typeof(OrCondition), xmlRootAttr);
					containedConditions.Add ((OrCondition)serializer.Deserialize (reader));
					currentNodeStillToBeConsumed = true;
					break;
				case GQML.NOT:
					serializer = new XmlSerializer (typeof(NotCondition), xmlRootAttr);
					containedConditions.Add ((NotCondition)serializer.Deserialize (reader));
					currentNodeStillToBeConsumed = true;
					break;
				// COMPARING CONDITIONS:
				case GQML.EQUAL:
					serializer = new XmlSerializer (typeof(EqualCondition), xmlRootAttr);
					containedConditions.Add ((EqualCondition)serializer.Deserialize (reader));
					currentNodeStillToBeConsumed = true;
					break;
				case GQML.GREATER_THAN:
					serializer = new XmlSerializer (typeof(GreaterCondition), xmlRootAttr);
					containedConditions.Add ((GreaterCondition)serializer.Deserialize (reader));
					currentNodeStillToBeConsumed = true;
					break;
				case GQML.GREATER_EQUAL:
					serializer = new XmlSerializer (typeof(GreaterEqualCondition), xmlRootAttr);
					containedConditions.Add ((GreaterEqualCondition)serializer.Deserialize (reader));
					currentNodeStillToBeConsumed = true;
					break;
				case GQML.LESS_THAN:
					serializer = new XmlSerializer (typeof(LessCondition), xmlRootAttr);
					containedConditions.Add ((LessCondition)serializer.Deserialize (reader));
					currentNodeStillToBeConsumed = true;
					break;
				case GQML.LESS_EQUAL:
					serializer = new XmlSerializer (typeof(LessEqualCondition), xmlRootAttr);
					containedConditions.Add ((LessEqualCondition)serializer.Deserialize (reader));
					currentNodeStillToBeConsumed = true;
					break;
				// UNKOWN CASE:
				default:
					Debug.LogError ("Unknown condition type found: " + reader.LocalName);
					break;
				}
			}

			return;
		}

		#endregion


		#region Function

		/// <summary>
		/// True if all contained condition are fulfilled (which is also the case if no condition at all is included). Computed in a lazy manner.
		/// </summary>
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