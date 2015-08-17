using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System;
using System.Linq;
using System.Text;
using GQ.Geo;
using GQ.Util;


public class questdatabase : MonoBehaviour
{
	public Quest currentquest;
	public Transform questdataprefab;
	public Transform currentquestdata;
	public List<Quest> allquests;
	public List<Quest> localquests;
	private WWW www;

	public List<string> wanttoload;
	public List<WWW> filedownloads;
	public Image publicquestslist;
	public actions actioncontroller;
	public QuestMessage message_prefab;
	public List<QuestRuntimeHotspot> hotspots;
	public Image questmilllogo;
	public Text webloadingmessage;
	public List<String> loadedfiles;
	public string webxml;
	public bool fixedposition = true;
	ScreenOrientation originalOrientation = ScreenOrientation.Portrait;
	private string PATH_2_PREDEPLOYED_QUEST;
	public string PATH_2_PREDEPLOYED_QUESTS;
	private string PREDEPLOYED_QUESTS_ZIP;
	private string LOCAL_QUESTS_ZIP;
	private string PATH_2_LOCAL_QUESTS;
	public int msgsactive = 0;
	public int files_all = 0;
	public int files_complete = 0;
	public int bytesloaded = 0;
	public float fakebytes = 0;
	public string currentxml;
	public loadinglogo loadlogo;
	public List<SpriteConverter> convertedSprites;
	public string spriteError;
	public List<string> allmetakeys;
	public string sortby = "Erstellungsdatum";
	public bool descending = false;
	public bool convertToSprites = false;
	bool predepzipfound = false;
	public menucontroller menu;



	public createquestbuttons buttoncontroller;


	IEnumerator Start ()
	{
		PATH_2_PREDEPLOYED_QUESTS = System.IO.Path.Combine (Application.streamingAssetsPath, "predeployed/quests");
		PREDEPLOYED_QUESTS_ZIP = System.IO.Path.Combine (Application.streamingAssetsPath, "predeployed/quests.zip");
		LOCAL_QUESTS_ZIP = System.IO.Path.Combine (Application.persistentDataPath, "tmp_predeployed_quests.zip");
		PATH_2_LOCAL_QUESTS = System.IO.Path.Combine (Application.persistentDataPath, "quests");

		//msgsactive = 0;

#if (UNITY_ANDROID && !UNITY_EDITOR)

		PREDEPLOYED_QUESTS_ZIP = "jar:file://" + Application.dataPath + "!/assets/" + "/predeployed/quests.zip";
		PATH_2_PREDEPLOYED_QUESTS = "jar:file://" + Application.dataPath + "!/assets/predeployed/quests";

		// PATH_2_PREDEPLOYED_QUESTS = "file:///android_asset/predeployed/quests"; NOT WORKING

#endif

		if (GameObject.Find ("QuestDatabase") != gameObject) {
			Destroy (GameObject.Find ("QuestDatabase"));		
		} else {
			DontDestroyOnLoad (gameObject);
			//			Debug.Log (Application.persistentDataPath);
		}





		if (Configuration.instance.questvisualization != "list") {

			GameObject.Find ("ListPanel").SetActive (false);

		}


	
		if (Configuration.instance.showcloudquestsimmediately && Configuration.instance.autostartQuestID == 0) {
		




			allquests.Clear ();
			
			Debug.Log("doing it");
			
			string url = "http://qeevee.org:9091/json/"+Configuration.instance.portalID+"/publicgamesinfo";
			
			WWW listwww = new WWW (url);
			
			
			
			
			
			StartCoroutine (DownloadPercentage (listwww));
			StartCoroutine (DownloadList (listwww));


			
		} else {
			buttoncontroller.DisplayList ();
			
		}


		if (Application.isWebPlayer) {
			if(webloadingmessage != null){

			webloadingmessage.enabled = true;
			}
			questmilllogo.enabled = true;
			loadlogo.enable ();
		} 


		autoStartQuest();


		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();

		if (GameObject.Find ("MenuCanvas") != null) {
			
			menu = GameObject.Find ("MenuCanvas").GetComponent<menucontroller>();
			
		}




	}



	void autoStartQuest(){

		if (Configuration.instance.autostartQuestID != 0) {
			GameObject questListPanel = GameObject.Find ("/Canvas");
			questmilllogo.enabled = true;
			Debug.Log ("Autostart: Starting quest " + Configuration.instance.autostartQuestID);

			if(Configuration.instance.autostartIsPredeployed){
			StartCoroutine(	startPredeployedQuest(Configuration.instance.autostartQuestID));

			} else {
			StartQuest (Configuration.instance.autostartQuestID);
			}
		}

	}

	IEnumerator InitPredeployedQuests ()
	{

		#if !UNITY_WEBPLAYER
		if(webloadingmessage != null){

		webloadingmessage.text = "Lade...";
		}
		//webloadingmessage.enabled = true;
		questmilllogo.enabled = true;
	
		yield return null;
		
		Debug.Log ("InitPredeployedQuests 1, looking for predep zip: " + PREDEPLOYED_QUESTS_ZIP);
		if (PREDEPLOYED_QUESTS_ZIP.Contains ("://")) {
			// on platforms which use an url type as asset path (e.g. Android):
			Debug.Log ("WWW questZIP = new WWW (PREDEPLOYED_QUESTS_ZIP);");
			WWW questZIP = new WWW (PREDEPLOYED_QUESTS_ZIP); // this is the path to your StreamingAssets in android
			yield return questZIP;
			
			Debug.Log ("PDir3: LOADED BY WWW. questZIP.text: " + questZIP.text);
			if (questZIP.error != null && !questZIP.error.Equals ("")) {
				Debug.Log ("PDir3: LOADED BY WWW. questZIP.error: " + questZIP.error);
			} else {				
				Debug.Log ("PDir3: LOADED BY WWW. questZIP.bytesDownloaded: " + questZIP.bytesDownloaded);
				if (File.Exists (LOCAL_QUESTS_ZIP)) {
					Debug.LogWarning ("Local copy of predeployment zip file was found. Should have been deleted at last initialization.");
					File.Delete (LOCAL_QUESTS_ZIP);
				}
			}
			File.WriteAllBytes (LOCAL_QUESTS_ZIP, questZIP.bytes);
			Debug.Log ("PDir3: ZIP FILE WRITTEN - ok? : " + File.Exists (LOCAL_QUESTS_ZIP));
			predepzipfound = true;
		} else {
			Debug.Log ("Not running on platforms which use an url type as asset path (e.g. Android):");
			initPreloadedQuestiOS();
		}


		if (predepzipfound) {
		
			ZipUtil.Unzip (LOCAL_QUESTS_ZIP, Application.persistentDataPath);
			File.Delete (LOCAL_QUESTS_ZIP);
			Debug.Log ("ZIP FILE DELETED");
			questmilllogo.enabled = false;
			if(webloadingmessage != null){

			webloadingmessage.enabled = false;
			}
			autoStartQuest ();
		}

#else
		yield return null;
#endif
	}



void initPreloadedQuestiOS(){

	// on platforms which have a straight file path (e.g. iOS):
	Debug.Log ("InitPredeployedQuests: on IOS.");
	if (!File.Exists (PREDEPLOYED_QUESTS_ZIP)) {
		Debug.Log ("InitPredeployedQuests: ZIP FILE NOT FOUND");
		return;
	}
	File.Copy (PREDEPLOYED_QUESTS_ZIP, LOCAL_QUESTS_ZIP);
	if (!File.Exists (LOCAL_QUESTS_ZIP)) {
		Debug.Log ("InitPredeployedQuests: LOCAL COPY NOT CREATED.");
	}

		predepzipfound = true;


}


	private int reloadButtonPressed = 0;
	private int numberOfPressesNeededToReload = 10;

	public bool ReloadButtonPressed ()
	{
		reloadButtonPressed++;
		if (reloadButtonPressed >= numberOfPressesNeededToReload) {
			reloadButtonPressed = 0;
			reloadAutoStartQuest ();
			return true;
		} else {
			int remainingPresses = numberOfPressesNeededToReload - reloadButtonPressed;
//			showmessage ("Wenn sie diesen Button noch " + (remainingPresses) + " mal drücken werden alle Medien gelöscht und neu geladen.", "OK");
			return false;
		}
	}

	public void reloadAutoStartQuest ()
	{





		List<Quest> alllocalquests = new List<Quest> ();
		alllocalquests.AddRange (localquests);

		foreach (Quest lq in alllocalquests) {

				
				
				removeQuest (lq);
				
				

		}

		localquests.Clear ();



		Application.LoadLevel (0);
	
		GameObject.Find ("ImpressumCanvas").GetComponent<showimpressum> ().closeImpressum ();



	}

	bool IsQuestInitialized (int id)
	{
		string questDirPath = System.IO.Path.Combine (PATH_2_LOCAL_QUESTS, id.ToString ());
		return Directory.Exists (questDirPath);
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
				
				//				Debug.Log("New Quest");
				
				currentquest = new Quest();
				if(allquests == null){
					allquests = new List<Quest>();
				}
				allquests.Add (currentquest);
				
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
				//Debug.Log("FOUND LONGITUDE");
				
				if(currentquest.start_longitude == null || currentquest.start_longitude == 0){
					//	Debug.Log("SETTING LONGITUDE");
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

	public IEnumerator DownloadList (WWW www)
	{



		Debug.Log ("getting list");
		if (loadlogo != null) {
			loadlogo.enable ();
		}
		if (webloadingmessage != null) {

			webloadingmessage.enabled = true;
		}
		yield return www;
		if (www.error == null) {

			//Debug.Log("WWW Ok!: " + www.data);
			buttoncontroller.filteredOnlineList.Clear ();

			allquests.Clear();
			JSONObject j = new JSONObject (www.text);
			accessData (j, "quest");
			foreach (Quest q in allquests) {
				buttoncontroller.filteredOnlineList.Add (q);
			}

			currentquest = null;

			if(Configuration.instance.questvisualization == "list"){
				
				buttoncontroller.DisplayList ();
				
			} else if(Configuration.instance.questvisualization == "map"){
				
				showQuestMap();
				
			}
			
			
		} else {
			Debug.Log ("WWW Error: " + www.error);
			if(webloadingmessage != null){

			webloadingmessage.text = www.error;
			}
		}    
		
		
	}

	public IEnumerator DownloadPercentage (WWW www)
	{
		yield return new WaitForSeconds (0.01f);
		
		
		
		if (www.progress < 1f && www.error == null) {
			if (loadlogo != null) {

			loadlogo.enable();
			}

			if(webloadingmessage != null){
			webloadingmessage.text = (www.progress * 100) + " %";
			}
			StartCoroutine (DownloadPercentage (www));
			
		} else {
			if (loadlogo != null) {

			loadlogo.disable();
			}
			
		}
		
		
	}

	



	IEnumerator startPredeployedQuest(int id){


		Quest q = new Quest ();

		q.id = id;
		q.predeployed = true;

		q.filepath = PATH_2_PREDEPLOYED_QUESTS +"/" + id + "/game.xml";
		currentquest = q;


		string pre = "file: //";
	
		
		if (Application.platform == RuntimePlatform.Android) {
			
			pre = "";
		}
		
		


		WWW wwwpdq = new WWW (pre+""+q.filepath);

		yield return wwwpdq;

		if (wwwpdq.error == null) {
			

			
			
			//currentquestdata = (Transform)Instantiate (questdataprefab, transform.position, Quaternion.identity);
			q.xmlcontent = UTF8Encoding.UTF8.GetString (wwwpdq.bytes); 

		} else {

			Debug.Log("Error: "+wwwpdq.error);
		}
		//Debug.Log ("creatign predeployed quest");
		//Debug.Log(q.xmlcontent);


		currentquest = q.LoadFromText (id, true);
		localquests.Add (currentquest);
		bool hasmorethanmetadata = true;
		currentquest.currentpage = currentquest.pages.First ();
		int c = 0;
		while (currentquest.currentpage.type == "MetaData") {
			
			if(currentquest.pages.Count >= c-1){
				currentquest.currentpage = currentquest.pages[c];
				c++;
			} else {
				
				hasmorethanmetadata = false;
				break;
			}
		}
		
		hotspots = new List<QuestRuntimeHotspot> ();
		foreach (QuestHotspot qh in currentquest.hotspots) {
			bool initialActivity = qh.hasAttribute ("initialActivity") && qh.getAttribute ("initialActivity") == "true";
			bool initialVisibility = qh.hasAttribute ("initialVisibility") && qh.getAttribute ("initialVisibility") == "true";
			
			hotspots.Add (new QuestRuntimeHotspot (qh, initialActivity, initialVisibility, qh.latlon));
		}
		
		if (canPlayQuest (currentquest) && hasmorethanmetadata) {
			
			
			
		
				StartCoroutine (waitForSpriteConversion (currentquest.currentpage.id));
				

			
		} else {
			Debug.Log ("showing message");
			showmessage ("Entschuldigung! Die Quest kann in dieser Version nicht abgespielt werden.");
			if(webloadingmessage != null){

			webloadingmessage.enabled = false;
			}
			if (loadlogo != null) {

			loadlogo.disable();
			}
			GameObject.Find ("List").GetComponent<createquestbuttons> ().resetList ();
			
		}

	}




	public void showQuestMap(){

		Debug.Log ("showing map");

		hotspots = new List<QuestRuntimeHotspot> ();

		hotspots.AddRange (getActiveHotspots ());

		GameObject.Find ("MenuCanvas").GetComponent<Animator> ().SetTrigger ("startMenu");


		GameObject.Find ("BgCam").GetComponent<Camera> ().enabled = false;
		Application.LoadLevelAdditive ("page_map");

	

	}



	void StartQuest (int id)
	{
		// if the given quest is already intialized start it, otherwise download it first and start it:
		List<Quest> localQuests = GetLocalQuests ();
		Quest q = null;
		foreach (Quest curQuest in localQuests) {
			
			if (curQuest.id == id) {
				q = curQuest;
			}
			
		}
		
		if (q == null) {
//			Debug.Log ("Problem 1 id: " + id);
			q = new Quest ();
			q.id = id;
			downloadQuest (q);
		} else {
			startQuest (q);
		}

	}

	IEnumerator CheckConnection (Quest q, float elapsedTime, WWW www)
	{

		while (msgsactive > 0) {
			yield return 0;
			//Debug.Log("messages active");
		}


		Debug.Log ("CheckConnection(): before ping");
		yield return new WaitForSeconds (0.1f);
		if (www.isDone) {
			bool ok = (www.error == null);
			downloadAfterConnectionChecked (q, ok);
		} else 
			if (elapsedTime < 2.0f)
			StartCoroutine (CheckConnection (q, elapsedTime + 0.1f, www));
		else {
			downloadAfterConnectionChecked (q, false);
		}
	}

	static public void CopyFolder (string sourceFolder, string destFolder)
	{
		if (!Directory.Exists (destFolder))
			Directory.CreateDirectory (destFolder);
		string[] files = Directory.GetFiles (sourceFolder);
		foreach (string file in files) {
			string name = Path.GetFileName (file);
			if (!name.EndsWith (".meta")) {
				string dest = Path.Combine (destFolder, name);
				File.Copy (file, dest);
			}
		}
		string[] folders = Directory.GetDirectories (sourceFolder);
		foreach (string folder in folders) {
			string name = Path.GetFileName (folder);
			string dest = Path.Combine (destFolder, name);
			CopyFolder (folder, dest);
		}
	}
	
	public List<QuestRuntimeHotspot> getActiveHotspots ()
	{

	

		List<QuestRuntimeHotspot> activehs = new List<QuestRuntimeHotspot> ();

		if (currentquest != null && currentquest.id != 0) {

			foreach (QuestRuntimeHotspot qrh in hotspots) {
			
			
			
				if (qrh.active) {

					activehs.Add (qrh);
				}


			}
		} else {




			foreach(Quest aq in allquests){

//				Debug.Log("Quest: "+aq.name);

				if(aq.start_longitude != null && aq.start_longitude != 0f){
				QuestHotspot qh = new QuestHotspot();
				
					QuestAttribute qa = new QuestAttribute("radius","20");

					qh.attributes = new List<QuestAttribute>();
					qh.attributes.Add(qa);
					QuestRuntimeHotspot qrh = new QuestRuntimeHotspot(qh,true,true,aq.start_latitude+","+aq.start_longitude);
					//Debug.Log("Longitude Latitude: "+aq.start_longitude+","+aq.start_latitude);

					if(aq.hasMeta("category")){

						qrh.category = aq.getMeta("category");

					}

					qrh.startquest = aq;

					activehs.Add(qrh);
				}
			}


		}

		return activehs;
	}

	public GeoPosition getCenter ()
	{
		float centerLat = 0f;
		float centerLong = 0f;

		foreach (QuestRuntimeHotspot curHotspot in hotspots) {
			centerLat += curHotspot.lat;
			centerLong += curHotspot.lon;
		}

		centerLat = centerLat / hotspots.Count;
		centerLong = centerLong / hotspots.Count;

		return new GeoPosition (centerLat, centerLong);
	}

	void Update ()
	{



		if (fakebytes > 0 && fakebytes < (int.MaxValue - 1000)) {

			fakebytes += Time.deltaTime;

		}

		if (Input.GetKey (KeyCode.G) && Input.GetKey (KeyCode.E) && Input.GetKey (KeyCode.O) && Input.GetKey (KeyCode.Q)) {
			Debug.Log ("Destroying GameObject");

			Destroy (gameObject);
			endQuest ();
		

		}

	}

	public void debug (string s)
	{
		
//		Debug.Log (s);
		if (Application.isWebPlayer) {
			

				
			Application.ExternalCall ("unitydebug", s + "<br/><br/>");
				

		}
		
	}

	public void passWebXml (string x)
	{


		if (currentquestdata != null) {

			Destroy (currentquestdata.gameObject);
						
					
				
		} 

		actioncontroller.reset ();
		actioncontroller.sendVartoWeb ();
		
		if (webloadingmessage != null) {
			webloadingmessage.text = "Loading...";
			webloadingmessage.enabled = true;
			if (loadlogo != null) {

			loadlogo.enable ();	
			}
		}

		www = new WWW (x);
		StartCoroutine (waitForWebXml ());
				
	}

	public void resetPlayer (string x)
	{
		Debug.Log ("Destroying GameObject");
		Destroy (gameObject);
		Application.LoadLevel (0);

	
		
	}

	IEnumerator waitForWebXml ()
	{

		yield return www;

		if (www.error == null) {

			currentquest = new Quest ();


		
			currentquestdata = (Transform)Instantiate (questdataprefab, transform.position, Quaternion.identity);

			currentquest.xmlcontent = UTF8Encoding.UTF8.GetString (www.bytes); 
			//ASCIIEncoding.ASCII.GetString (Encoding.Convert (Encoding.UTF32, Encoding.ASCII, www.bytes)); 


			installQuest (currentquest, false, false);

		} else {
			debug (www.error);

		}
	}

	public void startQuest (Quest q)
	{

		closeMap ();
		currentquest = q;
		currentquestdata = (Transform)Instantiate (questdataprefab, transform.position, Quaternion.identity);

		Debug.Log ("Starting: " + currentquest.id);



		if (!localquests.Contains (q)) {
			Debug.Log ("Problem 2");

			downloadQuest (q);
		} else {

			installQuest (q, true, true);
		}


	}

	public void removeQuest (Quest q)
	{


		if (localquests.Contains (q)) {

						
			#if UNITY_WEBPLAYER

			Debug.Log("cannot remove on web");

			# else 
			if (Directory.Exists (Application.persistentDataPath + "/quests/" + q.id)) {
				Directory.Delete (Application.persistentDataPath + "/quests/" + q.id, true);


			}
#endif
			localquests.Remove (q);

			if(currentquest != null){
			if (currentquest.id == q.id) {

				currentquest = null;

			}
			}

		}

	
		if (buttoncontroller != null) {

			buttoncontroller.resetList ();

		}


	}

	public void endQuest ()
	{

//		Debug.Log ("what?");
		debug ("Quest beendet");

		if (currentquestdata != null) {
			Destroy (currentquestdata.gameObject);
		}
		Debug.Log ("Destroying GameObject");
		Destroy (GameObject.Find("MsgCanvas"));
		Destroy (gameObject);
		if (menu.isActive) {
			menu.endQuestAnimation ();
		} else {

			returnToMainMenu ();

		}
	}





	public void returnToMainMenu(){

		Application.LoadLevel (0);


	}

	public void retryAllOpenWWW ()
	{

		List<WWW> todelete = new List<WWW> ();
		foreach (WWW awww in filedownloads) {

			todelete.Add (awww);

		}
	

	}

	public void downloadQuest (Quest q)
	{
		if (webloadingmessage != null) {

			webloadingmessage.enabled = true;
		}

		if (questmilllogo != null) {
			questmilllogo.enabled = true;
		}
		if (loadlogo != null) {

			loadlogo.enable ();
		}

		if (Configuration.instance.showinternetconnectionmessage) {
			showmessage ("Wir empfehlen eine gute WLAN Verbindung um alle Medien zu laden.", "OK");
		}

		StartCoroutine (CheckConnection (q, 0.0f, new WWW ("http://www.google.com")));

		               
		               
	}

	void downloadAfterConnectionChecked (Quest q, bool connected)
	{
		if (connected) {
			if(webloadingmessage != null){

			webloadingmessage.text = "Lade Quest ... " + q.name;
			}
			string url = "http://www.qeevee.org:9091/editor/" + q.id + "/clientxml";
			www = new WWW (url);
			if (loadlogo != null) {

			loadlogo.enable ();
			}
			if(webloadingmessage != null){

			webloadingmessage.text = "Bitte warten ... ";
			}
			StartCoroutine (DownloadFinished (q));
		} else {
			
			
			showmessage ("Wir konnten keine Verbindung mit dem Internet herstellen.", "Nochmal versuchen");
			
			
			StartCoroutine (CheckConnection (q, 0.0f, new WWW ("http://www.google.com")));

		}

	}

	public void downloadQuest (int id)
	{
		Quest q = new Quest ();
		q.id = id;
		currentquest = q;
		downloadQuest (q);
	}




	public void downloadAsset(string url,string filename){
		
		if(wanttoload == null){ wanttoload = new List<string>(); }
		if(!wanttoload.Contains(url)){
			
			wanttoload.Add(url);
		}

		StartCoroutine (downloadAssetAsync(url, filename));

	}

	public IEnumerator downloadAssetAsync (string url, string filename)
	{

		bool done = true;
		if (filedownloads != null) {
			foreach (WWW w in filedownloads) {

				if (!w.isDone) {

					done = false;
				}


			}
		}


		if (done) {


			if (!url.Contains ("/clientxml")) {
				WWW wwwfile = new WWW (url);

				if (filedownloads == null) {
					filedownloads = new List<WWW> ();
				}
				filedownloads.Add (wwwfile);
				files_all += 1;
				StartCoroutine (downloadAssetFinished (wwwfile, filename, 0f));
			} else {
				Debug.Log ("downloadAsset() with clientxml in url-arg called");
			}

		} else {




			yield return new WaitForEndOfFrame();
		downloadAsset(url,filename);

			            
		}
	}

	public IEnumerator downloadAssetFinished (WWW wwwfile, string filename, float timeout)
	{
		yield return new WaitForSeconds (0.3f);
		timeout += 0.3f;

		if (wwwfile.error != null) {
			Debug.Log ("error downloading " + wwwfile.url + " (" + wwwfile.error + ")");
				
			if (wwwfile.error != "unsupported URL") {
				Debug.Log ("redoing www");

						downloadAsset (wwwfile.url, filename);
			}
			filedownloads.Remove (wwwfile);
			wwwfile.Dispose ();
		} else {
			if (wwwfile.isDone) {
				if (!Directory.Exists (Path.GetDirectoryName (filename))) {
						
					Debug.Log ("creating folder:" + Path.GetDirectoryName (filename));
						
					Directory.CreateDirectory (Path.GetDirectoryName (filename));
				}
				if (wwwfile == null || wwwfile.bytes == null || wwwfile.bytes.Length == 0)
					Debug.Log ("Download Problem: Empty file " + filename);
				FileStream fs = File.Create (filename);
				fs.Write (wwwfile.bytes, 0, wwwfile.size);
				fs.Close ();
				files_complete += 1;
				bytesloaded += (int)(wwwfile.bytesDownloaded);
				filedownloads.Remove (wwwfile);



				if(wanttoload.Contains(wwwfile.url)){ wanttoload.Remove(wwwfile.url); }

				wwwfile.Dispose ();


		

				performSpriteConversion (filename);




				
			} else {
				if (timeout > Configuration.instance.downloadTimeOutSeconds) {
					showmessage ("Download fehlgeschlagen.");
					Application.LoadLevel (0);
				} else 
					if (timeout > 60f && wwwfile.progress < 0.1f) {
					Debug.Log ("Error: " + wwwfile.url + " - " + timeout);

					if (!wwwfile.url.Contains ("/clientxml")) {
						Debug.Log ("redoing www");
							
						filedownloads.Remove (wwwfile);
							downloadAsset (wwwfile.url, filename);
						wwwfile.Dispose ();
					} else {
						filedownloads.Remove (wwwfile);
						wwwfile.Dispose ();
					}
				} else {
					int bytesloaded = 0;
					StartCoroutine (downloadAssetFinished (wwwfile, filename, timeout));
				}
			}
		}
	}

	public List<Quest> GetLocalQuests ()
	{

#if !UNITY_WEBPLAYER

		if (!Application.isWebPlayer) {
			localquests.Clear (); 
			DirectoryInfo info = new DirectoryInfo (Application.persistentDataPath + "/quests/");
			if (!info.Exists) {
				info.Create ();
				return localquests;
			}
			var fileInfo = info.GetDirectories ();

			foreach (DirectoryInfo folder in fileInfo) { 
				if (File.Exists (folder.ToString () + "/game.xml")) {
					Quest n = new Quest ();
					string[] splitted = folder.ToString ().Split ('/');
					n.id = int.Parse (splitted [splitted.Length - 1]);
					n.filepath = folder.ToString () + "/";
					n = n.LoadFromText (int.Parse (splitted [splitted.Length - 1]), true);
					if (n != null)
						localquests.Add (n);
				}
			}
		}

#endif

		return localquests;

	}

	public void AllowAutoRotation (bool status)
	{
		if (status == true) {
			Screen.autorotateToLandscapeLeft = true;
			Screen.autorotateToLandscapeRight = true;
			originalOrientation = Screen.orientation;
			Screen.orientation = ScreenOrientation.AutoRotation;
		} else {

			Debug.Log ("rotatin' 1");
			if (originalOrientation != null) {
				Debug.Log ("rotatin' 1.5");

				Screen.orientation = originalOrientation;
			}
			Debug.Log ("rotatin' 2");

			Screen.autorotateToLandscapeLeft = false;
			Screen.autorotateToLandscapeRight = false;
			Debug.Log ("rotatin' 3");


		}
	}

	public void installQuest (Quest q, bool reload, bool localload)
	{


		if (filedownloads != null) {
			filedownloads.Clear ();
		}
		if (loadedfiles != null) {
			loadedfiles.Clear ();
		}

		convertToSprites = true;
		currentquest = q.LoadFromText (q.id, localload);
		if (currentquest == null) {
			questmilllogo.enabled = false;
			if(webloadingmessage != null){

			webloadingmessage.enabled = false;
			}
			if (loadlogo != null) {

			loadlogo.disable ();
			}
			return;
		}

		//q.deserializeAttributes ();
//		Debug.Log ("done installing...");



		if (!localload) {

			// resave xml
			string exportLocation = Application.persistentDataPath + "/quests/" + currentquest.id + "/";
			
			
			
			#if !UNITY_WEBPLAYER

			if (!Application.isWebPlayer && (!Directory.Exists (exportLocation) || reload)) {



				if (Directory.Exists (exportLocation)) {

					Directory.Delete (exportLocation, true);

				}
				Directory.CreateDirectory (exportLocation);






			}
#endif
		}


		bool hasmorethanmetadata = true;
		currentquest.currentpage = currentquest.pages.First ();
		int c = 0;
		while (currentquest.currentpage.type == "MetaData") {

			if(currentquest.pages.Count >= c-1){
			currentquest.currentpage = currentquest.pages[c];
			c++;
			} else {

				hasmorethanmetadata = false;
				break;
			}
		}

		hotspots = new List<QuestRuntimeHotspot> ();
		foreach (QuestHotspot qh in currentquest.hotspots) {
			bool initialActivity = qh.hasAttribute ("initialActivity") && qh.getAttribute ("initialActivity") == "true";
			bool initialVisibility = qh.hasAttribute ("initialVisibility") && qh.getAttribute ("initialVisibility") == "true";

			hotspots.Add (new QuestRuntimeHotspot (qh, initialActivity, initialVisibility, qh.latlon));
		}

		if (canPlayQuest (currentquest) && hasmorethanmetadata) {



			if (Application.isWebPlayer) {



				changePage (currentquest.currentpage.id);


			} else if (!localload) {

				Debug.Log ("WAITING FOR QUEST ASSETS");
				if(webloadingmessage != null){

				webloadingmessage.text = "Lade alle Medien vor.\n Das kann einige Minuten dauern. \n ";
				}
				//webloadingmessage.enabled = true;
				if (loadlogo != null) {

				loadlogo.enable ();
				}
				StartCoroutine (waitforquestassets (currentquest.currentpage.id, 0f));
					
			} else {
				StartCoroutine (waitForSpriteConversion (currentquest.currentpage.id));

			}

		} else {
			Debug.Log ("showing message");
			showmessage ("Entschuldigung! Die Quest kann in dieser Version nicht abgespielt werden.");
			if(webloadingmessage != null){

			webloadingmessage.enabled = false;
			}
			if (loadlogo != null) {

			loadlogo.disable();
			}
			GameObject.Find ("List").GetComponent<createquestbuttons> ().resetList ();

		}

	}

	public void performSpriteConversion (string value)
	{

		if (!Application.isWebPlayer) {

			if (convertToSprites) {
				bool doit = true;

				List<SpriteConverter> redo = new List<SpriteConverter> ();
				foreach (SpriteConverter asc in convertedSprites) {

					if (asc.filename == value) {

						if (asc.isDone) {


							doit = false;

						} else {

							redo.Add (asc);
						}
					}

				}

				foreach (SpriteConverter sc in redo) {

					convertedSprites.Remove (sc);
					sc.myWWW = null;
					sc.myTexture = null;

				}


				if (doit) {
					if (File.Exists (value)) {
						FileInfo fi = new FileInfo (value);

						List<string> imageextensions = new List<string> (){".jpg",".jpeg",".gif",".png"};
						//Debug.Log (imageextensions.Count);
						//	Debug.Log (fi.Extension);
						if (imageextensions.Contains (fi.Extension.ToLower ())) {

							SpriteConverter sc = new SpriteConverter (value);



							convertedSprites.Add (sc);

							sc.startConversion ();
						//	StartCoroutine (waitForSingleSpriteCompletion (sc));
						}
					} else {

						Debug.Log ("[ATTENTION] A file didn't exist: " + value);

					}
				}
			}
		}
	}

	IEnumerator waitForSpriteConversion (int pageid)
	{


		
		bool spritesConverted = true;
		
		foreach(SpriteConverter sc in convertedSprites){
			
			if(!sc.isDone){
				spritesConverted = false;
				
			}
			
		}
		
		if(spritesConverted){
			
			Debug.Log("Converted Sprites has "+convertedSprites.Count+" objects.");
			changePage (pageid);
			
			
		} else {
			Debug.Log("STARTE");
			if(webloadingmessage != null){

			webloadingmessage.text = "Starte "+Configuration.instance.nameForQuest+"... ";
			webloadingmessage.enabled = true;
			}
			if (loadlogo != null) {

			loadlogo.enable ();
			}
			if(spriteError != null && spriteError != ""){
				if(webloadingmessage != null){

				webloadingmessage.text = spriteError;
				}
				yield return new WaitForSeconds(2f);
				Application.LoadLevel(0);

			} else {
			
			yield return new WaitForSeconds (0.2f);

			StartCoroutine (waitForSpriteConversion (pageid));
			}

		}


		yield return null;

		
		
	}





	public bool nextSpriteToBeConverted(SpriteConverter sc){


		bool me = true;

		foreach (SpriteConverter asc in convertedSprites) {


			if(asc == sc){

				break;
			} else if(asc.isDone != true){

				me = false;

				break;
			}



		}

		return me;
	}

	IEnumerator waitForSingleSpriteCompletion (SpriteConverter sc)
	{

		WWW myWWW = sc.myWWW;
		//Debug.Log ("trying to acces: " + myWWW.url);
		
		if (myWWW.url == null || myWWW.url == "") {
			Debug.Log("nothing to do");
			sc.isDone = true;
			yield return null;
			
		} else {
			
			
			yield return myWWW;
			//Debug.Log ("not done with WWW object:" + myWWW.url);
			
			if (myWWW.error != null) {
				Debug.Log ("error:" + myWWW.error);
				//TODO: error handling

				spriteError = "Fehlerhafte Datei\nBitte lade diese Quest erneut.";
				
			} else {
				//Debug.Log ("DONE with WWW object");
			


				if(nextSpriteToBeConverted(sc)){


					if (myWWW.texture != null) {

						sc.myTexture = myWWW.texture;
						sc.width = myWWW.texture.width;
						sc.height = myWWW.texture.height;
						sc.convertSprite();


					} else {
						sc.isDone = true;
						myWWW = null;
						sc.myWWW = null;
					}
				} else {
					yield return new WaitForSeconds(0.5f);
					StartCoroutine(waitForSingleSpriteCompletion(sc));

				}
			}
		}
	
	}




	IEnumerator waitforquestassets (int pageid, float timeout)
	{
		//webloadingmessage.text = "Downloading Quest Assets ... 0 %";

		if (fakebytes == 0) {
			fakebytes = 1;
		}

		timeout += 0.5f;
		yield return new WaitForSeconds (0.5f);
		bool done = true;

		int percent = 100;
		int downloadsundone = 0;


		string error = "";


		int bytesloadedbutunfinished = 0;



		if (wanttoload.Count > 0) {

			done = false;
		} else 

		if (filedownloads != null) {
			Debug.Log ("WWW Objects" + filedownloads.Count);

			foreach (WWW www in filedownloads) {

				if (!www.isDone) {
					done = false;
					downloadsundone += 1;
				}

				if (www.error != null) {



					if (www.url.StartsWith ("http")) {

						done = false;
						downloadsundone += 1;
//					Debug.Log("WWW ERROR: "+ www.error + " ("+www.url+")");

						//error += www.url +"couldn't be downloaded.";
					}
				

				}

			}





			int bytes_finished = files_complete;
			int bytes_all = files_all;


			//	percent = 100 - ((bytes_all-bytes_finished) * 100 / bytes_finished);

		} else {

			done = true;


		}
//		Debug.Log ("percent done: " + percent);

		int bytescomplete = bytesloaded;
		int filesleft = 0;

		if (filedownloads != null) {

			filesleft = filedownloads.Count;

			string openfileloads = "Open WWW Files: ";

			foreach (WWW awww in filedownloads) {
				//Debug.Log(awww.bytesDownloaded);

				//bytescomplete += (int)(awww.bytesDownloaded);

				openfileloads += awww.url + "; ";

			}

			Debug.Log (openfileloads);
		}


		if (error == "") {
			int bytesloaded2 = (int)(bytesloaded + (fakebytes * 900));
			if(webloadingmessage != null){

			webloadingmessage.text = "Lade alle Medien vor.\n Das kann einige Minuten dauern. \n " + bytesloaded2 + " Bytes geladen";
			}
		} else {
			if(webloadingmessage != null){

			webloadingmessage.text = error;
			}

		}
		if (done) {

			string exportLocation = Application.persistentDataPath + "/quests/" + currentquest.id + "/";

			if (!File.Exists (exportLocation + "game.xml")) {

				var stream = new FileStream (exportLocation + "game.xml", FileMode.Create);
				
			
				stream.Close ();
				var stream2 = new StreamWriter (exportLocation + "game.xml");
				
			
				stream2.Write (currentxml);
				
				stream2.Close ();

			}



			StartCoroutine (waitForSpriteConversion (pageid));


		} else {
//			Debug.Log ("waitforquestassets: not done yet; timeout = " + timeout);
			StartCoroutine (waitforquestassets (pageid, timeout));

		}
	}

	public bool canPlayQuest (Quest q)
	{

		bool playable = true;
		foreach (QuestPage qp in q.pages) {


			if (qp.type != "StartAndExitScreen" && 
				qp.type != "NPCTalk" && 
				qp.type != "MultipleChoiceQuestion" && 
				qp.type != "VideoPlay" && 
				qp.type != "TagScanner" && 
				qp.type != "ImageCapture" && 
				qp.type != "AudioRecord" && 
				qp.type != "TextQuestion" && 
			    qp.type != "ImageWithText" &&
			    qp.type != "Menu" &&
				qp.type != "MapOSM" &&
			    qp.type != "MetaData" &&
				qp.type != "WebPage") {



				Debug.Log ("Can't play because it includes mission of type " + qp.type);
				playable = false;
			}

			

		}
		return playable;

	}


	public QuestPage getPage (int id)
	{
		QuestPage resultpage = null;



		foreach (QuestPage qp in currentquest.pages) {
			
			
			if (qp.id == id) {

				resultpage = qp;

			}


		}


		return resultpage;

	}



	public void closeMap(){


		if (GameObject.Find ("MapCanvas") != null) {
			Destroy (GameObject.Find ("MapCanvas"));
		}
		if (GameObject.Find ("PageController_Map") != null) {
			Destroy (GameObject.Find ("PageController_Map"));
		}
		if (GameObject.Find ("MapCam") != null) {
			Destroy (GameObject.Find ("MapCam"));
		}
		if (GameObject.Find ("[Map]") != null) {
			Destroy (GameObject.Find ("[Map]"));
		}

			
				

				

	}

	public void changePage (int id)
	{



	
		
		Debug.Log ("Changing page to " + id);


		if (GameObject.Find ("MapHider") != null) {


			GameObject.Find ("MapHider").GetComponent<Image> ().enabled = true;
		}

		if (GameObject.Find ("MapCam") != null) {
			
			GameObject.Find ("MapCam").GetComponent<Camera> ().enabled = false;

			GameObject.Find ("MapCam").GetComponent<AudioListener> ().enabled = false;
		}


		


		if (GameObject.Find ("BgCam")) {
			GameObject.Find ("BgCam").GetComponent<Camera> ().enabled = true;

			GameObject.Find ("BgCam").GetComponent<AudioListener> ().enabled = true;

		}


		if (GameObject.Find ("[Map]")) {

			GameObject.Find ("[Map]").GetComponent<mapdisplaytoggle>().hideMap();
		}


		foreach (QuestPage qp in currentquest.pages) {
		




			if (qp.id == id) {

				currentquest.currentpage = qp;



				currentquest.currentpage.state = "running";

			
				//GameObject.Find("BgCam").GetComponent<Camera>().enabled = true;

				
				GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject> ();



				foreach (GameObject go in allObjects)
					if (go != null && go.transform != null && go.name != "MapCanvas" && go.name != "PageController_Map" && go.name != "QuestDatabase" && go.name != "MsgCanvas"
					    && go.name != "ImpressumCanvas" && !go.transform.IsChildOf (GameObject.Find ("ImpressumCanvas").transform) && go.name != "MenuCanvas" && go.name != "EventSystem"
						&& go.name != "Configuration" && go.name != "MapCam" && go.name != "[Map]" && go.name != "[location marker]"
						&& go.name != "" && !go.name.Contains ("[Tile") && go.name != "EventSystem_Map" && go.name != "BgCam" && go.name != "QuestData(Clone)") {

						

						bool des = true;


					if (GameObject.Find ("MenuCanvas") != null) {
						
						if (go.transform.IsChildOf (GameObject.Find ("MenuCanvas").transform)) {
							des = false;
							//Debug.Log("is child of mapcanvas");
						}
						
					}
						if (GameObject.Find ("MapCanvas") != null) {

							if (go.transform.IsChildOf (GameObject.Find ("MapCanvas").transform)) {
								des = false;
								//Debug.Log("is child of mapcanvas");
							}

						}

						if (GameObject.Find ("[Map]")) {

							if (go.transform.IsChildOf (GameObject.Find ("[Map]").transform)) {

								des = false;

							}
							
							
							
						}

						if (GameObject.Find ("[location marker]")) {
						
							if (go.transform.IsChildOf (GameObject.Find ("[location marker]").transform)) {
							
								des = false;
							
							}
						
						
						
						}

						if (GameObject.Find ("PageController_Map")) {

							if (go == GameObject.Find ("PageController_Map").GetComponent<page_map> ().map) {

								des = false;

							}

						}

						if (des) {
//							Debug.Log ("destroying: " + go.name);
							Destroy (go);
						}

					}

			//	Debug.Log ("Resources GameObject # =" + Resources.FindObjectsOfTypeAll (typeof(GameObject)).Count ());
			//	Debug.Log ("Resources Sprite # =" + Resources.FindObjectsOfTypeAll (typeof(Sprite)).Count ());
				Resources.UnloadUnusedAssets ();


				//if(GameObject.Find("MapCam") != null){
				//GameObject.Find("MapCam").GetComponent<Camera>().enabled = false;
				//}

				bool needsCamera = false;

				if(!menu.isActive){
					menu.showTopBar();
				}
				
				if (qp.type == "NPCTalk") {
					Application.LoadLevelAdditive (1);
				} else if (qp.type == "ImageWithText") {
					Application.LoadLevelAdditive (1);
				} else if (qp.type == "StartAndExitScreen") {
					Application.LoadLevelAdditive (2);

				} else if (qp.type == "MultipleChoiceQuestion") {
					Application.LoadLevelAdditive (3);
				} else if (qp.type == "Menu") {
					Application.LoadLevelAdditive (3);
					
				} else if (qp.type == "VideoPlay") {
					Application.LoadLevelAdditive (4);

				} else if (qp.type == "TagScanner") {
					needsCamera = true;
					Application.LoadLevelAdditive (5);
					
				} else if (qp.type == "ImageCapture") {
					needsCamera = true;

					Application.LoadLevelAdditive (6);
					
				} else if (qp.type == "TextQuestion") {
					Application.LoadLevelAdditive (7);
				} else if (qp.type == "AudioRecord") {
					needsCamera = true;
					Application.LoadLevelAdditive (8);
				} else if (qp.type == "WebPage") {
					Application.LoadLevelAdditive (10);
				} else if (qp.type == "MapOSM") {




					if (GameObject.Find ("MapCam") == null) {


						StartCoroutine (loadMap ());

					} else {


						

						if (GameObject.Find ("MapHider") != null) {

							GameObject.Find ("MapHider").GetComponent<Image> ().enabled = false;
						}

						if (GameObject.Find ("BgCam")) {
							GameObject.Find ("BgCam").GetComponent<Camera> ().enabled = false;

							GameObject.Find ("BgCam").GetComponent<AudioListener> ().enabled = false;
							
						}

						if (GameObject.Find ("MapCanvas") != null) {
							
							GameObject.Find ("MapCanvas").GetComponent<Canvas> ().enabled = true;
						}

						GameObject.Find ("MapCam").GetComponent<Camera> ().enabled = true;
						GameObject.Find ("MapCam").GetComponent<AudioListener> ().enabled = true;

						if (GameObject.Find ("[Map]")) {
							
							GameObject.Find ("[Map]").GetComponent<mapdisplaytoggle>().showMap();
						}

					}
				}



				if(needsCamera){
					if (GameObject.Find ("MapCanvas") != null) {
						Debug.Log("Disabling Map Canvas");
						GameObject.Find ("MapCanvas").GetComponent<Canvas> ().enabled = false;
					}

					Debug.Log("needs Camera");
					GameObject.Find("BgCam").GetComponent<Camera>().enabled = false;
					if(GameObject.Find ("MapCam") != null){
					GameObject.Find ("MapCam").GetComponent<Camera> ().enabled = false;
						GameObject.Find ("MapCam").GetComponent<AudioListener> ().enabled = false;

					}
					GameObject.Find("BgCam").GetComponent<AudioListener>().enabled = false;

				}
				
				//GameObject.Find("BgCam").GetComponent<Camera>().enabled = false;

				
			}
		
		}
		

	}

	IEnumerator loadMap ()
	{
		AsyncOperation async = Application.LoadLevelAdditiveAsync (9);
		
	
		yield return async;
		if (GameObject.Find ("BgCam") != null) {
			
			GameObject.Find ("BgCam").GetComponent<Camera> ().enabled = false;
			
			
		}
		if (GameObject.Find ("MapCam") != null) {
			
			GameObject.Find ("MapCam").GetComponent<Camera> ().enabled = true;
			
			
		}
	}
	
	public void showmessage (string text)
	{
		Debug.Log ("MSGSActive before:" + msgsactive);

		msgsactive += 1;
		Debug.Log ("MSGSActive after:" + msgsactive);

		QuestMessage nqa = (QuestMessage)Instantiate (message_prefab, transform.position, Quaternion.identity);
			
		nqa.message = text;

		nqa.transform.SetParent (GameObject.Find ("Canvas").transform, false);
		nqa.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, 0f);

		
	}

	public void showmessage (string text, string button)
	{
		Debug.Log ("MSGSActive before:" + msgsactive);

		msgsactive += 1;
		Debug.Log ("MSGSActive after:" + msgsactive);

		QuestMessage nqa = (QuestMessage)Instantiate (message_prefab, transform.position, Quaternion.identity);
		
		nqa.message = text;
		nqa.setButtonText (button);
		nqa.transform.SetParent (GameObject.Find ("Canvas").transform, false);
		nqa.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, 0f);



	}

	public QuestRuntimeHotspot getHotspot (string str)
	{

		QuestRuntimeHotspot qh = null;
	
		foreach (QuestRuntimeHotspot qrh in hotspots) {



			if (qrh.hotspot.id == int.Parse (str)) {

				qh = qrh;


			}



		}

		return qh;

	}

	IEnumerator DownloadFinished (Quest q)
	{
		if (webloadingmessage != null) {

			webloadingmessage.enabled = true;
		}
		if (loadlogo != null) {

			loadlogo.enable ();
		}
		localquests.Add (q);
		yield return www;
		if (www.error == null) {



			if(webloadingmessage != null){

			webloadingmessage.text = "Bitte warten ...";
			}

			currentquest = new Quest ();
				
			currentquest.id = q.id;
				
			currentquestdata = (Transform)Instantiate (questdataprefab, transform.position, Quaternion.identity);
				
			currentquest.xmlcontent = UTF8Encoding.UTF8.GetString (www.bytes); 
			currentxml = UTF8Encoding.UTF8.GetString (www.bytes); 
			Debug.Log ("XML:" + currentxml);
			bool b = false;



			foreach (Quest lq in localquests) {
				if (lq.id == q.id) {

					b = true;
				}
			}

//				Debug.Log(q.id+","+b);

			installQuest (currentquest, b, false);

				
		
			
		} else {
			Debug.Log ("WWW Error: " + www.error);
			if(webloadingmessage != null){

			webloadingmessage.text = www.error;
			}

		}  

		//webloadingmessage.enabled = false;
		
		
	}


	

}

[System.Serializable]
[XmlRoot("game")]
public class Quest  : IComparable<Quest>
{
	
	[XmlAttribute("name")]
	public string
		name;
	[XmlAttribute("id")]
	public int
		id;
	[XmlAttribute("xmlformat")]
	public int
		xmlformat;
	public string filepath;
	[XmlElement("mission")]
	public List<QuestPage>
		pages;
	[XmlElement("hotspot")]
	public List<QuestHotspot>
		hotspots;
	[XmlAnyAttribute()]
	public XmlAttribute[]
	help_attributes;
	public List<QuestAttribute> attributes;

	public List<QuestMetaData> metadata;

	public bool hasData = false;
	public QuestPage currentpage;
	public List<QuestPage> previouspages;
	public string xmlcontent;
	public float start_longitude;
	public float start_latitude;
	public string meta_combined;

	public bool predeployed = false;

	public Quest ()
	{
		predeployed = false;

	}

	public static Quest CreateQuest (int id)
	{
		Quest q = new Quest ();
		return q.LoadFromText (id, true);
	}

	public string getCategory(){


		string x = "";

		if (hasMeta ("category")) {

			x = getMeta("category");

		}


		return x;


	}

	public int CompareTo (Quest q)
	{



		if (q == null) {
			return 1;
		} else {

			return this.name.ToUpper ().CompareTo (q.name.ToUpper ());
		}

	}

	public  Quest LoadFromText (int id, bool redo)
	{
	
		string fp = filepath;
		string xmlfilepath = filepath;
		string xmlcontent_copy = xmlcontent;

		if (xmlcontent_copy != null && xmlcontent_copy.StartsWith ("<error>")) {
			string errMsg = xmlcontent_copy;

			GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().showmessage (errMsg);
			return null;
		}

		if (filepath == null) {
			xmlfilepath = " ";

		}

		if (xmlcontent_copy == null) {

			xmlcontent_copy = " ";
		}



		Encoding enc = System.Text.Encoding.UTF8;

		
		TextReader txr = new StringReader (xmlcontent_copy);

//		Debug.Log ("XML:"+xmlcontent_copy);



		if (!predeployed && xmlfilepath != null && xmlfilepath.Length > 9) {

//			Debug.Log(xmlfilepath);


			if(!xmlfilepath.Contains("game.xml")){

				xmlfilepath = xmlfilepath+"game.xml";

			}
			txr = new StreamReader (xmlfilepath, enc);

		}
		XmlSerializer serializer = new XmlSerializer (typeof(Quest));

		Quest q = serializer.Deserialize (txr) as Quest; 
	
		q.predeployed = predeployed;



		q.filepath = fp;
		q.hasData = true;
	
		q.id = id;
//		Debug.Log ("my id is " + id + " -> " + q.id);
		q.deserializeAttributes (redo);




		q.meta_combined += q.name;

		if (metadata != null) {

			metadata.Clear ();
		} else {

			metadata = new List<QuestMetaData>();
		}






		if (q.hasAttribute ("author")) {

			q.addMetaData(new QuestMetaData("author",q.getAttribute("author")));

		}

		if (q.hasAttribute ("version")) {
			
			q.addMetaData(new QuestMetaData("version",q.getAttribute("version")));
			
		}



		foreach(QuestPage qp in q.pages){

		if(qp.type == "MetaData"){

				foreach(QuestContent qc in qp.contents_stringmeta){

					if(qc.hasAttribute("key") && qc.hasAttribute("value")){
						QuestMetaData newmeta = new QuestMetaData();
						newmeta.key = qc.getAttribute("key");
						newmeta.value = qc.getAttribute("value");
						q.addMetaData(newmeta);

					}

				}


			}

		}





		return q;
	}


	public void addMetaData (QuestMetaData meta)
	{


		string key = meta.key;


		List<QuestMetaData> todelete = new List<QuestMetaData> ();

		if (metadata == null) {


			metadata = new List<QuestMetaData>();

		} else {
			foreach (QuestMetaData qmd in metadata) {

				if (qmd.key == key) {
					todelete.Add (qmd);
				}

			}


			foreach (QuestMetaData qmd in todelete) {
				metadata.Remove (qmd);
			}

		}

			metadata.Add (meta);


		meta_combined += ";" + meta.value;


		questdatabase questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();

		if (!questdb.allmetakeys.Contains (meta.key)) {

			questdb.allmetakeys.Add(meta.key);
		}

	}



	public void deserializeAttributes (bool redo)
	{


		attributes = new List<QuestAttribute> ();
		
		if (help_attributes != null) {
			foreach (XmlAttribute xmla in help_attributes) {
				
				
				
				
				if (xmla.Value.StartsWith ("http://") || xmla.Value.StartsWith ("https://")) {
					
					
					string[] splitted = xmla.Value.Split ('/');
					
					
					questdatabase questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
					
					
					string filename = "files/" + splitted [splitted.Length - 1];
					
					int i = 0;
					while (questdb.loadedfiles.Contains(filename)) {
						i++;
						filename = "files/" + i + "_" + splitted [splitted.Length - 1];
						
					}
					
					questdb.loadedfiles.Add (filename);
					
					
					
					
					if (!Application.isWebPlayer) {
						
						
						
						if (!redo) {
							questdb.downloadAsset (xmla.Value, Application.persistentDataPath + "/quests/" + id + "/" + filename);
						}
						if (splitted.Length > 3) {


							if(predeployed){
								xmla.Value = questdb.PATH_2_PREDEPLOYED_QUESTS +"/" + id + "/" + filename;

							} else {
							
							xmla.Value = Application.persistentDataPath + "/quests/" + id + "/" + filename;
							
							}
							questdb.performSpriteConversion (xmla.Value);
							
						}
					}
					
					
				}	
				
				attributes.Add (new QuestAttribute (xmla.Name, xmla.Value));
				
				
			}
		}

		if (pages != null) {
			foreach (QuestPage qp in pages) {
				qp.deserializeAttributes (id, redo);
			}
		} else {

			Debug.Log ("no pages");
		}
		if (hotspots != null) {

			foreach (QuestHotspot qh in hotspots) {
				qh.deserializeAttributes (id, redo);
			}
		}

	}

	public string getAttribute (string k)
	{
		
		foreach (QuestAttribute qa in attributes) {
			
			
			if (qa.key.Equals (k)) {
				return qa.value;
			}
			
		}
		
		
		return "";
		
	}


	public string getMeta (string k)
	{
		if (metadata != null) {
			foreach (QuestMetaData qa in metadata) {

			
				if (qa.key.Equals (k)) {
					return qa.value;
				}
			
			}
		}
		
		return "";
		
	}

	public string getMetaComparer (string k)
	{
		if (metadata != null) {
			foreach (QuestMetaData qa in metadata) {
				
				
				if (qa.key.Equals (k)) {
					return qa.value;
				}
				
			}
		}
		
		return ((char)0xFF).ToString();
		
	}



	public bool hasMeta(string k){

		bool h = false;
		if (metadata != null) {
			foreach (QuestMetaData qa in metadata) {
			
				if (qa.key != null) {
					if (qa.key.Equals (k)) {
						h = true;
					}
				}
			}
		}
		
		return h;


	}
	public bool hasAttribute (string k)
	{
		
		bool h = false;
		foreach (QuestAttribute qa in attributes) {
			
			
			if (qa.key.Equals (k)) {
				h = true;
			}
			
		}
		
		
		return h;
		
	}
	
}



[System.Serializable]
public class QuestMetaData
{

	public string key;
	public string value;


	public QuestMetaData(){}
	public QuestMetaData(string k,string v){
		key = k;
		value = v;
	}
}




[System.Serializable]
public class QuestPage
{


	[XmlAttribute("id")]
	public int
		id;
	[XmlAttribute("type")]
	public string
		type;
	[XmlAnyAttribute()]
	public XmlAttribute[]
		help_attributes;
	public List<QuestAttribute> attributes;
	[XmlElement("dialogitem")]
	public List<QuestContent>
		contents_dialogitems;
	[XmlElement("expectedCode")]
	public List<QuestContent>
		contents_expectedcode;
	[XmlElement("answer")]
	public List<QuestContent>
		contents_answers;
	[XmlElement("question")]
	public QuestContent
		contents_question;
	[XmlElement("answers")]
	public List<QuestContent>
		contents_answersgroup;
	[XmlElement("stringmeta")]
	public List<QuestContent>
		contents_stringmeta;
	[XmlElement("onEnd")]
	public QuestTrigger
		onEnd;
	[XmlElement("onStart")]
	public QuestTrigger
		onStart;
	[XmlElement("onTap")]
	public QuestTrigger
		onTap;
	[XmlElement("onSuccess")]
	public QuestTrigger
		onSuccess;
	[XmlElement("onFail")]
	public QuestTrigger
		onFailure;
	public string state;
	public string result;

	public QuestPage ()
	{

		state = "new";
		result = null;
	}

	public string getAttribute (string k)
	{
		
		foreach (QuestAttribute qa in attributes) {


			if (qa.key.Equals (k)) {
				return qa.value;
			}
			
		}


		return "";
		
	}

	public bool hasAttribute (string k)
	{

		bool h = false;
		foreach (QuestAttribute qa in attributes) {
			
			
			if (qa.key.Equals (k)) {
				h = true;
			}
			
		}
		
		
		return h;
		
	}
	
	public void deserializeAttributes (int id, bool redo)
	{

		questdatabase questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();

		if (questdb.currentquest != null) {
			attributes = new List<QuestAttribute> ();

			if (help_attributes != null) {
				foreach (XmlAttribute xmla in help_attributes) {



							
					if (xmla.Value.StartsWith ("http://") || xmla.Value.StartsWith ("https://") && !(type == "WebPage" && xmla.Name.ToLower () == "url")) {


						string[] splitted = xmla.Value.Split ('/');




						string filename = "files/" + splitted [splitted.Length - 1];

						int i = 0;
						while (questdb.loadedfiles.Contains(filename)) {
							i++;
							filename = "files/" + i + "_" + splitted [splitted.Length - 1];

						}

						questdb.loadedfiles.Add (filename);




						if (!Application.isWebPlayer) {

					
				
							if (!redo) {
								questdb.downloadAsset (xmla.Value, Application.persistentDataPath + "/quests/" + id + "/" + filename);
							}
							if (splitted.Length > 3) {

								if (questdb.currentquest.predeployed) {
									Debug.Log ("is predeployed file: " + filename);

									xmla.Value = questdb.PATH_2_PREDEPLOYED_QUESTS + "/" + id + "/" + filename;
								
								} else {
								
									xmla.Value = Application.persistentDataPath + "/quests/" + id + "/" + filename;
								
								}						
								questdb.performSpriteConversion (xmla.Value);

							}
						}


					}	
								
					attributes.Add (new QuestAttribute (xmla.Name, xmla.Value));

			
				}
			}


			foreach (QuestContent qcdi in contents_dialogitems) {
				qcdi.deserializeAttributes (id, redo);
			}

			foreach (QuestContent qcdi in contents_answers) {
				qcdi.deserializeAttributes (id, redo);
			}

			if (contents_question != null) {
				contents_question.deserializeAttributes (id, redo);
			}
			foreach (QuestContent qcdi in contents_answersgroup) {
				qcdi.deserializeAttributes (id, redo);
			}


			foreach (QuestContent qcdi in contents_stringmeta) {
				qcdi.deserializeAttributes (id, redo);
			}
			

			foreach (QuestContent qcdi in contents_expectedcode) {
				qcdi.deserializeAttributes (id, redo);
			}

			if (onEnd != null) {
				foreach (QuestAction qa in onEnd.actions) {
					qa.deserializeAttributes (id, redo);
				}
			}
			if (onStart != null) {
				foreach (QuestAction qa in onStart.actions) {
					qa.deserializeAttributes (id, redo);
				}
			}
			if (onTap != null) {
				foreach (QuestAction qa in onTap.actions) {
					qa.deserializeAttributes (id, redo);
				}
			}
			if (onSuccess != null) {
				foreach (QuestAction qa in onSuccess.actions) {
					qa.deserializeAttributes (id, redo);
				}
			}
			if (onFailure != null) {
				foreach (QuestAction qa in onFailure.actions) {
					qa.deserializeAttributes (id, redo);
				}
			}
		}
	}

}

[System.Serializable]
public class QuestHotspot
{


	[XmlAttribute("id")]
	public int
		id;
	[XmlAttribute("latlong")]
	public string
		latlon;
	[XmlAnyAttribute()]
	public XmlAttribute[]
		help_attributes;
	public List<QuestAttribute> attributes;
	[XmlElement("onEnter")]
	public QuestTrigger
		onEnter;
	[XmlElement("onLeave")]
	public QuestTrigger
		onLeave;
	[XmlElement("onTap")]
	public QuestTrigger
		onTap;


	public int startquest = 0;

	public string getAttribute (string k)
	{
		if (attributes != null) {
			foreach (QuestAttribute qa in attributes) {
			
			
				if (qa.key.Equals (k)) {
					return qa.value;
				}
			
			}
		}
		
		return "";
		
	}
	
	public bool hasAttribute (string k)
	{
		
		bool h = false;
		foreach (QuestAttribute qa in attributes) {
			
			
			if (qa.key.Equals (k)) {
				h = true;
			}
			
		}
		
		
		return h;
		
	}

	public void deserializeAttributes (int id, bool redo)
	{
		
		attributes = new List<QuestAttribute> ();
		
		if (help_attributes != null) {
			foreach (XmlAttribute xmla in help_attributes) {
				
				if (xmla.Value.StartsWith ("http://") || xmla.Value.StartsWith ("https://")) {
								
								
					string[] splitted = xmla.Value.Split ('/');
								
								
					questdatabase questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
								
								
					string filename = "files/" + splitted [splitted.Length - 1];
								
					int i = 0;
					while (questdb.loadedfiles.Contains(filename)) {
						i++;
						filename = "files/" + i + "_" + splitted [splitted.Length - 1];
									
					}
								
					questdb.loadedfiles.Add (filename);
								
								
								
								
					if (!Application.isWebPlayer) {
									
									
									
						if (!redo) {
							questdb.downloadAsset (xmla.Value, Application.persistentDataPath + "/quests/" + id + "/" + filename);
						}
						if (splitted.Length > 3) {
										
							if(questdb != null && questdb.currentquest != null &&  questdb.currentquest.predeployed){
								xmla.Value = questdb.PATH_2_PREDEPLOYED_QUESTS +"/" + id + "/" + filename;
								
							} else {
								
								xmla.Value = Application.persistentDataPath + "/quests/" + id + "/" + filename;
								
							}											questdb.performSpriteConversion (xmla.Value);

						}							

					}
								
								
				}

				attributes.Add (new QuestAttribute (xmla.Name, xmla.Value));
				
				
			}
		}
		if (onEnter != null) {
			foreach (QuestAction qa in onEnter.actions) {
				qa.deserializeAttributes (id, redo);
			}
		}
		if (onLeave != null) {
			foreach (QuestAction qa in onLeave.actions) {
				qa.deserializeAttributes (id, redo);
			}
		}
		if (onTap != null) {
			foreach (QuestAction qa in onTap.actions) {
				qa.deserializeAttributes (id, redo);
			}
		}
		
		
		
		
	}
	
}

[System.Serializable]
public class QuestContent
{

	[XmlAttribute("id")]
	public int
		id;
	[XmlText()]
	public string
		content;
	[XmlAnyAttribute()]
	public XmlAttribute[]
		help_attributes;
	public List<QuestAttribute> attributes;
	[System.NonSerialized()]
	[XmlElement("questiontext")]
	public QuestContent
		questiontext;
	[System.NonSerialized()]
	[XmlElement("answer")]
	public List<QuestContent>
		answers;

	public string getAttribute (string k)
	{
		
		foreach (QuestAttribute qa in attributes) {
			if (qa.key.Equals (k)) {
				return qa.value;
			}
		}
		return "";
	}

	
	public bool hasAttribute (string k)
	{
		
		bool h = false;
		foreach (QuestAttribute qa in attributes) {
			
			
			if (qa.key.Equals (k)) {
				h = true;
			}
			
		}
		
		
		return h;
		
	}

	public void deserializeAttributes (int id, bool redo)
	{

		foreach (QuestContent qcdi in answers) {
			qcdi.deserializeAttributes (id, redo);
		}


		if (questiontext != null) {
			questiontext.deserializeAttributes (id, redo);
		}
		
		attributes = new List<QuestAttribute> ();
		
		if (help_attributes != null) {
			foreach (XmlAttribute xmla in help_attributes) {



				if (xmla.Value.StartsWith ("http://") || xmla.Value.StartsWith ("https://")) {
									
									
					string[] splitted = xmla.Value.Split ('/');
									
									
					questdatabase questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
									
									
					string filename = "files/" + splitted [splitted.Length - 1];
									
					int i = 0;
					while (questdb.loadedfiles.Contains(filename)) {
						i++;
						filename = "files/" + i + "_" + splitted [splitted.Length - 1];
										
					}
									
					questdb.loadedfiles.Add (filename);
									
									
									
									
					if (!Application.isWebPlayer) {
										
										

						if (!redo) {
							questdb.downloadAsset (xmla.Value, Application.persistentDataPath + "/quests/" + id + "/" + filename);
						}
						if (splitted.Length > 3) {
											
							if(questdb.currentquest.predeployed){
								
								xmla.Value = questdb.PATH_2_PREDEPLOYED_QUESTS +"/" + id + "/" + filename;
								
							} else {
								
								xmla.Value = Application.persistentDataPath + "/quests/" + id + "/" + filename;
								
							}											questdb.performSpriteConversion (xmla.Value);

						}
					}
									
									
				}
				
				attributes.Add (new QuestAttribute (xmla.Name, xmla.Value));
				
				
			}
		}



		
		
	}

}

[System.Serializable]
public class QuestValue
{



}

[System.Serializable]
public class QuestVariableValue
{
	

	[XmlElement("string")]
	public List<string>
		string_value;
	[XmlElement("num")]
	public List<float>
		num_value;
	[XmlElement("boolean")]
	public List<bool>
		bool_value;
	[XmlElement("var")]
	public List<string>
		var_value;

	
	
}

[System.Serializable]
public class QuestAction
{


	[XmlAttribute("type")]
	public string
		type;
	[XmlElement("value")]
	public QuestVariableValue
		value;
	[XmlAnyAttribute()]
	public XmlAttribute[]
		help_attributes;
	public List<QuestAttribute> attributes;
	[System.NonSerialized()]
	[XmlArray("then"),XmlArrayItem("action")]
	public List<QuestAction>
		thenactions;
	[System.NonSerialized()]
	[XmlArray("else"),XmlArrayItem("action")]
	public List<QuestAction>
		elseactions;
	[XmlElement("condition")]
	public QuestConditionGrouper
		condition;

	public string getAttribute (string k)
	{
		
		foreach (QuestAttribute qa in attributes) {
			
			
			if (qa.key.Equals (k)) {
				return qa.value;
			}
			
		}
		
		
		return "";
		
	}

	public bool hasMissionAction ()
	{

		bool b = false;



		if (type == "StartMission") {
			b = true;
		} else if (thenactions.Count > 0 || elseactions.Count > 0) {


			foreach (QuestAction qa in thenactions) {
				if (qa.hasMissionAction ()) {
					b = true;
				} 
			}
			foreach (QuestAction qa in elseactions) {
				if (qa.hasMissionAction ()) {
					b = true;
				} 
			}

		}

				

		return b;
	}

	public bool hasAttribute (string k)
	{
		
		bool h = false;
		foreach (QuestAttribute qa in attributes) {
			
			
			if (qa.key.Equals (k)) {
				h = true;
			}
			
		}
		
		
		return h;
		
	}

	public void Invoke ()
	{

		//Debug.Log (type);
		GameObject.Find ("QuestDatabase").GetComponent<actions> ().doAction (this);

	}

	public void InvokeThen ()
	{
		
		
		if (thenactions != null && thenactions.Count > 0) {
			foreach (QuestAction qa in thenactions) {
				
				qa.Invoke ();
				
			}
		}
		
		
	}

	public void InvokeElse ()
	{
		
		
		if (elseactions != null && thenactions.Count > 0) {
			foreach (QuestAction qa in elseactions) {
				
				qa.Invoke ();
				
			}
		}
		
		
	}

	public void deserializeAttributes (int id, bool redo)
	{

		attributes = new List<QuestAttribute> ();
		
		if (help_attributes != null) {
			foreach (XmlAttribute xmla in help_attributes) {
				

				if (xmla.Value.StartsWith ("http://") || xmla.Value.StartsWith ("https://")) {
									
									
					string[] splitted = xmla.Value.Split ('/');
									
									
					questdatabase questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
									
									
					string filename = "files/" + splitted [splitted.Length - 1];
									
					int i = 0;
					while (questdb.loadedfiles.Contains(filename)) {
						i++;
						filename = "files/" + i + "_" + splitted [splitted.Length - 1];
										
					}
									
					questdb.loadedfiles.Add (filename);
									
									
									
									
					if (!Application.isWebPlayer) {
										
										
										
						if (!redo) {
							questdb.downloadAsset (xmla.Value, Application.persistentDataPath + "/quests/" + id + "/" + filename);
						}
						if (splitted.Length > 3) {
											
							if(questdb.currentquest.predeployed){
								
								xmla.Value = questdb.PATH_2_PREDEPLOYED_QUESTS +"/" + id + "/" + filename;
								
							} else {
								
								xmla.Value = Application.persistentDataPath + "/quests/" + id + "/" + filename;
								
							}											questdb.performSpriteConversion (xmla.Value);

						}
					}
									
									
				}

				attributes.Add (new QuestAttribute (xmla.Name, xmla.Value));
				
				
			}
		}
		


		foreach (QuestAction qa in thenactions) {
			qa.deserializeAttributes (id, redo);
		}

		foreach (QuestAction qa in elseactions) {
			qa.deserializeAttributes (id, redo);
		}

	}
	
	

}

[System.Serializable]
public class QuestConditionGrouper
{


	[XmlElement("and")]
	public QuestConditionGrouper
		and;
	[XmlElement("or")]
	public QuestConditionGrouper
		or;
	[XmlElement("not")]
	public QuestConditionGrouper
		not;
	[XmlElement("lt")]
	public List<QuestConditionComparer>
		lt;
	[XmlElement("gt")]
	public List<QuestConditionComparer>
		gt;
	[XmlElement("leq")]
	public List<QuestConditionComparer>
		leq;
	[XmlElement("geq")]
	public List<QuestConditionComparer>
		geq;
	[XmlElement("eq")]
	public List<QuestConditionComparer>
		eq;

	public bool isfullfilled ()
	{

		return isfullfilled ("and");

	}

	public bool isfullfilled (string type)
	{


		List<bool> allbools = new List<bool> ();

		if (and != null) {
			allbools.Add (and.isfullfilled ("and"));
		}

		if (or != null) {
			allbools.Add (or.isfullfilled ("or"));
		}

		if (not != null) {
			allbools.Add (not.isfullfilled ("not"));
		}

		if (eq != null) {

			foreach (QuestConditionComparer qcc in eq) {

				allbools.Add (qcc.isFullfilled ("eq"));

			}

		}

		if (lt != null) {
				
			foreach (QuestConditionComparer qcc in lt) {
					
				allbools.Add (qcc.isFullfilled ("lt"));
					
			}

		}

		if (gt != null) {
				
			foreach (QuestConditionComparer qcc in gt) {
					
				allbools.Add (qcc.isFullfilled ("gt"));
					
			}
				
		}

		if (leq != null) {
					
			foreach (QuestConditionComparer qcc in leq) {
						
				allbools.Add (qcc.isFullfilled ("leq"));
						
			}
					
		}
				
		if (geq != null) {
					
			foreach (QuestConditionComparer qcc in geq) {
						
				allbools.Add (qcc.isFullfilled ("geq"));
						
			}
					
		}
	
			
		if (allbools.Count > 0) {

			if (type == "and") {

				bool ands = true;

				foreach (bool b in allbools) {

					if (!b) {
						ands = false;
					}

				}

				return ands;


			} else if (type == "or") {

				bool ors = false;
						
				foreach (bool b in allbools) {
							
					if (b) {
						ors = true;
					}
							
				}
						
				return ors;

			} else if (type == "not") {
						
				bool nots = true;
						
				foreach (bool b in allbools) {
							
					if (!b) {
						nots = false;
					}
							
				}
						
				return nots;
						
			}

		} 


			



		return true;

	}






}

[System.Serializable]
public class QuestConditionComparer
{

	[XmlElement("string")]
	public List<string>
		string_value;
	[XmlElement("num")]
	public List<float>
		num_value;
	[XmlElement("boolean")]
	public List<bool>
		bool_value;
	[XmlElement("var")]
	public List<string>
		var_value;

	public bool isFullfilled (string type)
	{



		if (type == "eq") {

			if (stringcomponents ().Count > 1) {
				bool equals = true;
				string last = null;
				foreach (string current in stringcomponents()) {

					if (last == null) {
						last = current;
					} else {

						if (current != last) {

							equals = false;

						}

					}

				}

				return equals;
			} else {

				return false;

			}


		} else  if (type == "lt") {



			if (intcomponents ().Count > 1) {

				bool lessthan = true;
				int last = int.MinValue;

				foreach (int i in intcomponents()) {

					if (last >= i) {
						lessthan = false;
					}
											
									


					last = i;

				}


				return lessthan;


			} else {

				return false;

			}

		} else  if (type == "leq") {
					
					
					
			if (intcomponents ().Count > 1) {
						
				bool lessthan = true;
				int last = int.MinValue;
						
				foreach (int i in intcomponents()) {
							
					if (last > i) {
						lessthan = false;
					}
							
							
							
							
					last = i;
							
				}
						
						
				return lessthan;
						
				
			} else {
				
				return false;
				
			}
			


		} else  if (type == "gt") {




			if (intcomponents ().Count > 1) {
							
				bool greaterthan = true;
				int last = int.MaxValue;
							
							
				foreach (int i in intcomponents()) {
								
									
					if (last <= i) {
						greaterthan = false;
					}
									
								
								
								
					last = i;
								
				}
							
							
				return greaterthan;
							
							
			} else {
							
				return false;
							
			}


		} else  if (type == "geq") {
			
			
			
			
			if (intcomponents ().Count > 1) {
				
				bool greaterthan = true;
				int last = int.MaxValue;
				
				
				foreach (int i in intcomponents()) {
					
					
					if (last < i) {
						greaterthan = false;
					}
					
					
					
					
					last = i;
					
				}
				
				
				return greaterthan;
				
				
			} else {
				
				return false;
				
			}
			

				
				
		} else {

			return false;

		}
	
	}

	public List<float> intcomponents ()
	{

		List<float> comp = new List<float> ();



		if (var_value != null) {


			foreach (string s in var_value) {

				QuestVariable qv = GameObject.Find ("QuestDatabase").GetComponent<actions> ().getVariable (s);

				if (qv != null) {
					if (qv.num_value != null && qv.num_value.Count > 0) {
						comp.Add (qv.num_value [0]);
					}
				}

			}




		} 

		if (num_value != null) {

			comp.AddRange (num_value);
		} 


		return comp;
	}

	public List<string> stringcomponents ()
	{
		
		List<string> comp = new List<string> ();

		if (string_value != null) {
			comp.AddRange (string_value);
		}
		if (num_value != null) {
			foreach (float n in num_value) {
				comp.Add ("" + n);
			}
		}
		if (bool_value != null) {

			foreach (bool b in bool_value) {
					
				if (b) {
					comp.Add ("true"); 
				} else {
					comp.Add ("false");
				}

			}
		}
		if (var_value != null && var_value.Count > 0) {

			foreach (string k in var_value) {



				string kk = new string (k.ToCharArray ()
				                       .Where (c => !Char.IsWhiteSpace (c))
				                       .ToArray ());

				//Debug.Log("-----starting to look for '"+kk+"'");

				QuestVariable qv = GameObject.Find ("QuestDatabase").GetComponent<actions> ().getVariable (kk);

				if (qv != null) {

					//Debug.Log("found");
					if (qv.getStringValue () != null) {
						comp.Add (qv.getStringValue ());
					}
				} else {

					Debug.Log ("couldn't find var " + kk);

				}	

			}

		}
		
		return comp;
		
	}




}

[System.Serializable]
public class QuestAttribute
{

	public string key;
	public string value;

	public QuestAttribute ()
	{

	}

	public QuestAttribute (string k, string v)
	{

		key = k;
		value = v;
	}
	
}

[System.Serializable]
public class QuestTrigger
{


	[XmlArray("rule"),XmlArrayItem("action")]
	public List<QuestAction>
		actions;

	public void Invoke ()
	{


		if (actions != null) {
			foreach (QuestAction qa in actions) {

				qa.Invoke ();

			}
		}


	}

	public bool hasMissionAction ()
	{
		
		bool b = false;
		foreach (QuestAction a in actions) {


			if (a.hasMissionAction ()) {

				b = true;

			}

		}

		return b;

	}


	
}

[System.Serializable]
public class QuestRuntimeHotspot
{
	
	
	public QuestHotspot hotspot;
	public bool active;
	public bool visible;
	public float lon;
	public float lat;
	public MeshRenderer renderer;
	public bool entered = false;
	public Quest startquest;
	public string category;

	public QuestRuntimeHotspot (QuestHotspot hp, bool a, bool v, string ll)
	{
		
		hotspot = hp;
		active = a;
		visible = v;


		if (ll.Contains (",")) {
			
			
			
			char[] splitter = ",".ToCharArray ();
			
			string[] splitted = ll.Split (splitter);

			foreach (string x in splitted) {

				if (lon == null || lon == 0.0f) {

					lon = float.Parse (x);


				} else {

					lat = float.Parse (x);

				}



			}


		} else {

			lon = 0.0f;
			lat = 0.0f;

		}


		
	}



	public Sprite getMarkerImage(){


		Sprite s = Configuration.instance.defaultmarker;
		Debug.Log ("Category: " + category);
		foreach (MarkerCategorySprite mcs in Configuration.instance.categoryMarker) {

			if(mcs.category == category){

				s = mcs.sprite;

			}

		}

		return s;
	}
	
	
}

