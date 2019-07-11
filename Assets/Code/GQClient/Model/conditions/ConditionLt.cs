using System.Xml;

namespace GQ.Client.Model
{

    public class ConditionLt : ComparingCondition
	{
        public ConditionLt(XmlReader reader) : base(reader) { }

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

			return firstVal.IsLessThan (secondVal);
		}
	}
}
