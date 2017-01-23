using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Editor.Util;
using GQTests;
using GQ.Client.Model;

namespace GQTests.Model {

	public class QuestManagerTest {

		string xml;
		QuestManager qm;

		[SetUp]
		public void Init () { 
		
			QuestManager.Reset();
			qm = QuestManager.Instance;
		}


		[Test]
		public void ImportMinimalQuest () {
			// Arrange:
			xml = Files.ReadText(Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/MinimalQuest/game.xml"));

			// Act:
			Quest q = qm.Import(xml);

			// Assert:
			Assert.AreEqual("Minimal Quest", q.Name);
			Assert.AreEqual(9801, q.Id);
			Assert.AreEqual(0, q.PageList.Count);
			Assert.AreEqual(0, q.hotspotList.Count);
		}

		[Test]
		public void ImportQuestWithNPCTalk () {
			// Arrange:
			xml = Files.ReadText(Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/QuestWithNPCTalk/game.xml"));

			// Act:
			Quest q = qm.Import(xml);

			// Assert:
			Assert.AreEqual("QuestWithNPCTalk", q.Name);
			Assert.AreEqual(9802, q.Id);
			Assert.AreEqual(1, q.PageList.Count);
			Assert.AreEqual(0, q.hotspotList.Count);
		}

		[Test]
		public void ImportOnlyHotspots () {
			// Arrange:
			xml = Files.ReadText(Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/OnlyHotspots/game.xml"));

			// Act:
			Quest q = qm.Import(xml);

			// Assert:
			Assert.AreEqual("OnlyHotspots", q.Name);
			Assert.AreEqual(9803, q.Id);
			Assert.AreEqual(0, q.PageList.Count);
			Assert.AreEqual(3, q.hotspotList.Count);
		}
	}
}
