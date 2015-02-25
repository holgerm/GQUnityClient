using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class page_multiplechoicequestion : MonoBehaviour {




	
	public Quest quest;
	public QuestPage multiplechoicequestion;
	private WWW www;
	private WWW www1;
	private WWW www2;
	private WWW www3;
	private WWW www4;
	private WWW www5;
	public Text questiontext;
	public VerticalLayoutGroup list;
	public multiplechoiceanswerbutton answerbuttonprefab;
	
	// Use this for initialization
	void Start () {
		quest = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest;
		multiplechoicequestion = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest.currentpage;


		if(multiplechoicequestion.onStart != null){
			
			multiplechoicequestion.onStart.Invoke();
		}





		if (multiplechoicequestion.contents_question != null) {


			//multiplechoicequestion.contents_question.questiontext.



				} else {



						questiontext.text = multiplechoicequestion.getAttribute ("question");

						foreach (QuestContent qc in multiplechoicequestion.contents_answers) {


								multiplechoiceanswerbutton btn = (multiplechoiceanswerbutton)Instantiate (answerbuttonprefab, transform.position, Quaternion.identity);
								btn.transform.SetParent (list.transform);
								btn.transform.localScale = new Vector3 (1f, 1f, 1f);
								btn.setText (qc.content);

								if (qc.getAttribute ("correct") == "1") {
										btn.correct = true; 
								} else {
										btn.correct = false;

								}
						}


				}
	}
	
	public void onEnd(){
		
		if (multiplechoicequestion.state != "failed") {
			multiplechoicequestion.state = "succeeded";
			
		}
		
		if (multiplechoicequestion.onEnd != null) {
			
			multiplechoicequestion.onEnd.Invoke ();
		} else if(!multiplechoicequestion.onSuccess.hasMissionAction()  && !multiplechoicequestion.onFailure.hasMissionAction()) {
			
			GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().endQuest();
			
		}
		
		
	}


	public void onSuccess(){
		
		multiplechoicequestion.state = "succeeded";

		
		if (multiplechoicequestion.onSuccess != null) {
			
			multiplechoicequestion.onSuccess.Invoke ();
		} 
		
		
	}
	public void onFailure(){
		
		multiplechoicequestion.state = "failed";

		
		if (multiplechoicequestion.onFailure != null) {
			
			multiplechoicequestion.onFailure.Invoke ();
		} 
		
		
	}
}
