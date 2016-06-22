using UnityEngine;
using UnityEngine.Networking;

using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using System.IO;
using System;
using GQ.Client.Net;
using GQ.Util;

namespace GQ.Client.Net {
	
	public class ConnectionClient : MonoBehaviour, IEnumerationWorker {

		public bool receivedExpectedMessageId = false;
		//		public List<SendQueueEntry> queue;

		private networkactions networkActionsObject;

		public networkactions NetworkActionsObject {
			set {
				networkActionsObject = value;
			}
		}

		public float messageTimer = 0.01f;
		float messageTimerSave = 0.2f;
		public float messageTimeout = 10f;
		public string deviceid;
		public int idCounter = 0;
		public bool triedEstablishingConnection = false;
		public float connectionTimeout = 10f;
		float connectionTimeoutSave = 10f;

		private ISendQueue sendQueue;

		void Start () {

//			queue = new List<SendQueueEntry>();
			messageTimerSave = messageTimer;
			connectionTimeoutSave = connectionTimeout;
			deviceid = SystemInfo.deviceUniqueIdentifier;

			sendQueue = new SendQueue(messageTimeout);
			sendQueue.reconstructSendQueue();

//			reconstructSendQueue();
		}

		public void setExpectedNextMessage (int nm) {
		}

		//		public void reconstructSendQueue () {
		//
		//			if ( Directory.Exists(Application.persistentDataPath + "/quests/") ) {
		//
		//				foreach ( string quest in	Directory.GetDirectories(Application.persistentDataPath + "/quests/") ) {
		//
		//					if ( Directory.Exists(quest + "/sendqueue/") ) {
		//						string FolderName = new DirectoryInfo(quest).Name;
		//
		//						foreach ( string file in	Directory.GetFiles(quest + "/sendqueue/") ) {
		//							int num1 = 0;
		//							int num2 = 0;
		//
		//							if ( int.TryParse(FolderName, out num1) ) {
		//								if ( int.TryParse(Path.GetFileNameWithoutExtension(file), out num2) ) {
		//									if ( File.Exists(Application.persistentDataPath + "/quests/" + num1 + "/sendqueue/" + num2 + ".json") ) {
		//										WWW www = LocalWWW.Create("/quests/" + num1 + "/sendqueue/" + num2 + ".json");
		//										StartCoroutine(deserialize(www));
		//
		//									}
		//								}
		//							}
		//						}
		//					}
		//				}
		//			}
		//		}

		public void enumerate (IEnumerator enumerator) {
			StartCoroutine(enumerator);
		}

		void Update () {
			if ( sendQueue.Count == 0 )
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

				if ( networkActionsObject != null ) {
					sendQueue.sendNext();
				}
				else {
					if ( !triedEstablishingConnection ) {
						triedEstablishingConnection = sendQueue.startConnectingToServer();
					}
				}
			}
//		else {
//			if ( NetworkManager.singleton.isNetworkActive ) {
//
//				if ( networkActionsObject != null ) {
//					Network.Disconnect();
//				}
//
//			}
//		}
		}

		//		void UpdateOld () {
		//			if ( queue.Count == 0 )
		//				return;
		//
		//			bool canSendMessage = false;
		//			messageTimer -= Time.deltaTime;
		//
		//			if ( networkActionsObject == null && triedEstablishingConnection ) {
		//
		//				connectionTimeout -= Time.deltaTime;
		//
		//				if ( connectionTimeout <= 0f ) {
		//					connectionTimeout = connectionTimeoutSave;
		//					triedEstablishingConnection = false;
		//					Network.Disconnect();
		//				}
		//			}
		//
		//			if ( messageTimer <= 0f ) {
		//				messageTimer = messageTimerSave;
		//				canSendMessage = true;
		//
		//				if ( networkActionsObject != null ) {
		//					foreach ( SendQueueEntry sqe in queue.GetRange(0,queue.Count) ) {
		//						if ( canSendMessage ) {
		//							sqe.timeout -= Time.deltaTime;
		//							if ( sqe.timeout <= 0f ) {
		//								sendQueue.send(sqe);
		//								//							send(sqe, messageTimeout);
		//								canSendMessage = false;
		//							}
		//						}
		//					}
		//				}
		//				else {
		//					if ( !triedEstablishingConnection ) {
		//						NetworkManager.singleton.networkAddress = queue[0].ip;
		//
		//						if ( !NetworkManager.singleton.IsClientConnected() )
		//							NetworkManager.singleton.StartClient();
		//
		//						triedEstablishingConnection = true;
		//					}
		//				}
		//			}
		//			//		else {
		//			//			if ( NetworkManager.singleton.isNetworkActive ) {
		//			//
		//			//				if ( networkActionsObject != null ) {
		//			//					Network.Disconnect();
		//			//				}
		//			//
		//			//			}
		//			//		}
		//		}

		public void addTextMessage (string ip, string var, string value, int questId) {
			sendQueue.addTextMessage(ip, var, value, questId);
//
//			SendQueueEntry
//			sqe = new SendQueueEntry();
//
//			if ( queue == null || queue.Count == 0 ) {
//
//				idCounter = 0;
//				sqe.resetid = true;
//
//			}
//
//			sqe.id = idCounter;
//			idCounter++;
//			sqe.questid = questId;
//
//			if ( idCounter == int.MaxValue ) {
//				idCounter = 0;
//			}
//
//			PlayerPrefs.SetInt("nextmessage_" + sqe.ip, idCounter);
//
//			sqe.mode = SendQueueHelper.MODE_VALUE;
//			sqe.timeout = 0f;
//
//			sqe.ip = ip;
//			sqe.var = var;
//			sqe.value = value;
//
//			queue.Add(sqe);
//			sqe.serialize();
//
		}

		public void addFileMessage (string ip, string var, string filetype, byte[] bytes, int part, int questId) {
			sendQueue.addFileMessage(ip, var, filetype, bytes, part, questId);
		
//			SendQueueEntry sqe = new SendQueueEntry();
//	
//			if ( queue == null || queue.Count == 0 ) {
//
//				idCounter = 0;
//				sqe.resetid = true;
//
//			}
//
//			sqe.id = idCounter;
//			idCounter++;
//			sqe.questid = questId;
//
//			if ( part == 0 ) {
//
//				sqe.mode = SendQueueHelper.MODE_FILE_START;
//
//			}
//			else {
//		
//				sqe.mode = SendQueueHelper.MODE_FILE_MID;
//
//
//			}
//			sqe.timeout = 0f;
//			sqe.ip = ip;
//			sqe.var = var;
//			sqe.filetype = filetype;
//			sqe.file = bytes;
//		
//			queue.Add(sqe);
//			sqe.serialize();
//
		}

		public void addFileFinishMessage (string ip, string var, string filetype, int questId) {
			sendQueue.addFileFinishMessage(ip, var, filetype, questId);
//
//			SendQueueEntry sqe = new SendQueueEntry();
//		
//		
//			sqe.id = idCounter;
//			sqe.questid = questId;
//			idCounter++;
//			sqe.timeout = 0f;
//
//			sqe.ip = ip;
//			sqe.var = var;
//		
//			sqe.mode = SendQueueHelper.MODE_FILE_FINISH;
//	
//			queue.Add(sqe);
//			sqe.serialize();
//
//	
		}

		public void setNetworkIdentity (networkactions na) {
			networkActionsObject = na;
	
			sendQueue.NetworkActionsObject = na;
		}

		public void messageReceived (int id) {
			sendQueue.removeMessage(id);
		}

		//		public IEnumerator deserialize (WWW www) {
		//
		//			yield return www;
		//
		//			if ( www.error == null || www.error == "" ) {
		//
		//				SendQueueEntry sqe = JsonMapper.ToObject<SendQueueEntry>(www.text);
		//				queue.Add(sqe);
		//
		//			}
		//			else {
		//
		//				Debug.Log(www.error);
		//
		//			}
	}

}