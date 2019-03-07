using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model;

namespace GQTests.Model
{
	public class ValueTest
	{
		[Test]
		public void NullValueType ()
		{
			// Assert:
			Assert.AreEqual (Value.Type.NULL, Value.Null.ValType);
			Assert.AreEqual ("", Value.Null.AsString ());
			Assert.AreEqual (0, Value.Null.AsInt ());
			Assert.AreEqual (0d, Value.Null.AsDouble ());
			Assert.AreEqual (false, Value.Null.AsBool ());
		}

		[Test]
		public void CreateIntFromRawStrings ()
		{
			Assert.AreEqual (
				new Value (10), 
				Value.CreateValueFromRawString ("10")
			);
			Assert.AreEqual (
				new Value (0), 
				Value.CreateValueFromRawString ("0")
			);
			Assert.AreEqual (
				new Value (-10), 
				Value.CreateValueFromRawString ("-10")
			);
            Assert.AreEqual(
                new Value(10),
                Value.CreateValueFromRawString(" 10")
            );
            Assert.AreEqual(
                new Value(10),
                Value.CreateValueFromRawString(" 10 ")
            );
            Assert.AreEqual(
                new Value(10),
                Value.CreateValueFromRawString("10  ")
            );
        }

        [Test]
		public void CreateFloatFromRawStrings ()
		{
			Assert.AreEqual (
				new Value (10.02d), 
				Value.CreateValueFromRawString ("10.02")
			);
			Assert.AreEqual (
				new Value (0d), 
				Value.CreateValueFromRawString ("0.0")
			);
			Assert.AreEqual (
				new Value (-10.02d), 
				Value.CreateValueFromRawString ("-10.02")
			);
		}

		[Test]
		public void CreateFloatUsingCommaFromRawStrings ()
		{
			Assert.AreEqual (
				new Value (10.02d), 
				Value.CreateValueFromRawString ("10,02")
			);
			Assert.AreEqual (
				new Value (0d), 
				Value.CreateValueFromRawString ("0,0")
			);
			Assert.AreEqual (
				new Value (-10.02d), 
				Value.CreateValueFromRawString ("-10,02")
			);
		}

		[Test]
		public void CreateBoolFromRawString ()
		{
			Assert.AreEqual (
				new Value (true), 
				Value.CreateValueFromRawString ("true")
			);
			Assert.AreEqual (
				new Value (true), 
				Value.CreateValueFromRawString ("TRUE")
			);
			Assert.AreEqual (
				new Value (false), 
				Value.CreateValueFromRawString ("false")
			);
			Assert.AreEqual (
				new Value (false), 
				Value.CreateValueFromRawString ("FALSE")
			);
		}

		[Test]
		public void CreateStringFromRawString ()
		{
			Assert.AreEqual (
				new Value ("Hallo", Value.Type.Text), 
				Value.CreateValueFromRawString ("Hallo")
			);
			Assert.AreEqual (
				new Value ("", Value.Type.Text), 
				Value.CreateValueFromRawString ("")
			);
			Assert.AreEqual (
				new Value ("...", Value.Type.Text), 
				Value.CreateValueFromRawString ("...")
			);
			Assert.AreEqual (
				new Value ("13...", Value.Type.Text), 
				Value.CreateValueFromRawString ("13...")
			);
		}
	}
}
