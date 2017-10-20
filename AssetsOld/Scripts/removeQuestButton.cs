using UnityEngine;
using System.Collections;
using GQ.Client.Conf;

public class removeQuestButton : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	

		if (!ConfigurationManager.Current.localQuestsDeletable) {

			gameObject.SetActive (false);

		}

	
	}
	

}
