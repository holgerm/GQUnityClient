using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.UI;
using GQ.Client.Util;
using TMPro;
using GQ.Client.Conf;

public class AnswerCtrl : MonoBehaviour
{


	#region Inspector & internal features

	public Image answerImage;
	public TextMeshProUGUI answerText;
	public Button answerButton;

	private PageMultipleChoiceQuestion page;
	private MCQAnswer answer;

	#endregion


	#region Runtime API

	public static AnswerCtrl Create (PageMultipleChoiceQuestion mcqPage, Transform rootTransform, MCQAnswer answer)
	{
		GameObject go = (GameObject)Instantiate (
			                AssetBundles.Asset ("prefabs", "Answer"),
			                rootTransform,
			                false
		                );
		go.SetActive (true);

		AnswerCtrl answerCtrl = go.GetComponent<AnswerCtrl> ();
		answerCtrl.page = mcqPage;
		answerCtrl.answer = answer;
		answerCtrl.answerText.text = answer.Text.Decode4TMP(false);
        answerCtrl.answerButton.onClick.AddListener (answerCtrl.Select);

		return answerCtrl;
	}

	public void Select ()
	{
		page.Result = answer.Text.MakeReplacements();
		if (answer.Correct) {
			page.Succeed (alsoEnd: true);
		} else {
            if (page.RepeatUntilSuccess)
            {
                    page.Fail(alsoEnd: false);
                    ((MultipleChoiceQuestionController)page.PageCtrl).Repeat();
            }
            else
            {
                page.Fail(alsoEnd: true);
            }
		}
	}

	#endregion

}
