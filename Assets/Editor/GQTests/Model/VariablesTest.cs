using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model;

namespace GQTests.Model
{

	public class VariablesTest
	{

		[Test]
		public void IsValidUserDefinedVariableName ()
		{
			// Assert positive:
			Assert.That (Variables.IsValidUserDefinedVariableName ("x"));
			Assert.That (Variables.IsValidUserDefinedVariableName ("X"));
			Assert.That (Variables.IsValidUserDefinedVariableName ("ABC"));
			Assert.That (Variables.IsValidUserDefinedVariableName ("ABC_12"));

			// Assert negative:
			Assert.IsFalse (Variables.IsValidUserDefinedVariableName ("AB-D"));
			Assert.IsFalse (Variables.IsValidUserDefinedVariableName ("$x"));
			Assert.IsFalse (Variables.IsValidUserDefinedVariableName ("1X"));
			Assert.IsFalse (Variables.IsValidUserDefinedVariableName ("AB+D"));
			Assert.IsFalse (Variables.IsValidUserDefinedVariableName ("AB*D"));
			Assert.IsFalse (Variables.IsValidUserDefinedVariableName ("AB/D"));
		}

		[Test]
		public void IsValidVariableName ()
		{
			// Assert positive:
			Assert.That (Variables.IsValidVariableName ("x"));
			Assert.That (Variables.IsValidVariableName ("X"));
			Assert.That (Variables.IsValidVariableName ("ABC"));
			Assert.That (Variables.IsValidVariableName ("ABC_12"));
			Assert.IsTrue (Variables.IsValidVariableName ("$x"));
			Assert.IsTrue (Variables.IsValidVariableName ("$_x"));
			Assert.IsTrue (Variables.IsValidVariableName ("$longitude"));
			Assert.IsTrue (Variables.IsValidVariableName ("$quest.name"));

			// Assert negative:
			Assert.IsFalse (Variables.IsValidVariableName ("AB-D"));
			Assert.IsFalse (Variables.IsValidVariableName ("1X"));
			Assert.IsFalse (Variables.IsValidVariableName ("AB+D"));
			Assert.IsFalse (Variables.IsValidVariableName ("AB*D"));
			Assert.IsFalse (Variables.IsValidVariableName ("AB/D"));
		}


		[Test]
		public void LongestValidVariableNameFromStart ()
		{
			// Assert:
			Assert.AreEqual ("x", Variables.LongestValidVariableNameFromStart ("x"));
			Assert.AreEqual ("AB", Variables.LongestValidVariableNameFromStart ("AB*D"));
			Assert.AreEqual ("xyz", Variables.LongestValidVariableNameFromStart ("xyz 123"));
		}

		[Test]
		public void UnknownVariableGetsNullValue ()
		{
			// Arrange:
			Variables.ClearAll ();

			// Act:
			Value notExistingVariableValue = Variables.GetValue ("X");

			// Assert:
			Assert.NotNull (notExistingVariableValue);
			Assert.AreEqual (Value.Null, notExistingVariableValue);
		}
	}
}
