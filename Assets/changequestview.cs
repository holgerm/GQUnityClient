using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class changequestview : MonoBehaviour {



	void Start () {
		gameObject.SetActive(Configuration.instance.questvisualizationchangable);
	}

	public void switchQuestView () {


		if ( Configuration.instance.questvisualization == "map" ) {
			Configuration.instance.questvisualization = "list";
			GameObject.Find("MenuCanvas").GetComponent<Animator>().ResetTrigger("startMenu");
			GameObject.Find("MenuCanvas").GetComponent<Animator>().SetTrigger("endmenu");
		}
		else
		if ( Configuration.instance.questvisualization == "list" ) {
			Configuration.instance.questvisualization = "map";
		}

		Application.LoadLevel(0);

	}

//	void Update () {
//
//
//		if ( GameObject.Find("QuestDatabase") != null && GameObject.Find("QuestDatabase").GetComponent<questdatabase>().currentquest == null
//		    ) {
//			GetComponent<Image>().enabled = true;
//			GetComponent<Button>().enabled = true;
//			GetComponentInChildren<Text>().enabled = true;
//
//		}
//		else {
//
//			if ( GameObject.Find("QuestDatabase").GetComponent<questdatabase>().currentquest.name != null && GameObject.Find("QuestDatabase").GetComponent<questdatabase>().currentquest.name.Length > 0 ) {
////				Debug.Log("currentquest:"+GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest.name);
//				GetComponent<Image>().enabled = false;
//				GetComponent<Button>().enabled = false;
//				GetComponentInChildren<Text>().enabled = false;
//			}
//			else {
//
//				GetComponent<Image>().enabled = true;
//				GetComponent<Button>().enabled = true;
//				GetComponentInChildren<Text>().enabled = true;
//
//
//			}
//
//		}
//
//	}
}
