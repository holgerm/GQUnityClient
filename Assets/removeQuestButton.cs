using UnityEngine;
using System.Collections;

public class removeQuestButton : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	

		if (!Configuration.instance.localQuestsDeletable) {

			gameObject.SetActive (false);

		}

	
	}
	

}
