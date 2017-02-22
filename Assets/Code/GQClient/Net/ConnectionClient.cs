using UnityEngine;
using UnityEngine.Networking;

using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System;
using GQ.Client.Net;
using GQ.Util;

namespace GQ.Client.Net {
	
	public class ConnectionClient : MonoBehaviour, IEnumerationWorker {

		public GameObject unsendMessagesMessage;

		public bool receivedExpectedMessageId = false;

		private networkactions networkActionsObject;

		public networkactions NetworkActionsObject {
			set {
				Debug.Log("CONNECTION_CLIENT set NWAO");
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

		public SendQueue sendQueue;

		void Start () {
			messageTimerSave = messageTimer;
			connectionTimeoutSave = connectionTimeout;
			deviceid = SystemInfo.deviceUniqueIdentifier;

			sendQueue = new SendQueue(messageTimeout);

			if ( sendQueue.hasUnsendMessages() ) {

				unsendMessagesMessage.SetActive(true);
				unsendMessagesMessage.GetComponent<Animator>().SetTrigger("in");

			}

			sendQueue.setEnumerationWorker(this);

		}

		public void setExpectedNextMessage (int nm) {
		}

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

		public void addTextMessage (string ip, string var, string value, int questId) {
			sendQueue.addTextMessage(ip, var, value, questId);
		}

		public void addFileMessage (string ip, string var, string filetype, byte[] bytes, int part, int questId) {
			sendQueue.addFileMessage(ip, var, filetype, bytes, part, questId);
		}

		public void addFileFinishMessage (string ip, string var, string filetype, int questId) {
			sendQueue.addFileFinishMessage(ip, var, filetype, questId);
		}

		public void setNetworkIdentity (networkactions na) {
			NetworkActionsObject = na;
	
			sendQueue.NetworkActionsObject = na;
		}

		public void messageReceived (int id) {
			sendQueue.removeMessage(id);
		}

	}

}