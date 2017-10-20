using UnityEngine;
using System.Collections;
using GQ.Client.Conf;

public class deactivateGameObjectInAutoStartMode : MonoBehaviour
{



	public bool orActivate = false;


	void Start ()
	{


		if (questdatabase.shouldPerformAutoStart ()) {


			gameObject.SetActive (orActivate);


		}
	


	}

}
