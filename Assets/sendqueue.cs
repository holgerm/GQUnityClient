using UnityEngine;
using UnityEngine.Networking;

using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using System.IO;
using System;

public class sendqueue : MonoBehaviour {

	public bool receivedExpectedMessageId = false;
	public List<SendQueueEntry> queue;
	public networkactions networkActionsObject;
	public float messageTimer = 0.2f;
	float messageTimerSave = 0.2f;
	public float messageTimeout = 10f;
	public string deviceid;
	public int idCounter = 0;
	public bool triedEstablishingConnection = false;
	public float connectionTimeout = 10f;
	float connectionTimeoutSave = 10f;
	const string MODE_VALUE = "value";
	const string MODE_FILE_START = "file_start";
	const string MODE_FILE_MID = "file_mid";
	const string MODE_FILE_FINISH = "file_finish";

	void Start () {

		queue = new List<SendQueueEntry>();
		messageTimerSave = messageTimer;
		connectionTimeoutSave = connectionTimeout;
		deviceid = SystemInfo.deviceUniqueIdentifier;

		reconstructSendQueue();
	}

	public void setExpectedNextMessage (int nm) {



	}

	void reconstructSendQueue () {

		string pre = "file: /";
		
		if ( Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ) {
			pre = "file:";
		}

		if ( Directory.Exists(Application.persistentDataPath + "/quests/") ) {
	
			foreach ( string quest in	Directory.GetDirectories(Application.persistentDataPath + "/quests/") ) {

				if ( Directory.Exists(quest + "/sendqueue/") ) {
					string FolderName = new DirectoryInfo(quest).Name;

					foreach ( string file in	Directory.GetFiles(quest + "/sendqueue/") ) {
						int num1 = 0;
						int num2 = 0;

						if ( int.TryParse(FolderName, out num1) ) {
							if ( int.TryParse(Path.GetFileNameWithoutExtension(file), out num2) ) {
								if ( File.Exists(Application.persistentDataPath + "/quests/" + num1 + "/sendqueue/" + num2 + ".json") ) {
									WWW www = new WWW(pre + "" + Application.persistentDataPath + "/quests/" + num1 + "/sendqueue/" + num2 + ".json");
									StartCoroutine(deserialize(www));

								}
							}
						}
					}
				}
			}
		}

		if ( queue.Count == 0 ) {
		}

	}

	void Update () {
		if ( queue.Count <= 0 )
			return;

		bool canSendMessage = false;
		messageTimer -= Time.deltaTime;

		if ( networkActionsObject == null && triedEstablishingConnection ) {

			connectionTimeout -= Time.deltaTime;
				
			if ( connectionTimeout <= 0f ) {
				connectionTimeout = connectionTimeoutSave;
				triedEstablishingConnection = false;
				Network.Disconnect();
			}
		}

		if ( messageTimer <= 0f ) {
			messageTimer = messageTimerSave;
			canSendMessage = true;

			if ( networkActionsObject != null ) {
				foreach ( SendQueueEntry sqe in queue.GetRange(0,queue.Count) ) {
					if ( canSendMessage ) {
						sqe.timeout -= Time.deltaTime;
						if ( sqe.timeout <= 0f ) {
							send(sqe, messageTimeout);
							canSendMessage = false;
						}
					}
				}
			}
			else {
				if ( !triedEstablishingConnection ) {
					NetworkManager.singleton.networkAddress = queue[0].ip;
					
					if ( !NetworkManager.singleton.IsClientConnected() )
						NetworkManager.singleton.StartClient();	

					triedEstablishingConnection = true;
				}
			}
		}
		else {
			if ( NetworkManager.singleton.isNetworkActive ) {

				if ( networkActionsObject != null ) {
					Network.Disconnect();
				}

			}
		}
	}

	public void addMessageToQueue (string ip, string var, string value) {

		SendQueueEntry sqe = new SendQueueEntry();



		if ( queue.Count > 0 ) {
			if ( idCounter == 0 ) {


				if ( PlayerPrefs.HasKey("nextmessage_" + sqe.ip) ) {

					idCounter = PlayerPrefs.GetInt("nextmessage_" + sqe.ip);

				}

			}
		}
		else {

			idCounter = 0;
			sqe.resetid = true;

		}
		


		sqe.id = idCounter;
		idCounter++;
		sqe.questid = GetComponent<questdatabase>().currentquest.id;

		if ( idCounter == int.MaxValue ) {
			idCounter = 0;
		}


		PlayerPrefs.SetInt("nextmessage_" + sqe.ip, idCounter);


		sqe.mode = MODE_VALUE;
		sqe.timeout = 0f;

		sqe.ip = ip;
		sqe.var = var;
		sqe.value = value;




		queue.Add(sqe);

		serialize(sqe);

	}

	public void addMessageToQueue (string ip, string var, string filetype, byte[] bytes, int part) {
		
		SendQueueEntry sqe = new SendQueueEntry();
	
		if ( queue.Count > 0 ) {
		
		}
		else {
			
			idCounter = 0;
			sqe.resetid = true;
			
		}
		
		sqe.id = idCounter;
		idCounter++;
		sqe.questid = GetComponent<questdatabase>().currentquest.id;


		if ( part == 0 ) {

			sqe.mode = MODE_FILE_START;

		}
		else {
		
			sqe.mode = MODE_FILE_MID;


		}
		sqe.timeout = 0f;
		
		sqe.ip = ip;
		sqe.var = var;
		sqe.filetype = filetype;

		sqe.file = bytes;
		
		
		
		
		queue.Add(sqe);
		serialize(sqe);

	}

	public void addFinishMessageToQueue (string ip, string var, string filetype) {

		SendQueueEntry sqe = new SendQueueEntry();
		
		
		sqe.id = idCounter;
		sqe.questid = GetComponent<questdatabase>().currentquest.id;
		idCounter++;
		sqe.timeout = 0f;

		sqe.ip = ip;
		sqe.var = var;
		
		sqe.mode = MODE_FILE_FINISH;
	
		queue.Add(sqe);
		serialize(sqe);

	
	}

	public void setNetworkIdentity (networkactions na) {
		
		networkActionsObject = na;
		
	}

	public void messageReceived (int id) {


		foreach ( SendQueueEntry sqe in queue.GetRange(0,queue.Count) ) {

			if ( sqe.id == id ) {


				queue.Remove(sqe);

				
				if ( File.Exists(Application.persistentDataPath + "/quests/" + sqe.questid + "/sendqueue/" + sqe.id + ".json") ) {
					File.Delete(Application.persistentDataPath + "/quests/" + sqe.questid + "/sendqueue/" + sqe.id + ".json");
				}
			}



		}


	}

	public void send (SendQueueEntry sqe, float timeout) {


		//queue.Remove (sqe);


		sqe.timeout = messageTimeout;


		if ( sqe.mode == MODE_VALUE ) {


			networkActionsObject.CmdSendVar(sqe.id, deviceid, sqe.var, sqe.value, sqe.resetid);


		}
		else
		if ( sqe.mode == MODE_FILE_START ) {

			networkActionsObject.CmdSendFile(sqe.id, deviceid, sqe.var, sqe.filetype, sqe.file, sqe.resetid);
					
		}
		else
		if ( sqe.mode == MODE_FILE_MID ) {

			networkActionsObject.CmdAddToFile(sqe.id, deviceid, sqe.var, sqe.filetype, sqe.file, sqe.resetid);

		}
		else
		if ( sqe.mode == MODE_FILE_FINISH ) {
			
			networkActionsObject.CmdFinishFile(sqe.id, deviceid, sqe.var, sqe.filetype, sqe.resetid);

		}



	}

	void serialize (SendQueueEntry sqe) {

		#if !UNITY_WEBPLAYER
		PlayerPrefs.SetInt("currentquestid", GetComponent<questdatabase>().currentquest.id);
		StringBuilder sb = new StringBuilder();
		JsonWriter jsonWriter = new JsonWriter(sb);
		jsonWriter.PrettyPrint = true;
		JsonMapper.ToJson(sqe, jsonWriter);

		if ( !Directory.Exists(Application.persistentDataPath + "/quests/" + GetComponent<questdatabase>().currentquest.id + "/sendqueue/") ) {

			Directory.CreateDirectory(Application.persistentDataPath + "/quests/" + GetComponent<questdatabase>().currentquest.id + "/sendqueue/");

		}

		if ( File.Exists(Application.persistentDataPath + "/quests/" + GetComponent<questdatabase>().currentquest.id + "/sendqueue/" + sqe.id + ".json") ) {
			File.Delete(Application.persistentDataPath + "/quests/" + GetComponent<questdatabase>().currentquest.id + "/sendqueue/" + sqe.id + ".json");
		}


		File.WriteAllText(Application.persistentDataPath + "/quests/" + GetComponent<questdatabase>().currentquest.id + "/sendqueue/" + sqe.id + ".json", sb.ToString());
		#endif
	}

	public IEnumerator deserialize (WWW www) {


		yield return www;



		if ( www.error == null || www.error == "" ) {



		
			SendQueueEntry sqe = JsonMapper.ToObject<SendQueueEntry>(www.text);
			queue.Add(sqe);



		}
		else {

			Debug.Log(www.error);

		}
	
	
	}


}


[System.Serializable]
public class SendQueueEntry {

	public int id;
	public int questid;
	public string ip;
	public float timeout;
	public string mode;
	public string var;
	public string value;
	public string filetype;
	public byte[] file;
	public bool resetid = false;



}