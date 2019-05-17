using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model;
using GQ.Client.Util;
using GQ.Client.Err;

namespace GQTests.Model
{

	public class ActionIncrementVariableTest : GQMLTest
	{

		[SetUp]
		public void Init ()
		{
			XmlRoot = "action";
		}

		[Test]
		public void IncExistingIntegerVar ()
		{
			// Arrange:
			ActionSetVariable actSetVar = parseXML<ActionSetVariable> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<num>10</num>
						</value>
					</action>");
			
			Variables.Clear ();
			actSetVar.Execute ();
			Assert.AreEqual (10, Variables.GetValue ("x").AsInt ()); 

			// Act:
			ActionIncrementVariable actIncVar = parseXML<ActionIncrementVariable> 
				(@"	<action type=""IncrementVariable"" var=""x""/>");
			actIncVar.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Integer, Variables.GetValue ("x").ValType);
			Assert.AreEqual (11, Variables.GetValue ("x").AsInt ()); 
		}

		[Test]
		public void UndefinedVar ()
		{
			// Arrange:
			Variables.Clear ();
			Assert.AreEqual (Value.Null, Variables.GetValue ("x")); 

			// Act:
			ActionIncrementVariable actIncVar = parseXML<ActionIncrementVariable> 
				(@"	<action type=""IncrementVariable"" var=""x""/>");
			actIncVar.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Integer, Variables.GetValue ("x").ValType);
			Assert.AreEqual (1, Variables.GetValue ("x").AsInt ()); 
		}

		[Test]
		public void IncBoolVar ()
		{
			// Arrange:
			ActionSetVariable actSetVarF = parseXML<ActionSetVariable> 
				(@"	<action type=""SetVariable"" var=""f"">
						<value>
							<bool>false</bool>
						</value>
					</action>");
			ActionSetVariable actSetVarT = parseXML<ActionSetVariable> 
				(@"	<action type=""SetVariable"" var=""t"">
						<value>
							<bool>true</bool>
						</value>
					</action>");
			
			Variables.Clear ();
			actSetVarF.Execute ();
			actSetVarT.Execute ();
			Assert.IsFalse (Variables.GetValue ("f").AsBool ()); 
			Assert.IsTrue (Variables.GetValue ("t").AsBool ()); 

			// Act:
			ActionIncrementVariable actIncVarF = parseXML<ActionIncrementVariable> 
				(@"	<action type=""IncrementVariable"" var=""f""/>");
			ActionIncrementVariable actIncVarT = parseXML<ActionIncrementVariable> 
				(@"	<action type=""IncrementVariable"" var=""t""/>");
			actIncVarF.Execute ();
			actIncVarT.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Bool, Variables.GetValue ("f").ValType);
			Assert.IsTrue (Variables.GetValue ("f").AsBool (), "Incrementing a bool var 'false' should change value to 'true'"); 
			Assert.AreEqual (Value.Type.Bool, Variables.GetValue ("t").ValType);
			Assert.IsTrue (Variables.GetValue ("t").AsBool (), "Incrementing a bool var 'true' should keep the value 'true'"); 
		}

		[Test]
		public void IncDoubleVar ()
		{
			// Arrange:
			ActionSetVariable actSetVar = parseXML<ActionSetVariable> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<num>10.05</num>
						</value>
					</action>");

			Variables.Clear ();
			actSetVar.Execute ();
			Assert.That (Values.NearlyEqual (10.05d, Variables.GetValue ("x").AsDouble ())); 

			// Act:
			ActionIncrementVariable actIncVar = parseXML<ActionIncrementVariable> 
				(@"	<action type=""IncrementVariable"" var=""x""/>");
			actIncVar.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Float, Variables.GetValue ("x").ValType);
			Assert.That (Values.NearlyEqual (11.05, Variables.GetValue ("x").AsDouble ())); 
		}


		[Test]
		public void IncStringVar ()
		{
			// Arrange:
			ActionSetVariable actSetVar = parseXML<ActionSetVariable> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<string>Hallo</string>
						</value>
					</action>");

			Variables.Clear ();
			actSetVar.Execute ();
			Assert.AreEqual (Value.Type.Text, Variables.GetValue ("x").ValType);
			Assert.AreEqual ("Hallo", Variables.GetValue ("x").AsString ()); 

			// Act:
			ActionIncrementVariable actIncVar = parseXML<ActionIncrementVariable> 
				(@"	<action type=""IncrementVariable"" var=""x""/>");
			actIncVar.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Text, Variables.GetValue ("x").ValType);
			Assert.AreEqual ("Hallp", Variables.GetValue ("x").AsString ()); 
		}

		[Test]
		public void EmbeddedInRule ()
		{
			// Arrange:
			XmlRoot = "rule";
			Rule rule = parseXML<Rule> 
				(@"	<rule>
						<action type=""SetVariable"" var=""x"">
							<value>
								<num>10</num>
							</value>
						</action>
						<action type=""IncrementVariable"" var=""x""/>
					</rule>");
			Variables.Clear ();
			Assert.AreEqual (Value.Null, Variables.GetValue ("x")); 

			// Act:
			rule.Apply ();

			// Assert:
			Assert.AreEqual (Value.Type.Integer, Variables.GetValue ("x").ValType);
			Assert.AreEqual (11, Variables.GetValue ("x").AsInt ());
		}


	}
}
