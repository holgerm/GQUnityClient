using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Editor.Util;
using GQTests;
using GQ.Client.Model;
using System;

namespace GQTests.Model
{

	public class HotspotTest : GQMLTest
	{

		[SetUp]
		public void Init ()
		{
			XmlRoot = GQML.HOTSPOT;
		}


		/// <summary>
		/// Tests deserializing old hotspot xml that do not have attributes for iBeacon etc.
		/// </summary>
		[Test]
		public void Hotspot_MinimalData ()
		{
			// Arrange:
			Hotspot hotspot = parseXML<Hotspot> 
				(@"	<hotspot 
						id=""11544"" 
						img=""http://qeevee.org:9091/assets/img/erzbistummarker.png"" 
						initialActivity=""true"" 
						initialVisibility=""true"" 
						latlong=""50.9606,7.01079"" 
						radius=""20""
					/>");
			
			// Assert:
			Assert.NotNull (hotspot, "Hotspot with id 11544 should not be null.");
			Assert.AreEqual (0, hotspot.IBeacon);
			Assert.AreEqual (0, hotspot.Number);
			Assert.AreEqual (0, hotspot.QrCode);
			Assert.AreEqual (0, hotspot.Nfc);
			Assert.AreEqual (0, hotspot.Nfc);
			Assert.AreEqual (true, hotspot.InitialActivity);
			Assert.AreEqual (true, hotspot.InitialVisibility);
			// TODO: reactivate the following line when hotspots load their img files in the new version!
			Assert.AreEqual("http://qeevee.org:9091/assets/img/erzbistummarker.png", hotspot.ImageURI);
			Assert.AreEqual (20.0d, hotspot.Radius);
		}


		[Test]
		public void Hotspot_FullData ()
		{
			Hotspot hotspot = parseXML<Hotspot> 
				(@"	<hotspot 
						iBeacon=""123"" 
						id=""11544"" 
						img=""http://qeevee.org:9091/assets/img/erzbistummarker.png"" 
						initialActivity=""true"" 
						initialVisibility=""false"" 
						latlong=""50.9606,7.01079""  
						nfc=""123"" number=""345""  
						qrcode=""456""  
						radius=""20"" 
					/>");
			
			// Assert:
			Assert.NotNull (hotspot, "Hotspot with id 11544 should not be null.");
			Assert.IsTrue(hotspot.InitialActivity);
			Assert.IsFalse (hotspot.InitialVisibility);
			Assert.AreEqual (123, hotspot.IBeacon);
			Assert.AreEqual (345, hotspot.Number);
			Assert.AreEqual (456, hotspot.QrCode);
			Assert.AreEqual (123, hotspot.Nfc);
			Assert.AreEqual (20, hotspot.Radius);
		}

		[Test]
		public void OnlyHotspots() {
			// Arrange:
			xml = Files.ReadText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/OnlyThreeHotspots/game.xml"));

			// Act:
			Quest q = qm.DeserializeQuest (xml);

			// Assert:
			Assert.AreEqual(3, q.AllHotspots.Count);

			Hotspot h = q.GetHotspotWithID (11541);
			Assert.NotNull (h);
			Assert.AreEqual (11541, h.Id);

			h = q.GetHotspotWithID (11542);
			Assert.NotNull (h);
			Assert.AreEqual (11542, h.Id);

			h = q.GetHotspotWithID (11543);
			Assert.NotNull (h);
			Assert.AreEqual (11543, h.Id);
		}

		[Test]
		public void HotspotTriggers() {
			// Arrange:
			xml = Files.ReadText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/HotspotTrigger/game.xml"));

			// Act:
			Quest q = qm.DeserializeQuest (xml);

			// Assert:
			Assert.AreEqual(3, q.AllHotspots.Count);

			Hotspot hEnter = q.GetHotspotWithID (12183);
			Hotspot hLeave = q.GetHotspotWithID (12182);
			Hotspot hTap = q.GetHotspotWithID (12184);

			// Beforehand Variable MyState is not set:
			Assert.AreEqual(Value.Null, Variables.GetValue("MyState"));

			// The Enter Trigger should set the variable to "Entered":
			hEnter.Enter ();
			Assert.AreEqual("Entered", Variables.GetValue("MyState").AsString());

			// The Leave Trigger should set the variable to "Left":
			hLeave.Leave ();
			Assert.AreEqual("Left", Variables.GetValue("MyState").AsString());

			// The Tap Trigger should set the variable to "Touched":
			hTap.Tap ();
			Assert.AreEqual("Touched", Variables.GetValue("MyState").AsString());
		}

		[Test]
		public void HotspotEmptyTriggers() {
			// Arrange:
			xml = Files.ReadText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/HotspotTrigger/game.xml"));

			// Act:
			Quest q = qm.DeserializeQuest (xml);

			// Assert:
			Assert.AreEqual(3, q.AllHotspots.Count);

			Hotspot hEnter = q.GetHotspotWithID (12183);
			Hotspot hLeave = q.GetHotspotWithID (12182);
			Hotspot hTap = q.GetHotspotWithID (12184);

			// Beforehand Variable MyState is not set:
			Assert.AreEqual(Value.Null, Variables.GetValue("MyState"));

			// The Leave Trigger should NOT set the variable:
			hEnter.Leave ();
			Assert.AreEqual(Value.Null, Variables.GetValue("MyState"));

			// The Leave Trigger should NOT set the variable:
			hEnter.Tap ();
			Assert.AreEqual(Value.Null, Variables.GetValue("MyState"));
		}
	}
}
