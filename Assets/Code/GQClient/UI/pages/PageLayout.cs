using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Conf;
using GQ.Client.Model;

namespace GQ.Client.UI
{

	public class PageLayout : ScreenLayout
	{

		protected override void setHeader ()
		{
			if (ConfigurationManager.Current.offerLeaveQuestOnEachPage)
				enableLeaveQuestButton ();

			base.setHeader ();
		}

		void enableLeaveQuestButton ()
		{
			// gather game objects and components:
			Transform menuButtonT = Header.transform.Find ("ButtonPanel/MenuButton");
			Button menuButton = menuButtonT.GetComponent<Button> ();
			Image image = menuButtonT.transform.Find ("Image").GetComponent<Image> ();

			// put icon:
			image.sprite = Resources.Load<Sprite> ("defaults/readable/endQuest");

			// put function and activate button:
			menuButton.onClick.AddListener (leaveQuest);
			menuButton.enabled = true;

			// show:
			menuButton.gameObject.SetActive (true);
			image.gameObject.SetActive (true);
		}

		void leaveQuest ()
		{
			Quest curQuest = QuestManager.Instance.CurrentQuest;
			if (curQuest != null)
				curQuest.End ();
		}
	}
}
