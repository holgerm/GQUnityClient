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

		[SetUp]
		public void ResetQMInstance () {
			QuestInfoManager.Reset();
		}

		[Test]
		public void PublishNewQuestInfoOnServer () {
			// Arrange Server has no quests:
			ConfigurationManager.Current.portal = 0;
			Mock.Use = true;
			Mock.DeclareGQServerResponseByString (
				"json/0/publicgamesinfo", 
				@"[]"
			);

			QuestInfoManager qm = QuestInfoManager.Instance;

			// Assert:
			Assert.NotNull(qm);
			Assert.AreEqual(0, qm.Count);

			// Publish new questInfo on server:
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

			// Update QuestInfoManager from Server:
			qm.UpdateQuestInfos();

			// Assert that the QM now knows the new quest:
			Assert.AreEqual(1, qm.Count);
			Assert.That(qm.ContainsQuestInfo (10557));
			// assert also some features for showing this quest info:
			QuestInfo info = qm.GetQuestInfo(10557);
			Assert.That (QuestListElementController.ShowDownloadOption (info));
			Assert.IsFalse (QuestListElementController.ShowStartOption (info));
			Assert.IsFalse (QuestListElementController.ShowUpdateOption (info));
			Assert.IsFalse (QuestListElementController.ShowDeleteOption (info));
		}

	}

}
