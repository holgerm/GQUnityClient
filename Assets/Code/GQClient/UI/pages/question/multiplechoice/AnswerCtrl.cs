using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.UI;
using GQ.Client.Util;

public class AnswerCtrl : MonoBehaviour
{


	#region Inspector & internal features

	public Image answerImage;
	public Text answerText;
	public Button answerButton;

	private PageMultipleChoiceQuestion page;
	private MCQAnswer answer;

	#endregion


	#region Runtime API

	public static AnswerCtrl Create (PageMultipleChoiceQuestion mcqPage, Transform rootTransform, MCQAnswer answer)
	{
		GameObject go = (GameObject)Instantiate (
			                Resources.Load ("Answer"),
			                rootTransform,
			                false
		                );
		go.SetActive (true);

		AnswerCtrl answerCtrl = go.GetComponent<AnswerCtrl> ();
		answerCtrl.page = mcqPage;
		answerCtrl.answer = answer;
		answerCtrl.answerText.text = answer.Text.MakeReplacements();
		answerCtrl.answerButton.onClick.AddListener (answerCtrl.Select);

		return answerCtrl;
	}

	public void Select ()
	{
		page.Result = answer.Text;
		if (answer.Correct) {
			page.Succeed ();
		} else {
            if (page.RepeatUntilSuccess)
            {
                ((MultipleChoiceQuestionController)page.PageCtrl).Repeat();
            }
            else
            {
                page.Fail();
            }
		}
	}

	#endregion

}
