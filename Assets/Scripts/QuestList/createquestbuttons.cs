
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;





public class createquestbuttons : MonoBehaviour {


	public SampleListDivider sampleListDivider;
	public GameObject sampleButton;
	public questdatabase questdb;

	public InputField filterinput;

	public Text downloadmsg;


	public Quest currentquest;


	public List<Quest> filteredOnlineList;
	public List<Quest> filteredOfflineList;


	public Text header;


	public bool sortbyname = false;

	public int portal_id = 1;

	public string namefilter;
	private WWW www;
	void Start () {


		questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();

		if (!Application.isWebPlayer) {

		foreach (Quest q in questdb.GetLocalQuests()) {
			filteredOfflineList.Add (q);
		}

						if (questdb.allquests.Count < 1 && filteredOfflineList.Count < 1) {
								getPublicQuests ();
						} else {
								DisplayList ();
			
						}
				}
	}


	public void restart(){

		questdb.allquests.Clear ();
		getPublicQuests();


		}




	public void setSortByName(bool b){

		sortbyname = b;

		DisplayList ();


		}


	public void filterForName(string s){

		namefilter = s;
		if (questdb.localquests.Count > 0) {
			filteredOfflineList.Clear ();
			

			
			
			
			if (s == "") {
				
				foreach (Quest q in questdb.localquests) {
					filteredOfflineList.Add (q);
				}
			} else {
				
				foreach (Quest q in questdb.localquests) {
					
					//Debug.Log (q.name + " contains " + s + "? " + q.name.Contains (s));
					if (q.name.ToUpper().Contains (s.ToUpper())) {
						filteredOfflineList.Add (q);
					}
				}
			}
			
		}

		if (questdb.allquests.Count > 0) {
						filteredOnlineList.Clear ();

						foreach (RectTransform go in GetComponentsInChildren<RectTransform>()) {
								if (go != transform) {
										Destroy (go.gameObject);
								}
						}



						if (s == "") {

								foreach (Quest q in questdb.allquests) {
										filteredOnlineList.Add (q);
								}
						} else {

								if(questdb.allquests.Count > 0){
												foreach (Quest q in questdb.allquests) {

														if (q.name.ToUpper().Contains (s.ToUpper())) {
																filteredOnlineList.Add (q);
														}
												}
								}
						}

				}
		DisplayList ();
		
		

		}
	public void getPublicQuests(){
		questdb.allquests.Clear ();

		string url = "http://www.qeevee.org:9091/json/"+portal_id+"/publicgames";
		
		www = new WWW (url);
		
		
		
		
		
		StartCoroutine (DownloadPercentage ());
		StartCoroutine (DownloadList ());
		
	}

	IEnumerator DownloadPercentage(){
		yield return new WaitForSeconds (0.01f);


		
		if (www.progress < 1f && www.error == null) {

						downloadmsg.text = (www.progress * 100) + " %";
						StartCoroutine (DownloadPercentage ());

				} else {


				}


	}


	IEnumerator DownloadList(){
		downloadmsg.enabled = true;

		yield return www;
		if (www.error == null)
		{
			DisplayList ();
			//Debug.Log("WWW Ok!: " + www.data);
			
			JSONObject j = new JSONObject(www.text);
			accessData(j,"quest");
			foreach (Quest q in questdb.allquests) {
				filteredOnlineList.Add (q);
			}			DisplayList();
		} else {
			Debug.Log("WWW Error: "+ www.error);
			downloadmsg.text = www.error;
		}    


		}





	public void resetList(){

		filteredOfflineList.Clear();

		foreach (Quest q in questdb.GetLocalQuests()) {
			filteredOfflineList.Add (q);
		}

		filterForName(namefilter);



	}

	public void  DisplayList () {
	


		foreach (RectTransform go in GetComponentsInChildren<RectTransform>()) {
			if (go != transform) {
				Destroy (go.gameObject);
			}
		}

		List<Quest> showonline = new List<Quest> ();
		List<Quest> showoffline = new List<Quest> ();

		foreach (Quest q in filteredOfflineList) {
						showoffline.Add (q);
				}
		foreach (Quest q in filteredOnlineList) {
			showonline.Add (q);
		}


		if (sortbyname) {
						showonline.Sort ();
						showoffline.Sort ();
				} else {
			showoffline.Reverse();
				}



		if (showonline.Count > 0 && showoffline.Count > 0) {


						header.text = "Alle Quests";
						SampleListDivider local = Instantiate (sampleListDivider) as SampleListDivider;
						local.title = "Local";
						local.transform.SetParent (transform);
						local.transform.localScale = new Vector3 (1f, 1f, 1f);

				} else {

			if(showonline.Count > 0){ 
				header.text = "Cloud Quests";
			} else if( showoffline.Count > 0){
				header.text = "Lokale Quests";
			}

				}

		foreach (var item in showoffline) {
			GameObject newButton = Instantiate (sampleButton) as GameObject;
			SampleButton button = newButton.GetComponent <SampleButton> ();
			button.nameLabel.text = item.name;
			button.q = item;
			newButton.transform.SetParent (transform);
			newButton.transform.localScale = new Vector3(1f,1f,1f);
			
		}

		if (showonline.Count > 0 && showoffline.Count > 0) {

						SampleListDivider cloud = Instantiate (sampleListDivider) as SampleListDivider;
						cloud.title = "Cloud";
						cloud.transform.SetParent (transform);
						cloud.transform.localScale = new Vector3 (1f, 1f, 1f);
				}
		foreach (var item in showonline) {
			GameObject newButton = Instantiate (sampleButton) as GameObject;
			SampleButton button = newButton.GetComponent <SampleButton> ();
			button.nameLabel.text = item.name;
			button.q = item;
			newButton.transform.SetParent (transform);
			newButton.transform.localScale = new Vector3(1f,1f,1f);

		}

		downloadmsg.enabled = false;
		filterinput.interactable = true;

	}

	
	void accessData(JSONObject obj, string kei){
		switch(obj.type){
		
		
		case JSONObject.Type.OBJECT:

			for(int i = 0; i < obj.list.Count; i++){
				string key = (string)obj.keys[i];
				JSONObject j = (JSONObject)obj.list[i];
				accessData(j,kei+"_"+key);
			}
			break; 
		case JSONObject.Type.ARRAY:
			foreach(JSONObject j in obj.list){
				accessData(j,kei);
			}
			break;
		case JSONObject.Type.STRING:
			if(kei == "quest_name"){
				currentquest.name = obj.str;
				questdb.allquests.Add(currentquest);
			}
			break;
		case JSONObject.Type.NUMBER:
			if(kei == "quest_id"){
			currentquest = new Quest();
			currentquest.id = (int)obj.n;
		//	Debug.Log(kei+":"+obj.n);
			}
			break;
		case JSONObject.Type.BOOL:
			break;
		case JSONObject.Type.NULL:
			break;
			
		}
	}






}





