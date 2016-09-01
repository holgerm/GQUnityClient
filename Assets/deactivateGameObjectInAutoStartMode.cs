using UnityEngine;
using System.Collections;
using GQ.Client.Conf;

public class deactivateGameObjectInAutoStartMode : MonoBehaviour
{



	public bool orActivate = false;


	void Start()
	{


		if (Configuration.instance.autostartQuestID != 0)
		{


			gameObject.SetActive(orActivate);


		}
	


	}

}
