using Code.GQClient.Util;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

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


		[Test]
		public void Capitalize() {
			Assert.AreEqual ("", "".Capitalize ());
			Assert.AreEqual ("Capitalized", "capitalized".Capitalize ());
			Assert.AreEqual ("Q", "q".Capitalize ());
			Assert.AreEqual ("_no_change", "_no_change".Capitalize ());
			Assert.AreEqual ("Q", "Q".Capitalize ());
			Assert.AreEqual ("Unchanged", "Unchanged".Capitalize ());
		}

	}
}