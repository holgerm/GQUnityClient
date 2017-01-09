using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model;
using GQ.Editor.Util;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace GQTests.Model {

	public class QuestManagerTest {

		static readonly string PUBLIC_GAMES_JSON_PATH = 
			Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "Server/JSON/publicgamesinfo.json");

		[SetUp]
		public void ResetQMInstance () {
			QuestManager.Reset();
		}

		[Test]
		public void InitQM () {
			// Arrange:
			QuestManager qm = null;

			// Act:
			qm = QuestManager.Instance;

			// Assert:
			Assert.NotNull(qm);
			Assert.AreEqual(0, qm.Count);
		}

		[Test]
		public void ImportNull () {
			// Arrange:
			QuestManager qm = QuestManager.Instance;
			QuestInfo[] quests = null;

			// Act:
			qm.Import(quests);

			// Assert:
			IEnumerable<QuestInfo> questInfos = 
				from entry in qm.QuestDict
				select entry.Value;
			Assert.AreEqual(0, questInfos.Count());
		}


		[Test]
		public void ImportFromTestFile () {
			// Arrange:
			QuestManager qm = QuestManager.Instance;
			QuestInfo[] quests = null;
			TestQuestImporter mockImporter = new TestQuestImporter();
			Assert.False(mockImporter.IsDone);
			Assert.Null(mockImporter.ImportedQuests);

			// Act:
			mockImporter.StartExtractQuestInfosFromFile(PUBLIC_GAMES_JSON_PATH);
			qm.Import(mockImporter.ImportedQuests);

			// Assert:
			IEnumerable<QuestInfo> questInfos = 
				from entry in qm.QuestDict
				select entry.Value;
			Assert.AreEqual(52, questInfos.Count());
			Assert.True(mockImporter.IsDone);
			Assert.NotNull(mockImporter.ImportedQuests);
		}

	}



	class TestQuestImporter : QuestImporter_I {

		public QuestInfo[] ImportedQuests = null;

		public bool IsDone = false;

		public void ImportDone (QuestInfo[] quests) {
			
			IsDone = true;
			ImportedQuests = quests;
		}

	}

}
