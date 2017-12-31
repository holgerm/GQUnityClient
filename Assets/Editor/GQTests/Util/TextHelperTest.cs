using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Util;

namespace GQTests.Util
{
	public class TextHelperTest
	{

		[Test]
		public void StripQuotes ()
		{
			Assert.AreEqual ("", "".StripQuotes());
			Assert.AreEqual ("ohneQuote", "ohneQuote".StripQuotes());
			Assert.AreEqual ("mitQuote", "\"mitQuote\"".StripQuotes());
		}

	}
}