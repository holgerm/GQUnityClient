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

namespace GQTests.Model
{

	public class QuestInfoTest
	{

		string publicGamesJson;

		/// <summary>
		/// The path to the JSON file with some quests (from WCC):
		/// </summary>
		static public readonly string JSON_InitFromServer = 
			Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "JSON/QuestInfos/initialFromServer.json");


		[SetUp]
		public void ResetQMInstance ()
		{
			QuestInfoManager.Reset ();
		}

		#region Main Use Cases:

		[Test]
		public void InitialImport ()
		{
			// Arrange:
			QuestInfoManager qm = QuestInfoManager.Instance;
			QuestInfo[] quests = null;

			// Act:
			qm.Update (JSON_InitFromServer);

			// Assert:
			IEnumerator<QuestInfo> questInfos = qm.GetEnumerator();
			while (questInfos.MoveNext()) {
				QuestInfo qi = questInfos.Current;
				Assert.False (qi.IsLocallyStored ());
				Assert.True (qi.IsNew ());
				Assert.True (qi.IsDownloadable ());
				Assert.False (qi.IsUpdatable ());
				Assert.AreEqual (GQ.Client.Model.QuestInfo.Deletability.CanNotDelete, qi.GetDeletability ());
			}
		}

		[Test, Ignore("todo?")]
		public void WhenNewQuestIsLoaded ()
		{
			// Arrange:
			QuestInfoManager qm = QuestInfoManager.Instance;
			QuestInfo[] quests = null;
			qm.Update (JSON_InitFromServer);

			// Act:
			// TODO

			// Assert:
			// TODO
			Assert.Fail ("Test not yet implemented!");
		}

		#endregion

	}
}
