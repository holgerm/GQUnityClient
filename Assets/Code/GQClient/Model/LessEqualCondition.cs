using UnityEngine;
using System.Collections;

namespace GQ.Client.Model
{

	public class LessEqualCondition : ComparingCondition
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
			Value firstVal = firstExpression.Evaluate ();
			Value secondVal = secondExpression.Evaluate ();

			Debug.Log (string.Format ("Less or equal? {0} : {1}", firstVal, secondVal));

			return firstVal.IsLessOrEqual (secondVal);
		}
	}
}
