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
using UnitySlippyMap;
using GQ.Client.Conf;
using GQ.Client.Model;
using UnityEngine.SceneManagement;

public class questdatabase : MonoBehaviour {
	public Quest currentquest;
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
	public bool individualReturnDefinitions = false;
	public bool allowReturn = false;
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

	public void OnEnable () {

		if ( !Configuration.instance.hasMenuWithinQuests ) {
			GameObject[] gos = GameObject.FindGameObjectsWithTag("MenuButton");
			if ( gos != null && gos.Length > 0 ) {
				menuButton = gos[0];
				menuButton.GetComponent<Image>().enabled = true;
				menuButton.GetComponent<Button>().enabled = true;
			}
		}
	}

	IEnumerator Start () {

		Debug.Log("ORDINARY Debug.Log() called.");

		#if GQ_DEBUG
		Debug.Log("GQ_DEBUG HAS BEEN called.");
		#else
		Debug.Log("NO GQ_DEBUG called.");
		#endif
			
//		PlayerPrefs.DeleteAll();

		if ( PlayerPrefs.HasKey("privacyagreementversion") ) {

			if ( PlayerPrefs.GetInt("privacyagreementversion") > Configuration.instance.privacyAgreementVersion ) {

				Configuration.instance.privacyAgreementVersion = PlayerPrefs.GetInt("privacyagreementversion");
				Configuration.instance.privacyAgreement = PlayerPrefs.GetString("privacyagreement");

			}

		}

		if ( PlayerPrefs.HasKey("agbsversion") ) {
			
			if ( PlayerPrefs.GetInt("agbsversion") > Configuration.instance.privacyAgreementVersion ) {
				
				Configuration.instance.agbsVersion = PlayerPrefs.GetInt("agbsversion");
				Configuration.instance.agbs = PlayerPrefs.GetString("agbs");
				
			}
			
		}

		if ( PlayerPrefs.HasKey("privacyAgreementVersionRead") ) {

			privacyAgreementVersionRead = PlayerPrefs.GetInt("privacyAgreementVersionRead");

		}


		GameObject.Find("ImpressumCanvas").GetComponent<showimpressum>().loadAGBs();
		GameObject.Find("ImpressumCanvas").GetComponent<showimpressum>().loadPrivacy();


		bool hideBlack = true;

		if ( Configuration.instance.showPrivacyAgreement ) {
			hideBlack = false;
			StartCoroutine(showPrivacyAgreement());
		}

		if ( PlayerPrefs.HasKey("agbVersionRead") ) {
			
			agbVersionRead = PlayerPrefs.GetInt("agbVersionRead");
			
		}
		
		if ( Configuration.instance.showAGBs ) {
			hideBlack = false;
			StartCoroutine(showAGBs());
			
		}

		if ( hideBlack ) {

			hideBlackCanvas();

		}



		if ( Configuration.instance.checkForAppversion ) {


			CheckForQuestVersion();

		}



		PATH_2_PREDEPLOYED_QUESTS = System.IO.Path.Combine(Application.streamingAssetsPath, "predeployed/quests");
		PREDEPLOYED_QUESTS_ZIP = System.IO.Path.Combine(Application.streamingAssetsPath, "predeployed/quests.zip");
		LOCAL_QUESTS_ZIP = System.IO.Path.Combine(Application.persistentDataPath, "tmp_predeployed_quests.zip");
		PATH_2_LOCAL_QUESTS = System.IO.Path.Combine(Application.persistentDataPath, "quests");

		//msgsactive = 0;

#if (UNITY_ANDROID && !UNITY_EDITOR)

		PREDEPLOYED_QUESTS_ZIP = "jar:file://" + Application.dataPath + "!/assets/" + "/predeployed/quests.zip";
		PATH_2_PREDEPLOYED_QUESTS = "jar:file://" + Application.dataPath + "!/assets/predeployed/quests";

		// PATH_2_PREDEPLOYED_QUESTS = "file:///android_asset/predeployed/quests"; NOT WORKING

#endif

		if ( GameObject.Find("QuestDatabase") != gameObject ) {
			Destroy(GameObject.Find("QuestDatabase"));		
		}
		else {
			DontDestroyOnLoad(gameObject);
			//			Debug.Log (Application.persistentDataPath);
		}

		if ( !Application.isWebPlayer ) {

			if ( Configuration.instance.questvisualization != "list" ) {
				GameObject.Find("ListPanel").SetActive(false);

			}


			
			if ( Configuration.instance.downloadAllCloudQuestOnStart || (Configuration.instance.showcloudquestsimmediately && Configuration.instance.autostartQuestID == 0) ) {
				buttoncontroller.DisplayList();

				ReloadQuestListAndRefresh();
			}
			else {
				if ( Configuration.instance.autostartQuestID != 0 ) {
					buttoncontroller.DisplayList();
				}

				webloadingmessage.enabled = false;
				loadlogo.disable();
			}
		}
		else {
			if ( webloadingmessage != null ) {
				webloadingmessage.enabled = true;
			}
			questmilllogo.enabled = true;
			loadlogo.enable();
		} 

		autoStartQuest();

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		if ( GameObject.Find("MenuCanvas") != null ) {
			
			menu = GameObject.Find("MenuCanvas").GetComponent<menucontroller>();
			
		}

	}

	

	public void CheckForQuestVersion () {


		string url = Configuration.instance.appVersionURL + "?version=" + Configuration.instance.appVersion;
		Debug.Log(url);
		Download download = new Download(url, timeout: 20000);
		download.OnSuccess = (Download.SuccessCallback)((Download d) => {

			string[] split = d.Www.text.Split(';');

			int num;
			if ( int.TryParse(split[0], out num) ) {
				
				if ( num > Configuration.instance.appVersion ) {

					if ( split[1].Equals("true") ) {
						showmessage(split[2], "Okay");
					}
					else {

						StartCoroutine(blockApp(split[2]));

					}

				}
			}
		});

		StartCoroutine(download.startDownload());
	}


	IEnumerator blockApp (string s) {
		yield return null;
		loadlogo.enable();
		webloadingmessage.enabled = true;
		webloadingmessage.text = s;
		StartCoroutine(blockApp(s));

	}

	public void ReloadQuestListAndRefresh () {
		allquests.Clear();
//		Debug.Log ("starting showing quests immediately (and no autostart)");
		string url = "http://qeevee.org:9091/json/" + Configuration.instance.portalID + "/publicgamesinfo";
		Download download = new Download(url, timeout: 20000);
		download.OnStart = new Download.StartCallback(whenQuestListDownloadStarts);
		download.OnProgress = new Download.ProgressUpdate(updateProgress);
		download.OnSuccess = new Download.SuccessCallback(updateAndShowQuestList) +
		new Download.SuccessCallback(whenQuestListDownloadSucceeds) +
		new Download.SuccessCallback(downloadAllQuests);
		download.OnError = new Download.ErrorCallback(retryAfterDownloadError);
		StartCoroutine(download.startDownload());
	}

	void retryAfterDownloadError (Download download, string msg) {
		if ( Configuration.instance.offlinePlayable && localquests != null && localquests.Count > 0 ) {
			webloadingmessage.enabled = false;
			loadlogo.disable();
			updateAndShowQuestList(download);
			whenQuestListDownloadSucceeds(download);
			downloadAllQuests(download);
		}
		else {
			Action retryAction = new Action(() => { 
				webloadingmessage.enabled = true;
				loadlogo.enable();
				ReloadQuestListAndRefresh();
			});
			string alertMsg = "Wir konnten keine Verbindung mit dem Internet herstellen.";
			if ( Configuration.instance.offlinePlayable && (localquests == null || localquests.Count == 0) ) {
				alertMsg = "Keine Daten vorhanden. Internetverbindung erforderlich.";
			}
			showmessage(alertMsg, "Erneut versuchen", retryAction); 
		}
	}

	public void hideBlackCanvas () {
		if ( GameObject.Find("[BLACK]") != null ) {
			GameObject.Find("[BLACK]").GetComponent<Animator>().SetTrigger("out");
		}
	}

	IEnumerator showPrivacyAgreement () {

		WWW www = new WWW("http://qeevee.org:9091/" + Configuration.instance.portalID + "/privacyagreement/version");
		yield return www;


		if ( www.error != null && www.error != "" ) {

			Debug.Log("Couldn't load privacy agreement: " + www.error);
			if ( Configuration.instance.privacyAgreementVersion > privacyAgreementVersionRead ) {
				GameObject.Find("ImpressumCanvas").GetComponent<showimpressum>().loadPrivacy();
				
				privacyAgreementObject.version = Configuration.instance.privacyAgreementVersion;
				privacyAgreementObject.gameObject.SetActive(true);
				
				
				GetComponent<actions>().localizeStringToDictionary(Configuration.instance.privacyAgreement);
				privacyAgreementObject.textObject.text = GetComponent<actions>().formatString(GetComponent<actions>().localizeString(Configuration.instance.privacyAgreement));
				privacyAgreementObject.GetComponent<Animator>().SetTrigger("in");
			}
			hideBlackCanvas();

		}
		else {

		
			string version = www.text;
			Debug.Log("Privacy Agreement Version: " + version);




			if ( int.Parse(version) > privacyAgreementVersionRead || Configuration.instance.privacyAgreementVersion > privacyAgreementVersionRead ) {


				string agreement = Configuration.instance.privacyAgreement;

				if ( int.Parse(version) > Configuration.instance.privacyAgreementVersion ) {

					WWW www2 = new WWW("http://qeevee.org:9091/" + Configuration.instance.portalID + "/privacyagreement");
					yield return www2;
					agreement = www2.text;

					PlayerPrefs.SetInt("privacyagreementversion", int.Parse(version));
					PlayerPrefs.SetString("privacyagreement", agreement);

					Configuration.instance.privacyAgreementVersion = int.Parse(version);
					Configuration.instance.privacyAgreement = agreement;
					GameObject.Find("ImpressumCanvas").GetComponent<showimpressum>().loadPrivacy();



				}
				else {
					version = Configuration.instance.privacyAgreementVersion.ToString();
					agreement = Configuration.instance.privacyAgreement;
				}


				privacyAgreementObject.version = int.Parse(version);
				privacyAgreementObject.gameObject.SetActive(true);


				GetComponent<actions>().localizeStringToDictionary(agreement);
				privacyAgreementObject.textObject.text = GetComponent<actions>().formatString(GetComponent<actions>().localizeString(agreement));
				privacyAgreementObject.GetComponent<Animator>().SetTrigger("in");
			}
			else {
				hideBlackCanvas();

			}




		}



	}

	IEnumerator showAGBs () {
		WWW www = new WWW("http://qeevee.org:9091/" + Configuration.instance.portalID + "/agbs/version");
		yield return www;
		
		
		if ( www.error != null && www.error != "" ) {
			
			Debug.Log("Couldn't load privacy agreement: " + www.error);
			if ( Configuration.instance.agbsVersion > agbVersionRead ) {
				GameObject.Find("ImpressumCanvas").GetComponent<showimpressum>().loadAGBs();

				agbObject.version = Configuration.instance.agbsVersion;
				agbObject.gameObject.SetActive(true);
				
				
				GetComponent<actions>().localizeStringToDictionary(Configuration.instance.agbs);
				agbObject.textObject.text = GetComponent<actions>().formatString(GetComponent<actions>().localizeString(Configuration.instance.agbs));
				agbObject.GetComponent<Animator>().SetTrigger("in");
				
				
			}
			hideBlackCanvas();
		}
		else {
			string version = www.text;

			if ( int.Parse(version) > agbVersionRead || Configuration.instance.agbsVersion > agbVersionRead ) {
				
				string agreement = Configuration.instance.agbs;
				
				if ( int.Parse(version) > Configuration.instance.agbsVersion ) {
					
					WWW www2 = new WWW("http://qeevee.org:9091/" + Configuration.instance.portalID + "/agbs");
					yield return www2;
					// TODO use Download class instead. Here wo forget to check whether it is already done...
					agreement = www2.text;

					PlayerPrefs.SetInt("agbsversion", int.Parse(version));
					PlayerPrefs.SetString("agbs", agreement);
					Configuration.instance.agbsVersion = int.Parse(version);
					Configuration.instance.agbs = agreement;
					GameObject.Find("ImpressumCanvas").GetComponent<showimpressum>().loadAGBs();

					agbObject.version = int.Parse(version);

				}
				else {
					agbObject.version = Configuration.instance.agbsVersion;

				}

				agbObject.gameObject.SetActive(true);
				
				
				GetComponent<actions>().localizeStringToDictionary(agreement);
				agbObject.textObject.text = GetComponent<actions>().formatString(GetComponent<actions>().localizeString(agreement));
				agbObject.GetComponent<Animator>().SetTrigger("in");
			}
			else {
				hideBlackCanvas();
				
			}

			
			
			
			
		}
		
	}

	void autoStartQuest () {

		if ( Configuration.instance.autostartQuestID != 0 ) {
			Debug.Log("Autostart: Starting quest " + Configuration.instance.autostartQuestID);
			GameObject questListPanel = GameObject.Find("/Canvas");
			if ( loadlogo != null ) {
				Debug.Log("Autostart: loadlogo != null ");

				loadlogo.enable();
				webloadingmessage.enabled = true;
			}
			else {
				Debug.Log("Autostart: loadlogo == null ");
			}

			if ( Configuration.instance.autostartIsPredeployed ) {
				Debug.Log("Autostart: is predeployed ");
				StartCoroutine(startPredeployedQuest(Configuration.instance.autostartQuestID));

			}
			else {
				Debug.Log("Autostart: is NOT predeployed ");
				StartQuest(Configuration.instance.autostartQuestID);
			}
		}

	}

	IEnumerator InitPredeployedQuests () {

		#if !UNITY_WEBPLAYER
		if ( webloadingmessage != null ) {

			webloadingmessage.text = "Lade...";
		}
		//webloadingmessage.enabled = true;
		questmilllogo.enabled = true;
	
		yield return null;
		
		Debug.Log("InitPredeployedQuests 1, looking for predep zip: " + PREDEPLOYED_QUESTS_ZIP);
		if ( PREDEPLOYED_QUESTS_ZIP.Contains("://") ) {
			// on platforms which use an url type as asset path (e.g. Android):
			Debug.Log("WWW questZIP = new WWW (PREDEPLOYED_QUESTS_ZIP);");
			WWW questZIP = new WWW(PREDEPLOYED_QUESTS_ZIP); // this is the path to your StreamingAssets in android
			yield return questZIP;
			
			Debug.Log("PDir3: LOADED BY WWW. questZIP.text: " + questZIP.text);
			if ( questZIP.error != null && !questZIP.error.Equals("") ) {
				Debug.Log("PDir3: LOADED BY WWW. questZIP.error: " + questZIP.error);
			}
			else {				
				Debug.Log("PDir3: LOADED BY WWW. questZIP.bytesDownloaded: " + questZIP.bytesDownloaded);
				if ( File.Exists(LOCAL_QUESTS_ZIP) ) {
					Debug.LogWarning("Local copy of predeployment zip file was found. Should have been deleted at last initialization.");
					File.Delete(LOCAL_QUESTS_ZIP);
				}
			}
			File.WriteAllBytes(LOCAL_QUESTS_ZIP, questZIP.bytes);
			Debug.Log("PDir3: ZIP FILE WRITTEN - ok? : " + File.Exists(LOCAL_QUESTS_ZIP));
			predepzipfound = true;
		}
		else {
			Debug.Log("Not running on platforms which use an url type as asset path (e.g. Android):");
			initPreloadedQuestiOS();
		}


		if ( predepzipfound ) {
		
			ZipUtil.Unzip(LOCAL_QUESTS_ZIP, Application.persistentDataPath);
			File.Delete(LOCAL_QUESTS_ZIP);
			Debug.Log("ZIP FILE DELETED");
			questmilllogo.enabled = false;
			if ( webloadingmessage != null ) {

				webloadingmessage.enabled = false;
			}
			autoStartQuest();
		}

#else
		yield return null;
#endif
	}

	void initPreloadedQuestiOS () {

		// on platforms which have a straight file path (e.g. iOS):
		Debug.Log("InitPredeployedQuests: on IOS.");
		if ( !File.Exists(PREDEPLOYED_QUESTS_ZIP) ) {
			Debug.Log("InitPredeployedQuests: ZIP FILE NOT FOUND");
			return;
		}
		File.Copy(PREDEPLOYED_QUESTS_ZIP, LOCAL_QUESTS_ZIP);
		if ( !File.Exists(LOCAL_QUESTS_ZIP) ) {
			Debug.Log("InitPredeployedQuests: LOCAL COPY NOT CREATED.");
		}

		predepzipfound = true;


	}

	private int reloadButtonPressed = 0;
	private int numberOfPressesNeededToReload = 10;

	public bool ReloadButtonPressed () {
		reloadButtonPressed++;
		if ( reloadButtonPressed >= numberOfPressesNeededToReload ) {
			reloadButtonPressed = 0;
			reloadAutoStartQuest();
			return true;
		}
		else {
			int remainingPresses = numberOfPressesNeededToReload - reloadButtonPressed;
//			showmessage ("Wenn sie diesen Button noch " + (remainingPresses) + " mal drücken werden alle Medien gelöscht und neu geladen.", "OK");
			return false;
		}
	}

	public void reloadAutoStartQuest () {





		List<Quest> alllocalquests = new List<Quest>();
		alllocalquests.AddRange(localquests);

		foreach ( Quest lq in alllocalquests ) {

				
				
			removeQuest(lq);
				
				

		}

		localquests.Clear();



		Application.LoadLevel(0);
	
		GameObject.Find("ImpressumCanvas").GetComponent<showimpressum>().closeImpressum();



	}

	bool IsQuestInitialized (int id) {
		string questDirPath = System.IO.Path.Combine(PATH_2_LOCAL_QUESTS, id.ToString());
		return Directory.Exists(questDirPath);
	}

	void accessData (JSONObject obj, string kei) {
		switch ( obj.type ) {
			
			
			case JSONObject.Type.OBJECT:
			
				for ( int i = 0; i < obj.list.Count; i++ ) {
					string key = (string)obj.keys[i];
					JSONObject j = (JSONObject)obj.list[i];
					accessData(j, kei + "_" + key);
				}
				break; 
			case JSONObject.Type.ARRAY:
			//			Debug.Log("ARRAY: "+kei);
				if ( kei == "quest_hotspots" ) {

					currentquest = new Quest();
					if ( allquests == null ) {
						allquests = new List<Quest>();
					}
					allquests.Add(currentquest);
				
					foreach ( JSONObject j in obj.list ) {
						accessData(j, kei);
					}
				
					//getFirstHotspot (obj);
				
				}
				else
				if ( kei == "quest" ) {
					//Debug.Log("here");
				
				
					foreach ( JSONObject j in obj.list ) {
						accessData(j, kei);
					}
				
				
				}
				if ( kei == "quest_metadata" ) {
					foreach ( JSONObject j in obj.list ) {
						createMetaData(j);
					}
				} 
			
				break;
			case JSONObject.Type.STRING:
				if ( kei == "quest_name" ) {
					currentquest.name = obj.str;
				}
				break;
			case JSONObject.Type.NUMBER:
				if ( kei == "quest_id" ) {
					currentquest.id = (int)obj.n;
				}
				else
				if ( kei == "quest_lastUpdate" ) {
					currentquest.lastUpdate = (long)obj.n;
				}
				else
				if ( kei == "quest_hotspots_latitude" ) {
				
					if ( currentquest.start_latitude == null || currentquest.start_latitude == 0 ) {
					
						currentquest.start_latitude = obj.n;
					
					}
				
				}
				else
				if ( kei == "quest_hotspots_longitude" ) {
					//Debug.Log("FOUND LONGITUDE");
				
					if ( currentquest.start_longitude == null || currentquest.start_longitude == 0 ) {
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

	void accessHotspotData (JSONObject obj) {
		
		
		for ( int i = 0; i < obj.list.Count; i++ ) {
			string key = (string)obj.keys[i];
			JSONObject j = (JSONObject)obj.list[i];
			
			Debug.Log(key);
			if ( key == "longitude" ) {
				
				Debug.Log("long found");
				if ( currentquest.start_longitude == null || currentquest.start_longitude == 0 ) {
					currentquest.start_longitude = obj.n;
					Debug.Log("long set: " + obj.str);
				}
				
			}
			else
			if ( key == "latitude" ) {
				Debug.Log("lat found");
				
				if ( currentquest.start_latitude == null || currentquest.start_latitude == 0 ) {
					currentquest.start_latitude = obj.n;
				}
				
			}
			
			
		}
		
		
	}

	void createMetaData (JSONObject obj) {
		
		QuestMetaData meta = new QuestMetaData();
		
		for ( int i = 0; i < obj.list.Count; i++ ) {
			string key = (string)obj.keys[i];
			JSONObject j = (JSONObject)obj.list[i];
			
			
			
			if ( key == "key" ) {
				
				meta.key = j.str;
				
			}
			else
			if ( key == "value" ) {
				//				Debug.Log("Meta Value found");
				meta.value = j.str;
				
			}
			
			
		}
		currentquest.addMetaData(meta);
		
		
	}


	void updateAndShowQuestList (Download download) {

//		Debug.Log ("UPDATE AND SHOW QUEST LIST");

		WWW www = download.Www;

		buttoncontroller.filteredOnlineList.Clear();
		
		allquests.Clear();
		JSONObject j = new JSONObject(www.text);
		accessData(j, "quest");
		foreach ( Quest q in allquests ) {
			buttoncontroller.filteredOnlineList.Add(q);
		}

		currentquest = null;
		
		if ( Configuration.instance.questvisualization == "list" ) {
			
			buttoncontroller.DisplayList();
			
		}
		else
		if ( Configuration.instance.questvisualization == "map" ) {
			
			showQuestMap();
			
		}
		if ( loadlogo != null ) {
			
			loadlogo.disable();
		}
	}

	void whenQuestListDownloadStarts (Download d) {
		//Debug.Log ("DOWNLOAD of QUEST LIST STARTED");
		if ( loadlogo != null ) {
			loadlogo.enable();
		}
		if ( webloadingmessage != null ) {
			webloadingmessage.enabled = true;
		}
	}

	void whenQuestListDownloadSucceeds (Download d) {
		if ( loadlogo != null ) {
			loadlogo.disable();
		}
		if ( webloadingmessage != null ) {
			webloadingmessage.enabled = false;
		}
	}

	void downloadAllQuests (Download d) {

		bool hasNoLocalQuestsYet = true;

		localquests = GetLocalQuests();

		if ( localquests != null && localquests.Count > 0 ) {
			hasNoLocalQuestsYet = false;
		}

		if ( !downloadedAll && Configuration.instance.downloadAllCloudQuestOnStart ) {
			downloadingAll = true;

			if ( hasNoLocalQuestsYet ) {
				questsToLoad = allquests.Count;

				foreach ( Quest q in allquests ) {


					downloadQuest(q);


				}
			}
			else {


				if ( PlayerPrefs.HasKey("lastUpdatedAllQuests") ) {
					//Store the current time when it starts
					DateTime currentDate = System.DateTime.Now;

					//Grab the old time from the player prefs as a long
					long temp = Convert.ToInt64(PlayerPrefs.GetString("lastUpdatedAllQuests"));

					//Convert the old time from binary to a DataTime variable
					DateTime oldDate = DateTime.FromBinary(temp);

					//Use the Subtract method and store the result as a timespan variable
					TimeSpan difference = currentDate.Subtract(oldDate);


					if ( difference.Days >= 1 ) {

						askUserForUpdatingQuests();


					}


				}
				else {

				
					askUserForUpdatingQuests();


				}


			}

			downloadedAll = true;


		}
		else
		if ( Configuration.instance.downloadAllCloudQuestOnStart ) {

			if ( webloadingmessage != null ) {

				webloadingmessage.enabled = false;
			}
			if ( loadlogo != null ) {

				loadlogo.disable();
			}

		}
	}



	public void askUserForUpdatingQuests () {

		PlayerPrefs.SetString("lastUpdatedAllQuests", System.DateTime.Now.ToBinary().ToString());


		if ( questsHaveUpdates() ) {


			updateQuestsMessage.gameObject.SetActive(true);
			updateQuestsMessage.GetComponent<Animator>().SetTrigger("in");



		}



	}



	/// <summary>
	/// 1. Deletes local quests that are no more on the server.
	/// 2. Looks for newer version of local quests and updates them. Additionally loads all "new" quests from the server.
	/// </summary>
	public void updateAllQuests () {




		/////////////////////////
		// 1. delete quests locally that are no more on the server:
		foreach ( Quest lq in localquests.GetRange(0, localquests.Count) ) {
			if ( allquests.FindIndex(x => x.id == lq.id) == -1 ) {
				// lq was not loaded from server, ehnce we delete it locally:
				removeQuest(lq);
			}

		}

		/////////////////////////
		// 2. get new quests from server and updates of existing quests:
		questsToLoad = 0;
		bool foundChanges = false;
		foreach ( Quest q in allquests ) {

			bool alreadyLocal = false;
		
			foreach ( Quest lq in localquests.GetRange(0, localquests.Count) ) {
				if ( lq.id == q.id ) {
					alreadyLocal = true;

					// update new versions of existing local quests:
					// sorry for the 100sec increase - we have such a stuodi json parser TODO
					if ( lq.getLastUpdate() + 100000 < q.getLastUpdate() ) {
						Debug.Log("<color=yellow>id: " + q.id + " times: " + lq.getLastUpdate() + "=" + q.getLastUpdate() + "</color>");

						removeQuest(lq);
						downloadQuest(q);
						foundChanges = true;
						questsToLoad += 1;
					}
					break;
				}
			}

			// load new quests from server:
			if ( !alreadyLocal ) {
				foundChanges = true;
				downloadQuest(q);
				questsToLoad += 1;

				Debug.Log("<color=yellow>Loading new: " + q.id + "</color>");

			}

		}

		if ( !foundChanges ) {
			
			backToMenuAfterDownloadedAll();

		}
		else {
			
			if ( GameObject.Find("PageController_Map") != null ) {
				hotspots = getActiveHotspots();
				GameObject.Find("PageController_Map").GetComponent<page_map>().updateMapMarkerInFoyer();
			}
		}
	}



	/// <summary>
	/// Looks for newer version of local quests and returns if there are any newer versions of local quests.
	/// </summary>
	public bool questsHaveUpdates () {

		bool foundChanges = false;

		/////////////////////////
		// 1. delete quests locally that are no more on the server:
		foreach ( Quest lq in localquests.GetRange(0, localquests.Count) ) {
			if ( allquests.FindIndex(x => x.id == lq.id) == -1 ) {
				// lq was not loaded from server, ehnce we delete it locally:
				foundChanges = true;
				return foundChanges;
			}

		}



		/////////////////////////
		// 2. get new quests from server
		foreach ( Quest q in allquests ) {

			bool alreadyLocal = false;

			foreach ( Quest lq in localquests.GetRange(0, localquests.Count) ) {
				if ( lq.id == q.id ) {
					alreadyLocal = true;

					// update new versions of existing local quests:
					// sorry for the 100sec increase - we have such a stuodi json parser TODO
					if ( lq.getLastUpdate() + 100000 < q.getLastUpdate() ) {

						foundChanges = true;
					}
					break;
				}
			}

			// load new quests from server?
			if ( !alreadyLocal ) {
				foundChanges = true;

			}

		}


		return foundChanges;
	}


	void updateProgress (Download download, float progress) {
		if ( webloadingmessage != null ) {
			webloadingmessage.text = String.Format("{0:N2}% loaded", progress * 100f);
		}
	}

	IEnumerator startPredeployedQuest (int id) {


		Quest q = new Quest();

		q.id = id;
		q.predeployed = true;

		q.filepath = PATH_2_PREDEPLOYED_QUESTS + "/" + id + "/game.xml";

		Debug.Log("PROBLEM: startPredeployedQuest");
		currentquest = q;

//		currentquestdata = (Transform)Instantiate(questdataprefab, transform.position, Quaternion.identity);



		WWW wwwpdq = LocalWWW.Create(q.filepath);

		yield return wwwpdq;

		if ( wwwpdq.error == null ) {
			

			
			
			//currentquestdata = (Transform)Instantiate (questdataprefab, transform.position, Quaternion.identity);
			q.xmlcontent = UTF8Encoding.UTF8.GetString(wwwpdq.bytes); 

		}
		else {

			Debug.Log("Error: " + wwwpdq.error + "www.url=" + www.url);
		}

		Debug.Log("PROBLEM: before LoadFromText (q.name = " + q.name + ", q.id = " + q.id + ")");
		currentquest = q.LoadFromText(id, true);
		localquests.Add(currentquest);
		bool hasmorethanmetadata = true;
		currentquest.currentpage = currentquest.pages.First();
		int c = 0;
		while ( currentquest.currentpage.type == "MetaData" ) {
			
			if ( currentquest.pages.Count >= c - 1 ) {
				currentquest.currentpage = currentquest.pages[c];
				c++;
			}
			else {
				
				hasmorethanmetadata = false;
				break;
			}
		}
		
		hotspots = new List<QuestRuntimeHotspot>();
		foreach ( QuestHotspot qh in currentquest.hotspots ) {
			bool initialActivity = qh.hasAttribute("initialActivity") && qh.getAttribute("initialActivity") == "true";
			bool initialVisibility = qh.hasAttribute("initialVisibility") && qh.getAttribute("initialVisibility") == "true";
			
			hotspots.Add(new QuestRuntimeHotspot(qh, initialActivity, initialVisibility, qh.latlon));
		}
		
		if ( canPlayQuest(currentquest) && hasmorethanmetadata ) {

			StartCoroutine(waitForSpriteConversion(currentquest.currentpage.id));

		}
		else {
			
			showmessage("Entschuldigung! Die Quest kann in dieser Version nicht abgespielt werden.");
			if ( webloadingmessage != null ) {

				webloadingmessage.enabled = false;
			}
			if ( loadlogo != null ) {

				loadlogo.disable();
			}
			GameObject.Find("List").GetComponent<createquestbuttons>().resetList();
			
		}
	}

	public void showQuestMap () {
		
		hotspots = new List<QuestRuntimeHotspot>();
		hotspots.AddRange(getActiveHotspots());

		GameObject.Find("MenuCanvas").GetComponent<Animator>().SetTrigger("startMenu");
		GameObject.Find("BgCam").GetComponent<Camera>().enabled = false;
		SceneManager.LoadSceneAsync("page_map", LoadSceneMode.Additive);
	
		if ( webloadingmessage != null ) {
			webloadingmessage.enabled = false;
		}

	}

	public void StartQuest (int id) {
		Debug.Log("StartQuest(): " + id);

		// if the given quest is already intialized start it, otherwise download it first and start it:
		List<Quest> localQuests = GetLocalQuests();
		Quest q = null;
		foreach ( Quest curQuest in localQuests ) {
			
			Debug.Log("StartQuest(): foreach cur=" + curQuest.id);
			if ( curQuest.id == id ) {
				q = curQuest;
			}
			
		}
		
		if ( q == null ) {
//			Debug.Log ("Problem 1 id: " + id);
			q = new Quest();
			q.id = id;
			Debug.Log("StartQuest(): downloading quest " + q.name);
			downloadQuest(q);
		}
		else {
			Debug.Log("StartQuest(): starting quest " + q.name);
			startQuest(q);
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
	IEnumerator DownloadQuestAfterConnectionCheck (Quest q, float elapsedTime, WWW wwwTestConn) {

		while ( msgsactive > 0 ) {
			yield return 0;
			//Debug.Log("messages active");
		}


//		Debug.Log ("CheckConnection(): before ping");
		yield return new WaitForSeconds(0.1f);
		if ( wwwTestConn.isDone ) {
			bool ok = (wwwTestConn.error == null);
			downloadAfterConnectionChecked(q, ok);
		}
		else {
			if ( elapsedTime < 10.0f ) {
				StartCoroutine(DownloadQuestAfterConnectionCheck(q, elapsedTime + 0.1f, wwwTestConn));
			}
			else {
				downloadAfterConnectionChecked(q, false);
			}
		}
	}

	static public void CopyFolder (string sourceFolder, string destFolder) {
		if ( !Directory.Exists(destFolder) ) {
			Directory.CreateDirectory(destFolder);
		}
		string[] files = Directory.GetFiles(sourceFolder);
		foreach ( string file in files ) {
			string name = Path.GetFileName(file);
			if ( !name.EndsWith(".meta") ) {
				string dest = Path.Combine(destFolder, name);
				File.Copy(file, dest);
			}
		}
		string[] folders = Directory.GetDirectories(sourceFolder);
		foreach ( string folder in folders ) {
			string name = Path.GetFileName(folder);
			string dest = Path.Combine(destFolder, name);
			CopyFolder(folder, dest);
		}
	}

	public List<QuestRuntimeHotspot> getActiveHotspots () {

	

		List<QuestRuntimeHotspot> activehs = new List<QuestRuntimeHotspot>();



//		Debug.Log ("Current Quest: "+currentquest.id);

		if ( currentquest != null && currentquest.id != 0 ) {

			foreach ( QuestRuntimeHotspot qrh in hotspots ) {
			
			
			
				if ( qrh.active ) {

					activehs.Add(qrh);
				}


			}
		}
		else {

			List<int> visitedQuests = new List<int>();

			foreach ( Quest aq in localquests ) {
				if ( aq.hotspots != null && aq.hotspots.Count > 0 ) {
					QuestRuntimeHotspot qrh = new QuestRuntimeHotspot(aq.hotspots[0], true, true, aq.hotspots[0].latlon);
					if ( aq.hasMeta("category") ) {

						qrh.category = aq.getMeta("category");

					}

					qrh.startquest = aq;

					activehs.Add(qrh);
					visitedQuests.Add(aq.id);
				}
			}

			if ( Configuration.instance.cloudQuestsVisible )
				foreach ( Quest aq in allquests ) {
					if ( visitedQuests.Contains(aq.id) )
						continue;
				
					if ( aq.start_longitude != 0f ) {
						QuestHotspot qh = new QuestHotspot();
				
						QuestAttribute qa = new QuestAttribute("radius", "20");

						qh.attributes = new List<QuestAttribute>();
						qh.attributes.Add(qa);
						QuestRuntimeHotspot qrh = new QuestRuntimeHotspot(qh, true, true, aq.start_latitude + "," + aq.start_longitude);
						//Debug.Log("Longitude Latitude: "+aq.start_longitude+","+aq.start_latitude);

						if ( aq.hasMeta("category") ) {

							qrh.category = aq.getMeta("category");

						}

						qrh.startquest = aq;

						activehs.Add(qrh);
					}
				}




		}

		return activehs;
	}

	public GeoPosition getQuestCenter () {
		float centerLat = 0f;
		float centerLong = 0f;
		int numberofUsedHotspots = 0;

		foreach ( QuestRuntimeHotspot curHotspot in hotspots ) {
			if ( curHotspot.lat != 0f && curHotspot.lon != 0f ) {
				centerLat += curHotspot.lat;
				centerLong += curHotspot.lon;
				numberofUsedHotspots++;
			}
		}

		if ( numberofUsedHotspots > 0 ) {
			centerLat = centerLat / numberofUsedHotspots;
			centerLong = centerLong / numberofUsedHotspots;
		}

		return new GeoPosition(centerLat, centerLong);
	}

	public double[] getQuestCenterPosition () {
		GeoPosition center = getQuestCenter();
		
		return new double[] {
			center.Lat,
			center.Long
		};
	}

	void Update () {
		if ( fakebytes > 0 && fakebytes < (int.MaxValue - 1000) ) {

			fakebytes += Time.deltaTime;

		}

		if ( Input.GetKey(KeyCode.G) && Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.Q) ) {
			Debug.Log("Destroying GameObject");

			Destroy(gameObject);
			endQuest();
		

		}

	}

	public void debug (string s) {
		
//		Debug.Log (s);
		if ( Application.isWebPlayer ) {
			

				
			Application.ExternalCall("unitydebug", s + "<br/><br/>");
				

		}
		
	}

	public void passWebXml (string x) {


		if ( currentquestdata != null ) {

			Destroy(currentquestdata.gameObject);
						
					
				
		} 

		actioncontroller.reset();
		actioncontroller.sendVartoWeb();
		
		if ( webloadingmessage != null ) {
			webloadingmessage.text = "Loading...";
			webloadingmessage.enabled = true;
			if ( loadlogo != null ) {

				loadlogo.enable();	
			}
		}

		www = new WWW(x);
		StartCoroutine(waitForWebXml());
				
	}

	public void resetPlayer (string x) {
		Debug.Log("Destroying GameObject");
		Destroy(gameObject);
		Application.LoadLevel(0);

	
		
	}

	IEnumerator waitForWebXml () {

		yield return www;

		if ( www.error == null ) {

			Quest nq = new Quest();


		
//			currentquestdata = (Transform)Instantiate(questdataprefab, transform.position, Quaternion.identity);

			nq.xmlcontent = UTF8Encoding.UTF8.GetString(www.bytes); 
			//ASCIIEncoding.ASCII.GetString (Encoding.Convert (Encoding.UTF32, Encoding.ASCII, www.bytes)); 

			if ( !downloadingAll ) {
				Debug.Log("PROBLEM: waitForWebXml");
				currentquest = nq;
			}

			installQuest(nq, false, false);

		}
		else {
			debug(www.error);

		}
	}


	public void startQuestAtEndOfFrame (Quest q) {


		StartCoroutine(startQuestAtEndOfFrameCoRoutine(q));

	}

	public IEnumerator startQuestAtEndOfFrameCoRoutine (Quest q) {

		yield return new WaitForEndOfFrame();

		yield return new WaitForEndOfFrame();

		startQuest(q);

	}

	public void startQuest (Quest q) {
		if ( !Configuration.instance.hasMenuWithinQuests ) {
			if ( menuButton != null ) {
				menuButton.GetComponent<Image>().enabled = false;
				menuButton.GetComponent<Button>().enabled = false;
			}
		}

		closeMap();

//		currentquestdata = (Transform)Instantiate(questdataprefab, transform.position, Quaternion.identity);

		bool islocal = false;
		Quest localq;

		foreach ( Quest lq in localquests ) {
		
			if ( lq.id == q.id ) {
				islocal = true;
				q = lq;
			}

		}

		currentquest = q;

		if ( !islocal ) {
			downloadQuest(q);
		}
		else {
			if ( q.xmlcontent == null || q.xmlcontent.Trim().Equals("") ) {
				initiateQuestStart(true, q);
			}
			else {
				installQuest(q, true, true);
			}
		}
			 
	}

	public void removeQuest (Quest q) {



						
		#if UNITY_WEBPLAYER

			Debug.Log("cannot remove on web");

		# else 
		if ( Directory.Exists(Application.persistentDataPath + "/quests/" + q.id) ) {
			Directory.Delete(Application.persistentDataPath + "/quests/" + q.id, true);


		}
#endif
		localquests.Remove(q);
		foreach ( QuestRuntimeHotspot curH in hotspots.GetRange(0, hotspots.Count) ) {
			if ( curH.startquest != null && curH.startquest.id == q.id ) {
				if ( curH.renderer != null )
					Destroy(curH.renderer.gameObject);
				hotspots.Remove(curH);
			}
		}

		if ( currentquest != null ) {
			if ( currentquest.id == q.id ) {

				Debug.Log("PROBLEM: removeQuest()");
				currentquest = null;

			}
		}

		

	
		if ( buttoncontroller != null ) {

			buttoncontroller.resetList();

		}


	}

	public void endQuest () {
		if ( currentquestdata != null ) {
			Destroy(currentquestdata.gameObject);
		}
		Destroy(GameObject.Find("MsgCanvas"));
		Destroy(gameObject);
		if ( menu.isActive ) {
			menu.endQuestAnimation();
		}
		else {

			returnToMainMenu();

		}
	}

	public void returnToMainMenu () {

		Application.LoadLevel(0);


	}

	public void retryAllOpenWWW () {

		List<WWW> todelete = new List<WWW>();
		foreach ( WWW awww in filedownloads ) {

			todelete.Add(awww);

		}
	

	}

	// TODO parameter should only be the id of the quest (hm)
	public void downloadQuest (Quest q) {
		Debug.Log("downloadQuest(Quest with id:" + q.id + ")");
		if ( webloadingmessage != null ) {

			webloadingmessage.enabled = true;
		}

		if ( questmilllogo != null ) {
			questmilllogo.enabled = true;
		}
		if ( loadlogo != null ) {

			loadlogo.enable();
		}

		if ( Configuration.instance.showinternetconnectionmessage &&
		     (q.alternateDownloadLink == "" || q.alternateDownloadLink == null) &&
		     msgsactive == 0 ) {
			showmessage("Wir empfehlen eine gute WLAN Verbindung um alle Medien zu laden.", "OK"); // TODO anpassen z. Bsp. für DownloadAllQuest auto)
		}
//		Debug.Log ("downloadQuest(): q.alternateDownloadLink =" + q.alternateDownloadLink);

		StartCoroutine(DownloadQuestAfterConnectionCheck(q, 0.0f, new WWW("http://qeevee.org:9091/testConnection")));
	}

	void downloadAfterConnectionChecked (Quest q, bool connected) {
		if ( connected ) {
			if ( webloadingmessage != null ) {

				webloadingmessage.text = "Lade Quest ... " + q.name;
			}


			string url = "http://www.qeevee.org:9091/editor/" + q.id + "/clientxml";

			if ( q.alternateDownloadLink != null && q.alternateDownloadLink != "" ) {

				url = q.alternateDownloadLink;
			}


			q.www = new WWW(url);
			if ( loadlogo != null ) {

				loadlogo.enable();
			}
			if ( webloadingmessage != null ) {

				webloadingmessage.text = "Bitte warten ... ";
			}
			StartCoroutine(DownloadFinished(q));
		}
		else {
			
			// TODO message with two buttons is needed here to enable cancel.
			showmessage("Wir konnten keine Verbindung mit dem Internet herstellen.", "Nochmal versuchen");
			
			
			StartCoroutine(DownloadQuestAfterConnectionCheck(q, 0.0f, new WWW("http://www.google.com")));

		}

	}

	// TODO delete this method.
	public void downloadQuest (int id) {
		Quest q = new Quest();
		q.id = id;

		Debug.Log("PROBLEM: downloadQuest(" + id + ")");
		currentquest = q;
		downloadQuest(q);
	}

	public void downloadAsset (string url, string localTargetPath) {
		Debug.Log("downloadAsset(" + url + ", " + localTargetPath + ")");
		if ( filedownloads != null )
			Debug.Log("filedownloads: " + filedownloads.Count);
		else
			Debug.Log("filedownloads: NULL ; url: " + url + " Local path: " + localTargetPath);
		
		if ( wanttoload == null ) {
			wanttoload = new List<string>();
		}
		if ( !wanttoload.Contains(url) ) {
			
			wanttoload.Add(url);
		}

		StartCoroutine(downloadAssetAsync(url, localTargetPath));

	}

	private static List<string> debugDownloadsStarted = new List<string>();

	private float time;

	public IEnumerator downloadAssetAsync (string url, string localTargetPath) {
		if ( filedownloads == null ) {
			filedownloads = new List<WWW>();
		}
//		else {
//			foreach ( WWW fileWWW in filedownloads ) {
//				Debug.Log("FILE WWW LOADING " + fileWWW.url + " : " + fileWWW.progress + " isDone? :" + fileWWW.isDone);
//			}
//		}
		//		Debug.Log("downloadAssetAsync(" + url + ", " + localTargetPath + ")" + " wanttoload# = " + wanttoload.Count + "; filedownloads# = " + filedownloads.Count);

		bool done = true;
		if ( filedownloads != null ) {
			foreach ( WWW w in filedownloads ) {

				if ( !w.isDone ) {

					done = false;
				}
				else {
				}


			}
		}


		if ( done ) {
//			Debug.Log("downloadAssetAsync##done new will be: " + url);

			if ( !url.Contains("/clientxml") ) {

				time = Time.time;

				WWW wwwfile = new WWW(url);
				debugDownloadsStarted.Add(url);

				if ( filedownloads == null ) {
					filedownloads = new List<WWW>();
				}

//				Debug.Log("Started new download for " + url + " in downloadAssetAsync(). Now we have: " + (filedownloads.Count + 1));

				filedownloads.Add(wwwfile);
				files_all += 1;
				StartCoroutine(downloadAssetFinished(wwwfile, localTargetPath, 0f));
			}
//			else {
//				Debug.Log("downloadAsset() with clientxml in url-arg called");
//			}

		}
		else {
//			Debug.Log("downloadAssetAsync##else(!done); not loading: " + url);

			yield return new WaitForEndOfFrame();
			downloadAsset(url, localTargetPath);

			            
		}
	}

	public IEnumerator downloadAssetFinished (WWW wwwfile, string filename, float timeout) {
//		Debug.Log("downloadAssetFinished(" + filename + ", " + timeout + ")");

		yield return new WaitForSeconds(0.3f);
		timeout += 0.3f;

		if ( wwwfile.error != null ) {
			Debug.Log("error downloading " + wwwfile.url + " (" + wwwfile.error + ")");
				
			if ( wwwfile.error != "unsupported URL" ) {
				Debug.Log("redoing www error: " + wwwfile.error);

				downloadAsset(wwwfile.url, filename);
			}
			filedownloads.Remove(wwwfile);
			wwwfile.Dispose();
		}
		else {
			if ( wwwfile.isDone ) {
				if ( !Directory.Exists(Path.GetDirectoryName(filename)) ) {
						
//					Debug.Log("creating folder:" + Path.GetDirectoryName(filename));
						
					Directory.CreateDirectory(Path.GetDirectoryName(filename));
				}
				if ( wwwfile == null || wwwfile.bytes == null || wwwfile.bytes.Length == 0 ) {
					Debug.Log("Download Problem: Empty file " + filename);
				}
				FileStream fs = File.Create(filename);
				fs.Write(wwwfile.bytes, 0, wwwfile.size);
				fs.Close();
				files_complete += 1;
				bytesloaded += (int)(wwwfile.bytesDownloaded);
				filedownloads.Remove(wwwfile);



				if ( wanttoload.Contains(wwwfile.url) ) {
					wanttoload.Remove(wwwfile.url);
				}

				wwwfile.Dispose();


		

				//performSpriteConversion (filename);




				
			}
			else {
				if ( timeout > (float)Configuration.instance.downloadTimeOutSeconds ) {
					showmessage("Download fehlgeschlagen.");
					Application.LoadLevel(0);
				}
				else
				if ( timeout > 60f && wwwfile.progress < 0.1f ) {
					Debug.Log("Error Timeout: " + wwwfile.url + " - " + timeout);

					if ( !wwwfile.url.Contains("/clientxml") ) {
						Debug.Log("redoing www");
							
						filedownloads.Remove(wwwfile);
						downloadAsset(wwwfile.url, filename);
						wwwfile.Dispose();
					}
					else {
						filedownloads.Remove(wwwfile);
						wwwfile.Dispose();
					}
				}
				else {
					int bytesloaded = 0;
					StartCoroutine(downloadAssetFinished(wwwfile, filename, timeout));
				}
			}
		}
	}

	public List<Quest> GetLocalQuests () {


#if !UNITY_WEBPLAYER


		if ( localquests != null && localquests.Count > 0 ) {
			return localquests;
		}

		if ( !Application.isWebPlayer ) {
			localquests.Clear(); 
			DirectoryInfo info = new DirectoryInfo(Application.persistentDataPath + "/quests/");
			if ( !info.Exists ) {
				info.Create();
				return localquests;
			}
			var fileInfo = info.GetDirectories();


			downloadingAll = false;

			foreach ( DirectoryInfo folder in fileInfo ) { 
				if ( File.Exists(folder.ToString() + "/game.xml") ) {
					Quest n = new Quest();
					string[] splitted = folder.ToString().Split('/');
					n.id = int.Parse(splitted[splitted.Length - 1]);
					n.filepath = folder.ToString() + "/";
					n = n.LoadFromText(int.Parse(splitted[splitted.Length - 1]), true);
					if ( n != null ) {
						localquests.Add(n);
					}
				}
			}

			GameObject.Find("[FILTERLIST]").GetComponent<categoryFilterList>().reInstantiateFilter();

			//downloadingAll = true;

		}

#endif

		return localquests;

	}

	public void AllowAutoRotation (bool status) {
		if ( status == true ) {
			Screen.autorotateToLandscapeLeft = true;
			Screen.autorotateToLandscapeRight = true;
			originalOrientation = Screen.orientation;
			Screen.orientation = ScreenOrientation.AutoRotation;
		}
		else {

			Debug.Log("rotatin' 1");
			if ( originalOrientation != null ) {
				Debug.Log("rotatin' 1.5");

				Screen.orientation = originalOrientation;
			}
			Debug.Log("rotatin' 2");

			Screen.autorotateToLandscapeLeft = false;
			Screen.autorotateToLandscapeRight = false;
			Debug.Log("rotatin' 3");


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
	public void installQuest (Quest q, bool reload, bool localload) {
		Debug.Log("installQuest(" + q.id + ") -> " + q.xmlcontent);

		if ( filedownloads != null ) {
			filedownloads.Clear();
		}
		if ( loadedfiles != null ) {
			loadedfiles.Clear();
		}

		convertToSprites = true;

		// TODO: Here we create the THIRD Quest object (in download case). 
		// We can only assume that q has an id here. Is this really a good idea? (hm)
//		Debug.Log("XXXXX VORHER");
		Quest nq = q.LoadFromText(q.id, localload);

		// store timestamp for old quests that miss lastUpdate in XML to prevent relaoding them always:
		if ( nq.lastUpdate == 0 && !reload && !localload ) {
			foreach ( Quest curQ in allquests ) {
				if ( curQ.id == nq.id ) {
					nq.lastUpdate = curQ.lastUpdate;
					PlayerPrefs.SetString(curQ.id + "_lastUpdate", curQ.lastUpdate.ToString());
					Debug.Log("<color=red>TIMESTAMP stored in PLAYER_PREFS for quest id = " + curQ.id + "</color>");
				}
			}
		}
//		Debug.Log("XXXXX NACHHER");

		bool alreadyStoredInLocalQuests = false;
		foreach ( Quest quest in localquests ) {
			if ( quest.id == nq.id ) {
				alreadyStoredInLocalQuests = true;
				break;
			}
		}
		if ( !alreadyStoredInLocalQuests )
			localquests.Add(nq);


		nq.id = q.id;
		if ( nq == null ) {
			questmilllogo.enabled = false;
			if ( webloadingmessage != null ) {

				webloadingmessage.enabled = false;
			}
			if ( loadlogo != null ) {

				loadlogo.disable();
			}
			return;
		}


		if ( downloadingAll ) {

			if ( downloadquests == null ) {
				downloadquests = new List<Quest>();
			}
			downloadquests.Add(nq);

		}
		else {
			currentquest = nq;
		}

		//q.deserializeAttributes ();
//		Debug.Log ("done installing...");


		// if not local load => set export location to fresh directory:
		if ( !localload ) {

			// resave xml
			string exportLocation = Application.persistentDataPath + "/quests/" + nq.id + "/";

#if !UNITY_WEBPLAYER

			if ( !Application.isWebPlayer && (!Directory.Exists(exportLocation) || reload) ) {

				if ( Directory.Exists(exportLocation) ) {
					Directory.Delete(exportLocation, true);
				}
				Directory.CreateDirectory(exportLocation);

			}
#endif
		}

		initiateQuestStart(localload, nq);



	}

	void initiateQuestStart (bool localload, Quest nq) {
		bool hasmorethanmetadata = false;
		if ( nq.pages != null && nq.pages.Count > 0 ) {
			// check if game has more than just metadata and set flag:
			nq.currentpage = nq.pages.First();
			int c = 0;
			while ( nq.currentpage.type == "MetaData" ) {
				// TODO I guess this is a bug and will crash if we have e.g. a quest with a single page of type meta data
				// TODO at least it will leave currentquest.currentpage being null.
				if ( nq.pages.Count >= c - 1 ) {
					nq.currentpage = nq.pages[c];
					c++;
				}
				else {
					hasmorethanmetadata = false;
					break;
				}
			}
			if ( !nq.currentpage.type.Equals("MetaData") ) {
				hasmorethanmetadata = true;
			}
		}
		if ( currentquest != null && currentquest.hotspots != null ) {
			hotspots = new List<QuestRuntimeHotspot>();
			foreach ( QuestHotspot qh in currentquest.hotspots ) {
				bool initialActivity = qh.hasAttribute("initialActivity") && qh.getAttribute("initialActivity") == "true";
				bool initialVisibility = qh.hasAttribute("initialVisibility") && qh.getAttribute("initialVisibility") == "true";
				hotspots.Add(new QuestRuntimeHotspot(qh, initialActivity, initialVisibility, qh.latlon));
			}
		}
		if ( canPlayQuest(currentquest) && hasmorethanmetadata ) {
			if ( Application.isWebPlayer ) {
				transferQuestHotspots(currentquest.currentpage.id);
			}
			else {
				if ( !localload ) {
					//				Debug.Log ("WAITING FOR QUEST ASSETS");
					if ( webloadingmessage != null ) {
						webloadingmessage.text = "Lade alle Medien vor.\n Das kann einige Minuten dauern. \n ";
					}
					if ( loadlogo != null ) {
						loadlogo.enable();
					}
					StartCoroutine(waitforquestassets(nq.currentpage.id, 0f));
				}
				else {
					StartCoroutine(waitForSpriteConversion(nq.currentpage.id));
				}
			}
		}
		else {
			Debug.Log("INVALID Quest Version. Quest: " + currentquest.id);
			showmessage("Entschuldigung! Die Quest kann in dieser Version nicht abgespielt werden.");
			if ( webloadingmessage != null ) {
				webloadingmessage.enabled = false;
			}
			if ( loadlogo != null ) {
				loadlogo.disable();
			}
			if ( GameObject.Find("List") != null && GameObject.Find("List").GetComponent<createquestbuttons>() != null ) {
				GameObject.Find("List").GetComponent<createquestbuttons>().resetList();
			}
		}
	}

	public void performSpriteConversion (string value) {

		if ( !Application.isWebPlayer ) {

			if ( convertToSprites ) {
				bool doit = true;

				List<SpriteConverter> redo = new List<SpriteConverter>();
				foreach ( SpriteConverter asc in convertedSprites ) {

					if ( asc.filename == value ) {

						if ( asc.isDone ) {


							doit = false;

						}
						else {

							redo.Add(asc);
						}
					}

				}

				foreach ( SpriteConverter sc in redo ) {

					convertedSprites.Remove(sc);
					sc.myWWW = null;
					sc.myTexture = null;

				}


				if ( doit ) {
					if ( File.Exists(value) ) {
						FileInfo fi = new FileInfo(value);

						List<string> imageextensions = new List<string>() {
								".jpg",
								".jpeg",
								".gif",
								".png"
						};
						//Debug.Log (imageextensions.Count);
						//	Debug.Log (fi.Extension);
						if ( imageextensions.Contains(fi.Extension.ToLower()) ) {

//							SpriteConverter sc = new SpriteConverter(value);
//
//
//
//							convertedSprites.Add(sc);
//
//							sc.startConversion();
							//	StartCoroutine (waitForSingleSpriteCompletion (sc));
						}
					}
					else {

						Debug.LogWarning("[ATTENTION] A file didn't exist: " + value);

					}
				}
			}
		}
	}

	IEnumerator waitForSpriteConversion (int pageid) {
		checkForDatasend(pageid);
		yield return null;
	}



	public void acceptedDatasend (int pageid) {


		currentquest.acceptedDS = true;

		transferQuestHotspots(pageid);



	}

	public void rejectedDatasend (int pageid) {
		
		
		currentquest.acceptedDS = false;

		//TODO: can I start the quest?

		transferQuestHotspots(pageid);
		
		
		
	}

	void checkForDatasend (int pageid) {

	
		if ( Configuration.instance.showMessageForDatasendAction && !currentquest.acceptedDS &&
		     currentquest.hasActionInChildren("SendVarToServer") ) {

			// TODO FRAGE: Warum wird das für jede Seite aufgerufen, wo es doch eh immer das ganze Quest durchsucht? (hm) Erst Test schreiben dann refactoren!

			// TODO: show message

			datasendAcceptMessage.pageid = pageid;
			datasendAcceptMessage.gameObject.SetActive(true);
			datasendAcceptMessage.GetComponent<Animator>().SetTrigger("in");


		}
		else {
			transferQuestHotspots(pageid);

			// TODO FRAGE (hm): Warum wird das hier aufgerufen? Ich habe das woanders speziell für den Webplayer gesehen.

		}
	
	}


	public bool nextSpriteToBeConverted (SpriteConverter sc) {


		bool me = true;

		foreach ( SpriteConverter asc in convertedSprites ) {


			if ( asc == sc ) {

				break;
			}
			else
			if ( asc.isDone != true ) {

				me = false;

				break;
			}



		}

		return me;
	}

	IEnumerator waitForSingleSpriteCompletion (SpriteConverter sc) {

		WWW myWWW = sc.myWWW;
		//Debug.Log ("trying to acces: " + myWWW.url);
		
		if ( myWWW.url == null || myWWW.url == "" ) {
			Debug.Log("nothing to do");
			sc.isDone = true;
			yield return null;
			
		}
		else {
			
			
			yield return myWWW;
			//Debug.Log ("not done with WWW object:" + myWWW.url);
			
			if ( myWWW.error != null ) {
				Debug.Log("error:" + myWWW.error);
				//TODO: error handling

				spriteError = "Fehlerhafte Datei\nBitte lade diese Quest erneut.";
				
			}
			else {
				//Debug.Log ("DONE with WWW object");
			


				if ( nextSpriteToBeConverted(sc) ) {


					if ( myWWW.texture != null ) {

						sc.myTexture = myWWW.texture;
						sc.width = myWWW.texture.width;
						sc.height = myWWW.texture.height;
						sc.convertSprite();


					}
					else {
						sc.isDone = true;
						myWWW = null;
						sc.myWWW = null;
					}
				}
				else {
					yield return new WaitForSeconds(0.5f);
					StartCoroutine(waitForSingleSpriteCompletion(sc));

				}
			}
		}
	
	}

	void transferQuestHotspots (int pageid) {

		// instatiate a quest clone at any start. This function is always called at quest start.
		currentquestdata = (Transform)Instantiate(questdataprefab, transform.position, Quaternion.identity);
		
		if ( currentquest.getAttribute("transferToUserPosition") != "true" ) {
		
			changePage(pageid);
		

		}
		else {
			foreach ( QuestRuntimeHotspot mainhs in hotspots ) {


				if ( mainhs.hotspot.id == int.Parse(currentquest.getAttribute("transferHotspot")) ) {

					
				


					if ( Application.isWebPlayer || Application.isEditor ) {


						GameObject.Find("QuestDatabase").GetComponent<GPSPosition>().CoordinatesWGS84 = 
						new double[] {
						
							7d,
							50d
						};
					}



					foreach ( QuestRuntimeHotspot subhs in hotspots ) {



						Debug.Log("editing sub hotspot:" + subhs.hotspot.id);


				
						if ( subhs.hotspot.id != mainhs.hotspot.id ) {



							subhs.lon -= mainhs.lon;
							subhs.lon += (float)GameObject.Find("QuestDatabase").GetComponent<GPSPosition>().CoordinatesWGS84[1];


							subhs.lat -= mainhs.lat;
	
							subhs.lat += (float)GameObject.Find("QuestDatabase").GetComponent<GPSPosition>().CoordinatesWGS84[0];

							string url = "http://www.yournavigation.org/api/1.0/gosmore.php?" +
							             "format=kml" +
							             "&flat=" + GameObject.Find("QuestDatabase").GetComponent<GPSPosition>().CoordinatesWGS84[1] +
							             "&flon=" + GameObject.Find("QuestDatabase").GetComponent<GPSPosition>().CoordinatesWGS84[0] +
							             "&tlat=" + subhs.lon +
							             "&tlon=" + subhs.lat +
							             "&v=foot&" +
							             "fast=1" +
							             "&layer=mapnik" +
							             "&instructions=1" +
							             "&lang=de";
							
							WWW routewww = new WWW(url);

							Debug.Log(url);

							if ( routewwws == null ) {

								routewwws = new List<WWW>();
							}

							routewwws.Add(routewww);

							StartCoroutine(waitForRouteFile(routewww, subhs));
						}
					}


					mainhs.lat = (float)GameObject.Find("QuestDatabase").GetComponent<GPSPosition>().CoordinatesWGS84[0];
					mainhs.lon = (float)GameObject.Find("QuestDatabase").GetComponent<GPSPosition>().CoordinatesWGS84[1];

				}


			}

			StartCoroutine(waitForTransferCompletion(pageid));

		}
	}

	public IEnumerator waitForTransferCompletion (int pageid) {

		yield return new WaitForEndOfFrame();

		bool b = true;

		foreach ( WWW mywww in routewwws ) {

			if ( !mywww.isDone ) {

				b = false;

			}
		}

		if ( b ) {
			changePage(pageid);
		}
		else {

			yield return new WaitForSeconds(0.1f);
			StartCoroutine(waitForTransferCompletion(pageid));

		}
	}

	public IEnumerator waitForRouteFile (WWW mywww, QuestRuntimeHotspot qrh) {
		
		yield return mywww;
		
		if ( mywww.error == null || mywww.error == "" ) {
			string routefile = mywww.text;
			
			routefile = routefile.Substring(routefile.IndexOf("<coordinates>"));
			routefile = routefile.Substring(14, routefile.IndexOf("</coordinates>") - 14);

			string[] coordinates = routefile.Split(new string[] {
				Environment.NewLine
			}, StringSplitOptions.None);
				
				

			int i = coordinates.Count() - 2;
			string s = coordinates[i];
					
					
			if ( s.Contains(",") ) {
						
						
				string[] co = s.Split(',');
						
						
				if ( float.Parse(co[0]) != 0f && float.Parse(co[1]) != 0f ) {
					qrh.lat = float.Parse(co[0]);


					qrh.lon = float.Parse(co[1]);
					Debug.Log("REARRANGED HOTSPOT:" + qrh.lat + "," + qrh.lon);

				}
				else {
					if ( savedmessages == null ) {

						savedmessages = new List<string>();
					}
					savedmessages.Add("An deinem Standort sind nicht genügend Weginformationen vorhanden. Manche Kartenobjekte könnten nicht erreichbar sein.");

					Debug.Log("REARRANGING FAILED:" + qrh.lat + "," + qrh.lon);
				}
			}
					
		}
		else {
			Debug.Log("Route WWW Error:" + mywww.error);
		}
	}

	int waitedFor = 0;

	IEnumerator waitforquestassets (int pageid, float timeout) {
		// TODO (hm) timeout seems never be used, i.e. checked to take some action like ending the trial ... why (is it still here)?
//		Debug.Log("waitforquestassets page: " + pageid + " wanttoload: " + wanttoload.Count + " filedownloads: " + filedownloads.Count);

		if ( fakebytes == 0 ) {
			fakebytes = 1;
		}

		timeout += 0.5f;
		yield return new WaitForSeconds(0.5f);

		bool done = true;
		int percent = 100;
		int downloadsundone = 0;
		string error = "";
		int bytesloadedbutunfinished = 0;

		if ( wanttoload.Count > 0 ) {
			done = false;
		}
		else {
			if ( filedownloads != null ) {
				Debug.Log("WWW Objects" + filedownloads.Count);

				foreach ( WWW www in filedownloads ) {

					if ( !www.isDone ) {
						done = false;
						downloadsundone += 1;
					}

					if ( www.error != null ) {
						if ( www.url.StartsWith("http") ) {
							done = false;
							downloadsundone += 1;
							Debug.Log("WWW ERROR: " + www.error + " (" + www.url + ")");

							//error += www.url +"couldn't be downloaded.";
						}
					}
				}

				int bytes_finished = files_complete;
				int bytes_all = files_all;

			}
			else {
				done = true;
			}
		}

		int bytescomplete = bytesloaded;
		int filesleft = 0;

		if ( filedownloads != null ) {

			filesleft = filedownloads.Count;

			string openfileloads = "Open WWW Files: ";

			int d = 0;
			foreach ( WWW awww in filedownloads ) {
				//Debug.Log(awww.bytesDownloaded);

				//bytescomplete += (int)(awww.bytesDownloaded);

				openfileloads += awww.url + "; ";
				d++;
			}
			//			Debug.Log (openfileloads);
		}

		if ( error == "" ) {
			int bytesloaded2 = (int)(bytesloaded + (fakebytes * 900));
			if ( webloadingmessage != null ) {

				webloadingmessage.text = "Lade alle Medien vor.\n Das kann einige Minuten dauern. \n " + bytesloaded2 + " Bytes geladen";
//				Debug.Log ("WEBLOADINGMESSAGE: " + webloadingmessage.text);
			}
		}
		else {
			if ( webloadingmessage != null ) {
				webloadingmessage.text = error;
			}

		}
		if ( done ) {
			
			if ( !downloadingAll ) {

				writeQuestXML(currentquest);
				StartCoroutine(waitForSpriteConversion(pageid));
			}
			else {
				waitedFor += 1;
				Debug.Log(waitedFor);
				if ( waitedFor >= questsToLoad ) {

					foreach ( Quest q in downloadquests ) {

						writeQuestXML(q);

					}

					backToMenuAfterDownloadedAll();
				}
			}
		}
		else {
			StartCoroutine(waitforquestassets(pageid, timeout));
		}
	}

	void backToMenuAfterDownloadedAll () {
		if ( webloadingmessage != null ) {
			webloadingmessage.enabled = false;
		}
		if ( loadlogo != null ) {
			loadlogo.disable();
		}
		downloadingAll = false;
		downloadedAll = true;
		downloadquests = null;
		buttoncontroller.loadLocalQuests();
		buttoncontroller.DisplayList();
		if ( GameObject.Find("PageController_Map") != null ) {
			hotspots = new List<QuestRuntimeHotspot>();
			hotspots.AddRange(getActiveHotspots());
			GameObject.Find("PageController_Map").GetComponent<page_map>().updateMapMarkerInFoyer();
		}
	}


	public void writeQuestXML (Quest q) {

		string exportLocation = Application.persistentDataPath + "/quests/" + q.id + "/";

		if ( !File.Exists(exportLocation + "game.xml") ) {

			var stream = new FileStream(exportLocation + "game.xml", FileMode.Create);

			//	Debug.Log ("writing xml #0");

			stream.Close();
			var stream2 = new StreamWriter(exportLocation + "game.xml");

			//	Debug.Log ("writing xml #1: " + q.xmlcontent);
			stream2.Write(q.xmlcontent);
			//	Debug.Log ("writing xml #2");

			stream2.Close();
			//	Debug.Log ("writing xml #3");

		}




	}

	public bool canPlayQuest (Quest q) {

		if ( downloadingAll ) {
			// TODO FRAGE: Ist das nicht eine fehlerhafte Abkürzung? (hm)
			return true;

		}

		return Platform.CanPlay(q);

	}

	public QuestPage getPage (int id) {
		QuestPage resultpage = null;



		foreach ( QuestPage qp in currentquest.pages ) {
			
			
			if ( qp.id == id ) {

				resultpage = qp;

			}


		}


		return resultpage;

	}

	public void closeMap () {


		if ( GameObject.Find("MapCanvas") != null ) {
			Destroy(GameObject.Find("MapCanvas"));
		}
		if ( GameObject.Find("PageController_Map") != null ) {
			Destroy(GameObject.Find("PageController_Map"));
		}
		if ( GameObject.Find("MapCam") != null ) {
			Destroy(GameObject.Find("MapCam"));
		}
		if ( GameObject.Find("[Map]") != null ) {
			Destroy(GameObject.Find("[Map]"));
		}
		if ( GameObject.Find("RouteRender") != null ) {
			Destroy(GameObject.Find("RouteRender"));
		}
	}

	public void changePage (int id) {
		if ( GameObject.Find("MapHider") != null ) {


			GameObject.Find("MapHider").GetComponent<Image>().enabled = true;
		}

		if ( GameObject.Find("MapCam") != null ) {
			
			GameObject.Find("MapCam").GetComponent<Camera>().enabled = false;

			GameObject.Find("MapCam").GetComponent<AudioListener>().enabled = false;
		}


		


		if ( GameObject.Find("BgCam") ) {
			GameObject.Find("BgCam").GetComponent<Camera>().enabled = true;

			GameObject.Find("BgCam").GetComponent<AudioListener>().enabled = true;

		}


		if ( GameObject.Find("[Map]") ) {

			GameObject.Find("[Map]").GetComponent<mapdisplaytoggle>().hideMap();
		}


		if ( GameObject.Find("PageController_Map") ) {

			GameObject.Find("PageController_Map").GetComponent<page_map>().unDrawCurrentRoute();

		}
		   
		//Debug.Log ("looking for page with id:" + id);
		foreach ( QuestPage qp in currentquest.pages ) {

//			Debug.Log (qp.id);

			if ( qp.id.Equals(id) ) {

				currentquest.currentpage = qp;
				currentquest.currentpage.state = "running";

			
				//GameObject.Find("BgCam").GetComponent<Camera>().enabled = true;

				
				GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();



				foreach ( GameObject go in allObjects )
					if ( go != null && go.transform != null && go.name != "MapCanvas" && go.name != "PageController_Map" && go.name != "QuestDatabase" && go.name != "MsgCanvas"
					     && go.name != "ImpressumCanvas" && go.name != "LanguageCanvas" && !go.transform.IsChildOf(GameObject.Find("ImpressumCanvas").transform) && go.name != "MenuCanvas" && go.name != "EventSystem"
					     && go.name != "Configuration" && go.name != "MapCam" && go.name != "[Map]" && go.name != "[location marker]"
					     && go.name != "" && !go.name.Contains("[Tile") && go.name != "EventSystem_Map" && go.name != "BgCam" && go.name != "QuestData(Clone)"
					     && go.name != "NetworkManager" && !go.name.Contains("NetworkIdentity")
					     && go.name != "RouteRender" && go.name != "VectorCanvas" && go.name != "VarOverlayCanvas" ) {

						

						bool des = true;


						if ( GameObject.Find("VarOverlayCanvas") != null ) {
						
							if ( go.transform.IsChildOf(GameObject.Find("VarOverlayCanvas").transform) ) {
								des = false;
								//Debug.Log("is child of mapcanvas");
							}
						
						}

						if ( GameObject.Find("LanguageCanvas") != null ) {
							
							if ( go.transform.IsChildOf(GameObject.Find("LanguageCanvas").transform) ) {
								des = false;
								//Debug.Log("is child of mapcanvas");
							}
							
						}

						if ( GameObject.Find("MenuCanvas") != null ) {
						
							if ( go.transform.IsChildOf(GameObject.Find("MenuCanvas").transform) ) {
								des = false;
								//Debug.Log("is child of mapcanvas");
							}
						
						}


						if ( GameObject.Find("MapCanvas") != null ) {

							if ( go.transform.IsChildOf(GameObject.Find("MapCanvas").transform) ) {
								des = false;
								//Debug.Log("is child of mapcanvas");
							}

						}

						if ( GameObject.Find("[Map]") ) {

							if ( go.transform.IsChildOf(GameObject.Find("[Map]").transform) ) {

								des = false;

							}
							
							
							
						}

						if ( GameObject.Find("[location marker]") ) {
						
							if ( go.transform.IsChildOf(GameObject.Find("[location marker]").transform) ) {
							
								des = false;
							
							}
						
						
						
						}

						if ( GameObject.Find("PageController_Map") ) {

							if ( go == GameObject.Find("PageController_Map").GetComponent<page_map>().map ) {

								des = false;

							}

						}

						if ( des ) {
							Destroy(go);
						}

					}

				//	Debug.Log ("Resources GameObject # =" + Resources.FindObjectsOfTypeAll (typeof(GameObject)).Count ());
				//	Debug.Log ("Resources Sprite # =" + Resources.FindObjectsOfTypeAll (typeof(Sprite)).Count ());
				Resources.UnloadUnusedAssets();


				//if(GameObject.Find("MapCam") != null){
				//GameObject.Find("MapCam").GetComponent<Camera>().enabled = false;
				//}

				bool needsCamera = false;

				if ( !menu.isActive ) {
					menu.showTopBar();
				}
				
				if ( qp.type == "NPCTalk" ) {
					SceneManager.LoadScene(1, LoadSceneMode.Additive);
				}
				else
				if ( qp.type == "ImageWithText" ) {
					SceneManager.LoadScene(1, LoadSceneMode.Additive);
				}
				else
				if ( qp.type == "StartAndExitScreen" ) {
					SceneManager.LoadScene(2, LoadSceneMode.Additive);

				}
				else
				if ( qp.type == "MultipleChoiceQuestion" ) {
					SceneManager.LoadScene(3, LoadSceneMode.Additive);
				}
				else
				if ( qp.type == "Menu" ) {
					SceneManager.LoadScene(3, LoadSceneMode.Additive);
					
				}
				else
				if ( qp.type == "VideoPlay" ) {
					SceneManager.LoadScene(4, LoadSceneMode.Additive);
				}
				else
				if ( qp.type == "TagScanner" ) {
					needsCamera = true;
					SceneManager.LoadScene(5, LoadSceneMode.Additive);
				}
				else
				if ( qp.type == "ImageCapture" ) {
					needsCamera = true;
					SceneManager.LoadScene(6, LoadSceneMode.Additive);
				}
				else
				if ( qp.type == "TextQuestion" ) {
					SceneManager.LoadScene(7, LoadSceneMode.Additive);
				}
				else
				if ( qp.type == "AudioRecord" ) {
					needsCamera = true;
					SceneManager.LoadScene(8, LoadSceneMode.Additive);
				}
				else
				if ( qp.type == "WebPage" ) {
					SceneManager.LoadScene(10, LoadSceneMode.Additive);
				}
				else
				if ( qp.type == "Custom" ) {
					SceneManager.LoadScene(11, LoadSceneMode.Additive);
				}
				else
				if ( qp.type == "ReadNFC" ) {
					SceneManager.LoadScene(12, LoadSceneMode.Additive);
				}
				else
				if ( qp.type == "MapOSM" || qp.type == "Navigation" ) {



					//	Debug.Log ("trying to start map");


					if ( GameObject.Find("MapCam") == null ) {
//						Debug.Log ("MapCam not found");


						StartCoroutine(loadMap());

					}
					else {

						//	Debug.Log ("MapCam found");

						if ( GameObject.Find("PageController_Map") != null ) {

							GameObject.Find("PageController_Map").GetComponent<page_map>().onStartInvoked = false;
						}

						if ( GameObject.Find("MapHider") != null ) {

							GameObject.Find("MapHider").GetComponent<Image>().enabled = false;
						}

						if ( GameObject.Find("BgCam") ) {
							GameObject.Find("BgCam").GetComponent<Camera>().enabled = false;

							GameObject.Find("BgCam").GetComponent<AudioListener>().enabled = false;
							
						}

						if ( GameObject.Find("MapCanvas") != null ) {
							
							GameObject.Find("MapCanvas").GetComponent<Canvas>().enabled = true;
						}

						GameObject.Find("MapCam").GetComponent<Camera>().enabled = true;
						GameObject.Find("MapCam").GetComponent<AudioListener>().enabled = true;

						if ( GameObject.Find("[Map]") ) {
							
							GameObject.Find("[Map]").GetComponent<mapdisplaytoggle>().showMap();
						}

					}





				}



				if ( needsCamera ) {
					if ( GameObject.Find("MapCanvas") != null ) {
						Debug.Log("Disabling Map Canvas");
						GameObject.Find("MapCanvas").GetComponent<Canvas>().enabled = false;
					}

					Debug.Log("needs Camera");
//					GameObject.Find ("BgCam").GetComponent<Camera> ().enabled = false;
//					if (GameObject.Find ("MapCam") != null) {
//						GameObject.Find ("MapCam").GetComponent<Camera> ().enabled = false;
//						GameObject.Find ("MapCam").GetComponent<AudioListener> ().enabled = false;
//
//					}
//					GameObject.Find ("BgCam").GetComponent<AudioListener> ().enabled = false;
//
				}
				
				//GameObject.Find("BgCam").GetComponent<Camera>().enabled = false;

				
			}
		
		}
		


		string lastmessage = "";

		foreach ( string s in savedmessages ) {

			if ( s != lastmessage ) {
				showmessage(s);
				lastmessage = s;
			}


		}
		savedmessages.Clear();

	}

	IEnumerator loadMap () {
		// TODO shouldn't we simply call SceneManager.LoadSceneAsync since it already works in a backgorund thread. 
		AsyncOperation loadLevelOperation = SceneManager.LoadSceneAsync(9, LoadSceneMode.Additive);		
	
		yield return loadLevelOperation;
		if ( GameObject.Find("BgCam") != null ) {
			
			GameObject.Find("BgCam").GetComponent<Camera>().enabled = false;
			
			
		}
		if ( GameObject.Find("MapCam") != null ) {
			
			GameObject.Find("MapCam").GetComponent<Camera>().enabled = true;
			
			
		}
	}
	
	//	public void showmessage (string text) {
	//		Debug.Log("MSGSActive before:" + msgsactive);
	//
	//		msgsactive += 1;
	//		Debug.Log("MSGSActive after:" + msgsactive);
	//
	//		QuestMessage nqa = (QuestMessage)Instantiate(message_prefab, transform.position, Quaternion.identity);
	//
	//
	//		nqa.message = text;
	//
	//		nqa.transform.SetParent(GameObject.Find("MsgCanvas").transform, false);
	//		nqa.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
	//
	//
	//	}
	//
	public void showmessage (string text, string button = null, Action action = null) {
		Debug.Log("showmessage(" + text + ")");
		Debug.Log("MSGSActive before:" + msgsactive);

		msgsactive += 1;

		QuestMessage nqa = (QuestMessage)Instantiate(message_prefab, transform.position, Quaternion.identity);
		
		nqa.message = text;
		if ( button != null ) {
			nqa.setButtonText(button);
		}
		if ( action != null ) {
			nqa.Action = action;
		}
		nqa.transform.SetParent(GameObject.Find("MsgCanvas").transform, false);
		nqa.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);



	}

	public QuestHotspot getHotspotObject (int i) {

		foreach ( QuestHotspot qh in currentquest.hotspots ) {

			if ( qh.id == i ) {

				return qh;
			}
		}

		return null;

	}

	public QuestRuntimeHotspot getHotspot (string str) {


		foreach ( QuestRuntimeHotspot qrh in hotspots ) {

			if ( qrh.hotspot.id == int.Parse(str) ) {

				return qrh;


			}






		}
		return null;
	}

	IEnumerator DownloadFinished (Quest q) {
		if ( webloadingmessage != null ) {

			webloadingmessage.enabled = true;
		}
		if ( loadlogo != null ) {

			loadlogo.enable();
		}

		// TODO: shouldn't we add it to the local quests only after the download of the quest xml has been completed, i.e. one line later? (hm)
//		localquests.Add(q);
		yield return q.www;

		// Quest XML has been downloaded now.

		if ( q.www.error == null ) {



			if ( webloadingmessage != null ) {

				webloadingmessage.text = "Bitte warten ...";
			}

			Quest nq = new Quest();
				
			nq.id = q.id;
				
//			currentquestdata = (Transform)Instantiate(questdataprefab, transform.position, Quaternion.identity);
//				
			nq.xmlcontent = UTF8Encoding.UTF8.GetString(q.www.bytes); 
//			Debug.Log ("XML:" + nq.xmlcontent);
			bool isLocal = false;


			// TODO: what is this good for? We already know that it is in the list, since we put it in there. (hm)
			foreach ( Quest lq in localquests ) {
				if ( lq.id == q.id ) {
					isLocal = true;
				}
			}

			if ( !downloadingAll ) {
				currentquest = nq;
			}

			// TODO: here b is always true! So we can elimiate the foreach loop above! (hm again ;-) )
			installQuest(nq, isLocal, false);
			// TODO: why do we use a new Quest object nq which only has the id from q. Shouldn't we just use q itself?
		}
		else {
			Debug.LogWarning("WWW Error: " + q.www.error);
			if ( webloadingmessage != null ) {

				webloadingmessage.text = q.www.error;
			}

		}  
	}

}













