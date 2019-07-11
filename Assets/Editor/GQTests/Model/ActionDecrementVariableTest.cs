using NUnit.Framework;
using GQ.Client.Model;
using GQ.Client.Util;

namespace GQTests.Model
{

    public class ActionDecrementVariableTest : GQMLTest
	{

		[SetUp]
		public void Init ()
		{
			XmlRoot = "action";
		}

		[Test]
		public void DecExistingIntegerVar ()
		{
			// Arrange:
			ActionSetVariable actSetVar = parseXML<ActionSetVariable>(
                @"	<action type=""SetVariable"" var=""x"">
					    <value>
						    <num>10</num>
					    </value>
				    </action>");
			
			Variables.Clear ();
			actSetVar.Execute ();
			Assert.AreEqual (10, Variables.GetValue ("x").AsInt ()); 

			// Act:
			ActionDecrementVariable actDecVar = parseXML<ActionDecrementVariable> 
				(@"	<action type=""DecrementVariable"" var=""x""/>");
			actDecVar.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Integer, Variables.GetValue ("x").ValType);
			Assert.AreEqual (9, Variables.GetValue ("x").AsInt ()); 
		}

		[Test]
		public void UndefinedVar ()
		{
			// Arrange:
			Variables.Clear ();
			Assert.AreEqual (Value.Null, Variables.GetValue ("x")); 

			// Act:
			ActionDecrementVariable actDecVar = parseXML<ActionDecrementVariable> 
				(@"	<action type=""DecrementVariable"" var=""x""/>");
			actDecVar.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Integer, Variables.GetValue ("x").ValType);
			Assert.AreEqual (-1, Variables.GetValue ("x").AsInt ()); 
		}

		[Test]
		public void DecBoolVar ()
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
			ActionDecrementVariable actDecVarF = parseXML<ActionDecrementVariable> 
				(@"	<action type=""DecrementVariable"" var=""f""/>");
			ActionDecrementVariable actDecVarT = parseXML<ActionDecrementVariable> 
				(@"	<action type=""DecrementVariable"" var=""t""/>");
			actDecVarF.Execute ();
			actDecVarT.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Bool, Variables.GetValue ("f").ValType);
			Assert.IsFalse (Variables.GetValue ("f").AsBool (), "Decrementing a bool var 'false' should keep the value 'false'"); 
			Assert.AreEqual (Value.Type.Bool, Variables.GetValue ("t").ValType);
			Assert.IsFalse (Variables.GetValue ("t").AsBool (), "Decrementing a bool var 'true' should change value to 'false'"); 
		}

		[Test]
		public void DecDoubleVar ()
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
			ActionDecrementVariable actDecVar = parseXML<ActionDecrementVariable> 
				(@"	<action type=""DecrementVariable"" var=""x""/>");
			actDecVar.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Float, Variables.GetValue ("x").ValType);
			Assert.That (Values.NearlyEqual (9.05, Variables.GetValue ("x").AsDouble ())); 
		}


		[Test]
		public void DecStringVar ()
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
			ActionDecrementVariable actDecVar = parseXML<ActionDecrementVariable> 
				(@"	<action type=""DecrementVariable"" var=""x""/>");
			actDecVar.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Text, Variables.GetValue ("x").ValType);
			Assert.AreEqual ("Halln", Variables.GetValue ("x").AsString ()); 
		}

		[Test]
		public void EmbeddedInRule ()
		{
			// Arrange:
			XmlRoot = "rule";
			Rule rule = parseXML<Rule> 
				(@"	<rule>
						<action type=""SetVariable"" var=""x"" id=""1"">
							<value>
								<num>10</num>
							</value>
						</action>
						<action type=""DecrementVariable"" var=""x"" id=""2""></action>
					</rule>");
			Variables.Clear ();
			Assert.AreEqual (Value.Null, Variables.GetValue ("x")); 

			// Act:
			rule.Apply ();

			// Assert:
			Assert.AreEqual (Value.Type.Integer, Variables.GetValue ("x").ValType);
			Assert.AreEqual (9, Variables.GetValue ("x").AsInt ());
		}


	}
}
