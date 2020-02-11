using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Model.pages;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

namespace GQTests.Model.Runtime
{
	public class StartPage : GQMLTest
	{

		[Test]
		public void InitialPageStates ()
		{
			// Arrange:
			string xml = parseXmlFromFile ("XML/Quests/TwoPages/game.xml");
			Quest quest = parseXML<Quest> (xml);
			Page page1 = quest.GetPageWithID (30194);
			Page page2 = quest.GetPageWithID (30195);

			// Assert page states before uest is started
			Assert.AreEqual (GQML.STATE_NEW, page1.State);
			Assert.AreEqual (GQML.STATE_NEW, page2.State);

			// Act:
			quest.Start ();

			// Assert:
			Assert.AreEqual (GQML.STATE_RUNNING, page1.State);
			Assert.AreEqual (GQML.STATE_NEW, page2.State);
		}

	}
}
