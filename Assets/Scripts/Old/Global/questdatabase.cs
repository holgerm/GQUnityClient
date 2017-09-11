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
using GQ.Client.Util;
using UnitySlippyMap;
using GQ.Client.Conf;
using GQ.Client.Model;
using UnityEngine.SceneManagement;

public class questdatabase : MonoBehaviour
{
	public Transform questdataprefab;
	public Transform currentquestdata;
	public List<Quest> allquests;
	public List<Quest> localquests;
	public List<Quest> downloadquests;
	public int questsToLoad = 0;

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
	public loadinglogo loadlogo;
	public List<SpriteConverter> convertedSprites;
	public string spriteError;
	public List<string> allmetakeys;
	public string sortby = "Erstellungsdatum";
	public bool descending = false;
	public bool convertToSprites = false;
	bool predepzipfound = false;
	public menucontroller menu;
	public List<string> savedmessages;
	public List<WWW> routewwws;
	public createquestbuttons buttoncontroller;
	public RectTransform messageCanvas;
	public privacyAgreement privacyAgreementObject;
	public int privacyAgreementVersionRead = -1;
	public privacyAgreement agbObject;
	public int agbVersionRead = -1;

	bool downloadedAll = false;
	bool downloadingAll = false;

	public GameObject menuButton;

	public updateQuestsMessage updateQuestsMessage;
	public datasendAccept datasendAcceptMessage;

	public void OnEnable ()
	{

		if (!ConfigurationManager.Current.hasMenuWithinQuests) {
			GameObject[] gos = GameObject.FindGameObjectsWithTag ("MenuButton");
			if (gos != null && gos.Length > 0) {
				menuButton = gos [0];
				menuButton.GetComponent<Image> ().enabled = true;
				menuButton.GetComponent<Button> ().enabled = true;
			}
		}
	}

	IEnumerator Start ()
	{
			
		Debug.Log ("questdatabase.Start()");

//		PlayerPrefs.DeleteAll();

		if (PlayerPrefs.HasKey ("privacyagreementversion")) {

			if (PlayerPrefs.GetInt ("privacyagreementversion") > Configuration.instance.privacyAgreementVersion) {

				Configuration.instance.privacyAgreementVersion = PlayerPrefs.GetInt ("privacyagreementversion");
				ConfigurationManager.PrivacyStatement = PlayerPrefs.GetString ("privacyagreement");

			}

		}

		if (PlayerPrefs.HasKey ("agbsversion")) {
			
			if (PlayerPrefs.GetInt ("agbsversion") > Configuration.instance.privacyAgreementVersion) {
				
				Configuration.instance.agbsVersion = PlayerPrefs.GetInt ("agbsversion");
				ConfigurationManager.Terms = PlayerPrefs.GetString ("agbs");
				
			}
			
		}

		if (PlayerPrefs.HasKey ("privacyAgreementVersionRead")) {

			privacyAgreementVersionRead = PlayerPrefs.GetInt ("privacyAgreementVersionRead");

		}


		GameObject.Find ("ImpressumCanvas").GetComponent<showimpressum> ().loadAGBs ();
		GameObject.Find ("ImpressumCanvas").GetComponent<showimpressum> ().loadPrivacy ();


		bool hideBlack = true;

		if (Configuration.instance.showPrivacyAgreement) {
			hideBlack = false;
			StartCoroutine (showPrivacyAgreement ());
		}

		if (PlayerPrefs.HasKey ("agbVersionRead")) {
			
			agbVersionRead = PlayerPrefs.GetInt ("agbVersionRead");
			
		}
		
		if (Configuration.instance.showAGBs) {
			hideBlack = false;
			StartCoroutine (showAGBs ());
			
		}

		if (hideBlack) {

			hideBlackCanvas ();

		}

		PATH_2_PREDEPLOYED_QUESTS = System.IO.Path.Combine (Application.streamingAssetsPath, "predeployed/quests");
		PREDEPLOYED_QUESTS_ZIP = System.IO.Path.Combine (Application.streamingAssetsPath, "predeployed/quests.zip");
		LOCAL_QUESTS_ZIP = System.IO.Path.Combine (Application.persistentDataPath, "tmp_predeployed_quests.zip");
		PATH_2_LOCAL_QUESTS = System.IO.Path.Combine (Application.persistentDataPath, "quests");


#if (UNITY_ANDROID && !UNITY_EDITOR)

		PREDEPLOYED_QUESTS_ZIP = "jar:file://" + Application.dataPath + "!/assets/" + "/predeployed/quests.zip";
		PATH_2_PREDEPLOYED_QUESTS = "jar:file://" + Application.dataPath + "!/assets/predeployed/quests";

#endif

		if (GameObject.Find ("QuestDatabase") != gameObject) {
			Destroy (GameObject.Find ("QuestDatabase"));		
		} else {
			DontDestroyOnLoad (gameObject);
			//			Debug.Log (Application.persistentDataPath);
		}

		if (!Application.isWebPlayer) {

			if (ConfigurationManager.Current.questVisualization != "list") {
				GameObject.Find ("ListPanel").SetActive (false);

			}


			
			if (ConfigurationManager.Current.downloadAllCloudQuestOnStart || (ConfigurationManager.Current.showCloudQuestsImmediately && !shouldPerformAutoStart ())) {
				buttoncontroller.DisplayList ();

				ReloadQuestListAndRefresh ();
			} else {
				if (shouldPerformAutoStart ()) {
					buttoncontroller.DisplayList ();
				}

				webloadingmessage.enabled = false;
				loadlogo.disable ();
			}
		} else {
			if (webloadingmessage != null) {
				webloadingmessage.enabled = true;
			}
			questmilllogo.enabled = true;
			loadlogo.enable ();
		} 

		if (shouldPerformAutoStart ()) {
			performAutoStart ();
		}

		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();

		if (GameObject.Find ("MenuCanvas") != null) {
			
			menu = GameObject.Find ("MenuCanvas").GetComponent<menucontroller> ();
			
		}

	}

	public static bool shouldPerformAutoStart ()
	{
		return ConfigurationManager.Current.autoStartQuestID != 0 && (ConfigurationManager.Current.keepAutoStarting || !autoStartPerformed);
	}

	public void ReloadQuestListAndRefresh ()
	{
		allquests.Clear ();

		string url = "http://qeevee.org:9091/json/" + ConfigurationManager.Current.portal + "/publicgamesinfo";
		Downloader download = new Downloader (url, timeout: 20000);
		download.OnStart += whenQuestListDownloadStarts;
		download.OnProgress += updateProgress;
		download.OnSuccess +=  updateAndShowQuestList;
		download.OnSuccess +=  whenQuestListDownloadSucceeds;
		download.OnSuccess += downloadAllQuests;
		download.OnError += retryAfterDownloadError;
		StartCoroutine (download.StartDownload ());
	}

	void retryAfterDownloadError (AbstractDownloader d, DownloadEvent e)
	{
		Debug.Log ("Retrying after download error: " + e.Message);
		if (Configuration.instance.offlinePlayable && localquests != null && localquests.Count > 0) {
			webloadingmessage.enabled = false;
			loadlogo.disable ();
			updateAndShowQuestList (d, e);
			whenQuestListDownloadSucceeds (d, e);
			downloadAllQuests (d, e);
		} else {
			Action retryAction = new Action (() => { 
				webloadingmessage.enabled = true;
				loadlogo.enable ();
				ReloadQuestListAndRefresh ();
			});
			string alertMsg = "Wir konnten keine Verbindung mit dem Internet herstellen.";
			if (Configuration.instance.offlinePlayable && (localquests == null || localquests.Count == 0)) {
				alertMsg = "Keine Daten vorhanden. Internetverbindung erforderlich.";
			}
			showmessage (alertMsg, "Erneut versuchen", retryAction); 
		}
	}

	public void hideBlackCanvas ()
	{
		if (GameObject.Find ("[BLACK]") != null) {
			GameObject.Find ("[BLACK]").GetComponent<Animator> ().SetTrigger ("out");
		}
	}

	IEnumerator showPrivacyAgreement ()
	{

		WWW www = new WWW ("http://qeevee.org:9091/" + ConfigurationManager.Current.portal + "/privacyagreement/version");
		yield return www;


		if (www.error != null && www.error != "") {

			if (Configuration.instance.privacyAgreementVersion > privacyAgreementVersionRead) {
				GameObject.Find ("ImpressumCanvas").GetComponent<showimpressum> ().loadPrivacy ();
				
				privacyAgreementObject.version = Configuration.instance.privacyAgreementVersion;
				privacyAgreementObject.gameObject.SetActive (true);
				
				
				GetComponent<actions> ().localizeStringToDictionary (ConfigurationManager.PrivacyStatement);
				privacyAgreementObject.textObject.text = GetComponent<actions> ().formatString (GetComponent<actions> ().localizeString (ConfigurationManager.PrivacyStatement));
				privacyAgreementObject.GetComponent<Animator> ().SetTrigger ("in");
			}
			hideBlackCanvas ();

		} else {

		
			string version = www.text;

			if (int.Parse (version) > privacyAgreementVersionRead || Configuration.instance.privacyAgreementVersion > privacyAgreementVersionRead) {


				string agreement = ConfigurationManager.PrivacyStatement;

				if (int.Parse (version) > Configuration.instance.privacyAgreementVersion) {

					WWW www2 = new WWW ("http://qeevee.org:9091/" + ConfigurationManager.Current.portal + "/privacyagreement");
					yield return www2;
					agreement = www2.text;

					PlayerPrefs.SetInt ("privacyagreementversion", int.Parse (version));
					PlayerPrefs.SetString ("privacyagreement", agreement);

					Configuration.instance.privacyAgreementVersion = int.Parse (version);
					ConfigurationManager.PrivacyStatement = agreement;
					GameObject.Find ("ImpressumCanvas").GetComponent<showimpressum> ().loadPrivacy ();



				} else {
					version = Configuration.instance.privacyAgreementVersion.ToString ();
					agreement = ConfigurationManager.PrivacyStatement;
				}


				privacyAgreementObject.version = int.Parse (version);
				privacyAgreementObject.gameObject.SetActive (true);


				GetComponent<actions> ().localizeStringToDictionary (agreement);
				privacyAgreementObject.textObject.text = GetComponent<actions> ().formatString (GetComponent<actions> ().localizeString (agreement));
				privacyAgreementObject.GetComponent<Animator> ().SetTrigger ("in");
			} else {
				hideBlackCanvas ();

			}




		}



	}

	IEnumerator showAGBs ()
	{
		WWW www = new WWW ("http://qeevee.org:9091/" + ConfigurationManager.Current.portal + "/agbs/version");
		yield return www;
		
		
		if (www.error != null && www.error != "") {
			
			Debug.Log ("Couldn't load privacy agreement: " + www.error);
			if (Configuration.instance.agbsVersion > agbVersionRead) {
				GameObject.Find ("ImpressumCanvas").GetComponent<showimpressum> ().loadAGBs ();

				agbObject.version = Configuration.instance.agbsVersion;
				agbObject.gameObject.SetActive (true);
				
				
				GetComponent<actions> ().localizeStringToDictionary (ConfigurationManager.Terms);
				agbObject.textObject.text = GetComponent<actions> ().formatString (GetComponent<actions> ().localizeString (ConfigurationManager.Terms));
				agbObject.GetComponent<Animator> ().SetTrigger ("in");
				
				
			}
			hideBlackCanvas ();
		} else {
			string version = www.text;

			if (int.Parse (version) > agbVersionRead || Configuration.instance.agbsVersion > agbVersionRead) {
				
				string agreement = ConfigurationManager.Terms;
				
				if (int.Parse (version) > Configuration.instance.agbsVersion) {
					
					WWW www2 = new WWW ("http://qeevee.org:9091/" + ConfigurationManager.Current.portal + "/agbs");
					yield return www2;
					// TODO use Download class instead. Here wo forget to check whether it is already done...
					agreement = www2.text;

					PlayerPrefs.SetInt ("agbsversion", int.Parse (version));
					PlayerPrefs.SetString ("agbs", agreement);
					Configuration.instance.agbsVersion = int.Parse (version);
					ConfigurationManager.Terms = agreement;
					GameObject.Find ("ImpressumCanvas").GetComponent<showimpressum> ().loadAGBs ();

					agbObject.version = int.Parse (version);

				} else {
					agbObject.version = Configuration.instance.agbsVersion;

				}

				agbObject.gameObject.SetActive (true);
				
				
				GetComponent<actions> ().localizeStringToDictionary (agreement);
				agbObject.textObject.text = GetComponent<actions> ().formatString (GetComponent<actions> ().localizeString (agreement));
				agbObject.GetComponent<Animator> ().SetTrigger ("in");
			} else {
				hideBlackCanvas ();
				
			}
		}
	}

	private static bool autoStartPerformed = false;

	void performAutoStart ()
	{
		if (loadlogo != null) {

			loadlogo.enable ();
			webloadingmessage.enabled = true;
		}

		if (ConfigurationManager.Current.autostartIsPredeployed) {
			StartCoroutine (startPredeployedQuest (ConfigurationManager.Current.autoStartQuestID));

		} else {
			StartQuest (ConfigurationManager.Current.autoStartQuestID);
		}

		autoStartPerformed = true;
	}

	IEnumerator InitPredeployedQuests ()
	{

		#if !UNITY_WEBPLAYER
		if (webloadingmessage != null) {

			webloadingmessage.text = "Lade...";
		}
		//webloadingmessage.enabled = true;
		questmilllogo.enabled = true;
	
		yield return null;
		
		if (PREDEPLOYED_QUESTS_ZIP.Contains ("://")) {
			// on platforms which use an url type as asset path (e.g. Android):
			WWW questZIP = new WWW (PREDEPLOYED_QUESTS_ZIP); // this is the path to your StreamingAssets in android
			yield return questZIP;
			
			if (questZIP.error != null && !questZIP.error.Equals ("")) {
				Debug.Log ("PDir3: LOADED BY WWW. questZIP.error: " + questZIP.error);
			} else {				
				if (File.Exists (LOCAL_QUESTS_ZIP)) {
					Debug.LogWarning ("Local copy of predeployment zip file was found. Should have been deleted at last initialization.");
					File.Delete (LOCAL_QUESTS_ZIP);
				}
			}
			File.WriteAllBytes (LOCAL_QUESTS_ZIP, questZIP.bytes);
			predepzipfound = true;
		} else {
			initPreloadedQuestiOS ();
		}

		if (predepzipfound) {
		
			ZipUtil.Unzip (LOCAL_QUESTS_ZIP, Application.persistentDataPath);
			File.Delete (LOCAL_QUESTS_ZIP);

			questmilllogo.enabled = false;
			if (webloadingmessage != null) {

				webloadingmessage.enabled = false;
			}
			if (shouldPerformAutoStart ()) {
				performAutoStart ();
			}
		}

#else
		yield return null;
#endif
	}

	void initPreloadedQuestiOS ()
	{

		// on platforms which have a straight file path (e.g. iOS):
		if (!File.Exists (PREDEPLOYED_QUESTS_ZIP)) {
			return;
		}
		File.Copy (PREDEPLOYED_QUESTS_ZIP, LOCAL_QUESTS_ZIP);

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

		SceneManager.LoadScene ("questlist");
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
				
			if (kei == "quest_hotspots") {
				QuestManager.Instance.CurrentQuest = new Quest ();
				if (allquests == null) {
					allquests = new List<Quest> ();
				}
				allquests.Add (QuestManager.Instance.CurrentQuest);
				
				foreach (JSONObject j in obj.list) {
					accessData (j, kei);
				}
				
				//getFirstHotspot (obj);
				
			} else if (kei == "quest") {
				
				foreach (JSONObject j in obj.list) {
					accessData (j, kei);
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
				QuestManager.Instance.CurrentQuest.Name = obj.str;
			}
			break;
		case JSONObject.Type.NUMBER:
			if (kei == "quest_id") {
				QuestManager.Instance.CurrentQuest.Id = (int)obj.n;
			} else if (kei == "quest_lastUpdate") {
				QuestManager.Instance.CurrentQuest.LastUpdate = (long)obj.n;
			} else if (kei == "quest_hotspots_latitude") {
				
				if (QuestManager.Instance.CurrentQuest.start_latitude == null || QuestManager.Instance.CurrentQuest.start_latitude == 0) {
					
					QuestManager.Instance.CurrentQuest.start_latitude = obj.n;
					
				}
				
			} else if (kei == "quest_hotspots_longitude") {
				
				if (QuestManager.Instance.CurrentQuest.start_longitude == null || QuestManager.Instance.CurrentQuest.start_longitude == 0) {
					QuestManager.Instance.CurrentQuest.start_longitude = obj.n;
						
				}
			}
			break;

		case JSONObject.Type.BOOL:
			break;
		case JSONObject.Type.NULL:
			break;
			
		}
	}

	void accessHotspotData (JSONObject obj)
	{
		
		
		for (int i = 0; i < obj.list.Count; i++) {
			string key = (string)obj.keys [i];
			JSONObject j = (JSONObject)obj.list [i];
			
			if (key == "longitude") {
				
				if (QuestManager.Instance.CurrentQuest.start_longitude == null || QuestManager.Instance.CurrentQuest.start_longitude == 0) {
					QuestManager.Instance.CurrentQuest.start_longitude = obj.n;
				}
				
			} else if (key == "latitude") {

				if (QuestManager.Instance.CurrentQuest.start_latitude == null || QuestManager.Instance.CurrentQuest.start_latitude == 0) {
					QuestManager.Instance.CurrentQuest.start_latitude = obj.n;
				}
				
			}
			
			
		}
		
		
	}

	void createMetaData (JSONObject obj)
	{
		
		QuestMetaData meta = new QuestMetaData ();
		
		for (int i = 0; i < obj.list.Count; i++) {
			string key = (string)obj.keys [i];
			JSONObject j = (JSONObject)obj.list [i];
			
			
			
			if (key == "key") {
				
				meta.key = j.str;
				
			} else if (key == "value") {
				meta.value = j.str;
				
			}
			
			
		}
		QuestManager.Instance.CurrentQuest.addMetaData (meta);
		
		
	}


	void updateAndShowQuestList (AbstractDownloader ad, DownloadEvent e)
	{

		Debug.Log ("UPDATE AND SHOW QUEST LIST");

		Downloader download = (Downloader)ad;

		WWW www = download.Www;

		buttoncontroller.filteredOnlineList.Clear ();
		
		allquests.Clear ();
		JSONObject j = new JSONObject (www.text);
		accessData (j, "quest");
		foreach (Quest q in allquests) {
			buttoncontroller.filteredOnlineList.Add (q);
		}

		QuestManager.Instance.CurrentQuest = null;
		
		if (ConfigurationManager.Current.questVisualization == "list") {
			
			buttoncontroller.DisplayList ();
			
		} else if (ConfigurationManager.Current.questVisualization == "map") {
			
			showQuestMap ();
			
		}
		if (loadlogo != null) {
			
			loadlogo.disable ();
		}
	}

	void whenQuestListDownloadStarts (AbstractDownloader d, DownloadEvent e)
	{
		//Debug.Log ("DOWNLOAD of QUEST LIST STARTED");
		if (loadlogo != null) {
			loadlogo.enable ();
		}
		if (webloadingmessage != null) {
			webloadingmessage.enabled = true;
		}
	}

	void whenQuestListDownloadSucceeds (AbstractDownloader d, DownloadEvent e)
	{
		if (loadlogo != null) {
			loadlogo.disable ();
		}
		if (webloadingmessage != null) {
			webloadingmessage.enabled = false;
		}
	}

	void downloadAllQuests (AbstractDownloader d, DownloadEvent e)
	{

		bool hasNoLocalQuestsYet = true;

		localquests = GetLocalQuests ();

		if (localquests != null && localquests.Count > 0) {
			hasNoLocalQuestsYet = false;
		}

		if (!downloadedAll && ConfigurationManager.Current.downloadAllCloudQuestOnStart) {
			downloadingAll = true;

			if (hasNoLocalQuestsYet) {
				questsToLoad = allquests.Count;

				foreach (Quest q in allquests) {


					downloadQuest (q);


				}
			} else {


				if (PlayerPrefs.HasKey ("lastUpdatedAllQuests")) {
					//Store the current time when it starts
					DateTime currentDate = System.DateTime.Now;

					//Grab the old time from the player prefs as a long
					long temp = Convert.ToInt64 (PlayerPrefs.GetString ("lastUpdatedAllQuests"));

					//Convert the old time from binary to a DataTime variable
					DateTime oldDate = DateTime.FromBinary (temp);

					//Use the Subtract method and store the result as a timespan variable
					TimeSpan difference = currentDate.Subtract (oldDate);


					if (difference.Days >= 1) {

						askUserForUpdatingQuests ();


					}


				} else {

				
					askUserForUpdatingQuests ();


				}


			}

			downloadedAll = true;


		} else if (ConfigurationManager.Current.downloadAllCloudQuestOnStart) {

			if (webloadingmessage != null) {

				webloadingmessage.enabled = false;
			}
			if (loadlogo != null) {

				loadlogo.disable ();
			}

		}
	}



	public void askUserForUpdatingQuests ()
	{

		PlayerPrefs.SetString ("lastUpdatedAllQuests", System.DateTime.Now.ToBinary ().ToString ());


		if (questsHaveUpdates ()) {


			updateQuestsMessage.gameObject.SetActive (true);
			updateQuestsMessage.GetComponent<Animator> ().SetTrigger ("in");



		}



	}



	/// <summary>
	/// 1. Deletes local quests that are no more on the server.
	/// 2. Looks for newer version of local quests and updates them. Additionally loads all "new" quests from the server.
	/// </summary>
	public void updateAllQuests ()
	{




		/////////////////////////
		// 1. delete quests locally that are no more on the server:
		foreach (Quest lq in localquests.GetRange(0, localquests.Count)) {
			if (allquests.FindIndex (x => x.Id == lq.Id) == -1) {
				// lq was not loaded from server, ehnce we delete it locally:
				removeQuest (lq);
			}

		}

		/////////////////////////
		// 2. get new quests from server and updates of existing quests:
		questsToLoad = 0;
		bool foundChanges = false;
		foreach (Quest q in allquests) {

			bool alreadyLocal = false;
		
			foreach (Quest lq in localquests.GetRange(0, localquests.Count)) {
				if (lq.Id == q.Id) {
					alreadyLocal = true;

					// update new versions of existing local quests:
					// sorry for the 100sec increase - we have such a stuodi json parser TODO
					if (lq.getLastUpdate () + 100000 < q.getLastUpdate ()) {

						removeQuest (lq);
						downloadQuest (q);
						foundChanges = true;
						questsToLoad += 1;
					}
					break;
				}
			}

			// load new quests from server:
			if (!alreadyLocal) {
				foundChanges = true;
				downloadQuest (q);
				questsToLoad += 1;
			}

		}

		if (!foundChanges) {
			
			backToMenuAfterDownloadedAll ();

		} else {
			
			if (GameObject.Find ("PageController_Map") != null) {
				hotspots = getActiveHotspots ();
				GameObject.Find ("PageController_Map").GetComponent<page_map> ().updateMapMarkerInFoyer ();
			}
		}
	}



	/// <summary>
	/// Looks for newer version of local quests and returns if there are any newer versions of local quests.
	/// </summary>
	public bool questsHaveUpdates ()
	{

		bool foundChanges = false;

		/////////////////////////
		// 1. delete quests locally that are no more on the server:
		foreach (Quest lq in localquests.GetRange(0, localquests.Count)) {
			if (allquests.FindIndex (x => x.Id == lq.Id) == -1) {
				// lq was not loaded from server, ehnce we delete it locally:
				foundChanges = true;
				return foundChanges;
			}

		}



		/////////////////////////
		// 2. get new quests from server
		foreach (Quest q in allquests) {

			bool alreadyLocal = false;

			foreach (Quest lq in localquests.GetRange(0, localquests.Count)) {
				if (lq.Id == q.Id) {
					alreadyLocal = true;

					// update new versions of existing local quests:
					// sorry for the 100sec increase - we have such a stuodi json parser TODO
					if (lq.getLastUpdate () + 100000 < q.getLastUpdate ()) {

						foundChanges = true;
					}
					break;
				}
			}

			// load new quests from server?
			if (!alreadyLocal) {
				foundChanges = true;

			}

		}


		return foundChanges;
	}


	void updateProgress (AbstractDownloader download, DownloadEvent e)
	{
		if (webloadingmessage != null) {
			webloadingmessage.text = String.Format ("{0:N2}% loaded", e.Progress * 100f);
		}
	}

	IEnumerator startPredeployedQuest (int id)
	{


		Quest q = new Quest ();

		q.Id = id;
		q.predeployed = true;

//		q.filepath = PATH_2_PREDEPLOYED_QUESTS + "/" + id + "/game.xml";
		Debug.Log ("STREAMING ASSET PATH: " + Application.streamingAssetsPath);
		q.filepath = System.IO.Path.Combine (Application.streamingAssetsPath, "predeployed/quests/" + id + "/game.xml");

		QuestManager.Instance.CurrentQuest = q;

		string pre = "file: /";

		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {

			pre = "file:";
		}

		if (Application.platform == RuntimePlatform.Android) {

			pre = "";
		}


		Debug.Log ("Predeployed quest searched at: " + pre + q.filepath);
		WWW wwwpdq = new WWW (pre + q.filepath);

		yield return wwwpdq;

		if (wwwpdq.error == null) {
			

			
			
			//QuestManager.Instance.CurrentQuestdata = (Transform)Instantiate (questdataprefab, transform.position, Quaternion.identity);
			q.xmlcontent = UTF8Encoding.UTF8.GetString (wwwpdq.bytes); 

		}

		Debug.Log ("QuestManager.Instance.CurrentQuest set to loadFromText() on " + q.Name + " l: 1067");
		QuestManager.Instance.CurrentQuest = q.LoadFromText (id, true);
		localquests.Add (QuestManager.Instance.CurrentQuest);
		bool hasmorethanmetadata = true;
		QuestManager.Instance.CurrentQuest.currentpage = QuestManager.Instance.CurrentQuest.PageList.First ();
		int c = 0;
		while (QuestManager.Instance.CurrentQuest.currentpage.type == "MetaData") {
			
			if (QuestManager.Instance.CurrentQuest.PageList.Count >= c - 1) {
				QuestManager.Instance.CurrentQuest.currentpage = QuestManager.Instance.CurrentQuest.PageList [c];
				c++;
			} else {
				
				hasmorethanmetadata = false;
				break;
			}
		}
		
		hotspots = new List<QuestRuntimeHotspot> ();
		foreach (QuestHotspot qh in QuestManager.Instance.CurrentQuest.hotspotList) {
			bool initialActivity = qh.hasAttribute ("initialActivity") && qh.getAttribute ("initialActivity") == "true";
			bool initialVisibility = qh.hasAttribute ("initialVisibility") && qh.getAttribute ("initialVisibility") == "true";
			
			hotspots.Add (new QuestRuntimeHotspot (qh, initialActivity, initialVisibility, qh.latlon));
		}
		
		if (canPlayQuest (QuestManager.Instance.CurrentQuest) && hasmorethanmetadata) {

			StartCoroutine (waitForSpriteConversion (QuestManager.Instance.CurrentQuest.currentpage.Id));

		} else {
			
			showmessage ("Entschuldigung! Die Quest kann in dieser Version nicht abgespielt werden.");
			if (webloadingmessage != null) {

				webloadingmessage.enabled = false;
			}
			if (loadlogo != null) {

				loadlogo.disable ();
			}
			GameObject.Find ("List").GetComponent<createquestbuttons> ().resetList ();
			
		}
	}

	public void showQuestMap ()
	{
		
		hotspots = new List<QuestRuntimeHotspot> ();
		hotspots.AddRange (getActiveHotspots ());

		GameObject.Find ("MenuCanvas").GetComponent<Animator> ().SetTrigger ("startMenu");
		GameObject.Find ("BgCam").GetComponent<Camera> ().enabled = false;
		SceneManager.LoadScene ("map", LoadSceneMode.Additive);
	
		if (webloadingmessage != null) {
			webloadingmessage.enabled = false;
		}

	}

	public void StartQuest (int id)
	{

		// if the given quest is already intialized start it, otherwise download it first and start it:
		List<Quest> localQuests = GetLocalQuests ();
		Quest q = null;
		foreach (Quest curQuest in localQuests) {
			
			if (curQuest.Id == id) {
				q = curQuest;
			}
			
		}
		
		if (q == null) {
			Debug.Log ("StartQuest not lokal - id: " + id);
			q = new Quest ();
			q.Id = id;
			downloadQuest (q);
		} else {
			StartCoroutine (startQuest (q));
		}

	}

	/// <summary>
	/// Checks the connection (to the www object given) and then calls download method. 
	/// Thereby it gives a true as parameter if connection has been ok, a false if it either wresulted in an error or 
	/// did not react for more than 10 sec.
	/// </summary>
	/// <returns>The quest after connection check.</returns>
	/// <param name="q">Q.</param>
	/// <param name="elapsedTime">Elapsed time.</param>
	/// <param name="www">Www.</param>
	// TODO replace by generic testConnection method which blocks and returns bool after success or timeout
	IEnumerator DownloadQuestAfterConnectionCheck (Quest q, float elapsedTime, WWW wwwTestConn)
	{

		while (msgsactive > 0) {
			yield return 0;
		}

		yield return new WaitForSeconds (0.1f);
		if (wwwTestConn.isDone) {
			bool ok = (wwwTestConn.error == null);
			downloadAfterConnectionChecked (q, ok);
		} else {
			if (elapsedTime < 10.0f) {
				StartCoroutine (DownloadQuestAfterConnectionCheck (q, elapsedTime + 0.1f, wwwTestConn));
			} else {
				downloadAfterConnectionChecked (q, false);
			}
		}
	}

	static public void CopyFolder (string sourceFolder, string destFolder)
	{
		if (!Directory.Exists (destFolder)) {
			Directory.CreateDirectory (destFolder);
		}
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



//		Debug.Log ("Current Quest: "+QuestManager.Instance.CurrentQuest.id);

		if (QuestManager.Instance.CurrentQuest != null && QuestManager.Instance.CurrentQuest.Id != 0) {
			// In Foyer:

			foreach (QuestRuntimeHotspot qrh in hotspots) {
			
			
			
				if (qrh.active) {

					activehs.Add (qrh);
				}


			}
		} else {
			// In-Quest:

			List<int> visitedQuests = new List<int> ();

			foreach (Quest aq in localquests) {
				if (aq.hotspotList != null && aq.hotspotList.Count > 0) {
					QuestRuntimeHotspot qrh = new QuestRuntimeHotspot (aq.hotspotList [0], true, true, aq.hotspotList [0].latlon);
					if (aq.hasMeta ("category")) {

						qrh.category = aq.getMeta ("category");

					}

					qrh.startquest = aq;

					activehs.Add (qrh);
					visitedQuests.Add (aq.Id);
				}
			}

			if (ConfigurationManager.Current.cloudQuestsVisible)
				foreach (Quest aq in allquests) {
					if (visitedQuests.Contains (aq.Id))
						continue;
				
					if (aq.start_longitude != 0f) {
						QuestHotspot qh = new QuestHotspot ();
				
						qh.radius = 20;
						QuestRuntimeHotspot qrh = new QuestRuntimeHotspot (qh, true, true, aq.start_latitude + "," + aq.start_longitude);

						if (aq.hasMeta ("category")) {

							qrh.category = aq.getMeta ("category");

						}

						qrh.startquest = aq;

						activehs.Add (qrh);
					}
				}




		}

		return activehs;
	}

	public GeoPosition getQuestCenter ()
	{
		float centerLat = 0f;
		float centerLong = 0f;
		int numberofUsedHotspots = 0;

		foreach (QuestRuntimeHotspot curHotspot in hotspots) {
			if (curHotspot.lat != 0f && curHotspot.lon != 0f) {
				centerLat += curHotspot.lat;
				centerLong += curHotspot.lon;
				numberofUsedHotspots++;
			}
		}

		if (numberofUsedHotspots > 0) {
			centerLat = centerLat / numberofUsedHotspots;
			centerLong = centerLong / numberofUsedHotspots;
		}

		return new GeoPosition (centerLat, centerLong);
	}

	public double[] getQuestCenterPosition ()
	{
		GeoPosition center = getQuestCenter ();
		
		return new double[] {
			center.Lat,
			center.Long
		};
	}

	void Update ()
	{
		if (fakebytes > 0 && fakebytes < (int.MaxValue - 1000)) {

			fakebytes += Time.deltaTime;

		}

		if (Input.GetKey (KeyCode.G) && Input.GetKey (KeyCode.E) && Input.GetKey (KeyCode.O) && Input.GetKey (KeyCode.Q)) {

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
		Destroy (gameObject);
		SceneManager.LoadScene ("questlist");
	}

	IEnumerator waitForWebXml ()
	{

		yield return www;

		if (www.error == null) {

			Quest nq = new Quest ();


		
//			QuestManager.Instance.CurrentQuestdata = (Transform)Instantiate(questdataprefab, transform.position, Quaternion.identity);

			nq.xmlcontent = UTF8Encoding.UTF8.GetString (www.bytes); 
			//ASCIIEncoding.ASCII.GetString (Encoding.Convert (Encoding.UTF32, Encoding.ASCII, www.bytes)); 

			if (!downloadingAll) {
				Debug.Log ("QuestManager.Instance.CurrentQuest set to " + nq.Name + " l: 1396");
				QuestManager.Instance.CurrentQuest = nq;
			}

			installQuest (nq, false, false);

		} else {
			debug (www.error);

		}
	}


	public void startQuestAtEndOfFrame (Quest q)
	{


		StartCoroutine (startQuestAtEndOfFrameCoRoutine (q));

	}

	public IEnumerator startQuestAtEndOfFrameCoRoutine (Quest q)
	{

		yield return new WaitForEndOfFrame ();

		yield return new WaitForEndOfFrame ();

		StartCoroutine (startQuest (q));

	}

	/// <summary>
	/// This is the method that is called when you press a quest button in the foyer.
	/// </summary>
	/// <returns>The quest.</returns>
	/// <param name="q">Q.</param>
	public IEnumerator startQuest (Quest q)
	{
		if (!ConfigurationManager.Current.hasMenuWithinQuests) {
			if (menuButton != null) {
				menuButton.GetComponent<Image> ().enabled = false;
				menuButton.GetComponent<Button> ().enabled = false;
			}
		}

		closeMap ();

		bool islocal = false;

		foreach (Quest lq in localquests) {
		
			if (lq.Id == q.Id) {
				islocal = true;
				q = lq;
			}

		}

//		Debug.Log("QuestManager.Instance.CurrentQuest set to " + q.Name + " l: 1448");
		QuestManager.Instance.CurrentQuest = q;

		if (q.UsesLocation && Input.location.status != LocationServiceStatus.Running) {
			Input.location.Start ();

			if (!Input.location.isEnabledByUser) {
				Debug.Log ("Location Service is disabled by user.");
			}

			while (Input.location.status == LocationServiceStatus.Initializing) {
				Debug.Log ("Waiting for LocationService for another second ...");
				yield return new WaitForSeconds (1f);
			}

			if (Input.location.status == LocationServiceStatus.Failed) {
				Debug.LogWarning ("Location Service failed to start.");
			}
			if (Input.location.status == LocationServiceStatus.Stopped) {
				Debug.LogWarning ("Location Service stopped.");
			}
		}

		if (!islocal) {
			downloadQuest (q);
		} else {
			if (q.xmlcontent == null || q.xmlcontent.Trim ().Equals ("")) {
				initiateQuestStart (true, q);
			} else {
				installQuest (q, true, true);
			}
		}
			 
	}

	public void removeQuest (Quest q)
	{



						
		#if UNITY_WEBPLAYER

			Debug.Log("cannot remove on web");

		# else 
		if (Directory.Exists (Application.persistentDataPath + "/quests/" + q.Id)) {
			Directory.Delete (Application.persistentDataPath + "/quests/" + q.Id, true);


		}
#endif
		localquests.Remove (q);
		foreach (QuestRuntimeHotspot curH in hotspots.GetRange(0, hotspots.Count)) {
			if (curH.startquest != null && curH.startquest.Id == q.Id) {
				if (curH.renderer != null)
					Destroy (curH.renderer.gameObject);
				hotspots.Remove (curH);
			}
		}

		if (QuestManager.Instance.CurrentQuest != null) {
			if (QuestManager.Instance.CurrentQuest.Id == q.Id) {

				QuestManager.Instance.CurrentQuest = null;
			}
		}
	
		if (buttoncontroller != null) {

			buttoncontroller.resetList ();
		}
	}

	public void endQuest ()
	{

		if (currentquestdata != null) {
			Destroy (currentquestdata.gameObject);
		}
		Destroy (GameObject.Find ("MsgCanvas"));
		Destroy (gameObject);
		if (menu.isActive) {
			menu.endQuestAnimation ();
		} else {

			returnToMainMenu ();

		}
	}

	public void returnToMainMenu ()
	{

		SceneManager.LoadScene ("questlist");
	}

	public void retryAllOpenWWW ()
	{

		List<WWW> todelete = new List<WWW> ();
		foreach (WWW awww in filedownloads) {

			todelete.Add (awww);

		}
	

	}

	// TODO parameter should only be the id of the quest (hm)
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

		if (ConfigurationManager.Current.showNetConnectionWarning &&
		    (q.alternateDownloadLink == "" || q.alternateDownloadLink == null) &&
		    msgsactive == 0) {
			showmessage ("Wir empfehlen eine gute WLAN Verbindung um alle Medien zu laden.", "OK"); // TODO anpassen z. Bsp. für DownloadAllQuest auto)
		}
//		Debug.Log ("downloadQuest(): q.alternateDownloadLink =" + q.alternateDownloadLink);

		StartCoroutine (DownloadQuestAfterConnectionCheck (q, 0.0f, new WWW ("http://qeevee.org:9091/testConnection")));
	}

	void downloadAfterConnectionChecked (Quest q, bool connected)
	{
		if (connected) {
			if (webloadingmessage != null) {

				webloadingmessage.text = "Lade Quest ... " + q.Name;
			}


			string url = "http://www.qeevee.org:9091/editor/" + q.Id + "/clientxml";

			if (q.alternateDownloadLink != null && q.alternateDownloadLink != "") {

				url = q.alternateDownloadLink;
			}


			q.www = new WWW (url);
			if (loadlogo != null) {

				loadlogo.enable ();
			}
			if (webloadingmessage != null) {

				webloadingmessage.text = "Bitte warten ... ";
			}
			StartCoroutine (DownloadFinished (q));
		} else {
			
			// TODO message with two buttons is needed here to enable cancel.
			showmessage ("Wir konnten keine Verbindung mit dem Internet herstellen.", "Nochmal versuchen");
			
			
			StartCoroutine (DownloadQuestAfterConnectionCheck (q, 0.0f, new WWW ("http://www.google.com")));

		}

	}

	// TODO delete this method.
	public void downloadQuest (int id)
	{
		Quest q = new Quest ();
		q.Id = id;

		Debug.Log ("QuestManager.Instance.CurrentQuest set to " + q.Name + " l: 1609");
		QuestManager.Instance.CurrentQuest = q;
		downloadQuest (q);
	}

	public void downloadAsset (string url, string localTargetPath)
	{

		if (url.StartsWith (page_videoplay.YOUTUBE_URL_PREFIX)) {
			Debug.Log ("YouTube Video Link found. Not loaded as Asset");
			return;
		}
		
		if (wanttoload == null) {
			wanttoload = new List<string> ();
		}
		if (!wanttoload.Contains (url)) {
			
			wanttoload.Add (url);
		}

		StartCoroutine (downloadAssetAsync (url, localTargetPath));

	}

	private static List<string> debugDownloadsStarted = new List<string> ();

	private float time;

	public IEnumerator downloadAssetAsync (string url, string localTargetPath)
	{
		if (filedownloads == null) {
			filedownloads = new List<WWW> ();
		}

		bool done = true;
		if (filedownloads != null) {
			foreach (WWW w in filedownloads) {

				if (!w.isDone) {

					done = false;
				} else {
				}


			}
		}


		if (done) {

			if (!url.Contains ("/clientxml") || !url.StartsWith (page_videoplay.YOUTUBE_URL_PREFIX)) {

				time = Time.time;

				WWW wwwfile = new WWW (url);
				debugDownloadsStarted.Add (url);

				if (filedownloads == null) {
					filedownloads = new List<WWW> ();
				}


				filedownloads.Add (wwwfile);
				files_all += 1;
				StartCoroutine (downloadAssetFinished (wwwfile, localTargetPath, 0f));
			}
		} else {

			yield return new WaitForEndOfFrame ();
			downloadAsset (url, localTargetPath);
		}
	}

	public IEnumerator downloadAssetFinished (WWW wwwfile, string filename, float timeout)
	{

		yield return new WaitForSeconds (0.3f);
		timeout += 0.3f;

		if (wwwfile.error != null) {
				
			if (wwwfile.error != "unsupported URL") {

				downloadAsset (wwwfile.url, filename);
			}
			filedownloads.Remove (wwwfile);
			wwwfile.Dispose ();
		} else {
			if (wwwfile.isDone) {
				if (!Directory.Exists (Path.GetDirectoryName (filename))) {
						
					Directory.CreateDirectory (Path.GetDirectoryName (filename));
				}
				if (wwwfile == null || wwwfile.bytes == null || wwwfile.bytes.Length == 0) {
					Debug.Log ("Download Problem: Empty file " + filename);
				}
				FileStream fs = File.Create (filename);
				fs.Write (wwwfile.bytes, 0, wwwfile.size);
				fs.Close ();
				files_complete += 1;
				bytesloaded += (int)(wwwfile.bytesDownloaded);
				filedownloads.Remove (wwwfile);



				if (wanttoload.Contains (wwwfile.url)) {
					wanttoload.Remove (wwwfile.url);
				}

				wwwfile.Dispose ();


		

				//performSpriteConversion (filename);




				
			} else {
				if (timeout > (float)ConfigurationManager.Current.downloadTimeOutSeconds) {
					showmessage ("Download fehlgeschlagen.");
					SceneManager.LoadScene ("questlist");
				} else if (timeout > 60f && wwwfile.progress < 0.1f) {
					Debug.Log ("Error Timeout: " + wwwfile.url + " - " + timeout);

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


		if (localquests != null && localquests.Count > 0) {
			return localquests;
		}

		if (!Application.isWebPlayer) {
			localquests.Clear (); 

			// collect all LOCAL quests:

			DirectoryInfo info = new DirectoryInfo (Application.persistentDataPath + "/quests/");
			if (!info.Exists) {
				info.Create ();
				return localquests;
			}
			var fileInfo = info.GetDirectories ();

			downloadingAll = false;

			foreach (DirectoryInfo folder in fileInfo) { 
				if (File.Exists (folder.ToString () + "/game.xml")) {
					Quest n = new Quest ();
					string[] splitted = folder.ToString ().Split ('/');
					n.Id = int.Parse (splitted [splitted.Length - 1]);
					n.filepath = folder.ToString () + "/";
					n = n.LoadFromText (int.Parse (splitted [splitted.Length - 1]), true);
					if (n != null) {
						localquests.Add (n);
					}
				}
			}

//			// collect all PREDEPLOYED quests:
//
//			info = new DirectoryInfo (PATH_2_PREDEPLOYED_QUESTS + "/");
//			if (!info.Exists) {
//				info.Create ();
//				return localquests;
//			}
//			fileInfo = info.GetDirectories ();
//
//			downloadingAll = false;
//
//			foreach (DirectoryInfo folder in fileInfo) { 
//				if (File.Exists (folder.ToString () + "/game.xml")) {
//					Quest n = new Quest ();
//					string[] splitted = folder.ToString ().Split ('/');
//					n.Id = int.Parse (splitted [splitted.Length - 1]);
//
//					// is this predelpoyed quest (n) already in our list as a local quest?
//					bool alreadyAsLocalQuestInList = false;
//					foreach (Quest lq in localquests) {
//						if (n.Id.Equals (lq.Id)) {
//							alreadyAsLocalQuestInList = true;
//							break;
//						}
//					}
//
//					if (!alreadyAsLocalQuestInList) {
//						n.filepath = folder.ToString () + "/";
//						n = n.LoadFromText (int.Parse (splitted [splitted.Length - 1]), true);
//						if (n != null) {
//							localquests.Add (n);
//						}
//					}
//				}
//			}

			GameObject.Find ("[FILTERLIST]").GetComponent<categoryFilterList> ().reInstantiateFilter ();

			//downloadingAll = true;

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
			
			if (originalOrientation != null) {

				Screen.orientation = originalOrientation;
			}

			Screen.autorotateToLandscapeLeft = false;
			Screen.autorotateToLandscapeRight = false;
		}
	}

	/// <summary>
	/// Installs the quest. reload is always true, when we call install after download. 
	/// localload is false in that case. 
	/// q is a new Quest object with only the id set to the id of the downloaded quest. 
	/// In localquests list there is another Quest object with the same id as q.id.
	/// </summary>
	/// <param name="q">Q.</param>
	/// <param name="reload">If set to <c>true</c> reload.</param>
	/// <param name="localload">If set to <c>true</c> localload.</param>
	public void installQuest (Quest q, bool reload, bool localload)
	{

		if (filedownloads != null) {
			filedownloads.Clear ();
		}
		if (loadedfiles != null) {
			loadedfiles.Clear ();
		}

		convertToSprites = true;

		// TODO: Here we create the THIRD Quest object (in download case). 
		// We can only assume that q has an id here. Is this really a good idea? (hm)
//		Debug.Log("XXXXX VORHER");
		Quest nq = q.LoadFromText (q.Id, localload);

		// store timestamp for old quests that miss lastUpdate in XML to prevent relaoding them always:
		if (nq != null && nq.LastUpdate == 0 && !reload && !localload) {
			foreach (Quest curQ in allquests) {
				if (curQ.Id == nq.Id) {
					nq.LastUpdate = curQ.LastUpdate;
					PlayerPrefs.SetString (curQ.Id + "_lastUpdate", curQ.LastUpdate.ToString ());
					Debug.Log ("<color=red>TIMESTAMP stored in PLAYER_PREFS for quest id = " + curQ.Id + "</color>");
				}
			}
		}
//		Debug.Log("XXXXX NACHHER");

		bool alreadyStoredInLocalQuests = false;
		if (nq != null)
			foreach (Quest quest in localquests) {
				if (quest.Id == nq.Id) {
					alreadyStoredInLocalQuests = true;
					break;
				}
			}
		if (!alreadyStoredInLocalQuests)
			localquests.Add (nq);

		nq.Id = q.Id;
		if (nq == null) {
			questmilllogo.enabled = false;
			if (webloadingmessage != null) {

				webloadingmessage.enabled = false;
			}
			if (loadlogo != null) {

				loadlogo.disable ();
			}
			return;
		}


		if (downloadingAll) {

			if (downloadquests == null) {
				downloadquests = new List<Quest> ();
			}
			downloadquests.Add (nq);

		} else {
			QuestManager.Instance.CurrentQuest = nq;
		}

		// if not local load => set export location to fresh directory:
		if (!localload) {

			// resave xml
			string exportLocation = Application.persistentDataPath + "/quests/" + nq.Id + "/";

#if !UNITY_WEBPLAYER

			if (!Application.isWebPlayer && (!Directory.Exists (exportLocation) || reload)) {

				if (Directory.Exists (exportLocation)) {
					Directory.Delete (exportLocation, true);
				}
				Directory.CreateDirectory (exportLocation);

			}
#endif
		}

		initiateQuestStart (localload, nq);



	}

	void initiateQuestStart (bool localload, Quest nq)
	{
		bool hasmorethanmetadata = false;
		if (nq.PageList != null && nq.PageList.Count > 0) {
			// check if game has more than just metadata and set flag:
			nq.currentpage = nq.PageList.First ();
			int c = 0;
			while (nq.currentpage.type == "MetaData") {
				// TODO I guess this is a bug and will crash if we have e.g. a quest with a single page of type meta data
				// TODO at least it will leave QuestManager.Instance.CurrentQuest.currentpage being null.
				if (nq.PageList.Count >= c - 1) {
					nq.currentpage = nq.PageList [c];
					c++;
				} else {
					hasmorethanmetadata = false;
					break;
				}
			}
			if (!nq.currentpage.type.Equals ("MetaData")) {
				hasmorethanmetadata = true;
			}
		}
		if (QuestManager.Instance.CurrentQuest != null && QuestManager.Instance.CurrentQuest.hotspotList != null) {
			hotspots = new List<QuestRuntimeHotspot> ();
			foreach (QuestHotspot qh in QuestManager.Instance.CurrentQuest.hotspotList) {
				bool initialActivity = qh.initialActivity;
				bool initialVisibility = qh.initialVisibility;
				hotspots.Add (new QuestRuntimeHotspot (qh, initialActivity, initialVisibility, qh.latlon));
			}
		}
		if (canPlayQuest (QuestManager.Instance.CurrentQuest) && hasmorethanmetadata) {
			if (Application.isWebPlayer) {
				reallyStartQuest (QuestManager.Instance.CurrentQuest.currentpage.Id);
			} else {
				if (!localload) {
					//				Debug.Log ("WAITING FOR QUEST ASSETS");
					if (webloadingmessage != null) {
						webloadingmessage.text = "Lade alle Medien vor.\n Das kann einige Minuten dauern. \n ";
					}
					if (loadlogo != null) {
						loadlogo.enable ();
					}
					StartCoroutine (waitforquestassets (nq.currentpage.Id, 0f));
				} else {
					StartCoroutine (waitForSpriteConversion (nq.currentpage.Id));
				}
			}
		} else {
			showmessage ("Entschuldigung! Die Quest kann in dieser Version nicht abgespielt werden.");
			if (webloadingmessage != null) {
				webloadingmessage.enabled = false;
			}
			if (loadlogo != null) {
				loadlogo.disable ();
			}
			if (GameObject.Find ("List") != null && GameObject.Find ("List").GetComponent<createquestbuttons> () != null) {
				GameObject.Find ("List").GetComponent<createquestbuttons> ().resetList ();
			}
		}
	}

	public void performSpriteConversion (string value)
	{

		return;

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

						List<string> imageextensions = new List<string> () {
							".jpg",
							".jpeg",
							".gif",
							".png"
						};
					} else {

						Debug.LogWarning ("[ATTENTION] A file didn't exist: " + value);

					}
				}
			}
		}
	}

	IEnumerator waitForSpriteConversion (int pageid)
	{
		checkForDatasend (pageid);
		yield return null;
	}



	public void acceptedDatasend (int pageid)
	{


		QuestManager.Instance.CurrentQuest.acceptedDS = true;

		reallyStartQuest (pageid);



	}

	public void rejectedDatasend (int pageid)
	{
		
		
		QuestManager.Instance.CurrentQuest.acceptedDS = false;

		//TODO: can I start the quest?

		reallyStartQuest (pageid);
		
		
		
	}

	void checkForDatasend (int pageid)
	{

	
		if (Configuration.instance.showMessageForDatasendAction && !QuestManager.Instance.CurrentQuest.acceptedDS &&
		    QuestManager.Instance.CurrentQuest.hasActionInChildren ("SendVarToServer")) {

			// TODO FRAGE: Warum wird das für jede Seite aufgerufen, wo es doch eh immer das ganze Quest durchsucht? (hm) Erst Test schreiben dann refactoren!

			// TODO: show message

			datasendAcceptMessage.pageid = pageid;
			datasendAcceptMessage.gameObject.SetActive (true);
			datasendAcceptMessage.GetComponent<Animator> ().SetTrigger ("in");


		} else {
			reallyStartQuest (pageid);
		}
	
	}


	public bool nextSpriteToBeConverted (SpriteConverter sc)
	{


		bool me = true;

		foreach (SpriteConverter asc in convertedSprites) {


			if (asc == sc) {

				break;
			} else if (asc.isDone != true) {

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
			


				if (nextSpriteToBeConverted (sc)) {


					if (myWWW.texture != null) {

						sc.myTexture = myWWW.texture;
						sc.width = myWWW.texture.width;
						sc.height = myWWW.texture.height;
						sc.convertSprite ();


					} else {
						sc.isDone = true;
						myWWW = null;
						sc.myWWW = null;
					}
				} else {
					yield return new WaitForSeconds (0.5f);
					StartCoroutine (waitForSingleSpriteCompletion (sc));

				}
			}
		}
	
	}

	void reallyStartQuest (int pageid)
	{

		// instatiate a quest clone at any start. This function is always called at quest start.
		currentquestdata = (Transform)Instantiate (questdataprefab, transform.position, Quaternion.identity);
		
		if (QuestManager.Instance.CurrentQuest.getAttribute ("transferToUserPosition") != "true") {
		
			changePage (pageid);
		

		} else {
			foreach (QuestRuntimeHotspot mainhs in hotspots) {


				if (mainhs.hotspot.id == int.Parse (QuestManager.Instance.CurrentQuest.getAttribute ("transferHotspot"))) {

					
				


					if (Application.isWebPlayer || Application.isEditor) {


						GameObject.Find ("QuestDatabase").GetComponent<GPSPosition> ().CoordinatesWGS84 = 
						new double[] {
						
							7d,
							50d
						};
					}

					foreach (QuestRuntimeHotspot subhs in hotspots) {
				
						if (subhs.hotspot.id != mainhs.hotspot.id) {

							subhs.lon -= mainhs.lon;
							subhs.lon += (float)GameObject.Find ("QuestDatabase").GetComponent<GPSPosition> ().CoordinatesWGS84 [1];

							subhs.lat -= mainhs.lat;
							subhs.lat += (float)GameObject.Find ("QuestDatabase").GetComponent<GPSPosition> ().CoordinatesWGS84 [0];

							string url = "http://www.yournavigation.org/api/1.0/gosmore.php?" +
							             "format=kml" +
							             "&flat=" + GameObject.Find ("QuestDatabase").GetComponent<GPSPosition> ().CoordinatesWGS84 [1] +
							             "&flon=" + GameObject.Find ("QuestDatabase").GetComponent<GPSPosition> ().CoordinatesWGS84 [0] +
							             "&tlat=" + subhs.lon +
							             "&tlon=" + subhs.lat +
							             "&v=foot&" +
							             "fast=1" +
							             "&layer=mapnik" +
							             "&instructions=1" +
							             "&lang=de";
							
							WWW routewww = new WWW (url);

							if (routewwws == null) {

								routewwws = new List<WWW> ();
							}

							routewwws.Add (routewww);

							StartCoroutine (waitForRouteFile (routewww, subhs));
						}
					}


					mainhs.lat = (float)GameObject.Find ("QuestDatabase").GetComponent<GPSPosition> ().CoordinatesWGS84 [0];
					mainhs.lon = (float)GameObject.Find ("QuestDatabase").GetComponent<GPSPosition> ().CoordinatesWGS84 [1];
				}
			}

			StartCoroutine (waitForTransferCompletion (pageid));
		}
	}

	public IEnumerator waitForTransferCompletion (int pageid)
	{

		yield return new WaitForEndOfFrame ();

		bool b = true;

		foreach (WWW mywww in routewwws) {

			if (!mywww.isDone) {

				b = false;

			}
		}

		if (b) {
			changePage (pageid);
		} else {

			yield return new WaitForSeconds (0.1f);
			StartCoroutine (waitForTransferCompletion (pageid));

		}
	}

	public IEnumerator waitForRouteFile (WWW mywww, QuestRuntimeHotspot qrh)
	{
		
		yield return mywww;
		
		if (mywww.error == null || mywww.error == "") {
			string routefile = mywww.text;
			
			routefile = routefile.Substring (routefile.IndexOf ("<coordinates>"));
			routefile = routefile.Substring (14, routefile.IndexOf ("</coordinates>") - 14);

			string[] coordinates = routefile.Split (new string[] {
				Environment.NewLine
			}, StringSplitOptions.None);
				
				

			int i = coordinates.Count () - 2;
			string s = coordinates [i];
					
					
			if (s.Contains (",")) {
						
						
				string[] co = s.Split (',');
						
						
				if (float.Parse (co [0]) != 0f && float.Parse (co [1]) != 0f) {
					qrh.lat = float.Parse (co [0]);


					qrh.lon = float.Parse (co [1]);

				} else {
					if (savedmessages == null) {

						savedmessages = new List<string> ();
					}
					savedmessages.Add ("An deinem Standort sind nicht genügend Weginformationen vorhanden. Manche Kartenobjekte könnten nicht erreichbar sein.");
				}
			}
					
		} else {
			Debug.Log ("Route WWW Error:" + mywww.error);
		}
	}

	int waitedFor = 0;

	IEnumerator waitforquestassets (int pageid, float timeout)
	{

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
		} else {
			if (filedownloads != null) {

				foreach (WWW www in filedownloads) {

					if (!www.isDone) {
						done = false;
						downloadsundone += 1;
					}

					if (www.error != null) {
						if (www.url.StartsWith ("http")) {
							done = false;
							downloadsundone += 1;
						}
					}
				}

				int bytes_finished = files_complete;
				int bytes_all = files_all;

			} else {
				done = true;
			}
		}

		int bytescomplete = bytesloaded;
		int filesleft = 0;

		if (filedownloads != null) {

			filesleft = filedownloads.Count;

			string openfileloads = "Open WWW Files: ";

			int d = 0;
			foreach (WWW awww in filedownloads) {

				openfileloads += awww.url + "; ";
				d++;
			}
		}

		if (error == "") {
			int bytesloaded2 = (int)(bytesloaded + (fakebytes * 900));
			if (webloadingmessage != null) {

				webloadingmessage.text = "Lade alle Medien vor.\n Das kann einige Minuten dauern. \n " + bytesloaded2 + " Bytes geladen";
			}
		} else {
			if (webloadingmessage != null) {
				webloadingmessage.text = error;
			}

		}
		if (done) {
			
			if (!downloadingAll) {

				writeQuestXML (QuestManager.Instance.CurrentQuest);
				StartCoroutine (waitForSpriteConversion (pageid));
			} else {
				
				waitedFor += 1;
				if (waitedFor >= questsToLoad) {

					foreach (Quest q in downloadquests) {

						writeQuestXML (q);

					}

					backToMenuAfterDownloadedAll ();
				}
			}
		} else {
			StartCoroutine (waitforquestassets (pageid, timeout));
		}
	}

	void backToMenuAfterDownloadedAll ()
	{
		if (webloadingmessage != null) {
			webloadingmessage.enabled = false;
		}
		if (loadlogo != null) {
			loadlogo.disable ();
		}
		downloadingAll = false;
		downloadedAll = true;
		downloadquests = null;
		buttoncontroller.loadLocalQuests ();
		buttoncontroller.DisplayList ();
		if (GameObject.Find ("PageController_Map") != null) {
			hotspots = new List<QuestRuntimeHotspot> ();
			hotspots.AddRange (getActiveHotspots ());
			GameObject.Find ("PageController_Map").GetComponent<page_map> ().updateMapMarkerInFoyer ();
		}
	}


	public void writeQuestXML (Quest q)
	{

		string exportLocation = Application.persistentDataPath + "/quests/" + q.Id + "/";

		if (!File.Exists (exportLocation + "game.xml")) {

			var stream = new FileStream (exportLocation + "game.xml", FileMode.Create);

			//	Debug.Log ("writing xml #0");

			stream.Close ();
			var stream2 = new StreamWriter (exportLocation + "game.xml");

			//	Debug.Log ("writing xml #1: " + q.xmlcontent);
			stream2.Write (q.xmlcontent);
			//	Debug.Log ("writing xml #2");

			stream2.Close ();
			//	Debug.Log ("writing xml #3");

		}




	}

	public bool canPlayQuest (Quest q)
	{

		if (downloadingAll) {
			// TODO FRAGE: Ist das nicht eine fehlerhafte Abkürzung? (hm)
			return true;

		}

		return Platform.CanPlay (q);

	}

	public Page getPage (int id)
	{
		Page resultpage = null;



		foreach (Page qp in QuestManager.Instance.CurrentQuest.PageList) {
			
			
			if (qp.Id == id) {

				resultpage = qp;

			}


		}


		return resultpage;

	}

	public void closeMap ()
	{

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
		if (GameObject.Find ("RouteRender") != null) {
			Destroy (GameObject.Find ("RouteRender"));
		}
	}

	public void changePage (int id)
	{


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

			GameObject.Find ("[Map]").GetComponent<mapdisplaytoggle> ().hideMap ();
		}


		if (GameObject.Find ("PageController_Map")) {

			GameObject.Find ("PageController_Map").GetComponent<page_map> ().unDrawCurrentRoute ();
		}

		   
		foreach (Page qp in QuestManager.Instance.CurrentQuest.PageList) {

			if (qp.Id.Equals (id)) {

				QuestManager.Instance.CurrentQuest.currentpage = qp;
				QuestManager.Instance.CurrentQuest.currentpage.stateOld = "running";

				GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject> ();

				int foundTiles = 0;
				int destroyedTiles = 0;

				foreach (GameObject go in allObjects) {
			
					if (go != null &&
					    go.transform != null &&
					    go.name != "MapCanvas" &&
					    go.name != "PageController_Map" &&
					    go.name != "QuestDatabase" &&
					    go.name != "MsgCanvas" &&
					    go.name != "ImpressumCanvas" &&
					    go.name != "LanguageCanvas" &&
					    !go.transform.IsChildOf (GameObject.Find ("ImpressumCanvas").transform) &&
					    go.name != "MenuCanvas" &&
					    go.name != "EventSystem" &&
					    go.name != "Configuration" &&
					    go.name != "MapCam" &&
					    go.name != "[Map]" &&
					    go.name != "[location marker]" &&
					    go.name != "" &&
					    !go.name.Contains ("[Tile") &&
					    go.name != "EventSystem_Map" &&
					    go.name != "BgCam" &&
					    go.name != "QuestData(Clone)" &&
					    go.name != "Audio Source(Clone)" &&
					    go.name != "NetworkManager" &&
					    !go.name.Contains ("NetworkIdentity") &&
					    go.name != "RouteRender" &&
					    go.name != "VectorCanvas" &&
					    go.name != "VarOverlayCanvas") {

						bool des = true;

						if (GameObject.Find ("VarOverlayCanvas") != null) {
						
							if (go.transform.IsChildOf (GameObject.Find ("VarOverlayCanvas").transform)) {
								des = false;
							}
						}

						if (GameObject.Find ("LanguageCanvas") != null) {
							
							if (go.transform.IsChildOf (GameObject.Find ("LanguageCanvas").transform)) {
								des = false;
							}
						}

						if (GameObject.Find ("MenuCanvas") != null) {
						
							if (go.transform.IsChildOf (GameObject.Find ("MenuCanvas").transform)) {
								des = false;
							}
						}

						if (GameObject.Find ("MapCanvas") != null) {

							if (go.transform.IsChildOf (GameObject.Find ("MapCanvas").transform)) {
								des = false;
							}
						}

						if (GameObject.Find ("[Map]")) {

							if (go.transform.IsChildOf (GameObject.Find ("[Map]").transform)) {
								destroyedTiles++;
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
							Destroy (go);
						}

					}
				} // foreach go

				Resources.UnloadUnusedAssets ();

				bool needsCamera = false;

				if (!menu.isActive) {
					menu.showTopBar ();
				}
				
				if (qp.type == "MapOSM" || qp.type == "Navigation") {

					if (GameObject.Find ("MapCam") == null) {

						StartCoroutine (loadMap ());
					} else {

						if (GameObject.Find ("PageController_Map") != null) {

							GameObject.Find ("PageController_Map").GetComponent<page_map> ().onStartInvoked = false;
						}

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
							
							GameObject.Find ("[Map]").GetComponent<mapdisplaytoggle> ().showMap ();
						}
					}
				} else {
					// we sometimes have to wait for the end of the frame to destroy the latest page controller etc. 
					// before we start the next one. Cf. a problem with onRead on NFCReader page when switching to a
					// multiplechoicequestion page.
					StartCoroutine (LoadPage (qp.type));
				}
			}
		}
		


		string lastmessage = "";

		foreach (string s in savedmessages) {

			if (s != lastmessage) {
				showmessage (s);
				lastmessage = s;
			}


		}
		savedmessages.Clear ();

	}

	IEnumerator LoadPage (string pageName)
	{
		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();

		string pageTypeXML = "";
		bool needsCamera = false;

		switch (pageName) {
		case "NPCTalk":
			pageTypeXML = "npctalk";
			break;
		case "ImageWithText":
			pageTypeXML = "npctalk";
			break;
		case "StartAndExitScreen":
			pageTypeXML = "fullscreen";
			break;
		case "MultipleChoiceQuestion":
		case "Menu":
			pageTypeXML = "multiplechoicequestion";
			break;
		case "VideoPlay":
			pageTypeXML = "videoplay";
			break;
		case "TagScanner":
			pageTypeXML = "qrcodereader";
			needsCamera = true;
			break;
		case "ImageCapture":
			pageTypeXML = "imagecapture";
			needsCamera = true;
			break;
		case "TextQuestion":
			pageTypeXML = "textquestion";
			break;
		case "AudioRecord":
			pageTypeXML = "audiorecord";
			needsCamera = true; // TODO why???
			break;
		case "WebPage":
			pageTypeXML = "website";
			break;
		case "Custom":
			pageTypeXML = "custom";
			break;
		case "ReadNFC":
			pageTypeXML = "readnfc";
			break;
		default:
			Debug.LogError ("Can not change to page of unknown type name: " + pageName);
			yield break;
		}

		if (needsCamera) {
			if (GameObject.Find ("MapCanvas") != null) {
				GameObject.Find ("MapCanvas").GetComponent<Canvas> ().enabled = false;
			}
		}

		SceneManager.LoadScene (pageTypeXML, LoadSceneMode.Additive);
	}

	IEnumerator loadMap ()
	{
		// TODO shouldn't we simply call SceneManager.LoadSceneAsync since it already works in a backgorund thread. 
		AsyncOperation loadLevelOperation = SceneManager.LoadSceneAsync (9, LoadSceneMode.Additive);		
	
		yield return loadLevelOperation;
		if (GameObject.Find ("BgCam") != null) {
			
			GameObject.Find ("BgCam").GetComponent<Camera> ().enabled = false;
			
			
		}
		if (GameObject.Find ("MapCam") != null) {
			
			GameObject.Find ("MapCam").GetComponent<Camera> ().enabled = true;
			
			
		}
	}

	public void showmessage (string text, string button = null, Action action = null)
	{

		msgsactive += 1;

		QuestMessage nqa = (QuestMessage)Instantiate (message_prefab, transform.position, Quaternion.identity);
		
		nqa.message = text;
		if (button != null) {
			nqa.setButtonText (button);
		}
		if (action != null) {
			nqa.Action = action;
		}
		nqa.transform.SetParent (GameObject.Find ("MsgCanvas").transform, false);
		nqa.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, 0f);



	}

	public QuestHotspot getHotspotObject (int i)
	{

		foreach (QuestHotspot qh in QuestManager.Instance.CurrentQuest.hotspotList) {

			if (qh.id == i) {

				return qh;
			}
		}

		return null;

	}

	public QuestRuntimeHotspot getHotspot (string str)
	{


		foreach (QuestRuntimeHotspot qrh in hotspots) {

			if (qrh.hotspot.id == int.Parse (str)) {

				return qrh;


			}






		}
		return null;
	}

	IEnumerator DownloadFinished (Quest q)
	{
		if (webloadingmessage != null) {

			webloadingmessage.enabled = true;
		}
		if (loadlogo != null) {

			loadlogo.enable ();
		}

		// TODO: shouldn't we add it to the local quests only after the download of the quest xml has been completed, i.e. one line later? (hm)
//		localquests.Add(q);
		yield return q.www;

		// Quest XML has been downloaded now.

		if (q.www.error == null) {



			if (webloadingmessage != null) {

				webloadingmessage.text = "Bitte warten ...";
			}

			Quest nq = new Quest ();
				
			nq.Id = q.Id;
				
//			QuestManager.Instance.CurrentQuestdata = (Transform)Instantiate(questdataprefab, transform.position, Quaternion.identity);
//				
			nq.xmlcontent = UTF8Encoding.UTF8.GetString (q.www.bytes); 
//			Debug.Log ("XML:" + nq.xmlcontent);
			bool isLocal = false;


			// TODO: what is this good for? We already know that it is in the list, since we put it in there. (hm)
			foreach (Quest lq in localquests) {
				if (lq.Id == q.Id) {
					isLocal = true;
				}
			}

			if (!downloadingAll) {
				QuestManager.Instance.CurrentQuest = nq;
			}

			// TODO: here b is always true! So we can elimiate the foreach loop above! (hm again ;-) )
			installQuest (nq, isLocal, false);
			// TODO: why do we use a new Quest object nq which only has the id from q. Shouldn't we just use q itself?
		} else {
			Debug.LogWarning ("WWW Error: " + q.www.error);
			if (webloadingmessage != null) {

				webloadingmessage.text = q.www.error;
			}

		}  
	}

}













