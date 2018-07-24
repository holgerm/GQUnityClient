using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Util;
using GQ.Client.Conf;

public class ChoiceCtrl : MonoBehaviour
{


	#region Inspector & internal features

	public Image answerImage;
	public Text choiceText;
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
		choiceCtrl.choiceText.color = ConfigurationManager.Current.mainColorFG;
		choiceCtrl.choiceText.text = choice.Text.Decode4HyperText(false);
		choiceCtrl.choiceButton.onClick.AddListener (choiceCtrl.Select);

		return choiceCtrl;
	}

	public void Select ()
	{
		page.Result = choice.Text;
		page.End ();
	}

	#endregion

}
