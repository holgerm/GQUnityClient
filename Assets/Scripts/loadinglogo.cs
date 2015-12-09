using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class loadinglogo : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
//		disable ();
	}
	



	public void disable ()
	{

		foreach (Image i in GetComponentsInChildren<Image>()) {

			i.enabled = false;

		}

		foreach (Text t in GetComponentsInChildren<Text>()) {
			
			t.enabled = false;
			
		}

	}



	public void enable ()
	{

		foreach (Image i in GetComponentsInChildren<Image>()) {
			
			i.enabled = true;
			
		}

		foreach (Text t in GetComponentsInChildren<Text>()) {
			
			t.enabled = true;
		}
		if (!Configuration.instance.showtextinloadinglogo) {



	 

			if (GameObject.Find ("QuestDatabase").GetComponent<questdatabase> () != null) {
				if (GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().webloadingmessage != null) {
					GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().webloadingmessage.enabled = false;
				}
			}
		}
		
	}
}
