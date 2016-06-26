using UnityEngine;
using System.Collections;
using GQ.Client.Conf;

public class removeQuestButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
	

		if ( !Configuration.instance.localQuestsDeletable ) {

			gameObject.SetActive(false);

		}

	
	}
	

}
