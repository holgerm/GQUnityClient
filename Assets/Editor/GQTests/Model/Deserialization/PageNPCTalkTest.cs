using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model;
using GQ.Editor.Util;
using System.IO;

namespace GQTests.Model.Deserialization
{
	public class PageNPCTalkTest : GQMLTest
	{

		[SetUp]
		public void Init ()
		{
			XmlRoot = GQML.PAGE;
		}

		[Test]
		public void OnStart_1Rule3Actions ()
		{
			// Arrange:
			Variables.ClearAll ();
			Assert.AreEqual (Value.Null, Variables.GetValue ("A")); 
			Assert.AreEqual (Value.Null, Variables.GetValue ("B")); 
			Assert.AreEqual (Value.Null, Variables.GetValue ("C")); 

			// Act 1:
			PageNPCTalk page = parseXML<PageNPCTalk> 
				(@"	<mission type=""NPCTalk""
						endbuttontext=""&gt;"" 
						id=""29827""  
						mode=""Komplett anzeigen""  
						nextdialogbuttontext=""&gt;""  
						skipwordticker=""true"" textsize=""20""  
						tickerspeed=""50"" >
						<onStart>
							<rule>
								<action type=""SetVariable"" var=""A"">
									<value>
										<num>42</num>
									</value>
								</action>
								<action type=""SetVariable"" var=""B"">
									<value>
										<num>24</num>
									</value>
								</action>
								<action type=""SetVariable"" var=""C"">
									<value>
										<num>2001</num>
									</value>
								</action>
							</rule>
						</onStart>
					</mission>");

			// Assert:
			Assert.IsNotNull (page);

			// Act 2:
			page.Start ();

			// Assert:
			Value varA = Variables.GetValue ("A");
			Assert.AreNotEqual (Value.Null, varA);
			Assert.AreEqual (42, varA.AsInt ());
			Value varB = Variables.GetValue ("B");
			Assert.AreNotEqual (Value.Null, varB);
			Assert.AreEqual (24, varB.AsInt ());
			Value varC = Variables.GetValue ("C");
			Assert.AreNotEqual (Value.Null, varC);
			Assert.AreEqual (2001, varC.AsInt ());
		}

		[Test]
		public void OnStart_3RulesEach1Action ()
		{
			// Arrange:
			Variables.ClearAll ();
			Assert.AreEqual (Value.Null, Variables.GetValue ("A")); 
			Assert.AreEqual (Value.Null, Variables.GetValue ("B")); 
			Assert.AreEqual (Value.Null, Variables.GetValue ("C")); 

			// Act 1:
			PageNPCTalk page = parseXML<PageNPCTalk> 
				(@"	<mission type=""NPCTalk""
						endbuttontext=""&gt;"" 
						id=""29827""  
						mode=""Komplett anzeigen""  
						nextdialogbuttontext=""&gt;""  
						skipwordticker=""true"" textsize=""20""  
						tickerspeed=""50"" >
						<onStart>
							<rule>
								<action type=""SetVariable"" var=""A"">
									<value>
										<num>42</num>
									</value>
								</action>
							</rule>
							<rule>
								<action type=""SetVariable"" var=""B"">
									<value>
										<num>24</num>
									</value>
								</action>
							</rule>
							<rule>
								<action type=""SetVariable"" var=""C"">
									<value>
										<num>2001</num>
									</value>
								</action>
							</rule>
						</onStart>
					</mission>");

			// Assert:
			Assert.IsNotNull (page);

			// Act 2:
			page.Start ();

			// Assert:
			Value varA = Variables.GetValue ("A");
			Assert.AreNotEqual (Value.Null, varA);
			Assert.AreEqual (42, varA.AsInt ());
			Value varB = Variables.GetValue ("B");
			Assert.AreNotEqual (Value.Null, varB);
			Assert.AreEqual (24, varB.AsInt ());
			Value varC = Variables.GetValue ("C");
			Assert.AreNotEqual (Value.Null, varC);
			Assert.AreEqual (2001, varC.AsInt ());
		}

		[Test]
		public void QuestWith1NPCTalk ()
		{
			// Arrange:
			xml = File.ReadAllText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/QuestWith1NPCTalk/game.xml"));
			XmlRoot = GQML.QUEST;

			// Act:
			qm.SetCurrentQuestFromXML (xml);
			Quest q = QuestManager.Instance.CurrentQuest;

			// Assert:
			Assert.AreEqual ("QuestWith1NPCTalk", q.Name);
			Assert.AreEqual (9802, q.Id);
			IPage npcPage = q.GetPageWithID (26821);
			Assert.NotNull (npcPage);
		}

		[Test]
		public void ZeroDialogItems ()
		{
			// Arrange:
			xml = File.ReadAllText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/NPCTalk_ZeroDialogItems/game.xml"));
			XmlRoot = GQML.QUEST;

			// Act:
			qm.SetCurrentQuestFromXML (xml);
			Quest q = QuestManager.Instance.CurrentQuest;

			// Assert:
			PageNPCTalk npcPage = (PageNPCTalk)q.GetPageWithID (30224);
			Assert.NotNull (npcPage);

			// Act: Start Page:
			npcPage.Start ();

			// Assert:
			Assert.AreEqual (0, npcPage.CurDialogItemNo);

			// Act: Try to proceed to next page:
			npcPage.Next ();

			// Assert:
			Assert.AreEqual (0, npcPage.CurDialogItemNo);
		}

		[Test]
		public void OneDialogItem ()
		{
			// Arrange:
			xml = File.ReadAllText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/NPCTalk_OneDialogItem/game.xml"));
			XmlRoot = GQML.QUEST;

			// Act:
			qm.SetCurrentQuestFromXML (xml);
			Quest q = QuestManager.Instance.CurrentQuest;

			// Assert:
			PageNPCTalk npcPage = (PageNPCTalk)q.GetPageWithID (30198);
			Assert.NotNull (npcPage);

			// Act: Start Page:
			npcPage.Start ();

			// Assert:
			Assert.AreEqual (1, npcPage.CurDialogItemNo);
			Assert.AreEqual ("DialogItem Nummer 1", npcPage.CurrentDialogItem.Text);
			Assert.AreEqual (34494, npcPage.CurrentDialogItem.Id);
			Assert.AreEqual ("Napoleon", npcPage.CurrentDialogItem.Speaker);
			Assert.IsTrue (npcPage.CurrentDialogItem.IsBlocking);
			Assert.AreEqual ("http://qeevee.org:9091/uploadedassets/21/editor/10370/1_ausweis_einscannen_bitte.mp3", npcPage.CurrentDialogItem.AudioURL);

			// Act: Try to proceed to next page:
			npcPage.Next ();

			// Assert:
			Assert.AreEqual (1, npcPage.CurDialogItemNo);
		}

		[Test]
		public void ThreeDialogItems ()
		{
			// Arrange:
			xml = File.ReadAllText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/NPCTalk_ThreeDialogItems/game.xml"));
			XmlRoot = GQML.QUEST;

			// Act:
			qm.SetCurrentQuestFromXML (xml);
			Quest q = QuestManager.Instance.CurrentQuest;

			// Assert:
			PageNPCTalk npcPage = (PageNPCTalk)q.GetPageWithID (30223);
			Assert.NotNull (npcPage);

			// Act: Start Page:
			npcPage.Start ();

			// Assert:
			Assert.AreEqual (1, npcPage.CurDialogItemNo);
			Assert.AreEqual ("DialogItem Nummer 1", npcPage.CurrentDialogItem.Text);

			// Act: Try to proceed to second item:
			npcPage.Next ();
			Assert.AreEqual (2, npcPage.CurDialogItemNo);
			Assert.AreEqual ("DialogItem Nummer 2", npcPage.CurrentDialogItem.Text);

			// Act: Try to proceed to third item:
			npcPage.Next ();
			Assert.AreEqual (3, npcPage.CurDialogItemNo);
			Assert.AreEqual ("DialogItem Nummer 3", npcPage.CurrentDialogItem.Text);

			// Act: Try to proceed to fourth item, but there is no forth dialogitem:
			npcPage.Next ();
			Assert.AreEqual (3, npcPage.CurDialogItemNo);
			Assert.AreEqual ("DialogItem Nummer 3", npcPage.CurrentDialogItem.Text);
		}

		[Test]
		public void NPCTalk_AllAttributes ()
		{
			// Arrange:
			xml = File.ReadAllText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/NPCTalk_OneDialogItem/game.xml"));
			XmlRoot = GQML.QUEST;

			// Act:
			qm.SetCurrentQuestFromXML (xml);
			Quest q = QuestManager.Instance.CurrentQuest;

			// Assert:
			PageNPCTalk npcPage = (PageNPCTalk)q.GetPageWithID (30198);
			Assert.NotNull (npcPage);
			Assert.AreEqual ("Ende", npcPage.EndButtonText);
			Assert.AreEqual ("http://qeevee.org:9091/uploadedassets/21/editor/10370/1_bibliothekneuaubing.jpg", npcPage.ImageUrl);
			Assert.AreEqual (GQML.PAGE_NPCTALK_DISPLAYMODE_ALL_AT_ONCE, npcPage.DisplayMode);
			Assert.AreEqual ("Weiter", npcPage.NextDialogButtonText);
			Assert.IsTrue (npcPage.SkipWordTicker);
			Assert.AreEqual (20, npcPage.TextSize);
			Assert.AreEqual (50, npcPage.TickerSpeed);
		}

		[Test]
		public void TwoNPCTalkPages_States ()
		{
			xml = File.ReadAllText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/TwoPages/game.xml"));
			XmlRoot = GQML.QUEST;

			// Act:
			qm.SetCurrentQuestFromXML (xml);
			Quest q = QuestManager.Instance.CurrentQuest;

			// Assert before started:
			IPage p1 = q.GetPageWithID (30194);
			Assert.NotNull (p1);
			Assert.AreEqual (GQML.STATE_NEW, p1.State);
			IPage p2 = q.GetPageWithID (30195);
			Assert.NotNull (p2);
			Assert.AreEqual (GQML.STATE_NEW, p2.State);

			// Act:
			q.Start ();

			// Assert:
			Assert.AreEqual (30194, QuestManager.Instance.CurrentPage.Id);
			Assert.AreEqual (GQML.STATE_RUNNING, QuestManager.Instance.CurrentPage.State);
			Assert.AreEqual (GQML.STATE_NEW, p2.State);

			// Act:
			p1.End ();

			// Assert:
			Assert.AreEqual (30195, QuestManager.Instance.CurrentPage.Id);
			Assert.AreEqual (GQML.STATE_RUNNING, QuestManager.Instance.CurrentPage.State);
			Assert.AreEqual (GQML.STATE_SUCCEEDED, p1.State);
		}

		[Test]
		public void MediaInfoGatherSingle ()
		{
			// Arrange:
			xml = File.ReadAllText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/NPCTalk_ZeroDialogItems/game.xml"));
			XmlRoot = GQML.QUEST;

			// Act:
			qm.SetCurrentQuestFromXML (xml);
			Quest q = QuestManager.Instance.CurrentQuest;

			// Assert:
			Assert.AreEqual (1, q.MediaStore.Count);
			Assert.IsTrue(
				q.MediaStore.ContainsKey(
					"http://qeevee.org:9091/uploadedassets/21/editor/10370/1_bibliothekneuaubing.jpg")
			);
		}

		[Test]
		public void MediaInfoGatherMultiple ()
		{
			// Arrange:
			xml = File.ReadAllText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/QuestWith5NPCTalks/game.xml"));
			XmlRoot = GQML.QUEST;

			// Act:
			qm.SetCurrentQuestFromXML (xml);
			Quest q = QuestManager.Instance.CurrentQuest;

			// Assert:
			Assert.AreEqual (4, q.MediaStore.Count);
			Assert.IsTrue(
				q.MediaStore.ContainsKey(
					"http://qeevee.org:9091/uploadedassets/21/editor/9804/1_shadow.png")
			);
			Assert.IsTrue(
				q.MediaStore.ContainsKey(
					"http://qeevee.org:9091/uploadedassets/21/editor/9804/1_delete.png")
			);
			Assert.IsTrue(
				q.MediaStore.ContainsKey(
					"http://qeevee.org:9091/uploadedassets/21/editor/9804/1_download.png")
			);
			Assert.IsTrue(
				q.MediaStore.ContainsKey(
					"http://qeevee.org:9091/uploadedassets/21/editor/9804/1_info.png")
			);
		}

	}
}
