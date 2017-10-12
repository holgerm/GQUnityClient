using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Err;

public class MenuCtrl : MonoBehaviour
{

	void Start ()
	{
		// TODO Assert that both map and list panels exist in the herarchy and have the correct name!
	}

	public GameObject ListPanel;
	public GameObject MapPanel;


	public void OnChangeQuestInfosViewer (GameObject viewer)
	{
		ListPanel.SetActive (viewer.name == "ViewAsList");
		MapPanel.SetActive (viewer.name == "ViewOnMap");
	}

}
