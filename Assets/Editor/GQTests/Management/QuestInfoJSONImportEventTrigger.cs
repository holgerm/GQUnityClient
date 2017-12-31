using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model;
using GQ.Editor.Util;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using GQ.Client.Util;
using GQTests.Util;

namespace GQTests.Management {

	public class QuestInfoJSONImportEventTrigger : JSONQuestInfoTest {

		[Test]
		public void ImportEmptyList () {
			// Arrange:
			StringProviderTask provideTestJSON = new StringProviderTask(
				"[]"
			);

			TaskSequence seq = new TaskSequence (provideTestJSON, importTask);

			// Act:
			seq.Start();

			// Assert:
			List<QuestInfo> qiList = qim.GetListOfQuestInfos();

			Assert.AreEqual(0, qiList.Count());
		}

		[Test]
		public void AddFirstQuest() {
			// Pre-Assert:
			Assert.AreEqual(0, testListener.added);

			// Act: import json that adds one quest:
			loadJSON (JSON_QUEST_A_10557);

			// Assert:
			Assert.AreEqual (1, testListener.added);
			Assert.AreEqual (1, qim.Count);
		}

		[Test]
		public void AddSecondQuest() {
			// Pre-Assert:
			Assert.AreEqual(0, testListener.added);

			// Arrange:
			loadJSON (JSON_QUEST_A_10557);

			// Act:
			loadJSON (JSON_QUEST_B_10558);

			// Assert:
			Assert.AreEqual (2, testListener.added);
			Assert.AreEqual (2, qim.Count);
		}

		[Test]
		public void RemoveQuests() {
			// Arrange:
			loadJSON (JSON_QUEST_A_10557);
			loadJSON (JSON_QUEST_B_10558);

			// Pre-Assert:
			Assert.AreEqual(2, testListener.added);

			// Act:
			loadJSON (JSON_QUEST_B_10558);

			// Assert:
			Assert.AreEqual (2, testListener.added);
			Assert.AreEqual (2, qim.Count);
		}

	}


}
