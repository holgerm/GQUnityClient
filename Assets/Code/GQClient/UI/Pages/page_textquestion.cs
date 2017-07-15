using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Text.RegularExpressions;
using GQ.Client.Model;
using UnityEngine.SceneManagement;

public class page_textquestion : MonoBehaviour
{

	private string feedbackTextOnRepeat = "X";
	public GameObject feedbackPanel;
	public GameObject questionPanel;

	public questdatabase questdb;
	public Quest quest;
	public Page textquestion;
	public Text questiontext;
	public Button submitbutton;
	public InputField input;
	
	// Use this for initialization
	void Start ()
	{

		GameObject questdbGO = GameObject.Find ("QuestDatabase");

		if (questdbGO == null) {

			SceneManager.LoadScene ("questlist");
			return;
		}

		questdb = questdbGO.GetComponent<questdatabase> ();
		quest = QuestManager.Instance.CurrentQuest;
		textquestion = QuestManager.Instance.CurrentQuest.currentpage;

		feedbackPanel.SetActive (false);
		questionPanel.SetActive (true);
		if (textquestion.hasAttribute ("loopText"))
			feedbackTextOnRepeat = textquestion.getAttribute ("loopText");
		Text feedbackText = feedbackPanel.transform.Find ("Text").gameObject.GetComponent<Text> ();
		if (feedbackText != null)
			feedbackText.text = feedbackTextOnRepeat;
		

		if (textquestion.onStart != null) {
			
			textquestion.onStart.Invoke ();
		}
		
		questiontext.text = questdb.GetComponent<actions> ().formatString (textquestion.getAttribute ("question"));
	}

	public void checkAnswerFinal ()
	{
		
		string x = input.text;
		textquestion.result = x;


		bool repeat = false;
		
		if (textquestion.hasAttribute ("loopUntilSuccess")) {
			if (textquestion.getAttribute ("loopUntilSuccess") == "true") {
				repeat = true;
			} 
		}

		if (textquestion.contents_answers.Count > 0) {

			bool correct = false;
			bool match;

			foreach (QuestContent y in textquestion.contents_answers) {
				if (textquestion.result == null || y == null || y.content == null)
					continue;
				
				match = Regex.IsMatch (textquestion.result, y.content, RegexOptions.IgnoreCase);

				questdb.debug ("REGEXP " + textquestion.result + " MATCH " + y.content + " -> " + match);

				if (match || questdb.GetComponent<actions> ().formatString (y.content).Equals (textquestion.result)) {
					correct = true;
					Debug.Log ("TextQuestion: MATCHED");
				}
			}

			if (correct) {

				textquestion.stateOld = "succeeded";
				onSuccess ();
			} else {
				
				if (repeat) {

					questionPanel.SetActive (false);
					feedbackPanel.SetActive (true);
				} else {
					
					textquestion.stateOld = "failed";
					onFailure ();
				}
			}
		} else {

			textquestion.stateOld = "succeeded";
		}

		if (textquestion.stateOld == "succeeded" || !repeat) {

			onEnd ();
		}
	}

	public void onEnd ()
	{
		
		if (textquestion.stateOld != "failed") {

			textquestion.stateOld = "succeeded";
		}
		
		if (textquestion.onEnd != null) {
			
			textquestion.onEnd.Invoke ();
		} else if (!hasMissionAction (textquestion.onSuccess) && !hasMissionAction (textquestion.onFailure)) {
			
			questdb.endQuest ();
		}
	}

	protected bool hasMissionAction (QuestTrigger evt)
	{
		if (evt == null)
			return false;

		foreach (QuestAction a in evt.actions) {
			if (a.hasMissionAction ()) {
				return true;
			}
		}
		return false;
	}

	public void onSuccess ()
	{
		
		textquestion.stateOld = "succeeded";
		
		if (textquestion.onSuccess != null) {
			
			textquestion.onSuccess.Invoke ();
		} 
	}

	public void onFailure ()
	{
		
		textquestion.stateOld = "failed";
		
		if (textquestion.onFailure != null) {
			
			textquestion.onFailure.Invoke ();
		} 
	}

	public void FeedbackButtonPressed ()
	{
		feedbackPanel.SetActive (false);
		questionPanel.SetActive (true);
	}


}
