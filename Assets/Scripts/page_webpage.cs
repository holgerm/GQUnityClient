using UnityEngine;
using System.Collections;

public class page_webpage : MonoBehaviour {
	
	
	public Quest quest;
	public QuestPage textquestion;
	

	
	// Use this for initialization
	void Start () {
		if (GameObject.Find ("QuestDatabase") != null) {
						quest = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest;
						textquestion = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest.currentpage;
				}
		
		if(textquestion.onStart != null){
			
			textquestion.onStart.Invoke();
		}
		
		
		
	
		
	}

	
	public void onEnd(){
		
			textquestion.state = "succeeded";
		
		
		if (textquestion.onEnd != null) {
			
			textquestion.onEnd.Invoke ();
		} else {
			
			GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().endQuest();
			
		}
		
		
	}

	

}
