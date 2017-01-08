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
		public void ImportFromTestFile () {
			// Arrange:
			QuestManager qm = QuestManager.Instance;

			// Act:
			qm.ImportQuestInfo(new TestQuestImporter());

			// Assert:
			IEnumerable<QuestInfo> questInfos = 
				from entry in qm.QuestDict
				select entry.Value;
			Assert.AreEqual(52, questInfos.Count());
		}

	}



	class TestQuestImporter : QuestImporter_I {

		static readonly string PUBLIC_GAMES_JSON_PATH = 
			Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "Server/JSON/publicgamesinfo.json");

		public QuestInfo[] import () {
			
			FileInfo file = new FileInfo(PUBLIC_GAMES_JSON_PATH);
			StreamReader reader = file.OpenText();
			string json = reader.ReadToEnd();
			reader.Close();

			return JsonConvert.DeserializeObject<QuestInfo[]>(json);
		}

	}

}
