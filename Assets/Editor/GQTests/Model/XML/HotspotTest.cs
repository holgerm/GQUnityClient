using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Editor.Util;
using GQTests;
using GQ.Client.Model;
using System;

namespace GQTests.Model.XML
{

	public class HotspotTest : DeserializationTest
	{

		/// <summary>
		/// Tests deserializing old hotspot xml that do not have attributes for iBeacon etc.
		/// </summary>
		[Test]
		public void Hotspot_1_to_5 ()
		{
			// Arrange:
			xml = Files.ReadText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/Hotspot_1_to_5/game.xml"));

			// Act:
			Quest q = qm.Import (xml);
			// Get hotspots:
			QuestHotspot hotspot = q.GetHotspotWithID (11544);

			// Assert:
			Assert.That (String.Compare (q.XmlFormat, "5.0") <= 0, "XML Format should be at most 5.0");
			Assert.That (String.Compare ("1.0", q.XmlFormat) <= 0, "XML Format should be at least 1.0");
			Assert.NotNull (hotspot, "Hotspot with id 11544 should not be null.");
			Assert.AreEqual (0, hotspot.iBeacon);
			Assert.AreEqual (0, hotspot.number);
			Assert.AreEqual (0, hotspot.qrcode);
			Assert.AreEqual (0, hotspot.nfc);
			Assert.AreEqual (0, hotspot.nfc);
			Assert.AreEqual (true, hotspot.initialActivity);
			Assert.AreEqual (true, hotspot.initialVisibility);
			// TODO: reactivate the following line when hotspots load their img files in the new version!
//			Assert.AreEqual("http://qeevee.org:9091/assets/img/erzbistummarker.png", hotspot.imageURI);
			Assert.AreEqual (20.0d, hotspot.radius);
		}


		[Test]
		public void Hotspot_5_1 ()
		{
			// Arrange:
			xml = Files.ReadText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/OneHotspot/game.xml"));

			// Act:
			Quest q = qm.Import (xml);
			// Get hotspots:
			QuestHotspot hotspot = q.GetHotspotWithID (11544);

			// Assert:
			Assert.NotNull (hotspot, "Hotspot with id 11544 should not be null.");
			Assert.AreEqual (123, hotspot.iBeacon);
			Assert.AreEqual (123, hotspot.number);
			Assert.AreEqual (123, hotspot.qrcode);
			Assert.AreEqual (123, hotspot.nfc);
		}
	}
}
