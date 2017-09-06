using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Editor.Util;
using GQ.Client.Model;

namespace GQTests.Model
{

	public class ActionIfTest : GQMLTest {

		[Test]
		public void CallThenBranch ()
		{
			// Arrange:
			xml = Files.ReadText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/IfActionCallThen/game.xml"));

			// Act:
			Quest q = qm.DeserializeQuest (xml);
			q.Start ();

			// Assert:
			Assert.AreEqual (30309, QuestManager.Instance.CurrentPage.Id);
			Assert.AreEqual ("Then called. Good.", Variables.GetValue("B").AsString());
		}

		[Test]
		public void CallElseBranch ()
		{
			// Arrange:
			xml = Files.ReadText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/IfActionCallElse/game.xml"));

			// Act:
			Quest q = qm.DeserializeQuest (xml);
			q.Start ();

			// Assert:
			Assert.AreEqual (30368, QuestManager.Instance.CurrentPage.Id);
			Assert.AreEqual ("Else Called. Good.", Variables.GetValue("B").AsString());
		}

		[Test]
		public void CallThenThenBranch ()
		{
			// Arrange:
			xml = Files.ReadText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/IfActionDeepThenThen/game.xml"));

			// Act:
			Quest q = qm.DeserializeQuest (xml);
			q.Start ();

			// Assert:
			Assert.AreEqual ("Reached the right branch (then, then) since (A=1, B=1).", Variables.GetValue("C").AsString());
		}

		[Test]
		public void CallElseElseBranch ()
		{
			// Arrange:
			xml = Files.ReadText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/IfActionDeepElseElse/game.xml"));

			// Act:
			Quest q = qm.DeserializeQuest (xml);
			q.Start ();

			// Assert:
			Assert.AreEqual ("Reached the right branch (else, else) since (A=22, B=22)", Variables.GetValue("C").AsString());
		}
	}
}
