using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class QuestMessage : MonoBehaviour {

	public Image boxbg;
	public Text boxtext;
	public Image buttonbg;
	public Text buttontext;

	public string message;



	public IEnumerator action;




	public void Start(){

		boxtext.text = GameObject.Find("QuestDatabase").GetComponent<actions> ().formatString (message);

	}


	public void setButtonText(string s){
		buttontext.text = GameObject.Find("QuestDatabase").GetComponent<actions> ().formatString (s);
		}
	

	public void done(){

		GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().msgsactive -= 1;
			Destroy (gameObject);


		}
}
