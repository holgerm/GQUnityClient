using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class createquestbuttons : MonoBehaviour
{


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

	void Start ()
	{
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

	public void restart ()
	{

		questdb.allquests.Clear ();
		getPublicQuests ();


	}

	public void setSortByName (bool b)
	{

		sortbyname = b;

		DisplayList ();


	}

	public void filterForName (string s)
	{

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
					if (q.name.ToUpper ().Contains (s.ToUpper ())) {
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

				if (questdb.allquests.Count > 0) {
					foreach (Quest q in questdb.allquests) {

						if (q.name.ToUpper ().Contains (s.ToUpper ())) {
							filteredOnlineList.Add (q);
						}
					}
				}
			}

		}
		DisplayList ();
		
		

	}

	public void getPublicQuests ()
	{
		questdb.allquests.Clear ();

//		string url = "http://www.qeevee.org:9091/json/" + portal_id + "/publicgames";
	//	string url = "http://www.qeevee.org:9091/json/" + Configuration.instance.portalID + "/publicgames";
		string url = "http://qeevee.org:9091/json/"+Configuration.instance.portalID+"/publicgamesinfo";
	//	string url = "https://quest-mill.com/temp/publicgamesinfo.json";

		www = new WWW (url);
		
		
		
		
		
		StartCoroutine (DownloadPercentage ());
		StartCoroutine (DownloadList ());
		
	}

	IEnumerator DownloadPercentage ()
	{
		yield return new WaitForSeconds (0.01f);


		
		if (www.progress < 1f && www.error == null) {

			downloadmsg.text = (www.progress * 100) + " %";
			StartCoroutine (DownloadPercentage ());

		} else {


		}


	}

	IEnumerator DownloadList ()
	{
		downloadmsg.enabled = true;

		yield return www;
		if (www.error == null) {
			DisplayList ();
			//Debug.Log("WWW Ok!: " + www.data);
			
			JSONObject j = new JSONObject (www.text);
			accessData (j, "quest");
			foreach (Quest q in questdb.allquests) {
				filteredOnlineList.Add (q);
			}
			DisplayList ();
		} else {
			Debug.Log ("WWW Error: " + www.error);
			downloadmsg.text = www.error;
		}    


	}

	public void resetList ()
	{

		filteredOfflineList.Clear ();

		foreach (Quest q in questdb.GetLocalQuests()) {
			filteredOfflineList.Add (q);
		}

		filterForName (namefilter);



	}

	public void  DisplayList ()
	{
	


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
			showoffline.Reverse ();
		}



		if (showonline.Count > 0 && showoffline.Count > 0) {


			header.text = "Alle Quests";
			SampleListDivider local = Instantiate (sampleListDivider) as SampleListDivider;
			local.title = "Local";
			local.transform.SetParent (transform);
			local.transform.localScale = new Vector3 (1f, 1f, 1f);

		} else {

			if (showonline.Count > 0) { 
				header.text = "Cloud Quests";
			} else if (showoffline.Count > 0) {
				header.text = "Lokale Quests";
			}

		}

		foreach (var item in showoffline) {
			GameObject newButton = Instantiate (sampleButton) as GameObject;
			SampleButton button = newButton.GetComponent <SampleButton> ();
			button.nameLabel.text = item.name;
			button.q = item;
			newButton.transform.SetParent (transform);
			newButton.transform.localScale = new Vector3 (1f, 1f, 1f);
			
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
			newButton.transform.localScale = new Vector3 (1f, 1f, 1f);

		}

		downloadmsg.enabled = false;
		filterinput.interactable = true;

	}



	void createMetaData(JSONObject obj){

		QuestMetaData meta = new QuestMetaData();

		for (int i = 0; i < obj.list.Count; i++) {
			string key = (string)obj.keys [i];
			JSONObject j = (JSONObject)obj.list [i];



			if(key == "key"){
				
				meta.key = j.str;

			} else if(key == "value"){
//				Debug.Log("Meta Value found");
				meta.value = j.str;
				
			}


		}
		currentquest.addMetaData(meta);


	}

	void accessHotspotData(JSONObject obj){
		

		for (int i = 0; i < obj.list.Count; i++) {
			string key = (string)obj.keys [i];
			JSONObject j = (JSONObject)obj.list [i];
			
			Debug.Log(key);
			if(key == "longitude"){

				Debug.Log("long found");
				if(currentquest.start_longitude == null || currentquest.start_longitude == 0){
					currentquest.start_longitude = obj.n;
					Debug.Log("long set: "+obj.str);
				}
				
			} else if(key == "latitude"){
				Debug.Log("lat found");

				if(currentquest.start_latitude == null || currentquest.start_latitude == 0){
					currentquest.start_latitude = obj.n;
				}
				
			}
			
			
		}

		
	}


	void accessData (JSONObject obj, string kei)
	{
		switch (obj.type) {
		
		
		case JSONObject.Type.OBJECT:

			for (int i = 0; i < obj.list.Count; i++) {
				string key = (string)obj.keys [i];
				JSONObject j = (JSONObject)obj.list [i];
				accessData (j, kei + "_" + key);
			}
			break; 
		case JSONObject.Type.ARRAY:
//			Debug.Log("ARRAY: "+kei);
			if (kei == "quest_hotspots") {

				Debug.Log("New Quest");

				currentquest = new Quest();
				if(questdb.allquests == null){
					questdb.allquests = new List<Quest>();
				}
				questdb.allquests.Add (currentquest);

				foreach (JSONObject j in obj.list) {
					accessData (j,kei);
				}
			
					//getFirstHotspot (obj);

			} else if(kei == "quest"){
				//Debug.Log("here");


				foreach (JSONObject j in obj.list) {
					accessData (j,kei);
				}


			}
			if (kei == "quest_metadata") {
				foreach (JSONObject j in obj.list) {
					createMetaData (j);
				}
			} 

			break;
		case JSONObject.Type.STRING:
			if (kei == "quest_name") {
				currentquest.name = obj.str;
			}
			break;
		case JSONObject.Type.NUMBER:
			if (kei == "quest_id") {
				currentquest.id = (int)obj.n;
			 	} else if(kei == "quest_hotspots_latitude"){
			
			if(currentquest.start_latitude == null || currentquest.start_latitude == 0){
				
				currentquest.start_latitude = obj.n;
				
			}
			
		} else if(kei == "quest_hotspots_longitude"){
			Debug.Log("FOUND LONGITUDE");
			
			if(currentquest.start_longitude == null || currentquest.start_longitude == 0){
				Debug.Log("SETTING LONGITUDE");
				currentquest.start_longitude = obj.n;
				
			}

			}


			break;
		case JSONObject.Type.BOOL:
			break;
		case JSONObject.Type.NULL:
			break;
			
		}
	}






}





