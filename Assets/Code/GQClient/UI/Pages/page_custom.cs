using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using System.IO;
using System;
using GQ.Util;
using Candlelight.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using GQ.Client.Model;

public class page_custom : MonoBehaviour
{




	public questdatabase questdb;
	public Quest quest;
	public Page questpage;
	public actions questactions;


	public GameObject prefab;


	public string param1;
	public string param2;
	public string param3;
	public string param4;
	public string param5;
	public string param6;
	public string param7;
	public string param8;
	public string param9;
	public string param10;



	void Start ()
	{ 
		if (GameObject.Find ("QuestDatabase") == null) {

			SceneManager.LoadScene ("questlist");
		} else {


			questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
			quest = QuestManager.Instance.CurrentQuest;
			questpage = QuestManager.Instance.CurrentQuest.currentpage;
			questactions = GameObject.Find ("QuestDatabase").GetComponent<actions> ();




			param1 = questpage.getAttribute ("param1");
			param2 = questpage.getAttribute ("param2");
			param3 = questpage.getAttribute ("param3");
			param4 = questpage.getAttribute ("param4");
			param5 = questpage.getAttribute ("param5");
			param6 = questpage.getAttribute ("param6");
			param7 = questpage.getAttribute ("param7");
			param8 = questpage.getAttribute ("param8");
			param9 = questpage.getAttribute ("param9");
			param10 = questpage.getAttribute ("param10");

			// Set prefab from ressources instead manually.
			switch (questpage.getAttribute ("modul")) {
			case "farbspiel":
				prefab = Resources.Load ("customPageTypes/farbspiel/farbspiel") as GameObject;
				break;
			case "nfcgainpoints":
				prefab = Resources.Load ("customPageTypes/nfcgainpoints/nfcgainpoints") as GameObject;
				break;
			default:
				break;
			}

			GameObject go = Instantiate (prefab);

			if (go.GetComponent<LinkToPageController> () != null) {
				go.GetComponent<LinkToPageController> ().controller = this;
			}

		}

	}




	public void onEnd ()
	{

		questpage.stateOld = "succeeded";

		if (questpage.onEnd != null && questpage.onEnd.actions != null && questpage.onEnd.actions.Count > 0) {

			questpage.onEnd.Invoke ();

		} else {
			//Debug.Log ("ending");
			GameObject questDBGO = GameObject.Find ("QuestDatabase");
			if (questDBGO != null) {
				questDBGO.GetComponent<questdatabase> ().endQuest ();
			}

		}


	}

}

