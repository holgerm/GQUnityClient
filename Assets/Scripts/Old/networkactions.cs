using UnityEngine;
using UnityEngine.Networking;

using System.Collections;
using GQ.Client.Net;

public class networkactions : NetworkBehaviour
{


	bool sendInitial = false;
	bool receivedInitial = false;
	bool complete = false;


	float timer = 10f;
	float timer_save = 10f;

	void Start ()
	{



		//CmdSendVar ();



		if (GameObject.Find ("QuestDatabase") != null) {

			timer_save = timer;
			CmdSendFirstMessageId (GameObject.Find ("QuestDatabase").GetComponent<ConnectionClient> ().sendQueue.getMin (), SystemInfo.deviceUniqueIdentifier);
			sendInitial = true;



		}


	}



	void Update ()
	{
		if (!complete && sendInitial) {

			if (!receivedInitial) {
				timer -= Time.deltaTime;

				if (timer <= 0) {

					timer = timer_save;

					CmdSendFirstMessageId (GameObject.Find ("QuestDatabase").GetComponent<ConnectionClient> ().sendQueue.getMin (), SystemInfo.deviceUniqueIdentifier);

				}
			} else {
				complete = true;
				GameObject.Find ("QuestDatabase").SendMessage ("setNetworkIdentity", this);


			}



		}



	}



	[Command]
	public void	CmdSendFirstMessageId (int id, string deviceid)
	{

		SendVariable v = new SendVariable ();
		v.id = id;
		v.messagetype = "setNextMessageId";
		v.deviceid = deviceid;
	

		GameObject.Find ("MediaServer").SendMessage ("setExpectedMessageId", v);
		RpcMessageSuccesful (-1);

	}







	[Command]
	public void	CmdSendVar (int id, string deviceid, string var, string value, bool reset)
	{

		SendVariable v = new SendVariable ();
		v.id = id;
		v.messagetype = "setVar";
		v.deviceid = deviceid;
		v.var = var;
		v.value = value;
		v.resetid = reset;

		GameObject.Find ("MediaServer").SendMessage ("addSendVariableToQueue", v);
		RpcMessageSuccesful (id);

	}



	
	[Command]
	public void	CmdSendFile (int id, string deviceid, string var, string filetype, byte[] file, bool reset)
	{
		
		SendVariable v = new SendVariable ();
		v.id = id;
		v.messagetype = "setFile";
		v.deviceid = deviceid;
		v.var = var;
		v.resetid = reset;
		v.filetype = filetype;
		v.bytes = file;
		
		GameObject.Find ("MediaServer").SendMessage ("addSendVariableToQueue", v);
		RpcMessageSuccesful (id);

	}






	
	[Command]
	public void	CmdAddToFile (int id, string deviceid, string var, string filetype, byte[] file, bool reset)
	{
		
		SendVariable v = new SendVariable ();
		v.id = id;
		v.messagetype = "addToFile";
		v.deviceid = deviceid;
		v.var = var;
		v.resetid = reset;
		v.filetype = filetype;
		v.bytes = file;
		
		GameObject.Find ("MediaServer").SendMessage ("addSendVariableToQueue", v);
		RpcMessageSuccesful (id);

	}



	
	[Command]
	public void	CmdFinishFile (int id, string deviceid, string var, string filetype, bool reset)
	{
		
		SendVariable v = new SendVariable ();
		v.id = id;
		v.messagetype = "finishFile";
		v.deviceid = deviceid;
		v.var = var;
		v.resetid = reset;
		v.filetype = filetype;
		GameObject.Find ("MediaServer").SendMessage ("addSendVariableToQueue", v);

		RpcMessageSuccesful (id);
	}





	[ClientRpc]
	public void RpcMessageSuccesful (int id)
	{

		if (id == -1) {

			receivedInitial = true;

		}

		GameObject.Find ("QuestDatabase").SendMessage ("messageReceived", id);

	}



	[Command]
	public void	CmdAskForNextExpectedMessage (string deviceid)
	{

		int expectedmessage = 0;

		if (PlayerPrefs.HasKey ("nextmessage_" + deviceid)) {


			expectedmessage = PlayerPrefs.GetInt ("nextmessage_" + deviceid);

		}

		RpcReturnNextExpectedMessage (expectedmessage);

	}


	[ClientRpc]
	public void RpcReturnNextExpectedMessage (int nextexpectedmessage)
	{

	

		GameObject.Find ("QuestDatabase").SendMessage ("setExpectedNextMessage", nextexpectedmessage);


	}


}

[System.Serializable]
public class SendVariable
{

	public int id;
	public string messagetype;
	public int fileid;

	public string deviceid;
	public string var;
	public string value;

	public string filepath;
	public byte[] bytes;
	public string filetype;

	public bool resetid;

}
