using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model;

namespace GQTests.Model
{

	public class VariablesTest
	{

		[Test]
		public void IsValidVariableName ()
		{
			// Assert positive:
			Assert.That (Variables.IsValidVariableName ("x"));
			Assert.That (Variables.IsValidVariableName ("X"));
			Assert.That (Variables.IsValidVariableName ("ABC"));
			Assert.That (Variables.IsValidVariableName ("ABC_12"));

			// Assert negative:
			Assert.IsFalse (Variables.IsValidVariableName ("AB-D"));
			Assert.IsFalse (Variables.IsValidVariableName ("$x"));
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
	}
}
