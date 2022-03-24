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
			// TODO SPECIAL Interpretation for Key-Value-Pairs (Key->Display, Value->Result)
			// TODO maybe we should move that into the Answer model as separate Display and Value fields also in editor.
			choiceCtrl.choiceText.text = choice.Text.HTMLDecode().DisplayString().Decode4TMP(false);
			choiceCtrl.choiceButton.onClick.AddListener (choiceCtrl.Select);

			return choiceCtrl;
		}

		public void Select ()
		{
			// TODO SPECIAL Interpretation for Key-Value-Pairs (Key->Display, Value->Result)
			// TODO maybe we should move that into the Answer model as separate Display and Value fields also in editor.
			page.Result = choice.Text.HTMLDecode().DisplayValueString().MakeReplacements();
			page.End ();
		}

		#endregion

	}
}
