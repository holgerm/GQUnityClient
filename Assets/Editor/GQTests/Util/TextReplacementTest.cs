using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Util;

namespace GQTests.Util
{
	public class TextReplacementTest
	{

		[Test]
		public void ReplaceNullWithEmptyString ()
		{
			Assert.AreEqual ("", TextHelper.MakeReplacements (null));
		}

		[Test]
		public void ReplaceBRWithNewLine ()
		{
			// Arrange:
			string original = "This<br>text<br>has<br>five<br>lines.";

			// Act:
			string transformed = TextHelper.MakeReplacements (original);

			// Assert:
			Assert.AreEqual ("This\ntext\nhas\nfive\nlines.", transformed);
		}

	}
}