using NUnit.Framework;
using GQ.Client.Model;
using GQ.Client.Util;

namespace GQTests.Model
{

    public class ExpressionBoolTest : GQMLTest
	{

		[SetUp]
		public void Init ()
		{
			XmlRoot = "bool";
		}

		[Test]
		public void Bool_XML_True ()
		{
			// Act:
			BoolExpression boolExpr = parseXML<BoolExpression> (@"<bool>true</bool>");

			// Assert:
			Assert.IsNotNull (boolExpr);
			Assert.AreEqual (true, boolExpr.Evaluate ().AsBool ());
			Assert.AreEqual ("true", boolExpr.Evaluate ().AsString ());
			Assert.AreEqual ("true", boolExpr.Evaluate ().AsVariableName ());
			Assert.AreEqual (0, boolExpr.Evaluate ().AsInt ());
			Assert.That (Values.NearlyEqual (0d, boolExpr.Evaluate ().AsDouble ()));
		}

		[Test]
		public void Bool_XML_False ()
		{
			// Act:
			BoolExpression boolExpr = parseXML<BoolExpression> (@"<bool>false</bool>");

			// Assert:
			Assert.IsNotNull (boolExpr);
			Assert.AreEqual (false, boolExpr.Evaluate ().AsBool ());
			Assert.AreEqual ("false", boolExpr.Evaluate ().AsString ());
			Assert.AreEqual ("false", boolExpr.Evaluate ().AsVariableName ());
			Assert.AreEqual (0, boolExpr.Evaluate ().AsInt ());
			Assert.That (Values.NearlyEqual (0d, boolExpr.Evaluate ().AsDouble ()));
		}

	}
}
