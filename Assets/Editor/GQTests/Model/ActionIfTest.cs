﻿using NUnit.Framework;
using GQ.Editor.Util;
using System.IO;
using Code.GQClient.Model.expressions;
using Code.GQClient.Model.mgmt.quests;

namespace GQTests.Model
{

	public class ActionIfTest : GQMLTest {

		[Test]
		public void CallThenBranch ()
		{
			// Arrange:
			xml = File.ReadAllText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/IfActionCallThen/game.xml"));

			// Act:
			qm.SetCurrentQuestFromXml (xml);
			QuestManager.Instance.CurrentQuest.Start ();

			// Assert:
			Assert.AreEqual (30309, QuestManager.Instance.CurrentPage.Id);
			Assert.AreEqual ("Then called. Good.", Variables.GetValue("B").AsString());
		}

		[Test]
		public void CallElseBranch ()
		{
			// Arrange:
			xml = File.ReadAllText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/IfActionCallElse/game.xml"));

			// Act:
			qm.SetCurrentQuestFromXml (xml);
			QuestManager.Instance.CurrentQuest.Start ();

			// Assert:
			Assert.AreEqual (30368, QuestManager.Instance.CurrentPage.Id);
			Assert.AreEqual ("Else Called. Good.", Variables.GetValue("B").AsString());
		}

		[Test]
		public void CallThenThenBranch ()
		{
			// Arrange:
			xml = File.ReadAllText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/IfActionDeepThenThen/game.xml"));

			// Act:
			qm.SetCurrentQuestFromXml (xml);
			QuestManager.Instance.CurrentQuest.Start ();

			// Assert:
			Assert.AreEqual ("Reached the right branch (then, then) since (A=1, B=1).", Variables.GetValue("C").AsString());
		}

		[Test]
		public void CallElseElseBranch ()
		{
			// Arrange:
			xml = File.ReadAllText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/IfActionDeepElseElse/game.xml"));

			// Act:
			qm.SetCurrentQuestFromXml (xml);
			QuestManager.Instance.CurrentQuest.Start ();

			// Assert:
			Assert.AreEqual ("Reached the right branch (else, else) since (A=22, B=22)", Variables.GetValue("C").AsString());
		}
	}
}
