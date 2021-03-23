using Code.GQClient.Model.actions;
using Code.GQClient.Model.expressions;
using NUnit.Framework;

namespace GQTests.Model
{

	public class ActionParseVariablesTest : GQMLTest
	{

		[SetUp]
		public void Init ()
		{
			XmlRoot = "action";
			Variables.Clear ();
		}

        [Test]
        public void ParseEmptyStringVar()
        {
            // Arrange:
            ActionSetVariable actSetVar = parseXML<ActionSetVariable>
                (@" <action type=""SetVariable"" var=""x"">
                        <value>
                            <string></string>
                        </value>
                    </action>");
            actSetVar.Execute();

            // Act:
            ActionParseVariables actParseVars = parseXML<ActionParseVariables>
                (@" <action FromVar=""x"" type=""ParseVariables""/>");
            actParseVars.Execute();

            // Assert:
            Assert.AreEqual(Value.Type.NULL, Variables.GetValue("a"));
        }

        [Test]
		public void ParseSingleNumVar ()
		{
			// Arrange:
			ActionSetVariable actSetVar = parseXML<ActionSetVariable> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<string>a:10</string>
						</value>
					</action>");
			actSetVar.Execute ();

			// Act:
			ActionParseVariables actParseVars = parseXML<ActionParseVariables> 
				(@"	<action FromVar=""x"" type=""ParseVariables""/>");
			actParseVars.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Integer, Variables.GetValue ("a").ValType);
			Assert.AreEqual (10, Variables.GetValue ("a").AsInt ()); 
		}

		[Test]
		public void ParseSingleFloatVar ()
		{
			// Arrange:
			ActionSetVariable actSetVar = parseXML<ActionSetVariable> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<string>a:10.06</string>
						</value>
					</action>");
			actSetVar.Execute ();

			// Act:
			ActionParseVariables actParseVars = parseXML<ActionParseVariables> 
				(@"	<action FromVar=""x"" type=""ParseVariables""/>");
			actParseVars.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Float, Variables.GetValue ("a").ValType);
			Assert.AreEqual (10.06d, Variables.GetValue ("a").AsDouble ()); 
		}

		[Test]
		public void ParseSingleBoolVar ()
		{
			// Arrange:
			ActionSetVariable actSetVar = parseXML<ActionSetVariable> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<string>a:true</string>
						</value>
					</action>");
			actSetVar.Execute ();

			// Act:
			ActionParseVariables actParseVars = parseXML<ActionParseVariables> 
				(@"	<action FromVar=""x"" type=""ParseVariables""/>");
			actParseVars.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Bool, Variables.GetValue ("a").ValType);
			Assert.AreEqual (true, Variables.GetValue ("a").AsBool ()); 
		}

		[Test]
		public void ParseSingleTextVar ()
		{
			// Arrange:
			ActionSetVariable actSetVar = parseXML<ActionSetVariable> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<string>a:hallo</string>
						</value>
					</action>");
			actSetVar.Execute ();

			// Act:
			ActionParseVariables actParseVars = parseXML<ActionParseVariables> 
				(@"	<action FromVar=""x"" type=""ParseVariables""/>");
			actParseVars.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Text, Variables.GetValue ("a").ValType);
			Assert.AreEqual ("hallo", Variables.GetValue ("a").AsString ()); 
		}

		[Test]
		public void ParseTextVarWithMaskedComma ()
		{
			// Arrange:
			ActionSetVariable actSetVar = parseXML<ActionSetVariable> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<string>a:hallo,, this is a comma in a sentence.</string>
						</value>
					</action>");
			actSetVar.Execute ();

			// Act:
			ActionParseVariables actParseVars = parseXML<ActionParseVariables> 
				(@"	<action FromVar=""x"" type=""ParseVariables""/>");
			actParseVars.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Text, Variables.GetValue ("a").ValType);
			Assert.AreEqual ("hallo, this is a comma in a sentence.", Variables.GetValue ("a").AsString ()); 
		}

		[Test]
		public void ParseTextVarWithColon ()
		{
			// Arrange:
			ActionSetVariable actSetVar = parseXML<ActionSetVariable> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<string>a:hallo: this is a colon in a sentence.</string>
						</value>
					</action>");
			actSetVar.Execute ();

			// Act:
			ActionParseVariables actParseVars = parseXML<ActionParseVariables> 
				(@"	<action FromVar=""x"" type=""ParseVariables""/>");
			actParseVars.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Text, Variables.GetValue ("a").ValType);
			Assert.AreEqual ("hallo: this is a colon in a sentence.", Variables.GetValue ("a").AsString ()); 
		}

		[Test]
		public void ParseTextVarWithQuotes ()
		{
			// Arrange:
			ActionSetVariable actSetVar = parseXML<ActionSetVariable> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<string>a:""hallo""</string>
						</value>
					</action>");
			actSetVar.Execute ();

			// Act:
			ActionParseVariables actParseVars = parseXML<ActionParseVariables> 
				(@"	<action FromVar=""x"" type=""ParseVariables""/>");
			actParseVars.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Text, Variables.GetValue ("a").ValType);
			Assert.AreEqual (@"""hallo""", Variables.GetValue ("a").AsString ()); 
		}

		[Test]
		public void ParseMultipleMixedVars ()
		{
			// Arrange:
			ActionSetVariable actSetVar = parseXML<ActionSetVariable> 
				(@"	<action type=""SetVariable"" var=""x"">
						<value>
							<string>a:""hallo"",b:10.02,c:true,d:,,,,</string>
						</value>
					</action>");
			actSetVar.Execute ();

			// Act:
			ActionParseVariables actParseVars = parseXML<ActionParseVariables> 
				(@"	<action FromVar=""x"" type=""ParseVariables""/>");
			actParseVars.Execute ();

			// Assert:
			Assert.AreEqual (Value.Type.Text, Variables.GetValue ("a").ValType);
			Assert.AreEqual (@"""hallo""", Variables.GetValue ("a").AsString ()); 
			Assert.AreEqual (Value.Type.Float, Variables.GetValue ("b").ValType);
			Assert.AreEqual (10.02d, Variables.GetValue ("b").AsDouble ()); 
			Assert.AreEqual (Value.Type.Bool, Variables.GetValue ("c").ValType);
			Assert.AreEqual (true, Variables.GetValue ("c").AsBool ()); 
			Assert.AreEqual (Value.Type.Text, Variables.GetValue ("d").ValType);
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
			Assert.AreEqual (Value.Type.Float, Variables.GetValue ("a").ValType);
			Assert.AreEqual (10.02d, Variables.GetValue ("a").AsDouble ()); 
		}


	}
}
