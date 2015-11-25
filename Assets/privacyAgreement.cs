using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Candlelight.UI;

public class privacyAgreement : MonoBehaviour {


	public HyperText textObject;

	public string version;


	public void disableGameObject(){

		gameObject.SetActive (false);

	}



	public void accept(){

		GetComponent<Animator> ().SetTrigger ("out");

		PlayerPrefs.SetString ("privacyAgreementVersionRead", version);

	}
}
