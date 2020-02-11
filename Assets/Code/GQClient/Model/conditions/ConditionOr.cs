using System.Xml;

namespace Code.GQClient.Model.conditions
{
    public class ConditionOr : CompoundCondition
	{
        public ConditionOr(XmlReader reader) : base(reader) { }

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