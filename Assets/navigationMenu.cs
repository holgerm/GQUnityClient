using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using UnityEngine.SceneManagement;

public class navigationMenu : MonoBehaviour
{


	public GameObject number;

	public GameObject map;

	public GameObject ibeacon;

	public GameObject qr;

	public GameObject nfc;


	public GameObject button_number;

	public GameObject button_map;

	public GameObject button_ibeacon;

	public GameObject button_qr;

	public GameObject button_nfc;



	questdatabase questdb;

	// Use this for initialization
	void Start ()
	{
		if (GameObject.Find ("QuestDatabase") == null) {

			SceneManager.LoadScene ("questlist");
			return;
		}

		questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();

		if (QuestManager.Instance.CurrentQuest != null && QuestManager.Instance.CurrentQuest.Id != 0) {

			Quest	quest = QuestManager.Instance.CurrentQuest;
			Page	mappage = QuestManager.Instance.CurrentQuest.currentpage;
		
		
		
			if (mappage.type.Equals ("Navigation")) {


				bool defaultFound = false;



				if (mappage.getAttribute ("map") == "true") {
					if (!defaultFound) {
						defaultFound = true;
					}
					button_map.SetActive (true);

				} else {
					button_map.SetActive (false);

					number.SetActive (false);

				}




				if (ibeacon != null) {
					if (mappage.getAttribute ("ibeacon") == "true") {

						if (!defaultFound) {
							ibeacon.SetActive (true);
							defaultFound = true;
						}
						button_ibeacon.SetActive (true);


					} else {
						button_ibeacon.SetActive (false);

						ibeacon.SetActive (false);

					}
				} else {
					button_ibeacon.SetActive (false);


				}

			

				if (qr != null) {
					if (mappage.getAttribute ("qr") == "true") {
						if (!defaultFound) {
							
							qr.SetActive (true);
							defaultFound = true;
						}
						button_qr.SetActive (true);

					} else {
						button_qr.SetActive (false);

						qr.SetActive (false);

					}
				} else {

					button_qr.SetActive (false);


				}



				if (nfc != null) {
					if (Application.platform == RuntimePlatform.Android) {
						
						if (mappage.getAttribute ("nfc") == "true") {
							if (!defaultFound) {
								nfc.SetActive (true);
								defaultFound = true;
							}
							button_nfc.SetActive (true);
						} else {
							button_nfc.SetActive (false);

							nfc.SetActive (false);

						}
					} else {
						nfc.SetActive (false);
						button_nfc.SetActive (false);

					}
				} else {

					button_nfc.SetActive (false);

				}





				if (number != null) {
					if (mappage.getAttribute ("number") == "true") {
						if (!defaultFound) {
									
							number.SetActive (true);
							defaultFound = true;
						}
						button_number.SetActive (true);

					} else {
						button_number.SetActive (false);

						number.SetActive (false);

					}
				} else {

					button_number.SetActive (false);

				}



			


			}
		
		
		}
	}






	public void goTo (string s)
	{


		if (s == "number" && number != null) {

			number.SetActive (true);
			//map.SetActive (false);
			ibeacon.SetActive (false);
			qr.SetActive (false);
			nfc.SetActive (false);

		} else if (s == "map") {

			number.SetActive (false);
			//map.SetActive (true);
			ibeacon.SetActive (false);
			qr.SetActive (false);
			nfc.SetActive (false);

		} else if (s == "ibeacon" && ibeacon != null) {

			number.SetActive (false);
			//map.SetActive (false);
			ibeacon.SetActive (true);
			qr.SetActive (false);
			nfc.SetActive (false);

		} else if (s == "qr" && qr != null) {

			number.SetActive (false);
			//map.SetActive (false);
			ibeacon.SetActive (false);
			qr.SetActive (true);
			nfc.SetActive (false);

		} else if (s == "nfc" && nfc != null) {

			number.SetActive (false);
			//map.SetActive (false);
			ibeacon.SetActive (false);
			qr.SetActive (false);
			nfc.SetActive (true);

		} 







	}




}
