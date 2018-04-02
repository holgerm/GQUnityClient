using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model;
using GQ.Editor.Util;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using QM.Mocks;
using GQ.Client.Conf;
using System;
using GQ.Client.UI.Foyer;

namespace GQTests.Management {

	public class QuestInfoChanges : AbstractMockTest {

		#region SetUp
		QuestInfoManager QM;

		[SetUp]
		public void SetUp () {
			QuestInfoManager.Reset();
			QM = QuestInfoManager.Instance;
			ConfigurationManager.Current.portal = 0;
			Mock.Use = true;
		}
		#endregion

		#region Tests
		[Test]
		public void PublishNewQuestInfoOnServer () {
			MOCK_EmptyServer();
			ASSERT_ServerHasNoQuests ();

			MOCK_PublishNewQuest ();
			QM.UpdateQuestInfos();
			ASSERT_QM_ShowsNewQuest_OffersDownload ();
		}
		#endregion

		#region Helpers
		void MOCK_EmptyServer() {
			Mock.DeclareGQServerResponseByString (
				"json/0/publicgamesinfo", 
				@"[]"
			);
		}

		void ASSERT_ServerHasNoQuests() {
			Assert.NotNull(QM);
			Assert.AreEqual(0, QM.Count);
		}

		void MOCK_PublishNewQuest() {
			Mock.DeclareGQServerResponseByString (
				"json/0/publicgamesinfo", 
				@"[
				    {
				        ""hotspots"": [
				            {
				                ""latitude"": 50.73447,
				                ""longitude"": 7.104104
				            },
				            {
				                ""latitude"": 50.73447,
				                ""longitude"": 7.104104
				            }
				        ],
				        ""id"": 10557,
				        ""lastUpdate"": 1505465979827,
				        ""metadata"": [
				            {
				                ""key"": ""category"",
				                ""value"": ""wcc.tour.beethoven""
				            }
				        ],
				        ""name"": ""Franziskanerkirche"",
				        ""typeID"": 3318
				    }
				]"
			);
		}

		void ASSERT_QM_ShowsNewQuest_OffersDownload() {
			Assert.AreEqual(1, QM.Count);
			Assert.That(QM.ContainsQuestInfo (10557));
			// assert also some features for showing this quest info:
			QuestInfo info = QM.GetQuestInfo(10557);
			Assert.That (QuestListElementController.ShowDownloadOption (info));
			Assert.IsFalse (QuestListElementController.ShowStartOption (info));
			Assert.IsFalse (QuestListElementController.ShowUpdateOption (info));
			Assert.IsFalse (QuestListElementController.ShowDeleteOption (info));
		}
		#endregion

	}

}
