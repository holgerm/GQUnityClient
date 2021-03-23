using Code.GQClient.Model.actions;
using Code.GQClient.Model.expressions;
using Code.GQClient.Model.gqml;
using NUnit.Framework;

namespace GQTests.Model
{
	public class RuleTest : GQMLTest
	{

		[SetUp]
		public void Init ()
		{
			XmlRoot = GQML.RULE;
		}

		[Test]
		public void ApplySingleAction ()
		{
			// Arrange:
			Rule rule = parseXML<Rule> 
				(@"	<rule>
						<action type=""SetVariable"" var=""A"">
							<value>
								<bool>true</bool>
							</value>
						</action>
					</rule>");

			Variables.Clear ();
			Assert.AreEqual (Value.Null, Variables.GetValue ("A")); 

			// Act:
			Assert.IsNotNull (rule);
			rule.Apply ();

			// Assert:
			Assert.AreEqual (Value.Type.Bool, Variables.GetValue ("A").ValType);
			Assert.AreEqual (true, Variables.GetValue ("A").AsBool ());
		}

		[Test]
		public void ApplyThreeActions ()
		{
			// Arrange:
			Rule rule = parseXML<Rule> 
				(@"	<rule>
						<action type=""SetVariable"" var=""A"">
							<value>
								<bool>true</bool>
							</value>
						</action>
						<action type=""SetVariable"" var=""B"">
							<value>
								<num>100</num>
							</value>
						</action>
						<action type=""SetVariable"" var=""C"">
							<value>
								<string>Hallo</string>
							</value>
						</action>
					</rule>");

			Variables.Clear ();
			Assert.AreEqual (Value.Null, Variables.GetValue ("A")); 
			Assert.AreEqual (Value.Null, Variables.GetValue ("B")); 
			Assert.AreEqual (Value.Null, Variables.GetValue ("C")); 

			// Act:
			Assert.IsNotNull (rule);
			rule.Apply ();

			// Assert:
			Assert.AreEqual (Value.Type.Bool, Variables.GetValue ("A").ValType);
			Assert.AreEqual (true, Variables.GetValue ("A").AsBool ());

			Assert.AreEqual (Value.Type.Integer, Variables.GetValue ("B").ValType);
			Assert.AreEqual (100, Variables.GetValue ("B").AsInt ());

			Assert.AreEqual (Value.Type.Text, Variables.GetValue ("C").ValType);
			Assert.AreEqual ("Hallo", Variables.GetValue ("C").AsString ());
		}
	}
}
