using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model.XML;
using System.IO;
using System.Xml.Serialization;
using GQ.Client.Model;
using GQ.Client.Util;
using System;

namespace GQTests.Model.XML
{

	public class NumberExpressionTest : XMLTest
	{

		protected override string XmlRoot {
			get {
				return "num";
			}
		}


		[Test]
		public void Number_XML_0_Test ()
		{
			// Act:
			NumberExpression numberExpr = parseXML<NumberExpression> (@"<num>0</num>");

			// Assert:
			Assert.IsNotNull (numberExpr);
			Value val = numberExpr.evaluate ();
			Assert.IsNotNull (val);
			Assert.AreEqual (0, numberExpr.evaluate ().asInt ());
			Assert.That (Values.NearlyEqual (0d, numberExpr.evaluate ().asDouble ()));
			Assert.AreEqual ("0", numberExpr.evaluate ().asString ());
			Assert.AreEqual (Variables.UNDEFINED_VAR, numberExpr.evaluate ().asVariableName ());
			Assert.AreEqual (false, numberExpr.evaluate ().asBool ());
		}

		[Test]
		public void Number_XML_123456_Test ()
		{
			// Act:
			NumberExpression numberExpr = parseXML<NumberExpression> (@"<num>123456</num>");

			// Assert:
			Assert.IsNotNull (numberExpr);
			Value val = numberExpr.evaluate ();
			Assert.IsNotNull (val);
			Assert.AreEqual (123456, numberExpr.evaluate ().asInt ());
			Assert.That (Values.NearlyEqual (123456d, numberExpr.evaluate ().asDouble ()));
			Assert.AreEqual ("123456", numberExpr.evaluate ().asString ());
			Assert.AreEqual (Variables.UNDEFINED_VAR, numberExpr.evaluate ().asVariableName ());
			Assert.AreEqual (true, numberExpr.evaluate ().asBool ());
		}

		[Test]
		public void Number_XML_MaxInt_Test ()
		{
			// Act:
			NumberExpression numberExpr = parseXML<NumberExpression> (@"<num>2147483647</num>");

			// Assert:
			Assert.IsNotNull (numberExpr);
			Value val = numberExpr.evaluate ();
			Assert.IsNotNull (val);
			Assert.AreEqual (2147483647, numberExpr.evaluate ().asInt ());
			Assert.That (Values.NearlyEqual (2147483647d, numberExpr.evaluate ().asDouble ()));
			Assert.AreEqual ("2147483647", numberExpr.evaluate ().asString ());
			Assert.AreEqual (Variables.UNDEFINED_VAR, numberExpr.evaluate ().asVariableName ());
			Assert.AreEqual (true, numberExpr.evaluate ().asBool ());
		}

		[Test]
		public void Number_XML_IntOverflow_Test ()
		{
			// Act:
			NumberExpression numberExpr = parseXML<NumberExpression> (@"<num>2147483648</num>");

			// Assert:
			Assert.IsNotNull (numberExpr);
			Value val = numberExpr.evaluate ();
			Assert.IsNotNull (val);
			Assert.AreEqual (Int32.MaxValue, numberExpr.evaluate ().asInt ());
			Assert.That (Values.NearlyEqual (2147483648d, numberExpr.evaluate ().asDouble ()));
			Assert.AreEqual ("2147483648", numberExpr.evaluate ().asString ());
			Assert.AreEqual (Variables.UNDEFINED_VAR, numberExpr.evaluate ().asVariableName ());
			Assert.AreEqual (true, numberExpr.evaluate ().asBool ());
		}

		[Test]
		public void Number_XML_MinInt_Test ()
		{
			// Act:
			NumberExpression numberExpr = parseXML<NumberExpression> (@"<num>-2147483648</num>");

			// Assert:
			Assert.IsNotNull (numberExpr);
			Value val = numberExpr.evaluate ();
			Assert.IsNotNull (val);
			Assert.AreEqual (-2147483648, numberExpr.evaluate ().asInt ());
			Assert.That (Values.NearlyEqual (-2147483648d, numberExpr.evaluate ().asDouble ()));
			Assert.AreEqual ("-2147483648", numberExpr.evaluate ().asString ());
			Assert.AreEqual (Variables.UNDEFINED_VAR, numberExpr.evaluate ().asVariableName ());
			Assert.AreEqual (true, numberExpr.evaluate ().asBool ());
		}

		[Test]
		public void Number_XML_IntUnderflow_Test ()
		{
			// Act:
			NumberExpression numberExpr = parseXML<NumberExpression> (@"<num>-2147483649</num>");

			// Assert:
			Assert.IsNotNull (numberExpr);
			Value val = numberExpr.evaluate ();
			Assert.IsNotNull (val);
			Assert.AreEqual (Int32.MinValue, numberExpr.evaluate ().asInt ());
			Assert.That (Values.NearlyEqual (-2147483649d, numberExpr.evaluate ().asDouble ()));
			Assert.AreEqual ("-2147483649", numberExpr.evaluate ().asString ());
			Assert.AreEqual (Variables.UNDEFINED_VAR, numberExpr.evaluate ().asVariableName ());
			Assert.AreEqual (true, numberExpr.evaluate ().asBool ());
		}

		[Test, Ignore]
		public void Number_XML_IntWithThousandsSepDE_Test ()
		{
			// Act:
			Base.Init ();
			NumberExpression numberExpr = parseXML<NumberExpression> (@"<num>123.456.789</num>");

			// Assert:
			Assert.IsNotNull (numberExpr);
			Value val = numberExpr.evaluate ();
			Assert.IsNotNull (val);
			Assert.AreEqual (123456789, numberExpr.evaluate ().asInt ());
			Assert.That (Values.NearlyEqual (123456789d, numberExpr.evaluate ().asDouble ()));
			Assert.AreEqual ("123.456.789", numberExpr.evaluate ().asString ());
			Assert.AreEqual (Variables.UNDEFINED_VAR, numberExpr.evaluate ().asVariableName ());
			Assert.AreEqual (true, numberExpr.evaluate ().asBool ());
		}

		[Test]
		public void NumberFloat_Test ()
		{
			// Act:
//			Base.Init ();
			NumberExpression numberExpr = parseXML<NumberExpression> (@"<num>100.007</num>");

			// Assert:
			Assert.IsNotNull (numberExpr);
			Assert.IsNotNull (numberExpr.evaluate ());
			Assert.AreEqual (100, numberExpr.evaluate ().asInt ());
			Assert.That (Values.NearlyEqual (100.007d, numberExpr.evaluate ().asDouble ()));
			Assert.AreEqual ("100.007", numberExpr.evaluate ().asString ());
			Assert.AreEqual (Variables.UNDEFINED_VAR, numberExpr.evaluate ().asVariableName ());
			Assert.AreEqual (true, numberExpr.evaluate ().asBool ());
		}


		[Test]
		public void NumberFloat_Comma_Test ()
		{
			// Act:
			//			Base.Init ();
			NumberExpression numberExpr = parseXML<NumberExpression> (@"<num>100,007</num>");

			// Assert:
			Assert.IsNotNull (numberExpr);
			Assert.IsNotNull (numberExpr.evaluate ());
			Assert.AreEqual (100, numberExpr.evaluate ().asInt ());
			Assert.That (Values.NearlyEqual (100.007d, numberExpr.evaluate ().asDouble ()));
			Assert.AreEqual ("100,007", numberExpr.evaluate ().asString ());
			Assert.AreEqual (Variables.UNDEFINED_VAR, numberExpr.evaluate ().asVariableName ());
			Assert.AreEqual (true, numberExpr.evaluate ().asBool ());
		}

		[Test]
		public void Number_Invalid_XML_Test ()
		{
			// Act:
			//			Base.Init ();
			NumberExpression numberExpr = parseXML<NumberExpression> (@"<num>thisIsNotANumber</num>");

			// Assert:
			Assert.IsNotNull (numberExpr);
			Assert.IsNotNull (numberExpr.evaluate ());
			Assert.AreEqual (0, numberExpr.evaluate ().asInt ());
			Assert.That (Values.NearlyEqual (0d, numberExpr.evaluate ().asDouble ()));
			Assert.AreEqual ("0", numberExpr.evaluate ().asString ());
			Assert.AreEqual (Variables.UNDEFINED_VAR, numberExpr.evaluate ().asVariableName ());
			Assert.AreEqual (false, numberExpr.evaluate ().asBool ());
		}
	}
}
