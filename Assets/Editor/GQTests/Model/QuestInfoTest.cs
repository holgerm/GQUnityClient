using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.IO;
using GQ.Editor.Util;
using Newtonsoft.Json;
using GQ.Client.Model;
using System;

namespace GQTests.Model {

	public class QuestInfoTest {

		string publicGamesJson;

		/// <summary>
		/// The path to the JSON file with some quests (from WCC):
		/// </summary>
		static readonly string PUBLIC_GAMES_JSON_PATH = Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "Server/JSON/publicgamesinfo.json");

		[SetUp]
		public void ReadJSON () {
			FileInfo file = new FileInfo(PUBLIC_GAMES_JSON_PATH);
			StreamReader reader = file.OpenText();
			publicGamesJson = reader.ReadToEnd();
			reader.Close();
		}

		[Test]
		public void ShowReadQuests () {
			// Arrange:
			QuestInfo[] quests = null;

			// Act:
			try {
				quests = JsonConvert.DeserializeObject<QuestInfo[]>(publicGamesJson);
			} catch ( Exception e ) {
				Debug.LogWarning("Exception occurred while reading public games json: " + e.Message);
			}

			foreach ( QuestInfo qi in quests ) {
				Debug.Log(qi.ToString());
			}

			Debug.Log(String.Format("We have read {0} quests.", quests != null ? quests.Length : 0));
		}

	}
}
