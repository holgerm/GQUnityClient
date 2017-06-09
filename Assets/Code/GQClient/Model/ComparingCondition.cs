using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.Generic;

namespace GQ.Client.Model
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

		protected List<IExpression> expressions;

		/// <summary>
		/// Reads one xml element for a comparative condition, like equal, greaterthan or lessorequal. 
		/// It consumes the whole element with all contents including the closing end_element.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public void ReadXml (System.Xml.XmlReader reader)
		{
			expressions = ExpressionHelper.ParseExpressionListFromXML (reader);
		}

		#endregion


		#region Function

		public virtual bool IsFulfilled ()
		{
			// handle case with no expressions at all, i.e. empty list:
			if (expressions.Count == 0)
				return isFulfilledEmptyComparison ();

			// handle case with only one expression in list:
			IExpression firstExpr = expressions [0];
			if (expressions.Count == 1)
				return isFulfilledCompare (firstExpr);

			// handle case with two or more expressions in list:
			IExpression secondExpr;
			bool fulfilled = true;
			int i = 1;

			while (fulfilled && expressions.Count > i) {
				secondExpr = expressions [i++];
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
