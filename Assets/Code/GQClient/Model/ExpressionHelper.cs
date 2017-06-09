using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using GQ.Client.Err;

namespace GQ.Client.Model
{

	public class ExpressionHelper
	{

		static XmlRootAttribute xmlRootAttr;

		protected static XmlRootAttribute XmlRootAttr {
			get {
				if (xmlRootAttr == null) {
					xmlRootAttr = new XmlRootAttribute ();
					xmlRootAttr.IsNullable = true;
				}
				return xmlRootAttr;
			}
		}

		/// <summary>
		/// Reads one xml element surrounding a list of expressions, for example a comparative condition, like equal, greaterthan or lessorequal. 
		/// It consumes the whole surrounding element with all contents including the closing end_element.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public static List<IExpression> ParseExpressionListFromXML (System.Xml.XmlReader reader)
		{
			List<IExpression> containedExpressions = new List<IExpression> ();

			string surroundingElementName = reader.LocalName;

			reader.MoveToContent ();

			bool currentNodeStillToBeConsumed = false;

			while (currentNodeStillToBeConsumed || reader.Read ()) {
				currentNodeStillToBeConsumed = false;

				// if we reach the end of this condition element we are ready to leave this method.
				if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals (surroundingElementName)) {
					break;
				}

				if (reader.NodeType != XmlNodeType.Element) {
					continue;
				}

				IExpression expr = parseExpression (reader);
				if (expr != null) {
					containedExpressions.Add (expr);
					currentNodeStillToBeConsumed = true;
				}
			}

			return containedExpressions;
		}

		/// <summary>
		/// Reads one xml element surrounding a single expression, for example the value element in a SetVariable Action. 
		/// It consumes the whole surrounding element with its content including the closing end_element.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public static IExpression ParseSingleExpressionFromXML (System.Xml.XmlReader reader)
		{
			XmlSerializer serializer;
			IExpression containedExpression = null;

			string surroundingElementName = reader.LocalName;

			reader.MoveToContent ();

			while (containedExpression == null && reader.Read ()) {

				// if we reach the end of this condition element we are ready to leave this method.
				if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals (surroundingElementName)) {
					break;
				}

				if (reader.NodeType != XmlNodeType.Element) {
					continue;
				}


				containedExpression = parseExpression (reader);
			}

			return containedExpression;
		}

		protected static IExpression parseExpression (XmlReader reader)
		{
			XmlRootAttr.ElementName = reader.LocalName;

			XmlSerializer serializer;

			IExpression resultExpression = null;

			switch (reader.LocalName) {
			// COMPOUND CONDITIONS:
			case GQML.VARIABLE:
				serializer = new XmlSerializer (typeof(VariableExpression), XmlRootAttr);
				resultExpression = (VariableExpression)serializer.Deserialize (reader);
				break;
			case GQML.BOOL:
				serializer = new XmlSerializer (typeof(BoolExpression), XmlRootAttr);
				resultExpression = (BoolExpression)serializer.Deserialize (reader);
				break;
			case GQML.NUMBER:
				serializer = new XmlSerializer (typeof(NumberExpression), XmlRootAttr);
				resultExpression = (NumberExpression)serializer.Deserialize (reader);
				break;
			case GQML.STRING:
				serializer = new XmlSerializer (typeof(TextExpression), XmlRootAttr);
				resultExpression = (TextExpression)serializer.Deserialize (reader);
				break;
			default:
				Log.SignalErrorToDeveloper ("Expression found with unknown type {0}", reader.LocalName);
				break;
			}

			return resultExpression;
		}
	}
}