using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Editor.Util;
using GQTests;
using GQ.Client.Model;

namespace GQTests.Model {

	public class DeserializeHotspotTest : DeserializationTest {

		/// <summary>
		/// Tests deserializing old hotspot xml that do not have attributes for iBeacon etc.
		/// </summary>
		[Test]
		public void Hotspot_1_to_5 () {
			// Arrange:
			xml = Files.ReadText(Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/Hotspot_1_to_5/game.xml"));

			// Act:
			Quest q = qm.Import(xml);
			// Get hotspots:
			QuestHotspot hotspot = q.GetHotspotWithID(11544);

			// Assert:
			Assert.NotNull(hotspot, "Hotspot with id 11544 should not be null.");
			Assert.AreEqual(0, hotspot.iBeacon);
			Assert.AreEqual(0, hotspot.number);
			Assert.AreEqual(0, hotspot.qrcode);
			Assert.AreEqual(0, hotspot.nfc);
			Assert.AreEqual(0, hotspot.nfc);
			Assert.AreEqual(true, hotspot.initialActivity);
			Assert.AreEqual(true, hotspot.initialVisibility);
			Assert.AreEqual("http://qeevee.org:9091/assets/img/erzbistummarker.png", hotspot.imageURI);
			Assert.AreEqual(20.0d, hotspot.radius);
		}


		[Test]
		public void Hotspot_5_1 () {
			// Arrange:
			xml = Files.ReadText(Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/OneHotspot/game.xml"));

			// Act:
			Quest q = qm.Import(xml);
			// Get hotspots:
			QuestHotspot hotspot = q.GetHotspotWithID(11544);

			// Assert:
			Assert.NotNull(hotspot, "Hotspot with id 11544 should not be null.");
			Assert.AreEqual(123, hotspot.iBeacon);
			Assert.AreEqual(123, hotspot.number);
			Assert.AreEqual(123, hotspot.qrcode);
			Assert.AreEqual(123, hotspot.nfc);
		}
	}
}
