using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using GQ.Util;

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

	public questdatabase questdb;
	public actions 	questactions;
		public Image image;

	// Use this for initialization
	void Start () {


		if (GameObject.Find ("QuestDatabase") == null) {
			
			Application.LoadLevel (0);
			
		} else {


			questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
			quest = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest;
			multiplechoicequestion = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest.currentpage;
			questactions = GameObject.Find ("QuestDatabase").GetComponent<actions> ();


			if (multiplechoicequestion.onStart != null) {
			
				multiplechoicequestion.onStart.Invoke ();
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







			if (multiplechoicequestion.getAttribute ("bg") != "") {
				if (multiplechoicequestion.getAttribute ("bg").StartsWith ("@_")) {
				
				
				
				
					foreach (QuestRuntimeAsset qra in questactions.photos) {
					
						//Debug.Log("KEY:"+qra.key);
					
						if (qra.key == multiplechoicequestion.getAttribute ("bg")) {
						
						
						
						
							Sprite s = Sprite.Create (qra.texture, new Rect (0, 0, qra.texture.width, qra.texture.height), new Vector2 (0.5f, 0.5f));
						

							
							image.sprite = s;
							image.enabled = true;
							
							

						
						}
					}
					//Debug.Log ("donewithforeach");
				
				
				
				
				} else {
				
				
				
				
					foreach (SpriteConverter sc in questdb.convertedSprites) {
					
					
					
						if (sc.filename == multiplechoicequestion.getAttribute ("bg")) {
						
							if (sc.isDone) {
								if (sc.sprite != null) {
						
									image.sprite = sc.sprite;
									image.enabled = true;
									
									

								
								
								
								
								
								} else {
								
									Debug.Log ("Sprite was null");
								}
							} else {
							
								Debug.Log ("SpriteConverter was not done.");
							
							}
						}
					}
				
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
		} else if((multiplechoicequestion.onSuccess == null && multiplechoicequestion.onFailure == null)
		          ||
		          ( multiplechoicequestion.onSuccess != null && !multiplechoicequestion.onSuccess.hasMissionAction()  && multiplechoicequestion.onFailure != null && !multiplechoicequestion.onFailure.hasMissionAction())
		          ) {
			
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
