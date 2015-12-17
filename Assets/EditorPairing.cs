using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class EditorPairing : MonoBehaviour {




	public bool active;
	public bool hasPairingKey;
	public string key;
	public Text codeTextObject;
	public Text activationButtonTextObject;



	public Color activeColor;
	public Color inactiveColor;



	public float checkEvery = 6f;

	float checkEverySaved = 6f;


	void Start(){
		checkEverySaved = checkEvery;

		loadFromPlayerPrefs ();

	}


	void Update(){


		if (hasPairingKey && active) {

			checkEvery -= Time.deltaTime;

			if(checkEvery <= 0f){
				checkEvery = checkEverySaved;

				WWW www = new WWW("http://qeevee.org:9091/device/"+SystemInfo.deviceUniqueIdentifier+"/update");
				StartCoroutine(checkForPush(www));
			}

		}


	}


	IEnumerator checkForPush(WWW www){


		yield return www;


		if (www.error == null || www.error != "") {
			



			if(www.text != ""){

				Quest q = new Quest();
				q.id = int.Parse(www.text);

				q.alternateDownloadLink = "http://qeevee.org:9091/device/"+SystemInfo.deviceUniqueIdentifier+"/getxml";


				GetComponent<questdatabase>().downloadQuest(q);
			}
			
			
			
		} 
	




	}



	void loadFromPlayerPrefs(){
		


		if(PlayerPrefs.HasKey("pairing_key")){
			key = PlayerPrefs.GetString("pairing_key");
			                      }
		                      
		                      


		if (PlayerPrefs.HasKey ("pairing_active")) {

			int aktiv = PlayerPrefs.GetInt ("pairing_active");

		
			if (aktiv == 1) {
				active = true;
			} else {
				active = false;

			}
		

		}


		
		if (PlayerPrefs.HasKey ("pairing_hasKey")) {
			
			int hatSchluessel = PlayerPrefs.GetInt ("pairing_hasKey");
			
			
			if (hatSchluessel == 1) {
				hasPairingKey = true;
			} else {
				hasPairingKey = false;
				
			}
			
			
		}

		codeTextObject.text = key;

		if (!active) {
			activationButtonTextObject.color = inactiveColor;
			activationButtonTextObject.text = "Inaktiv";
		} else {
			activationButtonTextObject.color = activeColor;
			activationButtonTextObject.text = "Aktiv";
		}

		
		}

	
	void saveToPlayerPrefs(){


								PlayerPrefs.SetString("pairing_key",key);



			                      int aktiv = 0;
			                      if(active){ aktiv = 1; }

		PlayerPrefs.SetInt("pairing_active",aktiv);

								int hatSchluessel = 0;
								if(hasPairingKey){ hatSchluessel = 1; }


		PlayerPrefs.SetInt("pairing_hasKey",hatSchluessel);






	}


	public void getKey(){



		if (!hasPairingKey) {


			WWW kwww = new WWW("http://qeevee.org:9091/devices/pair/"+SystemInfo.deviceModel+"/"+SystemInfo.deviceUniqueIdentifier);

			StartCoroutine(waitForKey(kwww));


		}




	}



	public void toggleActivation(){

		if (active) {
			active = false;
			activationButtonTextObject.color = inactiveColor;
			activationButtonTextObject.text = "Inaktiv";
		} else {
			active = true;
			activationButtonTextObject.color = activeColor;
			activationButtonTextObject.text = "Aktiv";
		}

		saveToPlayerPrefs ();

	}


	public void setActive(){


		active = true;
		activationButtonTextObject.color = Color.green;
		activationButtonTextObject.text = "Aktiv";
		saveToPlayerPrefs ();
	}

	IEnumerator waitForKey(WWW www){


		Debug.Log (www.url);

				yield return www;



				if (www.error == null || www.error != "") {


					codeTextObject.text = www.text;
					key = www.text;
					hasPairingKey = true;
					setActive();

		



		} else {

					codeTextObject.text = www.error;

		}



	}







}
