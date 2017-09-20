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

namespace GQTests.Managent {

	public class ImportQuestInfosFromJSONTest {

		ImportQuestInfosFromJSON importTask;
		QuestInfoManager qim;

		[SetUp]
		public void SetupTask () {
			qim = QuestInfoManager.Instance;
			importTask = new ImportQuestInfosFromJSON (true);
		}


		[Test]
		public void ImportNull () {
			// Arrange:
			StringProviderTask t1 = new StringProviderTask(
				"[]"
			);

			TaskSequence seq = new TaskSequence (t1, importTask);

			// Act:
			seq.Start();

			// Assert:
			List<QuestInfo> qiList = qim.GetListOfQuestInfos();

			Assert.AreEqual(0, qiList.Count());
		}



	}

	class StringProviderTask : Task {

		public StringProviderTask(string providedString) : base() {
			Result = providedString;
		}
	}


}
