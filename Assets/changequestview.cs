using UnityEngine;
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
}
