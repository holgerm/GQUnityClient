using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GQ.Util;
using LitJson;
using UnityEngine.Networking;

namespace GQ.Client.Net {

	[System.Serializable]
	public class SendQueue : ISendQueue {

		public List<SendQueueEntry> _queue;
		private int _idCounter;

		private float _timeout = 10.0f;
		private networkactions _networkActionsObject;

		// TODO set directly in constructor via GameObject Find with Tag
		public networkactions NetworkActionsObject {
			set {
				Debug.Log("SENDQUEUE set networkactionsobject");
				_networkActionsObject = value;
			}
		}

		public SendQueue (float timeout) {
			_timeout = timeout;
			_queue = new List<SendQueueEntry>();
			_idCounter = 0;
		}

		public int Count {
			get {
				return _queue.Count;
			}
		}

		public void sendNext () {
			bool canSendMessage = true;
			foreach ( SendQueueEntry sqe in _queue.GetRange(0, _queue.Count) ) {
				if ( canSendMessage ) {
					sqe.timeout -= Time.deltaTime;
					if ( sqe.timeout <= 0f ) {
						send(sqe);
						canSendMessage = false;
					}
				}
			}

		}

		private void send (SendQueueEntry message) {
			Debug.Log("SENDQUEUE: send()");

			message.timeout = _timeout;

			if ( message.mode == SendQueueHelper.MODE_VALUE ) {
				_networkActionsObject.CmdSendVar(message.id, SystemInfo.deviceUniqueIdentifier, message.var, message.value, message.resetid);
			}
			else
			if ( message.mode == SendQueueHelper.MODE_FILE_START ) {

				_networkActionsObject.CmdSendFile(message.id, SystemInfo.deviceUniqueIdentifier, message.var, message.filetype, message.file, message.resetid);

			}
			else
			if ( message.mode == SendQueueHelper.MODE_FILE_MID ) {

				_networkActionsObject.CmdAddToFile(message.id, SystemInfo.deviceUniqueIdentifier, message.var, message.filetype, message.file, message.resetid);

			}
			else
			if ( message.mode == SendQueueHelper.MODE_FILE_FINISH ) {

				_networkActionsObject.CmdFinishFile(message.id, SystemInfo.deviceUniqueIdentifier, message.var, message.filetype, message.resetid);

			}
		}

		public void addTextMessage (string ip, string var, string text, int questId) {

			SendQueueEntry sqe = new SendQueueEntry();

			if ( _queue == null || _queue.Count == 0 ) {

				_idCounter = 0;
				sqe.resetid = true;

			}

			sqe.id = _idCounter;
			_idCounter++;
			sqe.questid = questId;

			if ( _idCounter == int.MaxValue ) {
				_idCounter = 0;
			}

			PlayerPrefs.SetInt("nextmessage_" + sqe.ip, _idCounter);

			sqe.mode = SendQueueHelper.MODE_VALUE;
			sqe.timeout = 0f;

			sqe.ip = ip;
			sqe.var = var;
			sqe.value = text;

			_queue.Add(sqe);
			sqe.serialize();
		}


		public void addFileMessage (string ip, string var, string filetype, byte[] bytes, int part, int questId) {

			SendQueueEntry sqe = new SendQueueEntry();

			if ( _queue == null || _queue.Count == 0 ) {

				_idCounter = 0;
				sqe.resetid = true;

			}

			sqe.id = _idCounter;
			_idCounter++;
			sqe.questid = questId;


			if ( part == 0 ) {

				sqe.mode = SendQueueHelper.MODE_FILE_START;

			}
			else {

				sqe.mode = SendQueueHelper.MODE_FILE_MID;


			}
			sqe.timeout = 0f;
			sqe.ip = ip;
			sqe.var = var;
			sqe.filetype = filetype;
			sqe.file = bytes;

			_queue.Add(sqe);
			sqe.serialize();
		}

		public void addFileFinishMessage (string ip, string var, string filetype, int questId) {

			SendQueueEntry sqe = new SendQueueEntry();


			sqe.id = _idCounter;
			sqe.questid = questId;
			_idCounter++;
			sqe.timeout = 0f;

			sqe.ip = ip;
			sqe.var = var;

			sqe.mode = SendQueueHelper.MODE_FILE_FINISH;

			_queue.Add(sqe);
			sqe.serialize();
		}

		private IEnumerationWorker enumerationWorker;

		void setEnumerationWorker (IEnumerationWorker worker) {
			this.enumerationWorker = worker;
		}

		public void reconstructSendQueue () {

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
										WWW www = LocalWWW.Create("/quests/" + num1 + "/sendqueue/" + num2 + ".json");
										enumerationWorker.enumerate(deserialize(www));

									}
								}
							}
						}
					}
				}
			}
		}

		private IEnumerator deserialize (WWW www) {

			yield return www;

			if ( www.error == null || www.error == "" ) {
				SendQueueEntry sqe = JsonMapper.ToObject<SendQueueEntry>(www.text);
				_queue.Add(sqe);
			}
			else {
				Debug.Log(www.error);
			}
		}

		public bool startConnectingToServer () {
			if ( Count == 0 )
				return false;

			NetworkManager.singleton.networkAddress = _queue[0].ip;
			// TODO replace by something good. 
			// Either set one fixed ip in constructor or create pool of cennections and ips in dictionary whenever we add a message with new ip.



			if ( !NetworkManager.singleton.IsClientConnected() )
				NetworkManager.singleton.StartClient();	

			return true;
		}

		public void removeMessage (int id) {
			foreach ( SendQueueEntry sqe in _queue.GetRange(0, Count) ) {
				if ( sqe.id == id ) {
					_queue.Remove(sqe);

					if ( File.Exists(Application.persistentDataPath + "/quests/" + sqe.questid + "/sendqueue/" + sqe.id + ".json") ) {
						File.Delete(Application.persistentDataPath + "/quests/" + sqe.questid + "/sendqueue/" + sqe.id + ".json");
					}
				}
			}

		}

	}

}