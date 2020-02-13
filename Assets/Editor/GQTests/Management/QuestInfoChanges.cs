using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Editor.Util;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using QM.Mocks;
using System;
using Code.GQClient.Conf;
using GQClient.Model;
using Code.GQClient.Model.mgmt.quests;

namespace GQTests.Management {

	public class QuestInfoChanges : AbstractMockTest {

		#region SetUp
		QuestInfoManager QM;

		[SetUp]
		public void SetUp () {
			Mock.Use = true;
			MOCK_Server_Empty();

			ConfigurationManager.Current.portal = 0;

			QuestInfoManager.Reset();
			QM = QuestInfoManager.Instance;
		}
		#endregion

		#region Tests
		/// <summary>
		/// When the server lists no quests, 
		/// then the app should display no quests.
		/// </summary>
		[Test]
		public void EmptyServerEmptyQuestManager () {
			QM.UpdateQuestInfos();
			ASSERT_QM_ShowsNoQuests ();
		}

		/// <summary>
		/// When the server publishes a new quest and the app refreshes, 
		/// then the app should show that quest and offer to download it.
		/// </summary>
		[Test]
		public void PublishNewQuestInfoOnServer () {
			MOCK_Server_PublishQuest ();
			QM.UpdateQuestInfos();
			ASSERT_ShowOptions_Download ();
		}

		/// <summary>
		/// When the app downloads a new published quest from the server, 
		/// then the app should offer play and delete options for that quest.
		/// </summary>
		[Test]
		public void DownloadQuest() {
			MOCK_Server_PublishQuest ();
			QM.UpdateQuestInfos();
			// before download app offers us to download the quest:
			ASSERT_ShowOptions_Download ();
			MOCK_DownloadQuest();
			// after download no download option is shown, instead we can play or delete the local quest:
			ASSERT_ShowOptions_Play_Delete ();
		}

		/// <summary>
		/// When the server changes the category of a quest that the app has NOT yet downloaded,
		/// then the quest category should be updated when the app gets refreshed and then offer download as before.
		/// </summary>
		[Test]
		public void ChangeInfoOfUnloadedQuestByCategory() {
			// PREPARATION: published quest:
			MOCK_Server_PublishQuest ();
			QM.UpdateQuestInfos();

			// TEST:
			MOCK_ChangeCategoryOfQuestInfo ();
			QM.UpdateQuestInfos();
			ASSERT_ShowOptions_Download ();
			ASSERT_ShowsNewCategory ();
		}

		/// <summary>
		/// When the server changes the category of a quest that the app has NOT yet downloaded,
		/// then the quest category should be updated when the app gets refreshed and then offer download as before.
		/// </summary>
		[Test]
		public void ChangeInfoOfUnloadedQuestByName() {
			// PREPARATION: published quest:
			MOCK_Server_PublishQuest ();
			QM.UpdateQuestInfos();

			// TEST:
			MOCK_ChangeNameOfQuestInfo ();
			QM.UpdateQuestInfos();
			ASSERT_ShowOptions_Download ();
			ASSERT_ShowsNewName ();
		}

		/// <summary>
		/// When a quest that is already downloaded on the app changes category on the server, 
		/// then the app should offer to update it.
		/// </summary>
		[Test]
		public void ChangeInfoOfLoadedQuestByCategory() {
			// PREPARATION: published downloaded quest:
			MOCK_Server_PublishQuest ();
			QM.UpdateQuestInfos();
			MOCK_DownloadQuest();

			// TEST:
			MOCK_ChangeCategoryOfQuestInfo ();
			QM.UpdateQuestInfos();
			ASSERT_ShowOptions_Play_Delete_Update ();
			ASSERT_ShowsOldCategory ();
		}

		/// <summary>
		/// When a downloaded quest that changed category on the server is updated in the app, 
		/// then the app should not offer update anymore, but still offer play and delete.
		/// </summary>
		[Test]
		public void UpdateChangedQuestByCategory() {
			// PREPARATION: publish and download quest and change it on server:
			MOCK_Server_PublishQuest ();
			QM.UpdateQuestInfos();
			MOCK_DownloadQuest();
			MOCK_ChangeCategoryOfQuestInfo ();
			QM.UpdateQuestInfos();

			MOCK_UpdateQuest ();

			ASSERT_ShowOptions_Play_Delete ();
			ASSERT_ShowsNewCategory ();
		}

		/// <summary>
		/// When a quest that is already downloaded on the app changes category on the server, 
		/// then the app should offer to update it.
		/// </summary>
		[Test]
		public void ChangeInfoOfLoadedQuestByName() {
			// PREPARATION: published downloaded quest:
			MOCK_Server_PublishQuest ();
			QM.UpdateQuestInfos();
			MOCK_DownloadQuest();

			// TEST:
			MOCK_ChangeNameOfQuestInfo ();
			QM.UpdateQuestInfos();
			ASSERT_ShowOptions_Play_Delete_Update ();
			ASSERT_ShowsOldName ();
		}

		[Test]
		public void ChangeGameXmlOnly() {
			// PREPARATION: publish quest and download it:
			MOCK_Server_PublishQuest ();
			QM.UpdateQuestInfos();

            // Now only the download option should be shown:
            ASSERT_ShowOptions_Download();

			MOCK_DownloadQuest();

			// ACT: publish new version of game.xml where only content changed but metadata and media stay the same:
			MOCK_Server_ChangeQuestXML ();
			QM.UpdateQuestInfos();

			// TEST:
			ASSERT_ShowOptions_Play_Delete_Update ();

			// ACT: update on client:
			MOCK_UpdateQuest ();

			// TEST: new game.xml is now downloaded:
			ASSERT_ChangedQuestXmlOnDevice();
            // Now update is not shown any more but play and delete still are:
            ASSERT_ShowOptions_Play_Delete();
		}
		#endregion

		// ###############################################################################################################

		#region Assertion to check on Client App
		void ASSERT_QM_ShowsNoQuests() {
			Assert.NotNull(QM);
			Assert.AreEqual(0, QM.Count);
		}

		void ASSERT_ShowOptions_Download() {
			QuestInfo info = QM.GetQuestInfo(10557);
			Assert.That (info.ShowDownloadOption);
			Assert.IsFalse (info.ShowStartOption);
			Assert.IsFalse (info.ShowUpdateOption);
			Assert.IsFalse (info.ShowDeleteOption);
		}

		void ASSERT_ShowOptions_Play_Delete () {
			QuestInfo info = QM.GetQuestInfo(10557);
			Assert.IsFalse (info.ShowDownloadOption);
			Assert.That (info.ShowStartOption);
			Assert.IsFalse (info.ShowUpdateOption);
			Assert.That (info.ShowDeleteOption);
		}

		void ASSERT_ShowOptions_Play_Delete_Update() {
			QuestInfo info = QM.GetQuestInfo(10557);
			Assert.IsFalse (info.ShowDownloadOption);
			Assert.That (info.ShowStartOption);
			Assert.That (info.ShowUpdateOption);
			Assert.That (info.ShowDeleteOption);
		}

		void ASSERT_ShowsOldCategory() {
			QuestInfo info = QM.GetQuestInfo(10557);
			Assert.AreEqual ("wcc.tour.beethoven", info.Categories [0]);
		}

		void ASSERT_ShowsNewCategory() {
			QuestInfo info = QM.GetQuestInfo(10557);
			Assert.AreEqual ("wcc.tour.macke", info.Categories [0]);
		}

		void ASSERT_ShowsOldName() {
			QuestInfo info = QM.GetQuestInfo(10557);
			Assert.AreEqual ("Franziskanerkirche", info.Name);
		}

		void ASSERT_ShowsNewName() {
			QuestInfo info = QM.GetQuestInfo(10557);
			Assert.AreEqual ("Neuer Name", info.Name);
		}

		void ASSERT_ChangedQuestXmlOnDevice() {
			string gameXmlPath = Files.CombinePath(QuestManager.GetLocalPath4Quest (10557), "game.xml");
			string localGameXml = File.ReadAllText (gameXmlPath);
			Assert.AreEqual (questXMLChangedContent, localGameXml);
		}
		#endregion

		#region Mock Server Behaviour
		void MOCK_Server_Empty() {
			Mock.DeclareGQServerResponseByString ("json/0/publicgamesinfo", @"[]");
		}

		void MOCK_Server_PublishQuest() {
			Mock.DeclareGQServerResponseByString ("editor/10557/clientxml", questXMLOriginal);
			Mock.DeclareGQServerResponseByString ("json/0/publicgamesinfo", questInfoOriginal);
		}

		void MOCK_ChangeCategoryOfQuestInfo () {
			// we change category from beethoven to macke ... and increase the lastupdate time stamp by one:
			Mock.DeclareGQServerResponseByString ("editor/10557/clientxml", questXMLNewCategory);
			Mock.DeclareGQServerResponseByString ("json/0/publicgamesinfo", questInfoNewCategory);
		}

		void MOCK_ChangeNameOfQuestInfo () {
			// we change category from beethoven to macke ... and increase the lastupdate time stamp by one:
			Mock.DeclareGQServerResponseByString ("editor/10557/clientxml", questXMLNewName);
			Mock.DeclareGQServerResponseByString ("json/0/publicgamesinfo", questInfoNewName);
		}

		void MOCK_Server_ChangeQuestXML() {
			// we change the game.xml on the server:
			Mock.DeclareGQServerResponseByString ("editor/10557/clientxml", questXMLChangedContent);
			Mock.DeclareGQServerResponseByString ("json/0/publicgamesinfo", questInfoChangedContent);
		}

		void MOCK_DownloadQuest() {
			QM.GetQuestInfo (10557).Download ();
		}

		void MOCK_UpdateQuest() {
			QM.GetQuestInfo (10557).Update ();
		}
		#endregion

		#region Long Content Strings
		string questXMLOriginal = 
			@"<game id=""10557"" lastUpdate=""1505465979827"" name=""Franziskanerkirche"" xmlformat=""5"">
				<mission endbuttontext=""Zurück zur Karte"" id=""30917"" mode=""Komplett anzeigen"" nextdialogbuttontext=""Zurück zur Karte"" skipwordticker=""true"" textsize=""20"" tickerspeed=""50"" type=""NPCTalk"">
					<onStart>
						<rule>
							<action type=""SetVariable"" var=""content"">
								<value>
									<string>Aus den Erinnerungen des Bäckermeisters Fischer, Eigentümer des Wohnhauses in der Rheingasse 24, ist bekannt, dass der kleine Ludwig van Beethoven Orgelunterricht erhielt bei Bruder Willibaldus im damaligen Franziskanerkloster. In der Franziskanerkirche erlernte er nicht nur das Orgelspiel, sondern wurde auch in kirchlichen Ritualen unterrichtet. Wenig später wurde Ludwig der Gehilfe von Willibaldus.</string>
								</value>
							</action>
							<action type=""SetVariable"" var=""link"">
								<value>
									<string>http://www.buergerfuerbeethoven.de/start/index.html</string>
								</value>
							</action>
							<action type=""SetVariable"" var=""autor"">
								<value>
									<string>BN-BfB</string>
								</value>
							</action>
							<action type=""SetVariable"" var=""bildrechte"">
								<value>
									<string/>
								</value>
							</action>
						</rule>
					</onStart>
					<onEnd>
						<rule>
							<action type=""EndGame""/>
						</rule>
					</onEnd>
					<dialogitem blocking=""false"" id=""35913"">&lt;b&gt;@quest.name@&lt;/b&gt;&lt;br&gt;@content@&lt;br&gt;&lt;br&gt;Autor: @autor@&lt;br&gt;&lt;br&gt;&lt;br&gt;&lt;a href=&quot;@link@&quot;&gt;@link@&lt;/a&gt;&lt;br&gt;</dialogitem>
				</mission>
				<mission id=""30918"" type=""MetaData"">
					<stringmeta id=""35914"" key=""category"" value=""wcc.tour.beethoven""/>
					<stringmeta id=""35915"" key=""city"" value=""Bonn""/>
					<stringmeta id=""35916"" key=""administrative"" value=""Bonn""/>
					<stringmeta id=""35917"" key=""administrative"" value=""Regierungsbezirk Köln""/>
					<stringmeta id=""35918"" key=""state"" value=""Nordrhein-Westfalen""/>
					<stringmeta id=""35919"" key=""country"" value=""Deutschland""/>
				</mission>
				<hotspot id=""12341"" initialActivity=""true"" initialVisibility=""true"" latlong=""50.73447,7.104104"" radius=""20"">
					<onEnter>
						<rule>
							<action allowReturn=""0"" id=""30917"" type=""StartMission""/>
						</rule>
					</onEnter>
					<onLeave>
						<rule>
							<action allowReturn=""0"" id=""30917"" type=""StartMission""/>
						</rule>
					</onLeave>
				</hotspot>
			</game>";

		string questXMLChangedContent = 
			@"<game id=""10557"" lastUpdate=""1505465979828"" name=""Franziskanerkirche"" xmlformat=""5"">
				<mission endbuttontext=""Zurück zur Karte"" id=""30917"" mode=""Komplett anzeigen"" nextdialogbuttontext=""Zurück zur Karte"" skipwordticker=""true"" textsize=""20"" tickerspeed=""50"" type=""NPCTalk"">
					<onStart>
						<rule>
							<action type=""SetVariable"" var=""content"">
								<value>
									<string>NEUER TEXT. Aus den Erinnerungen des Bäckermeisters Fischer, Eigentümer des Wohnhauses in der Rheingasse 24, ist bekannt, dass der kleine Ludwig van Beethoven Orgelunterricht erhielt bei Bruder Willibaldus im damaligen Franziskanerkloster. In der Franziskanerkirche erlernte er nicht nur das Orgelspiel, sondern wurde auch in kirchlichen Ritualen unterrichtet. Wenig später wurde Ludwig der Gehilfe von Willibaldus.</string>
								</value>
							</action>
							<action type=""SetVariable"" var=""link"">
								<value>
									<string>http://www.buergerfuerbeethoven.de/start/index.html</string>
								</value>
							</action>
							<action type=""SetVariable"" var=""autor"">
								<value>
									<string>BN-BfB</string>
								</value>
							</action>
							<action type=""SetVariable"" var=""bildrechte"">
								<value>
									<string/>
								</value>
							</action>
						</rule>
					</onStart>
					<onEnd>
						<rule>
							<action type=""EndGame""/>
						</rule>
					</onEnd>
					<dialogitem blocking=""false"" id=""35913"">&lt;b&gt;@quest.name@&lt;/b&gt;&lt;br&gt;@content@&lt;br&gt;&lt;br&gt;Autor: @autor@&lt;br&gt;&lt;br&gt;&lt;br&gt;&lt;a href=&quot;@link@&quot;&gt;@link@&lt;/a&gt;&lt;br&gt;</dialogitem>
				</mission>
				<mission id=""30918"" type=""MetaData"">
					<stringmeta id=""35914"" key=""category"" value=""wcc.tour.beethoven""/>
					<stringmeta id=""35915"" key=""city"" value=""Bonn""/>
					<stringmeta id=""35916"" key=""administrative"" value=""Bonn""/>
					<stringmeta id=""35917"" key=""administrative"" value=""Regierungsbezirk Köln""/>
					<stringmeta id=""35918"" key=""state"" value=""Nordrhein-Westfalen""/>
					<stringmeta id=""35919"" key=""country"" value=""Deutschland""/>
				</mission>
				<hotspot id=""12341"" initialActivity=""true"" initialVisibility=""true"" latlong=""50.73447,7.104104"" radius=""20"">
					<onEnter>
						<rule>
							<action allowReturn=""0"" id=""30917"" type=""StartMission""/>
						</rule>
					</onEnter>
					<onLeave>
						<rule>
							<action allowReturn=""0"" id=""30917"" type=""StartMission""/>
						</rule>
					</onLeave>
				</hotspot>
			</game>";

		string questInfoOriginal = 
			@"[
				    {
				        ""hotspots"": [
				            {
				                ""latitude"": 50.73447,
				                ""longitude"": 7.104104
				            },
				            {
				                ""latitude"": 50.73447,
				                ""longitude"": 7.104104
				            }
				        ],
				        ""id"": 10557,
				        ""lastUpdate"": 1505465979827,
				        ""metadata"": [
				            {
				                ""key"": ""category"",
				                ""value"": ""wcc.tour.beethoven""
				            }
				        ],
				        ""name"": ""Franziskanerkirche"",
				        ""typeID"": 3318
				    }
				]";

		string questInfoChangedContent = 
			@"[
				    {
				        ""hotspots"": [
				            {
				                ""latitude"": 50.73447,
				                ""longitude"": 7.104104
				            },
				            {
				                ""latitude"": 50.73447,
				                ""longitude"": 7.104104
				            }
				        ],
				        ""id"": 10557,
				        ""lastUpdate"": 1505465979828,
				        ""metadata"": [
				            {
				                ""key"": ""category"",
				                ""value"": ""wcc.tour.beethoven""
				            }
				        ],
				        ""name"": ""Franziskanerkirche"",
				        ""typeID"": 3318
				    }
				]";
				
		string questXMLNewCategory = 
			@"<game id=""10557"" lastUpdate=""1505465979828"" name=""Franziskanerkirche"" xmlformat=""5"">
				<mission endbuttontext=""Zurück zur Karte"" id=""30917"" mode=""Komplett anzeigen"" nextdialogbuttontext=""Zurück zur Karte"" skipwordticker=""true"" textsize=""20"" tickerspeed=""50"" type=""NPCTalk"">
					<onStart>
						<rule>
							<action type=""SetVariable"" var=""content"">
								<value>
									<string>Aus den Erinnerungen des Bäckermeisters Fischer, Eigentümer des Wohnhauses in der Rheingasse 24, ist bekannt, dass der kleine Ludwig van Beethoven Orgelunterricht erhielt bei Bruder Willibaldus im damaligen Franziskanerkloster. In der Franziskanerkirche erlernte er nicht nur das Orgelspiel, sondern wurde auch in kirchlichen Ritualen unterrichtet. Wenig später wurde Ludwig der Gehilfe von Willibaldus.</string>
								</value>
							</action>
							<action type=""SetVariable"" var=""link"">
								<value>
									<string>http://www.buergerfuerbeethoven.de/start/index.html</string>
								</value>
							</action>
							<action type=""SetVariable"" var=""autor"">
								<value>
									<string>BN-BfB</string>
								</value>
							</action>
							<action type=""SetVariable"" var=""bildrechte"">
								<value>
									<string/>
								</value>
							</action>
						</rule>
					</onStart>
					<onEnd>
						<rule>
							<action type=""EndGame""/>
						</rule>
					</onEnd>
					<dialogitem blocking=""false"" id=""35913"">&lt;b&gt;@quest.name@&lt;/b&gt;&lt;br&gt;@content@&lt;br&gt;&lt;br&gt;Autor: @autor@&lt;br&gt;&lt;br&gt;&lt;br&gt;&lt;a href=&quot;@link@&quot;&gt;@link@&lt;/a&gt;&lt;br&gt;</dialogitem>
				</mission>
				<mission id=""30918"" type=""MetaData"">
					<stringmeta id=""35914"" key=""category"" value=""wcc.tour.macke""/>
					<stringmeta id=""35915"" key=""city"" value=""Bonn""/>
					<stringmeta id=""35916"" key=""administrative"" value=""Bonn""/>
					<stringmeta id=""35917"" key=""administrative"" value=""Regierungsbezirk Köln""/>
					<stringmeta id=""35918"" key=""state"" value=""Nordrhein-Westfalen""/>
					<stringmeta id=""35919"" key=""country"" value=""Deutschland""/>
				</mission>
				<hotspot id=""12341"" initialActivity=""true"" initialVisibility=""true"" latlong=""50.73447,7.104104"" radius=""20"">
					<onEnter>
						<rule>
							<action allowReturn=""0"" id=""30917"" type=""StartMission""/>
						</rule>
					</onEnter>
					<onLeave>
						<rule>
							<action allowReturn=""0"" id=""30917"" type=""StartMission""/>
						</rule>
					</onLeave>
				</hotspot>
			</game>";

		string questInfoNewCategory = 
			@"[
				    {
				        ""hotspots"": [
				            {
				                ""latitude"": 50.73447,
				                ""longitude"": 7.104104
				            },
				            {
				                ""latitude"": 50.73447,
				                ""longitude"": 7.104104
				            }
				        ],
				        ""id"": 10557,
				        ""lastUpdate"": 1505465979828,
				        ""metadata"": [
				            {
				                ""key"": ""category"",
				                ""value"": ""wcc.tour.macke""
				            }
				        ],
				        ""name"": ""Franziskanerkirche"",
				        ""typeID"": 3318
				    }
				]";

		string questXMLNewName = 
			@"<game id=""10557"" lastUpdate=""1505465979828"" name=""Neuer Name"" xmlformat=""5"">
				<mission endbuttontext=""Zurück zur Karte"" id=""30917"" mode=""Komplett anzeigen"" nextdialogbuttontext=""Zurück zur Karte"" skipwordticker=""true"" textsize=""20"" tickerspeed=""50"" type=""NPCTalk"">
					<onStart>
						<rule>
							<action type=""SetVariable"" var=""content"">
								<value>
									<string>Aus den Erinnerungen des Bäckermeisters Fischer, Eigentümer des Wohnhauses in der Rheingasse 24, ist bekannt, dass der kleine Ludwig van Beethoven Orgelunterricht erhielt bei Bruder Willibaldus im damaligen Franziskanerkloster. In der Franziskanerkirche erlernte er nicht nur das Orgelspiel, sondern wurde auch in kirchlichen Ritualen unterrichtet. Wenig später wurde Ludwig der Gehilfe von Willibaldus.</string>
								</value>
							</action>
							<action type=""SetVariable"" var=""link"">
								<value>
									<string>http://www.buergerfuerbeethoven.de/start/index.html</string>
								</value>
							</action>
							<action type=""SetVariable"" var=""autor"">
								<value>
									<string>BN-BfB</string>
								</value>
							</action>
							<action type=""SetVariable"" var=""bildrechte"">
								<value>
									<string/>
								</value>
							</action>
						</rule>
					</onStart>
					<onEnd>
						<rule>
							<action type=""EndGame""/>
						</rule>
					</onEnd>
					<dialogitem blocking=""false"" id=""35913"">&lt;b&gt;@quest.name@&lt;/b&gt;&lt;br&gt;@content@&lt;br&gt;&lt;br&gt;Autor: @autor@&lt;br&gt;&lt;br&gt;&lt;br&gt;&lt;a href=&quot;@link@&quot;&gt;@link@&lt;/a&gt;&lt;br&gt;</dialogitem>
				</mission>
				<mission id=""30918"" type=""MetaData"">
					<stringmeta id=""35914"" key=""category"" value=""wcc.tour.beethoven""/>
					<stringmeta id=""35915"" key=""city"" value=""Bonn""/>
					<stringmeta id=""35916"" key=""administrative"" value=""Bonn""/>
					<stringmeta id=""35917"" key=""administrative"" value=""Regierungsbezirk Köln""/>
					<stringmeta id=""35918"" key=""state"" value=""Nordrhein-Westfalen""/>
					<stringmeta id=""35919"" key=""country"" value=""Deutschland""/>
				</mission>
				<hotspot id=""12341"" initialActivity=""true"" initialVisibility=""true"" latlong=""50.73447,7.104104"" radius=""20"">
					<onEnter>
						<rule>
							<action allowReturn=""0"" id=""30917"" type=""StartMission""/>
						</rule>
					</onEnter>
					<onLeave>
						<rule>
							<action allowReturn=""0"" id=""30917"" type=""StartMission""/>
						</rule>
					</onLeave>
				</hotspot>
			</game>";

		string questInfoNewName = 
			@"[
				    {
				        ""hotspots"": [
				            {
				                ""latitude"": 50.73447,
				                ""longitude"": 7.104104
				            },
				            {
				                ""latitude"": 50.73447,
				                ""longitude"": 7.104104
				            }
				        ],
				        ""id"": 10557,
				        ""lastUpdate"": 1505465979828,
				        ""metadata"": [
				            {
				                ""key"": ""category"",
				                ""value"": ""wcc.tour.beethoven""
				            }
				        ],
				        ""name"": ""Neuer Name"",
				        ""typeID"": 3318
				    }
				]";
		#endregion

	}

}
