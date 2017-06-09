using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model;
using System.IO;
using System.Xml.Serialization;
using GQ.Client.Model;
using GQ.Client.Util;
using System;

namespace GQTests.Model
{

	public class ExpressionNumberTest : XMLTest
	{

		[SetUp]
		public void Init ()
		{
			XmlRoot = "num";
		}

		[Test]
		public void Number_XML_0_Test ()
		{
			// Act:
			NumberExpression numberExpr = parseXML<NumberExpression> (@"<num>0</num>");

			// Assert:
			Assert.IsNotNull (numberExpr);
			Value val = numberExpr.Evaluate ();
			Assert.IsNotNull (val);
			Assert.AreEqual (0, numberExpr.Evaluate ().AsInt ());
			Assert.That (Values.NearlyEqual (0d, numberExpr.Evaluate ().AsDouble ()));
			Assert.AreEqual ("0", numberExpr.Evaluate ().AsString ());
			Assert.AreEqual (Variables.UNDEFINED_VAR, numberExpr.Evaluate ().AsVariableName ());
			Assert.AreEqual (false, numberExpr.Evaluate ().AsBool ());
		}

		[Test]
		public void Number_XML_123456_Test ()
		{
			// Act:
			NumberExpression numberExpr = parseXML<NumberExpression> (@"<num>123456</num>");

			// Assert:
			Assert.IsNotNull (numberExpr);
			Value val = numberExpr.Evaluate ();
			Assert.IsNotNull (val);
			Assert.AreEqual (123456, numberExpr.Evaluate ().AsInt ());
			Assert.That (Values.NearlyEqual (123456d, numberExpr.Evaluate ().AsDouble ()));
			Assert.AreEqual ("123456", numberExpr.Evaluate ().AsString ());
			Assert.AreEqual (Variables.UNDEFINED_VAR, numberExpr.Evaluate ().AsVariableName ());
			Assert.AreEqual (true, numberExpr.Evaluate ().AsBool ());
		}

		[Test]
		public void Number_XML_MaxInt_Test ()
		{
			// Act:
			NumberExpression numberExpr = parseXML<NumberExpression> (@"<num>2147483647</num>");

			// Assert:
			Assert.IsNotNull (numberExpr);
			Value val = numberExpr.Evaluate ();
			Assert.IsNotNull (val);
			Assert.AreEqual (2147483647, numberExpr.Evaluate ().AsInt ());
			Assert.That (Values.NearlyEqual (2147483647d, numberExpr.Evaluate ().AsDouble ()));
			Assert.AreEqual ("2147483647", numberExpr.Evaluate ().AsString ());
			Assert.AreEqual (Variables.UNDEFINED_VAR, numberExpr.Evaluate ().AsVariableName ());
			Assert.AreEqual (true, numberExpr.Evaluate ().AsBool ());
		}

		[Test]
		public void Number_XML_IntOverflow_Test ()
		{
			// Act:
			NumberExpression numberExpr = parseXML<NumberExpression> (@"<num>2147483648</num>");

			// Assert:
			Assert.IsNotNull (numberExpr);
			Value val = numberExpr.Evaluate ();
			Assert.IsNotNull (val);
			Assert.AreEqual (Int32.MaxValue, numberExpr.Evaluate ().AsInt ());
			Assert.That (Values.NearlyEqual (2147483648d, numberExpr.Evaluate ().AsDouble ()));
			Assert.AreEqual ("2147483648", numberExpr.Evaluate ().AsString ());
			Assert.AreEqual (Variables.UNDEFINED_VAR, numberExpr.Evaluate ().AsVariableName ());
			Assert.AreEqual (true, numberExpr.Evaluate ().AsBool ());
		}

		[Test]
		public void Number_XML_MinInt_Test ()
		{
			// Act:
			NumberExpression numberExpr = parseXML<NumberExpression> (@"<num>-2147483648</num>");

			// Assert:
			Assert.IsNotNull (numberExpr);
			Value val = numberExpr.Evaluate ();
			Assert.IsNotNull (val);
			Assert.AreEqual (-2147483648, numberExpr.Evaluate ().AsInt ());
			Assert.That (Values.NearlyEqual (-2147483648d, numberExpr.Evaluate ().AsDouble ()));
			Assert.AreEqual ("-2147483648", numberExpr.Evaluate ().AsString ());
			Assert.AreEqual (Variables.UNDEFINED_VAR, numberExpr.Evaluate ().AsVariableName ());
			Assert.AreEqual (true, numberExpr.Evaluate ().AsBool ());
		}

		[Test]
		public void Number_XML_IntUnderflow_Test ()
		{
			// Act:
			NumberExpression numberExpr = parseXML<NumberExpression> (@"<num>-2147483649</num>");

			// Assert:
			Assert.IsNotNull (numberExpr);
			Value val = numberExpr.Evaluate ();
			Assert.IsNotNull (val);
			Assert.AreEqual (Int32.MinValue, numberExpr.Evaluate ().AsInt ());
			Assert.That (Values.NearlyEqual (-2147483649d, numberExpr.Evaluate ().AsDouble ()));
			Assert.AreEqual ("-2147483649", numberExpr.Evaluate ().AsString ());
			Assert.AreEqual (Variables.UNDEFINED_VAR, numberExpr.Evaluate ().AsVariableName ());
			Assert.AreEqual (true, numberExpr.Evaluate ().AsBool ());
		}

		[Test, Ignore]
		public void Number_XML_IntWithThousandsSepDE_Test ()
		{
			// Act:
			Base.Init ();
			NumberExpression numberExpr = parseXML<NumberExpression> (@"<num>123.456.789</num>");

			// Assert:
			Assert.IsNotNull (numberExpr);
			Value val = numberExpr.Evaluate ();
			Assert.IsNotNull (val);
			Assert.AreEqual (123456789, numberExpr.Evaluate ().AsInt ());
			Assert.That (Values.NearlyEqual (123456789d, numberExpr.Evaluate ().AsDouble ()));
			Assert.AreEqual ("123.456.789", numberExpr.Evaluate ().AsString ());
			Assert.AreEqual (Variables.UNDEFINED_VAR, numberExpr.Evaluate ().AsVariableName ());
			Assert.AreEqual (true, numberExpr.Evaluate ().AsBool ());
		}

		[Test]
		public void NumberFloat_Test ()
		{
			// Act:
//			Base.Init ();
			NumberExpression numberExpr = parseXML<NumberExpression> (@"<num>100.007</num>");

			// Assert:
			Assert.IsNotNull (numberExpr);
			Assert.IsNotNull (numberExpr.Evaluate ());
			Assert.AreEqual (100, numberExpr.Evaluate ().AsInt ());
			Assert.That (Values.NearlyEqual (100.007d, numberExpr.Evaluate ().AsDouble ()));
			Assert.AreEqual ("100.007", numberExpr.Evaluate ().AsString ());
			Assert.AreEqual (Variables.UNDEFINED_VAR, numberExpr.Evaluate ().AsVariableName ());
			Assert.AreEqual (true, numberExpr.Evaluate ().AsBool ());
		}


		[Test]
		public void NumberFloat_Comma_Test ()
		{
			// Act:
			//			Base.Init ();
			NumberExpression numberExpr = parseXML<NumberExpression> (@"<num>100,007</num>");

			// Assert:
			Assert.IsNotNull (numberExpr);
			Assert.IsNotNull (numberExpr.Evaluate ());
			Assert.AreEqual (100, numberExpr.Evaluate ().AsInt ());
			Assert.That (Values.NearlyEqual (100.007d, numberExpr.Evaluate ().AsDouble ()));
			Assert.AreEqual ("100,007", numberExpr.Evaluate ().AsString ());
			Assert.AreEqual (Variables.UNDEFINED_VAR, numberExpr.Evaluate ().AsVariableName ());
			Assert.AreEqual (true, numberExpr.Evaluate ().AsBool ());
		}

		[Test]
		public void Number_Invalid_XML_Test ()
		{
			// Act:
			//			Base.Init ();
			NumberExpression numberExpr = parseXML<NumberExpression> (@"<num>thisIsNotANumber</num>");

			// Assert:
			Assert.IsNotNull (numberExpr);
			Assert.IsNotNull (numberExpr.Evaluate ());
			Assert.AreEqual (0, numberExpr.Evaluate ().AsInt ());
			Assert.That (Values.NearlyEqual (0d, numberExpr.Evaluate ().AsDouble ()));
			Assert.AreEqual ("0", numberExpr.Evaluate ().AsString ());
			Assert.AreEqual (Variables.UNDEFINED_VAR, numberExpr.Evaluate ().AsVariableName ());
			Assert.AreEqual (false, numberExpr.Evaluate ().AsBool ());
		}
	}
}
