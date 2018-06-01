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
			enableLeaveQuestButton (ConfigurationManager.Current.offerLeaveQuestOnEachPage);

			base.setHeader ();

			Canvas headerCanvas = Header.GetComponent<Canvas> ();
			if (headerCanvas != null) {
				headerCanvas.overrideSorting = true;
				headerCanvas.sortingOrder = 3;
			}
		}

		void enableLeaveQuestButton (bool enable)
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
			menuButton.gameObject.SetActive (enable);
			image.gameObject.SetActive (enable);
		}

		void leaveQuest ()
		{
			Quest curQuest = QuestManager.Instance.CurrentQuest;
			if (curQuest != null)
				curQuest.End ();
		}
	}
}
