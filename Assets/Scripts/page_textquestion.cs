using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Text.RegularExpressions;

public class page_textquestion : MonoBehaviour {

	
	
	public questdatabase questdb;

	public Quest quest;
	public QuestPage textquestion;

	public Text questiontext;

	public Button submitbutton;

	public InputField input;
	
	// Use this for initialization
	void Start () {
		questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();

		quest = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest;
		textquestion = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest.currentpage;
		
		
		if(textquestion.onStart != null){
			
			textquestion.onStart.Invoke();
		}
		
		
		
		
		questiontext.text = textquestion.getAttribute ("question");
		

	}




	public void checkAnswerFinal(){
		string x = input.text;

		bool repeat = false;
		
		if(textquestion.hasAttribute("loopUntilSuccess")){
			if(textquestion.getAttribute("loopUntilSuccess") == "true"){
				repeat = true;
			} 
		}

		if (textquestion.contents_answers.Count > 0) {

						bool b = false;


			foreach(QuestContent y in textquestion.contents_answers){


				questdb.debug("REGEXP "+x+" MATCH "+y.content+" -> "+Regex.IsMatch(x, y.content, RegexOptions.IgnoreCase));

				if(y.content == x || Regex.IsMatch(x, y.content, RegexOptions.IgnoreCase)){
					b = true;
				}


			}

			if(b){
				textquestion.state = "succeeded";

				onSuccess();
			} else {



				if(!repeat){
				textquestion.state = "failed";

				onFailure();
				}

			}
	
				}
		textquestion.result = x;


		if (textquestion.state == "succeeded" || !repeat) {

						onEnd ();

				}




		}



	public void checkAnswerMid(string c){
		if (textquestion.contents_answers.Count > 0) {
			
			string x = input.text;
			bool b = false;
			
			
			foreach(QuestContent y in textquestion.contents_answers){

				if(y.content == x || Regex.IsMatch(x, y.content, RegexOptions.IgnoreCase)){
					b = true;

				}
				
				
			}
			textquestion.result = x;

			if(b){
				textquestion.state = "succeeded";
				onSuccess();
				onEnd ();
			} 
			
		} 
		

		


	}



	
	public void onEnd(){
		
		if (textquestion.state != "failed") {
			textquestion.state = "succeeded";
			
		}
		
		if (textquestion.onEnd != null) {
			
			textquestion.onEnd.Invoke ();
		} else if(!textquestion.onSuccess.hasMissionAction()  && !textquestion.onFailure.hasMissionAction()) {
			
			GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().endQuest();
			
		}
		
		
	}
	
	
	public void onSuccess(){
		
		textquestion.state = "succeeded";
		
		
		if (textquestion.onSuccess != null) {
			
			textquestion.onSuccess.Invoke ();
		} 
		
		
	}
	public void onFailure(){
		
		textquestion.state = "failed";
		
		
		if (textquestion.onFailure != null) {
			
			textquestion.onFailure.Invoke ();
		} 
		
		
	}
}
