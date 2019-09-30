using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Util;
using GQ.Client.Conf;
using TMPro;

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
			                Resources.Load ("Choice"),
			                rootTransform,
			                false
		                );
		go.SetActive (true);

		ChoiceCtrl choiceCtrl = go.GetComponent<ChoiceCtrl> ();
		choiceCtrl.page = myPage;
		choiceCtrl.choice = choice;
		choiceCtrl.choiceText.color = ConfigurationManager.Current.mainFgColor;
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
