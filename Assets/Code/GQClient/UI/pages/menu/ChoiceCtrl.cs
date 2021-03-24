using Code.GQClient.Conf;
using Code.GQClient.Model.pages;
using Code.GQClient.start;
using Code.GQClient.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.pages.menu
{
	public class ChoiceCtrl : MonoBehaviour
	{


		#region Inspector & internal features

		public Image answerImage;
		public TextMeshProUGUI choiceText;
		public Button choiceButton;

		private PageMenu page;
		private MenuChoice choice;

		#endregion


		#region Runtime API

		public static ChoiceCtrl Create (PageMenu myPage, Transform rootTransform, MenuChoice choice)
		{
			GameObject go = (GameObject)Instantiate (
				AssetBundles.Asset("prefabs", "Choice"),
				rootTransform,
				false
			);
			go.SetActive (true);

			ChoiceCtrl choiceCtrl = go.GetComponent<ChoiceCtrl> ();
			choiceCtrl.page = myPage;
			choiceCtrl.choice = choice;
			choiceCtrl.choiceText.color = Config.Current.mainFgColor;
			choiceCtrl.choiceText.text = choice.Text.Decode4TMP(false);
			choiceCtrl.choiceButton.onClick.AddListener (choiceCtrl.Select);

			return choiceCtrl;
		}

		public void Select ()
		{
			page.Result = choice.Text.MakeReplacements();
			page.End ();
		}

		#endregion

	}
}
