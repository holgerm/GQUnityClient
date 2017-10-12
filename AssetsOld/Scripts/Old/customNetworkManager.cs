using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using UnityEngine.Networking;

//using System.Net.NetworkInformation;
using System;

public  class customNetworkManager : NetworkManager {




	void Start () {


		NetworkManager.singleton.StartClient();

	}



	public void connectToServer (string a4) {

	
		NetworkManager.singleton.networkAddress = a4;
		NetworkManager.singleton.StartClient();
		
	}




}
