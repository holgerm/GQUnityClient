using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Model;

public class AnswerCtrl : MonoBehaviour
{


	#region Inspector Features

	public Image answerImage;
	public Text answerText;
	public Button answerButton;

	#endregion


	#region Runtime API

	public static AnswerCtrl Create (PageMultipleChoiceQuestion mcqPage, Transform rootTransform, Answer answer)
	{
		GameObject go = (GameObject)Instantiate (
			                Resources.Load ("Answer"),
			                rootTransform,
			                false
		                );
		go.SetActive (true);

		AnswerCtrl answerCtrl = go.GetComponent<AnswerCtrl> ();
		answerCtrl.answerText.text = answer.Text;
		if (answer.Correct) {
			answerCtrl.answerButton.onClick.AddListener (mcqPage.Succeed);
		} else {
			answerCtrl.answerButton.onClick.AddListener (mcqPage.Fail);
		}

		return answerCtrl;
	}


	#endregion

}
