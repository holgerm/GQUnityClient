using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Editor.Util;
using GQTests;
using GQ.Client.Model;

namespace GQTests.Model {

	public class DeserializePageNPCTalkTest : DeserializationTest {

		[Test]
		public void QuestWith1NPCTalk () {
			// Arrange:
			xml = Files.ReadText(Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/QuestWith1NPCTalk/game.xml"));

			// Act:
			Quest q = qm.Import(xml);

			// Assert:
			Assert.AreEqual("QuestWith1NPCTalk", q.Name);
			Assert.AreEqual(9802, q.Id);
			QuestPage npcPage = q.GetPageWithID(26821);
			Assert.NotNull(npcPage);
		}

	}
}
