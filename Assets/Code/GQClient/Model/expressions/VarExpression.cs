using System;
using System.Linq;
using System.Xml;

namespace GQ.Client.Model
{

    public class VarExpression : SimpleExpression
    {
        #region Structure
        public VarExpression(XmlReader reader) : base(reader) { }

        protected override void setValue(string valueAsString)
        {
            valueAsString = valueAsString.Trim();

            value = new Value(valueAsString, Value.Type.VarExpression);
        }
        #endregion


        #region Runtime
        /// <summary>
        /// Currently we can only deal with two cases: a simple variable name or arithemtic expressions. 
        /// TODO: Boolean or string expressions are to come.
        /// </summary>
        /// <returns>The evaluate.</returns>
        public override Value Evaluate()
        {
            if (Variables.IsDefined(value.AsString()))
            {
                Value varValue = Variables.GetValue(value.AsString());
                return varValue;
            }

            return new Value(evaluateArithmetics(value.AsString()));
        }

        public double evaluateArithmetics(string input)
        {

            double currentvalue = 0.0d;
            bool needsstartvalue = true;
            input = new string(input.ToCharArray()
                             .Where(c => !Char.IsWhiteSpace(c))
                             .ToArray());

            string arithmetics = "";

            foreach (Char c in input.ToCharArray())
            {
                if (c == '+')
                {
                    arithmetics = arithmetics + "+";
                }
                if (c == '-')
                {
                    arithmetics = arithmetics + "-";
                }
                if (c == '*')
                {
                    arithmetics = arithmetics + "*";
                }
                if (c == '/')
                {
                    arithmetics = arithmetics + "/";
                }
                if (c == ':')
                {
                    arithmetics = arithmetics + ":";
                }

            }

            char[] splitter = "+-/*:".ToCharArray();
            string[] splitted = input.Split(splitter);
            int count = 0;

            foreach (string s in splitted)
            {

                double n;
                bool isNumeric = double.TryParse(s, out n);

                if (isNumeric)
                {

                    if (needsstartvalue)
                    {
                        currentvalue = n;
                        needsstartvalue = false;
                    }
                    else
                    {
                        if (arithmetics.Substring(count, 1) == "+")
                        {
                            currentvalue += n;
                        }
                        else if (arithmetics.Substring(count, 1) == "-")
                        {
                            currentvalue -= n;
                        }
                        else if (arithmetics.Substring(count, 1) == "*")
                        {
                            currentvalue *= n;
                        }
                        else if ((arithmetics.Substring(count, 1) == "/") || (arithmetics.Substring(count, 1) == ":"))
                        {
                            currentvalue = currentvalue / n;
                        }
                    }

                }
                else
                {

                    Value qv = Variables.GetValue(s);

                    switch (qv.ValType)
                    {
                        case Value.Type.Float:
                        case Value.Type.Integer:
                            if (needsstartvalue)
                            {
                                currentvalue = qv.AsDouble();
                                needsstartvalue = false;
                            }
                            else
                            {
                                n = qv.AsDouble();

                                if (arithmetics.Substring(count, 1) == "+")
                                {
                                    currentvalue += n;
                                }
                                else if (arithmetics.Substring(count, 1) == "-")
                                {
                                    currentvalue -= n;
                                }
                                else if (arithmetics.Substring(count, 1) == "*")
                                {
                                    currentvalue *= n;
                                }
                                else if ((arithmetics.Substring(count, 1) == "/") || (arithmetics.Substring(count, 1) == ":"))
                                {

                                    currentvalue = currentvalue / n;
                                }
                                count += 1;

                            }
                            break;
                    }
                }
            }

            return currentvalue;
        }

        #endregion
    }
}