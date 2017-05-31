using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model.XML;
using System.IO;
using System.Xml.Serialization;
using GQ.Client.Model;
using GQ.Client.Util;

namespace GQTests.Model.XML
{

	public class BoolExpressionTest : XMLTest
	{

		protected override string XmlRoot {
			get {
				return "bool";
			}
		}

		[Test]
		public void Bool_XML_True ()
		{
			// Act:
			BoolExpression boolExpr = parseXML<BoolExpression> (@"<bool>true</bool>");

			// Assert:
			Assert.IsNotNull (boolExpr);
			Assert.AreEqual (true, boolExpr.evaluate ().asBool ());
			Assert.AreEqual ("true", boolExpr.evaluate ().asString ());
			Assert.AreEqual ("true", boolExpr.evaluate ().asVariableName ());
			Assert.AreEqual (0, boolExpr.evaluate ().asInt ());
			Assert.That (Values.NearlyEqual (0d, boolExpr.evaluate ().asDouble ()));
		}

		[Test]
		public void Bool_XML_False ()
		{
			// Act:
			BoolExpression boolExpr = parseXML<BoolExpression> (@"<bool>false</bool>");

			// Assert:
			Assert.IsNotNull (boolExpr);
			Assert.AreEqual (false, boolExpr.evaluate ().asBool ());
			Assert.AreEqual ("false", boolExpr.evaluate ().asString ());
			Assert.AreEqual ("false", boolExpr.evaluate ().asVariableName ());
			Assert.AreEqual (0, boolExpr.evaluate ().asInt ());
			Assert.That (Values.NearlyEqual (0d, boolExpr.evaluate ().asDouble ()));
		}

	}
}
