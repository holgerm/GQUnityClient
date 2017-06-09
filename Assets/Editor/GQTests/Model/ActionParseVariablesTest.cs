using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model;
using GQ.Client.Util;
using GQ.Client.Err;

namespace GQTests.Model
{

	public class ActionParseVariablesTest : XMLTest
	{

		[SetUp]
		public void Init ()
		{
			XmlRoot = "action";
			Variables.ClearAll ();
		}

		[Test]
		public void ParseSingleNumVar ()
		{
			// Arrange:
			SetVariableAction actSetVar = parseXML<SetVariableAction> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<string>a:10</string>
						</value>
					</action>");
			actSetVar.Execute ();

			// Act:
			ParseVariablesAction actParseVars = parseXML<ParseVariablesAction> 
				(@"	<action FromVar=""x"" type=""ParseVariables""/>");
			actParseVars.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Integer, Variables.GetValue ("a").GetType ());
			Assert.AreEqual (10, Variables.GetValue ("a").AsInt ()); 
		}

		[Test]
		public void ParseSingleFloatVar ()
		{
			// Arrange:
			SetVariableAction actSetVar = parseXML<SetVariableAction> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<string>a:10.06</string>
						</value>
					</action>");
			actSetVar.Execute ();

			// Act:
			ParseVariablesAction actParseVars = parseXML<ParseVariablesAction> 
				(@"	<action FromVar=""x"" type=""ParseVariables""/>");
			actParseVars.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Float, Variables.GetValue ("a").GetType ());
			Assert.AreEqual (10.06d, Variables.GetValue ("a").AsDouble ()); 
		}

		[Test]
		public void ParseSingleBoolVar ()
		{
			// Arrange:
			SetVariableAction actSetVar = parseXML<SetVariableAction> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<string>a:true</string>
						</value>
					</action>");
			actSetVar.Execute ();

			// Act:
			ParseVariablesAction actParseVars = parseXML<ParseVariablesAction> 
				(@"	<action FromVar=""x"" type=""ParseVariables""/>");
			actParseVars.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Bool, Variables.GetValue ("a").GetType ());
			Assert.AreEqual (true, Variables.GetValue ("a").AsBool ()); 
		}

		[Test]
		public void ParseSingleTextVar ()
		{
			// Arrange:
			SetVariableAction actSetVar = parseXML<SetVariableAction> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<string>a:hallo</string>
						</value>
					</action>");
			actSetVar.Execute ();

			// Act:
			ParseVariablesAction actParseVars = parseXML<ParseVariablesAction> 
				(@"	<action FromVar=""x"" type=""ParseVariables""/>");
			actParseVars.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Text, Variables.GetValue ("a").GetType ());
			Assert.AreEqual ("hallo", Variables.GetValue ("a").AsString ()); 
		}

		[Test]
		public void ParseTextVarWithMaskedComma ()
		{
			// Arrange:
			SetVariableAction actSetVar = parseXML<SetVariableAction> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<string>a:hallo,, this is a comma in a sentence.</string>
						</value>
					</action>");
			actSetVar.Execute ();

			// Act:
			ParseVariablesAction actParseVars = parseXML<ParseVariablesAction> 
				(@"	<action FromVar=""x"" type=""ParseVariables""/>");
			actParseVars.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Text, Variables.GetValue ("a").GetType ());
			Assert.AreEqual ("hallo, this is a comma in a sentence.", Variables.GetValue ("a").AsString ()); 
		}

		[Test]
		public void ParseTextVarWithColon ()
		{
			// Arrange:
			SetVariableAction actSetVar = parseXML<SetVariableAction> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<string>a:hallo: this is a colon in a sentence.</string>
						</value>
					</action>");
			actSetVar.Execute ();

			// Act:
			ParseVariablesAction actParseVars = parseXML<ParseVariablesAction> 
				(@"	<action FromVar=""x"" type=""ParseVariables""/>");
			actParseVars.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Text, Variables.GetValue ("a").GetType ());
			Assert.AreEqual ("hallo: this is a colon in a sentence.", Variables.GetValue ("a").AsString ()); 
		}

		[Test]
		public void ParseTextVarWithQuotes ()
		{
			// Arrange:
			SetVariableAction actSetVar = parseXML<SetVariableAction> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<string>a:""hallo""</string>
						</value>
					</action>");
			actSetVar.Execute ();

			// Act:
			ParseVariablesAction actParseVars = parseXML<ParseVariablesAction> 
				(@"	<action FromVar=""x"" type=""ParseVariables""/>");
			actParseVars.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Text, Variables.GetValue ("a").GetType ());
			Assert.AreEqual (@"""hallo""", Variables.GetValue ("a").AsString ()); 
		}

		[Test]
		public void ParseMultipleMixedVars ()
		{
			// Arrange:
			SetVariableAction actSetVar = parseXML<SetVariableAction> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<string>a:""hallo"",b:10.02,c:true,d:,,,,</string>
						</value>
					</action>");
			actSetVar.Execute ();

			// Act:
			ParseVariablesAction actParseVars = parseXML<ParseVariablesAction> 
				(@"	<action FromVar=""x"" type=""ParseVariables""/>");
			actParseVars.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Text, Variables.GetValue ("a").GetType ());
			Assert.AreEqual (@"""hallo""", Variables.GetValue ("a").AsString ()); 
			Assert.AreEqual (Value.Type.Float, Variables.GetValue ("b").GetType ());
			Assert.AreEqual (10.02d, Variables.GetValue ("b").AsDouble ()); 
			Assert.AreEqual (Value.Type.Bool, Variables.GetValue ("c").GetType ());
			Assert.AreEqual (true, Variables.GetValue ("c").AsBool ()); 
			Assert.AreEqual (Value.Type.Text, Variables.GetValue ("d").GetType ());
			Assert.AreEqual (@",,", Variables.GetValue ("d").AsString ()); 
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
								<string>a:10.02</string>
							</value>
						</action>
						<action FromVar=""x"" type=""ParseVariables""/>
					</rule>");

			// Act:
			rule.Apply ();

			// Assert:
			Assert.AreEqual (Value.Type.Float, Variables.GetValue ("a").GetType ());
			Assert.AreEqual (10.02d, Variables.GetValue ("a").AsDouble ()); 
		}


	}
}
