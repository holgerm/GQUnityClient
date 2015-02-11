using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class QuestMessage : MonoBehaviour {

	public Image boxbg;
	public Text boxtext;
	public Image buttonbg;
	public Text buttontext;

	public string message;



	public void Start(){

		boxtext.text = message;

	}


	public void setButtonText(string s){
		buttontext.text = s;
		}
	

	public void done(){

		Destroy (gameObject);


		}
}
