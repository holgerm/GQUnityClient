using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Util;

namespace GQTests.Util {
	public class TextReplacementTest {

		[Test]
		public void ReplaceNullWithEmptyString () {
			Assert.AreEqual("", TextHelper.makeReplacements(null));
		}

		[Test]
		public void ReplaceBRWithNewLine () {
			// Arrange:
			string original = "This<br>text<br>has<br>five<br>lines.";

			// Act:
			string transformed = TextHelper.makeReplacements(original);

			// Assert:
			Assert.AreEqual("This\ntext\nhas\nfive\nlines.", transformed);
		}

		// TODO refactor Variables so that they can be tested and used for testing too.
		[Test, Ignore("todo")]
		public void ReplaceVarnameByValue () {
			// Arrange:
			actions questactions = GameObject.Find("QuestDatabase").GetComponent<actions>();
			questactions.setVariable("v1", "valueOne");

			// Assert:
			Assert.AreEqual("valueOne", TextHelper.makeReplacements("@v1@"));
		}
	}
}