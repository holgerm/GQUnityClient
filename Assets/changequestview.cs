using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using GQ.Client.Conf;
using UnityEngine.SceneManagement;

public class changequestview : MonoBehaviour {



	void Start () {
		bool qvc = ConfigurationManager.Current.questVisualizationChangeable;
		gameObject.SetActive(qvc);
	}

	public void switchQuestView () {


		if ( ConfigurationManager.Current.questVisualization == "map" ) {
			ConfigurationManager.Current.questVisualization = "list";
			GameObject.Find("MenuCanvas").GetComponent<Animator>().ResetTrigger("startMenu");
			GameObject.Find("MenuCanvas").GetComponent<Animator>().SetTrigger("endmenu");
		}
		else
		if ( ConfigurationManager.Current.questVisualization == "list" ) {
			ConfigurationManager.Current.questVisualization = "map";
		}

		SceneManager.LoadScene("questlist");
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
