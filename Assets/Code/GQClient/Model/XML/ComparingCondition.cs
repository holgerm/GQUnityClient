using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.Generic;

namespace GQ.Client.Model.XML
{

	public abstract class ComparingCondition : ICondition, IXmlSerializable
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
		/// Stores the structure of the comparison set for the represented xml comparison cindition element, 
		/// e.g. a "gt" or a "leq".
		/// </summary>
		protected List<IExpression> containedExpressions = new List<IExpression> ();

		/// <summary>
		/// Reads one xml element for a comparative condition, like equal, greaterthan or lessorequal. 
		/// It consumes the whole element with all contents including the closing end_element.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public void ReadXml (System.Xml.XmlReader reader)
		{
			XmlSerializer serializer;

			string conditionName = reader.LocalName;

			reader.MoveToContent ();

			XmlRootAttribute xmlRootAttr = new XmlRootAttribute ();
			xmlRootAttr.IsNullable = true;

			bool currentNodeStillToBeConsumed = false;

			while (currentNodeStillToBeConsumed || reader.Read ()) {

				// if we reach the end of this condition element we are ready to leave this method.
				if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals (conditionName)) {
					break;
				}

				if (reader.NodeType != XmlNodeType.Element) {
					continue;
				}
					
				xmlRootAttr.ElementName = reader.LocalName;

				switch (reader.LocalName) {
				// COMPOUND CONDITIONS:
				case GQML.VARIABLE:
					serializer = new XmlSerializer (typeof(VariableExpression), xmlRootAttr);
					containedExpressions.Add ((VariableExpression)serializer.Deserialize (reader));
					currentNodeStillToBeConsumed = true;
					break;
				case GQML.BOOL:
					serializer = new XmlSerializer (typeof(BoolExpression), xmlRootAttr);
					containedExpressions.Add ((BoolExpression)serializer.Deserialize (reader));
					currentNodeStillToBeConsumed = true;
					break;
				case GQML.NUMBER:
					serializer = new XmlSerializer (typeof(NumberExpression), xmlRootAttr);
					containedExpressions.Add ((NumberExpression)serializer.Deserialize (reader));
					currentNodeStillToBeConsumed = true;
					break;
				case GQML.STRING:
					serializer = new XmlSerializer (typeof(TextExpression), xmlRootAttr);
					containedExpressions.Add ((TextExpression)serializer.Deserialize (reader));
					currentNodeStillToBeConsumed = true;
					break;
				default:
					Debug.LogError ("Unknown expression type found: " + reader.LocalName);
					break;
				}
			}
		}

		#endregion


		#region Function

		public virtual bool IsFulfilled ()
		{
			// handle case with no expressions at all, i.e. empty list:
			if (containedExpressions.Count == 0)
				return isFulfilledEmptyComparison ();

			// handle case with only one expression in list:
			IExpression firstExpr = containedExpressions [0];
			if (containedExpressions.Count == 1)
				return isFulfilledCompare (firstExpr);

			// handle case with two or more expressions in list:
			IExpression secondExpr;
			bool fulfilled = true;
			int i = 1;

			while (fulfilled && containedExpressions.Count > i) {
				secondExpr = containedExpressions [i++];
				fulfilled &= isFulfilledCompare (firstExpr, secondExpr);
				firstExpr = secondExpr;
			}
			return fulfilled;
		}

		protected abstract bool isFulfilledEmptyComparison ();

		protected abstract bool isFulfilledCompare (IExpression expression);

		protected abstract bool isFulfilledCompare (IExpression firstExpression, IExpression secondExpression);

		#endregion
	}
}
