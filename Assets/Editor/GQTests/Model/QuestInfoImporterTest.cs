using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using GQ.Editor.Util;
using System;
using System.IO;

namespace GQTests.Model {
	 
	public class QuestInfoImporterTest {

		[Test]
		public void ShowReadQuests () {
			// Arrange:
			QuestInfo[] quests = null;
			string publicGamesJson = Files.ReadText(QuestInfoTest.JSON_InitFromServer);

			// Act:
			try {
				quests = QuestInfoImportExtension.ParseQuestInfoJSON(publicGamesJson);
			} catch ( Exception e ) {
				Debug.LogWarning("Exception occurred while reading public games json: " + e.Message);
			}

			foreach ( QuestInfo qi in quests ) {
				Debug.Log(qi.ToString());
			}

			Debug.Log(String.Format("We have read {0} quests.", quests != null ? quests.Length : 0));
		}

		[Test]
		public void ParseJSONString () {
			// Arrange:
			// TODO
			string json52 = Files.ReadText(QuestInfoTest.JSON_InitFromServer);
			QuestInfo[] quests = null;

			// Act:
			quests = QuestInfoImportExtension.ParseQuestInfoJSON(json52);


			// Assert:
			// TODO
			Assert.AreEqual(52, quests.Length);
		}

		[Test]
		public void ImportDoneGetsCalled () {
			// Arrange:
			QuestInfoManager qm = QuestInfoManager.Instance;
			QuestInfoImporter_I mockImporter = Substitute.For<QuestInfoImporter_I>();

			// Act:
			mockImporter.StartImportQuestInfos(new TestQuestInfoImporter());

			// Assert:
			mockImporter.Received(1).ImportQuestInfoDone();
		}

	}


	class TestQuestInfoImporter : QuestInfoLoader_I {

		public void StartLoadingJSON (QuestInfoImporter_I importer) {

			string json = Files.ReadText(QuestInfoTest.JSON_InitFromServer);

			importer.QuestInfoLoaded(json);				
		}

	}

}