using UnityEngine;
using System.Collections;

namespace GQ.Client.Model.XML
{

	public class LessCondition : ComparingCondition
	{

		protected override bool isFulfilledEmptyComparison ()
		{
			return true;
		}

		protected override bool isFulfilledCompare (IExpression expression)
		{
			return true;
		}

		protected override bool isFulfilledCompare (IExpression firstExpression, IExpression secondExpression)
		{
			Value firstVal = firstExpression.evaluate ();
			Value secondVal = secondExpression.evaluate ();

			Debug.Log (string.Format ("Less? {0} : {1}", firstVal, secondVal));

			return firstVal.IsLessThan (secondVal);
		}
	}
}
