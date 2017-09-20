using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model;
using GQ.Editor.Util;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace GQTests.Managent {

	public class QuestInfoManagerTest {

		[SetUp]
		public void ResetQMInstance () {
			QuestInfoManager.Reset();
		}

		[Test]
		public void InitQM () {
			// Arrange:
			QuestInfoManager qm = null;

			// Act:
			qm = QuestInfoManager.Instance;

			// Assert:
			Assert.NotNull(qm);
			Assert.AreEqual(0, qm.Count);
		}
	}




}
