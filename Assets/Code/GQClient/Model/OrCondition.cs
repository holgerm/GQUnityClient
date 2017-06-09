using UnityEngine;
using System.Collections;

namespace GQ.Client.Model
{

	public class OrCondition : CompoundCondition
	{
		/// <summary>
		/// True if at least one of the contained conditions is fulfilled.
		/// </summary>
		public override bool IsFulfilled ()
		{
			bool oneFulfilled = false;
			foreach (ICondition condition in containedConditions) {
				oneFulfilled |= condition.IsFulfilled ();
				if (oneFulfilled)
					break;
			}
			return oneFulfilled;
		}

	}
}