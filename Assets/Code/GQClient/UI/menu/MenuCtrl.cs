using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Err;

public class MenuCtrl : MonoBehaviour
{

	public void OnChangeQuestInfosViewer (GameObject viewer)
	{
		Debug.Log (("We need to change the Quest Infos Viewer to: " + viewer.name).Red ());
	}

}
