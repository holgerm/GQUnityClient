using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.IO;
using GQ.Editor.Util;
using Newtonsoft.Json;
using GQ.Client.Model;
using System;
using System.Linq;
using System.Collections.Generic;


using System.Collections.Generic;

namespace GQTests.Model {

	public class QuestInfoTest {

		string publicGamesJson;

		/// <summary>
		/// The path to the JSON file with some quests (from WCC):
		/// </summary>
		static public readonly string JSON_InitFromServer = Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "JSON/QuestInfos/initialFromServer.json");


		[SetUp]
		public void ResetQMInstance () {
			QuestInfoManager.Reset();
		}

		#region Main Use Cases:

		[Test]
		public void InitialImport () {
			// Arrange:
			QuestInfoManager qm = QuestInfoManager.Instance;
			QuestInfo[] quests = null;

			// Act:
			string serverJSON = Files.ReadText(JSON_InitFromServer);
			qm.Import(QuestInfoImportExtension.ParseQuestInfoJSON(serverJSON));

			// Assert:
			IEnumerable<QuestInfo> questInfos = 
				from entry in qm.QuestDict
				select entry.Value;
			foreach ( QuestInfo qi in questInfos ) {
				Assert.False(qi.IsLocallyAvailable());
				Assert.True(qi.IsNew());
				Assert.True(qi.IsDownloadable());
				Assert.False(qi.IsUpdatable());
				Assert.False(qi.IsDeletable());
				Assert.False(qi.WarnBeforeDeletion());
			}
		}

		[Test]
		public void WhenNewQuestIsLoaded () {
			// Arrange:
			QuestInfoManager qm = QuestInfoManager.Instance;
			QuestInfo[] quests = null;
			string serverJSON = Files.ReadText(JSON_InitFromServer);
			qm.Import(QuestInfoImportExtension.ParseQuestInfoJSON(serverJSON));

			// Act:
			// TODO

			// Assert:
			// TODO
			Assert.Fail("Test not yet implemented!");
		}

		#endregion

	}
}
