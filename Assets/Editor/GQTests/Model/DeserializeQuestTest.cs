using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Editor.Util;
using GQTests;
using GQ.Client.Model;

namespace GQTests.Model
{

	public class DeserializeQuestTest : GQMLTest
	{

		[Test]
		public void Quest_1_to_5 ()
		{
			// Arrange:
			xml = parseXmlFromFile ("XML/Quests/Quest_1_to_5/game.xml");

			// Act:
			Quest q = parseXML<Quest> (xml);

			// Assert:
			Assert.AreEqual (0, q.LastUpdate);
		}

		[Test]
		public void EmptyQuest ()
		{
			// Arrange:
			xml = parseXmlFromFile ("XML/Quests/MinimalQuest/game.xml");

			// Act:
			qm.SetCurrentQuestFromXML (xml);
			Quest q = QuestManager.Instance.CurrentQuest;

			// Assert:
			Assert.AreEqual ("Minimal Quest", q.Name);
			Assert.AreEqual (9801, q.Id);
		}

		[Test]
		public void QuestWith5NPCTalks ()
		{
			// Arrange:
			xml = parseXmlFromFile ("XML/Quests/QuestWith5NPCTalks/game.xml");

			// Act:
			qm.SetCurrentQuestFromXML (xml);
			Quest q = QuestManager.Instance.CurrentQuest;

			// Get pages:
			int[] ids = new int[] {
				26824,
				26829,
				26830,
				26831,
				26832
			};
			IPage[] pages = new IPage[ids.Length];

			int i = 0;
			foreach (int id in ids) {
				pages [i++] = q.GetPageWithID (id);
			}

			// Assert:
			Assert.AreEqual ("QuestWith5NPCTalks", q.Name);
			Assert.AreEqual (9804, q.Id);
			i = 0;
			foreach (var p in pages) {
				Assert.NotNull (p, "Page with id " + ids [i] + " should not be null.");
				i++;
			}
		}

		[Test]
		public void OnlyHotspots ()
		{
			// Arrange:
			xml = parseXmlFromFile ("XML/Quests/OnlyThreeHotspots/game.xml");

			// Act:
			qm.SetCurrentQuestFromXML (xml);
			Quest q = QuestManager.Instance.CurrentQuest;

			// Get hotspots:
			int[] ids = new int[] {
				11541,
				11542,
				11543
			};
			Hotspot[] hotspots = new Hotspot[ids.Length];
			int i = 0;
			foreach (int id in ids) {
				hotspots [i++] = q.GetHotspotWithID (id);
			}

			// Assert:
			Assert.AreEqual ("OnlyThreeHotspots", q.Name);
			Assert.AreEqual (9803, q.Id);
			i = 0;
			foreach (var h in hotspots) {
				Assert.NotNull (h, "Hotspot with id " + ids [i] + " should not be null.");
				i++;
			}
		}

	}
}
