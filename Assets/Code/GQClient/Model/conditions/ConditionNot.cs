using GQ.Client.Err;
using System.Xml;

namespace GQ.Client.Model
{

    public class ConditionNot : CompoundCondition
	{
        public ConditionNot(XmlReader reader) : base(reader) { }

        public const string NOT_CONDITION_PROBLEM_EMPTY = "Empty not Condition found. This is not allowed.";
		public const string NOT_CONDITION_PROBLEM_TOO_MANY_ATOMIC_CONIDITIONS = "Not Condition may only contain one Subcondition.";

		/// <summary>
		/// True if the (one and only) contained condition is NOT fulfilled. 
		/// </summary>
		public override bool IsFulfilled ()
		{
			if (containedConditions.Count == 0) {
				Log.WarnAuthor (NOT_CONDITION_PROBLEM_EMPTY);
				return false;
			}

			if (containedConditions.Count > 1) {
				Log.WarnAuthor (NOT_CONDITION_PROBLEM_TOO_MANY_ATOMIC_CONIDITIONS);
				return false;
			}

			return !(containedConditions [0].IsFulfilled ());
		}
	}
}