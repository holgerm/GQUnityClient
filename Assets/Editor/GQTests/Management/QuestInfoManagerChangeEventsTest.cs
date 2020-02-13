using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Editor.Util;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using GQClient.Model;

namespace GQTests.Management {

	public class QuestInfoManagerChangeEventsTest {

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
