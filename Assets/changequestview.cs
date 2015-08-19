using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class changequestview : MonoBehaviour {



	void Start(){


	}
	public void switchQuestView(){


		if (Configuration.instance.questvisualization == "map") {
			Configuration.instance.questvisualization = "list";
			GameObject.Find("MenuCanvas").GetComponent<Animator>().SetTrigger("endmenu");
		} else if (Configuration.instance.questvisualization == "list") {
			Configuration.instance.questvisualization = "map";
		}

		Application.LoadLevel (0);

	}


	void Update(){


		if (GameObject.Find ("QuestDatabase") != null && GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest == null
		    ) {
			GetComponent<Image> ().enabled = true;
			GetComponent<Button> ().enabled = true;
			GetComponentInChildren<Text> ().enabled = true;

		} else {

			if(GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest.name != ""){
				Debug.Log("currentquest:"+GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest.name);
			GetComponent<Image> ().enabled = false;
			GetComponent<Button> ().enabled = false;
			GetComponentInChildren<Text> ().enabled = false;
			}

		}

	}
}
