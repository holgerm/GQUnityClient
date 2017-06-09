using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model;
using GQ.Client.Util;
using GQ.Client.Err;

namespace GQTests.Model
{

	public class ActionIncrementVariableTest : XMLTest
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
			SetVariableAction actSetVar = parseXML<SetVariableAction> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<num>10</num>
						</value>
					</action>");
			
			Variables.ClearAll ();
			actSetVar.Execute ();
			Assert.AreEqual (10, Variables.GetValue ("x").AsInt ()); 

			// Act:
			IncrementVariableAction actIncVar = parseXML<IncrementVariableAction> 
				(@"	<action type=""IncrementVariable"" var=""x""/>");
			actIncVar.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Integer, Variables.GetValue ("x").GetType ());
			Assert.AreEqual (11, Variables.GetValue ("x").AsInt ()); 
		}

		[Test]
		public void UndefinedVar ()
		{
			// Arrange:
			Variables.ClearAll ();
			Assert.AreEqual (Value.Null, Variables.GetValue ("x")); 

			// Act:
			IncrementVariableAction actIncVar = parseXML<IncrementVariableAction> 
				(@"	<action type=""IncrementVariable"" var=""x""/>");
			actIncVar.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Integer, Variables.GetValue ("x").GetType ());
			Assert.AreEqual (1, Variables.GetValue ("x").AsInt ()); 
		}

		[Test]
		public void IncBoolVar ()
		{
			// Arrange:
			SetVariableAction actSetVarF = parseXML<SetVariableAction> 
				(@"	<action type=""SetVariable"" var=""f"">
						<value>
							<bool>false</bool>
						</value>
					</action>");
			SetVariableAction actSetVarT = parseXML<SetVariableAction> 
				(@"	<action type=""SetVariable"" var=""t"">
						<value>
							<bool>true</bool>
						</value>
					</action>");
			
			Variables.ClearAll ();
			actSetVarF.Execute ();
			actSetVarT.Execute ();
			Assert.IsFalse (Variables.GetValue ("f").AsBool ()); 
			Assert.IsTrue (Variables.GetValue ("t").AsBool ()); 

			// Act:
			IncrementVariableAction actIncVarF = parseXML<IncrementVariableAction> 
				(@"	<action type=""IncrementVariable"" var=""f""/>");
			IncrementVariableAction actIncVarT = parseXML<IncrementVariableAction> 
				(@"	<action type=""IncrementVariable"" var=""t""/>");
			actIncVarF.Execute ();
			actIncVarT.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Bool, Variables.GetValue ("f").GetType ());
			Assert.IsTrue (Variables.GetValue ("f").AsBool (), "Incrementing a bool var 'false' should change value to 'true'"); 
			Assert.AreEqual (Value.Type.Bool, Variables.GetValue ("t").GetType ());
			Assert.IsTrue (Variables.GetValue ("t").AsBool (), "Incrementing a bool var 'true' should keep the value 'true'"); 
		}

		[Test]
		public void IncDoubleVar ()
		{
			// Arrange:
			SetVariableAction actSetVar = parseXML<SetVariableAction> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<num>10.05</num>
						</value>
					</action>");

			Variables.ClearAll ();
			actSetVar.Execute ();
			Assert.That (Values.NearlyEqual (10.05d, Variables.GetValue ("x").AsDouble ())); 

			// Act:
			IncrementVariableAction actIncVar = parseXML<IncrementVariableAction> 
				(@"	<action type=""IncrementVariable"" var=""x""/>");
			actIncVar.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Float, Variables.GetValue ("x").GetType ());
			Assert.That (Values.NearlyEqual (11.05, Variables.GetValue ("x").AsDouble ())); 
		}


		[Test]
		public void IncStringVar ()
		{
			// Arrange:
			SetVariableAction actSetVar = parseXML<SetVariableAction> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<string>Hallo</string>
						</value>
					</action>");

			Variables.ClearAll ();
			actSetVar.Execute ();
			Assert.AreEqual (Value.Type.Text, Variables.GetValue ("x").GetType ());
			Assert.AreEqual ("Hallo", Variables.GetValue ("x").AsString ()); 

			// Act:
			IncrementVariableAction actIncVar = parseXML<IncrementVariableAction> 
				(@"	<action type=""IncrementVariable"" var=""x""/>");
			actIncVar.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Text, Variables.GetValue ("x").GetType ());
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
			Variables.ClearAll ();
			Assert.AreEqual (Value.Null, Variables.GetValue ("x")); 

			// Act:
			rule.Apply ();

			// Assert:
			Assert.AreEqual (Value.Type.Integer, Variables.GetValue ("x").GetType ());
			Assert.AreEqual (11, Variables.GetValue ("x").AsInt ());
		}


	}
}
