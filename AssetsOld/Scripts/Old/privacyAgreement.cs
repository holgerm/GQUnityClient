using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Candlelight.UI;

public class privacyAgreement : MonoBehaviour {


	public HyperText textObject;

	public int version;

	public bool privacy = false;
	public bool agb = false;

	public void disableGameObject(){
		gameObject.SetActive (false);

	}



	public void accept(){

		GameObject.Find("QuestDatabase").GetComponent<questdatabase>().hideBlackCanvas ();
		GetComponent<Animator> ().SetTrigger ("out");

		if (privacy) {
			PlayerPrefs.SetInt ("privacyAgreementVersionRead", version);
		} else if (agb) {
			PlayerPrefs.SetInt ("agbVersionRead", version);

		}

	}
}
