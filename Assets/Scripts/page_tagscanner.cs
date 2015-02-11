using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using CielaSpike.Unity.Barcode;
using System.Threading;

public class page_tagscanner : MonoBehaviour {
	
	public questdatabase questdb;
	public Quest quest;
	public QuestPage tagscanner;

	public Text text;
	public Image textbg;
	public Text ergebnis_text;
	public Image ergebnis_textbg;


	public string qrresult = "";
	public bool showresult = false;



	WebCamTexture cameraTexture;
	
	Material cameraMat;
	GameObject plane;
	
	
	
	WebCamDecoder decoder;


	
	
	void Update(){
		
		if (qrresult == "") {
						var result = decoder.Result;
			if(result.BarcodeType != 0){
			Debug.Log(result.BarcodeType);
			}
			if (result.Success && result.BarcodeType == BarcodeType.QrCode) {
			
			
								checkResult (result.Text);

						}
				}
		
	}


	// Use this for initialization
	IEnumerator Start()
	{



		questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
		quest = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest;
		tagscanner = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest.currentpage;

		
		if(tagscanner.onStart != null){
			
			tagscanner.onStart.Invoke();
		}
		
		if (tagscanner.hasAttribute ("taskdescription")) {
			text.text = tagscanner.getAttribute ("taskdescription");
		} else {
			
			text.enabled = false;
			textbg.enabled = false;
			
		}



		if(tagscanner.hasAttribute("showTagContent")){

			if(tagscanner.getAttribute("showTagContent") == "true"){

				showresult = true;
			} else {

				showresult = false;

			}

		} else {
			showresult = false;
		}


		// get render target;
		plane = GameObject.Find("Plane");
		cameraMat = plane.GetComponent<MeshRenderer>().material;
		
		// get a reference to web cam decoder component;
		decoder = GetComponent<WebCamDecoder>();
		
		
		// init web cam;
		if (Application.platform == RuntimePlatform.OSXWebPlayer ||
		    Application.platform == RuntimePlatform.WindowsWebPlayer)
		{
			yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
		}
		
		var devices = WebCamTexture.devices;
		var deviceName = devices[0].name;
		cameraTexture = new WebCamTexture(deviceName, 1920, 1080);
		cameraTexture.Play();
		
		// start decoding;
		yield return StartCoroutine(decoder.StartDecoding(cameraTexture));
		
		cameraMat.mainTexture = cameraTexture;
		
		//image.renderer.material.mainTexture = cameraTexture;
		
		
		
		
		// adjust texture orientation;
		// plane.transform.rotation = plane.transform.rotation * 
		//   Quaternion.AngleAxis(cameraTexture.videoRotationAngle, Vector3.up);
	


	}




	void checkResult(string r){

		if (r != qrresult) {
						qrresult = r;

						if (r.Length > 0 && r != "0000") {
								questdb.debug ("QR CODE gescannt:" + r);


								tagscanner.result = r;

								if (showresult) {
										ergebnis_text.text = r;
										ergebnis_text.enabled = true;
										ergebnis_textbg.enabled = true;
								}


								bool didit = false;
								if (tagscanner.contents_expectedcode != null && tagscanner.contents_expectedcode.Count > 0) {



										foreach (QuestContent qc in tagscanner.contents_expectedcode) {

												if (qc.content == r) {

														//// TODO: go on

														didit = true;

														text.enabled = false;
														textbg.enabled = false;

											

														

												}


										}



										if (didit) {

			
												StartCoroutine (onSuccess ());


										} else {

												StartCoroutine (onFailure ());

										}
					
								}

								StartCoroutine (onEnd ());
						}
	
				}
	}





	IEnumerator onSuccess(){
		
		yield return new WaitForSeconds (1f);

		
		if (tagscanner.onSuccess != null) {

			tagscanner.state = "succeeded";
			tagscanner.onSuccess.Invoke ();
		} 
		
		
	}
	IEnumerator onFailure(){
		
		yield return new WaitForSeconds (1f);
		
		if (tagscanner.onFailure != null) {
			tagscanner.state = "failed";

			tagscanner.onFailure.Invoke ();
		} 
		
		
	}



	IEnumerator  onEnd(){
		
		
		yield return new WaitForSeconds (1.5f);


		if (tagscanner.state != "failed") {
			tagscanner.state = "succeeded";

		}
		
		if (tagscanner.onEnd != null) {
						Debug.Log ("onEnd");
						tagscanner.onEnd.Invoke ();
				} else if (!tagscanner.onSuccess.hasMissionAction () && !tagscanner.onFailure.hasMissionAction ()) {

						GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().endQuest ();
			
				} else {


			if(showresult){

				ergebnis_text.enabled = false;
				ergebnis_textbg.enabled = false;
			}


				}
		
		
	}
}
