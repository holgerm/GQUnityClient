using UnityEngine;
using System.Collections;
using System.IO;

public class page_videoplay : MonoBehaviour {
	
	
	private WWW www;
	
	
	public questdatabase questdb;
	public Quest quest;
	public QuestPage npctalk;


	public VideoPlayer video;


	// Use this for initialization
	void Start () {

						questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
						quest = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest;
						npctalk = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest.currentpage;
				

		if(npctalk.onStart != null){
			
			npctalk.onStart.Invoke();
		}



		string url = npctalk.getAttribute ("file");

		if (!url.StartsWith ("http:") && !url.StartsWith ("https:")) {
						 url =  npctalk.getAttribute ("file");
				}



		if (url.EndsWith ("ogg") || url.EndsWith("ogv")) {
						video.Video = url;
						video.Play = true;

				} else {

			questdb.debug("Video muss im OGV-Theora-Format vorliegen");

				}
	
		}


	void OnVideoStop(){


		onEnd ();

	}


	
		
		





	public void onEnd(){


		npctalk.state = "succeeded";
		
		
		if (npctalk.onEnd != null) {
			Debug.Log ("onEnd");
			npctalk.onEnd.Invoke ();
		} else {
			
			GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().endQuest();
			
		}
		
		
	}
}
