using UnityEngine;
using System.Collections;
using GQ.Client.Conf;

public class deactivateGameObjectInAutoStartMode : MonoBehaviour {



	public bool orActivate = false;


	void Start () {


		if ( ConfigurationManager.Current.autoStartQuestID != 0 ) {


			gameObject.SetActive(orActivate);


		}
	


	}

}
