using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model;

namespace GQTests.Model
{
	public class RuleTest : XMLTest
	{

		[SetUp]
		public void Init ()
		{
			XmlRoot = "rule";
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

			Variables.ClearAll ();
			Assert.AreEqual (Value.Null, Variables.GetValue ("A")); 

			// Act:
			Assert.IsNotNull (rule);
			rule.Apply ();

			// Assert:
			Assert.AreEqual (Value.Type.Bool, Variables.GetValue ("A").GetType ());
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

			Variables.ClearAll ();
			Assert.AreEqual (Value.Null, Variables.GetValue ("A")); 
			Assert.AreEqual (Value.Null, Variables.GetValue ("B")); 
			Assert.AreEqual (Value.Null, Variables.GetValue ("C")); 

			// Act:
			Assert.IsNotNull (rule);
			rule.Apply ();

			// Assert:
			Assert.AreEqual (Value.Type.Bool, Variables.GetValue ("A").GetType ());
			Assert.AreEqual (true, Variables.GetValue ("A").AsBool ());

			Assert.AreEqual (Value.Type.Integer, Variables.GetValue ("B").GetType ());
			Assert.AreEqual (100, Variables.GetValue ("B").AsInt ());

			Assert.AreEqual (Value.Type.Text, Variables.GetValue ("C").GetType ());
			Assert.AreEqual ("Hallo", Variables.GetValue ("C").AsString ());
		}
	}
}
