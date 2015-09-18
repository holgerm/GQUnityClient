using UnityEngine;
using UnityEngine.Networking;

using System.Collections;

public class networkactions : NetworkBehaviour{







	void Start(){



		//CmdSendVar ();



		if (GameObject.Find ("QuestDatabase") != null) {


			GameObject.Find ("QuestDatabase").SendMessage("setNetworkIdentity",this);


		}


	}









	[Command]
public void	CmdSendVar(string deviceid, string var, string value){

		SendVariable v = new SendVariable ();
		v.deviceid = deviceid;
		v.var = var;
		v.value = value;

		GameObject.Find("MediaServer").SendMessage("setVar",v);

	}



	
	[Command]
	public void	CmdSendFile(string deviceid, string var, string filetype, byte[] file){
		
		SendVariable v = new SendVariable ();
		v.deviceid = deviceid;
		v.var = var;

		v.filetype = filetype;
		v.bytes = file;
		
		GameObject.Find("MediaServer").SendMessage("setFile",v);
		
	}






	
	[Command]
	public void	CmdAddToFile(string deviceid, string var, string filetype, byte[] file){
		
		SendVariable v = new SendVariable ();
		v.deviceid = deviceid;
		v.var = var;
		
		v.filetype = filetype;
		v.bytes = file;
		
		GameObject.Find("MediaServer").SendMessage("addToFile",v);
		
	}



	
	[Command]
	public void	CmdFinishFile(string deviceid, string var, string filetype){
		
		SendVariable v = new SendVariable ();
		v.deviceid = deviceid;
		v.var = var;
		v.filetype = filetype;
		GameObject.Find("MediaServer").SendMessage("finishFile",v);
		
	}



}

[System.Serializable]
public class SendVariable{


	public string deviceid;
	public string var;
	public string value;

	public string filepath;
	public byte[] bytes;
	public string filetype;

}
