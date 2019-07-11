using System.Collections.Generic;
using System.Xml;

namespace GQ.Client.Model
{

    public abstract class ComparingCondition : ICondition
    {
        #region Structure
        public I_GQML Parent { get; set; }

        protected List<IExpression> expressions;

        /// <summary>
        /// Reads one xml element for a comparative condition, like equal, greaterthan or lessorequal. 
        /// It consumes the whole element with all contents including the closing end_element.
        /// </summary>
        /// <param name="reader">Reader.</param>
        public ComparingCondition(XmlReader reader)
        {
            expressions = ExpressionHelper.ParseExpressionListFromXML(reader);
        }
        #endregion

        #region Function
        public virtual bool IsFulfilled()
        {
            // handle case with no expressions at all, i.e. empty list:
            if (expressions.Count == 0)
                return isFulfilledEmptyComparison();

            // handle case with only one expression in list:
            IExpression firstExpr = expressions[0];
            if (expressions.Count == 1)
                return isFulfilledCompare(firstExpr);

            // handle case with two or more expressions in list:
            IExpression secondExpr;
            bool fulfilled = true;
            int i = 1;

            while (fulfilled && expressions.Count > i)
            {
                secondExpr = expressions[i++];
                fulfilled &= isFulfilledCompare(firstExpr, secondExpr);
                firstExpr = secondExpr;
            }
            return fulfilled;
        }

        protected abstract bool isFulfilledEmptyComparison();

        protected abstract bool isFulfilledCompare(IExpression expression);

        protected abstract bool isFulfilledCompare(IExpression firstExpression, IExpression secondExpression);
        #endregion
    }
}
