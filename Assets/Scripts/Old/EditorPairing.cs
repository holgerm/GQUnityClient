using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using GQ.Client.Model;

public class EditorPairing : MonoBehaviour {




	public bool active;
	public bool hasPairingKey;
	public string key;
	public Text codeTextObject;
	public Text activationButtonTextObject;

	private static int checkingCounter;

	public Color activeColor;
	public Color inactiveColor;



	public float checkEvery = 6f;

	float checkEverySaved = 6f;


	void Start () {
		checkEverySaved = checkEvery;

		loadFromPlayerPrefs();

		checkingCounter = 0;

	}


	void Update () {


		if ( hasPairingKey && active && checkingCounter < 2 ) {

			checkEvery -= Time.deltaTime;

			if ( checkEvery <= 0f ) {
				checkEvery = checkEverySaved;

				WWW www = new WWW("http://qeevee.org:9091/device/" + SystemInfo.deviceUniqueIdentifier + "/update");
				checkingCounter++;
				StartCoroutine(checkForPush(www));
			}

		}


	}


	IEnumerator checkForPush (WWW www) {
//		Debug.Log("PARING: CHECKING FOR PUSH: " + checkingCounter);

		yield return www;

		checkingCounter--;

		if ( www.error == null || www.error != "" ) {

			if ( www.text != "" ) {

				Quest q = new Quest();
				q.Id = int.Parse(www.text);
				q.alternateDownloadLink = "http://qeevee.org:9091/device/" + SystemInfo.deviceUniqueIdentifier + "/getxml";

				GetComponent<questdatabase>().downloadQuest(q);
			}
		} 
	}



	void loadFromPlayerPrefs () {
		


		if ( PlayerPrefs.HasKey("pairing_key") ) {
			key = PlayerPrefs.GetString("pairing_key");
		}
		                      
		                      


		if ( PlayerPrefs.HasKey("pairing_active") ) {

			int aktiv = PlayerPrefs.GetInt("pairing_active");

		
			if ( aktiv == 1 ) {
				active = true;
			}
			else {
				active = false;

			}
		

		}


		
//		if ( PlayerPrefs.HasKey("pairing_hasKey") ) {
//			
//			int hatSchluessel = PlayerPrefs.GetInt("pairing_hasKey");
//			
//			
//			if ( hatSchluessel == 1 ) {
//				hasPairingKey = true;
//			}
//			else {
//				hasPairingKey = false;
//				
//			}
//			
//			
//		}

		codeTextObject.text = key;

		if ( !active ) {
			activationButtonTextObject.color = inactiveColor;
			activationButtonTextObject.text = "Inaktiv";
		}
		else {
			activationButtonTextObject.color = activeColor;
			activationButtonTextObject.text = "Aktiv";
		}

		
	}

	
	void saveToPlayerPrefs () {
		PlayerPrefs.SetString("pairing_key", key);
//		int aktiv = 0;
//		if ( active ) {
//			aktiv = 1;
//		}
//
//		PlayerPrefs.SetInt("pairing_active", aktiv);
		PlayerPrefs.SetInt("pairing_active", (active ? 1 : 0));

//		int hatSchluessel = 0;
//		if ( hasPairingKey ) {
//			hatSchluessel = 1;
//		}
//
//
//		PlayerPrefs.SetInt("pairing_hasKey", (hatSchluessel));
		PlayerPrefs.SetInt("pairing_hasKey", (hasPairingKey ? 1 : 0));
	}


	public void getKey () {
		if ( !hasPairingKey ) {

			string keyURL = "http://qeevee.org:9091/devices/pair/"
			                + SystemInfo.deviceModel +
			                "/" + SystemInfo.deviceUniqueIdentifier;
			keyURL = keyURL.Replace(" ", string.Empty);
			WWW kwww = new WWW(keyURL);

			StartCoroutine(waitForKey(kwww));
		}
	}



	public void toggleActivation () {

		if ( active ) {
			active = false;
			activationButtonTextObject.color = inactiveColor;
			activationButtonTextObject.text = "Inaktiv";
		}
		else {
			active = true;
			activationButtonTextObject.color = activeColor;
			activationButtonTextObject.text = "Aktiv";
		}

		saveToPlayerPrefs();
	}


	public void setActive () {
		active = true;
		activationButtonTextObject.color = Color.green;
		activationButtonTextObject.text = "Aktiv";
		saveToPlayerPrefs();
	}

	IEnumerator waitForKey (WWW www) {

		yield return www;

		if ( www.error == null || www.error != "" ) {
			codeTextObject.text = www.text;
			key = www.text;
			hasPairingKey = true;
			setActive();
		}
		else {
			codeTextObject.text = www.error;
		}
	}







}
