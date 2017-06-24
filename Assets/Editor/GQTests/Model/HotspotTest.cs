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
		public void Hotspot_1_to_5_PLEASE_RENAME ()
		{
			// Arrange:
			QuestHotspot hotspot = parseXML<QuestHotspot> 
				(@"	<hotspot 
						id=""11544"" 
						img=""http://qeevee.org:9091/assets/img/erzbistummarker.png"" 
						initialActivity=""true"" 
						initialVisibility=""true"" 
						latlong=""50.9606,7.01079"" 
						radius=""20""
					/>");
			
			// Assert:
//			Assert.That (String.Compare (q.XmlFormat, "5.0") <= 0, "XML Format should be at most 5.0");
//			Assert.That (String.Compare ("1.0", q.XmlFormat) <= 0, "XML Format should be at least 1.0");
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
		public void Hotspot_5_1_PLEASE_RENAME ()
		{
			QuestHotspot hotspot = parseXML<QuestHotspot> 
				(@"	<hotspot 
						iBeacon=""123"" 
						id=""11544"" 
						img=""http://qeevee.org:9091/assets/img/erzbistummarker.png"" 
						initialActivity=""true"" 
						initialVisibility=""true"" 
						latlong=""50.9606,7.01079""  
						nfc=""123"" number=""123""  
						qrcode=""123""  
						radius=""20"" 
					/>");
			
			// Assert:
			Assert.NotNull (hotspot, "Hotspot with id 11544 should not be null.");
			Assert.AreEqual (123, hotspot.iBeacon);
			Assert.AreEqual (123, hotspot.number);
			Assert.AreEqual (123, hotspot.qrcode);
			Assert.AreEqual (123, hotspot.nfc);
		}
	}
}
