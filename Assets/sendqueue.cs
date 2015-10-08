using UnityEngine;
using UnityEngine.Networking;

using System.Collections;
using System.Collections.Generic;

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

	void Start(){

		queue = new List<SendQueueEntry> ();
		messageTimerSave = messageTimer;
		connectionTimeoutSave = connectionTimeout;
		deviceid = SystemInfo.deviceUniqueIdentifier;


		idCounter = 10000023;
	}



	void Update(){



		if (queue.Count > 0) {


			bool canSendMessage = false;
			messageTimer -= Time.deltaTime;

		
			if(networkActionsObject == null && triedEstablishingConnection){

				connectionTimeout -= Time.deltaTime;
				
				if(connectionTimeout <= 0f){
					
					connectionTimeout = connectionTimeoutSave;
					triedEstablishingConnection = false;
					Network.Disconnect();
				}

			}



			if (messageTimer <= 0f) {


				messageTimer = messageTimerSave;
			
				canSendMessage = true;


				if (networkActionsObject != null) {

					if(!receivedExpectedMessageId){



						networkActionsObject.CmdAskForNextExpectedMessage(deviceid);


					} else {


					foreach (SendQueueEntry sqe in queue.GetRange(0,queue.Count)) {
						if(canSendMessage){
								sqe.timeout -= Time.deltaTime;
								if(sqe.timeout <= 0f){
									send (sqe,messageTimeout);
									canSendMessage = false;
								}
						}
					}

					}

				} else {


					if(!triedEstablishingConnection){
					
					NetworkManager.singleton.networkAddress = queue[0].ip;
					
					NetworkManager.singleton.StartClient();

					triedEstablishingConnection = true;


					}

				}


			} else if (NetworkManager.singleton.isNetworkActive) {

				if(networkActionsObject != null){
					Network.Disconnect();
				}

			}

		
		}

	}



	public void setExpectedNextMessage(int nextmessage){

		idCounter = nextmessage;


	
		foreach (SendQueueEntry sqe in queue) {


			sqe.id = idCounter;
			idCounter++;

			if (idCounter == int.MaxValue) {
				idCounter = 0;
			}

		}

		PlayerPrefs.SetInt ("nextmessage_" + NetworkManager.singleton.networkAddress, idCounter);

		receivedExpectedMessageId = true;



	}


	public void addMessageToQueue(string ip, string var, string value){

		SendQueueEntry sqe = new SendQueueEntry ();

		if (idCounter == 0) {


			if(PlayerPrefs.HasKey("nextmessage_" + sqe.ip)){

				idCounter = PlayerPrefs.GetInt ("nextmessage_" + sqe.ip);

			}

		}


		sqe.id = idCounter;
		idCounter++;

		if (idCounter == int.MaxValue) {
			idCounter = 0;
		}


		PlayerPrefs.SetInt ("nextmessage_" + sqe.ip, idCounter);


		sqe.mode = MODE_VALUE;
		sqe.timeout = 0f;

		sqe.ip = ip;
		sqe.var = var;
		sqe.value = value;




		queue.Add (sqe);

	}


	public void addMessageToQueue(string ip, string var,string filetype, byte[] bytes,int part){
		
		SendQueueEntry sqe = new SendQueueEntry ();
		
		
		sqe.id = idCounter;
		idCounter++;


		if (part == 0) {

			sqe.mode = MODE_FILE_START;

		} else {
		
			sqe.mode = MODE_FILE_MID;


		}
		sqe.timeout = 0f;
		
		sqe.ip = ip;
		sqe.var = var;
		sqe.filetype = filetype;

		sqe.file = bytes;
		
		
		
		
		queue.Add (sqe);
		
	}

	public void addFinishMessageToQueue (string ip, string var, string filetype)
	{

		SendQueueEntry sqe = new SendQueueEntry ();
		
		
		sqe.id = idCounter;
		idCounter++;
		sqe.timeout = 0f;

		sqe.ip = ip;
		sqe.var = var;
		
		sqe.mode = MODE_FILE_FINISH;
	
		queue.Add (sqe);

	
	}

	public void setNetworkIdentity(networkactions na){
		
		networkActionsObject = na;
		
	}






	public void messageReceived(int id){


		foreach (SendQueueEntry sqe in queue.GetRange(0,queue.Count)) {

			if(sqe.id == id){


				queue.Remove(sqe);
			}



		}


	}


	public void send(SendQueueEntry sqe, float timeout){


		//queue.Remove (sqe);


		sqe.timeout = messageTimeout;


		if (sqe.mode == MODE_VALUE) {


		networkActionsObject.CmdSendVar(sqe.id, deviceid,sqe.var,sqe.value);


		} else if (sqe.mode == MODE_FILE_START) {

			networkActionsObject.CmdSendFile(sqe.id, deviceid,sqe.var,sqe.filetype,sqe.file);
					
		} else if (sqe.mode == MODE_FILE_MID) {

			networkActionsObject.CmdAddToFile(sqe.id, deviceid,sqe.var,sqe.filetype,sqe.file);

		} else if (sqe.mode == MODE_FILE_FINISH) {
			
			networkActionsObject.CmdFinishFile(sqe.id, deviceid,sqe.var,sqe.filetype);

		}



	}


}


[System.Serializable]
public class SendQueueEntry{

	public int id;

	public string ip;

	public float timeout;
	public string mode;

	public string var;

	public string value;

	public string filetype;
	public byte[] file;



}