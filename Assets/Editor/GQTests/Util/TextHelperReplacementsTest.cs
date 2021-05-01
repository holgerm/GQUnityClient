using Code.GQClient.Util;
using NUnit.Framework;

namespace GQTests.Util
{
	public class TextHelperReplacementsTest
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
			string transformed = original.MakeReplacements ();

			// Assert:
			Assert.AreEqual ("This\ntext\nhas\nfive\nlines.", transformed);
		}

	}
}