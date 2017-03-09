using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using System.Threading;

using ZXing;
using UnityEngine.SceneManagement;
using GQ.Client.Model;

public class page_tagscanner : MonoBehaviour {

	public questdatabase questdb;
	public Quest quest;
	public Page tagscanner;

	public Text text;
	public Image textbg;
	public Text ergebnis_text;
	public Image ergebnis_textbg;
	public string qrresult = "";
	public bool showresult = false;
	WebCamTexture camTexture;
	public Quaternion baseRotation;

	public RawImage camQRImage;

	private Thread qrThread;
	private Color32[] c;
	private sbyte[] d;
	private int W, H;
	private int x, y, z;
	Material cameraMat;
	//	public MeshRenderer plane;
	private string qrcontent;
	//	public MessageReceiver receiver;

	IEnumerator Start () {

		if ( GameObject.Find("QuestDatabase") == null ) {

			SceneManager.LoadScene("questlist");
			yield break;
		}

		questdb = GameObject.Find("QuestDatabase").GetComponent<questdatabase>();
		quest = GameObject.Find("QuestDatabase").GetComponent<questdatabase>().currentquest;
		tagscanner = GameObject.Find("QuestDatabase").GetComponent<questdatabase>().currentquest.currentpage;



		if ( tagscanner.onStart != null ) {

			tagscanner.onStart.Invoke();
		}

		if ( tagscanner.hasAttribute("taskdescription") ) {
			text.text = questdb.GetComponent<actions>().formatString(tagscanner.getAttribute("taskdescription"));
		}
		else {

			text.enabled = false;
			textbg.enabled = false;

		}



		if ( tagscanner.hasAttribute("showTagContent") ) {

			if ( tagscanner.getAttribute("showTagContent") == "true" ) {

				showresult = true;
			}
			else {

				showresult = false;

			}

		}
		else {
			showresult = false;
		}

		// init web cam;
		if ( Application.isWebPlayer ) {
			yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
		}

		var devices = WebCamTexture.devices;

		string debugdevices = "WEBCAMDEVICES: ";

		foreach ( WebCamDevice wcd in devices ) {

			debugdevices += wcd.name + "(" + wcd.isFrontFacing + "), ";


		}

		Debug.Log(debugdevices);

		var deviceName = devices[0].name;
		camTexture = new WebCamTexture(deviceName);
		camTexture.requestedHeight = 480;
		camTexture.requestedWidth = 640;
//		plane.material.mainTexture = camTexture;

		camTexture.Play();

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		camQRImage.transform.rotation *= Quaternion.AngleAxis(camTexture.videoRotationAngle, Vector3.back);
		float xScale = camTexture.videoVerticallyMirrored ? -1.0F : 1.0F;
		float yScale = ((float)camTexture.height / (float)camTexture.width) * (camTexture.videoVerticallyMirrored ? -1.0F : 1.0F);
		camQRImage.transform.localScale = new Vector3(1, yScale, 1.0F);

//		Debug.Log(
//			string.Format(
//				"ROTATION: ({ 0}, { 1}, { 2}), Angle: { 3}, Mirrored: { 4}, isPLaying: { 5}, xScale: { 6}, yScale: { 7}", 
//				camQRImage.transform.rotation.x, 
//				camQRImage.transform.rotation.y, 
//				camQRImage.transform.rotation.z,
//				camTexture.videoRotationAngle,
//				camTexture.videoVerticallyMirrored,
//				camTexture.isPlaying,
//				xScale,
//				yScale
//			)
//		);
		camQRImage.texture = camTexture;
		W = camTexture.width;
		H = camTexture.height;

		StartCoroutine(OnEnableCam());

		qrThread = new Thread(DecodeQR);
		qrThread.Start();
	}

	void Update () {
		if ( camTexture != null )
			c = camTexture.GetPixels32();

		if ( qrcontent != null && qrcontent != "" && qrcontent != "!XEMPTY_GEOQUEST_QRCODEX!28913890123891281283012" ) {
			checkResult(qrcontent);
		}
	}

	IEnumerator OnEnableCam () {
		if ( camTexture != null ) {
			//camTexture.Play ();



			if ( camTexture.didUpdateThisFrame && c != null ) {

//				W = camTexture.width;
//				H = camTexture.height;
			}
			else {
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();

				StartCoroutine(OnEnableCam());
			}
		}
	}

	void OnDisable () {
		Debug.Log("OnDisable()");
		if ( camTexture != null ) {
			camTexture.Pause();
		}
	}

	void OnDestroy () {
		Debug.Log("OnDestroy()");

		if ( qrThread != null ) {
			qrThread.Abort();
			Debug.Log("QR Decoder THREAD ABORTED");
		}

		if ( camTexture != null ) {

			camTexture.Stop();
		}
	}




	void DecodeQR () {  
		// create a reader with a custom luminance source

		var barcodeReader = new BarcodeReader {
			AutoRotate = false,
			TryHarder = false
		};

		while ( true ) {


			try {
				string result = ""; 

				// decode the current frame

				if ( c != null ) {

					result = barcodeReader.Decode(c, W, H).Text; //This line of code is generating unknown exceptions for some arcane reason
					Debug.Log("THREAD: DecodeQR() result given.");
				}        
				if ( result != null ) {           
					qrcontent = result;   

					print(result);
				}
				// Sleep a little bit and set the signal to get the next frame
				c = null;
				Thread.Sleep(200); 
			} catch {   
				continue;
			}

		}
	}


	//old


	void checkResult (string r) {

		if ( r != qrresult ) {
			qrresult = r;

			if ( r.Length > 0 ) {
				questdb.debug("QR CODE gescannt:" + r);


				tagscanner.result = r;

				if ( showresult ) {
					ergebnis_text.text = r;
					ergebnis_text.enabled = true;
					ergebnis_textbg.enabled = true;
				}


				bool didit = false;
				if ( tagscanner.contents_expectedcode != null && tagscanner.contents_expectedcode.Count > 0 ) {



					foreach ( QuestContent qc in tagscanner.contents_expectedcode ) {

						if ( qc.content == r ) {

							//// TODO: go on

							didit = true;

							text.enabled = false;
							textbg.enabled = false;





						}


					}



					if ( didit ) {


						StartCoroutine(onSuccess());


					}
					else {

						StartCoroutine(onFailure());

					}

				}

				StartCoroutine(onEnd());
			}

		}
	}





	IEnumerator onSuccess () {

		yield return new WaitForSeconds(0f);


		if ( tagscanner.onSuccess != null ) {

			tagscanner.state = "succeeded";
			tagscanner.onSuccess.Invoke();
		}  


	}

	IEnumerator onFailure () {

		yield return new WaitForSeconds(0f);

		if ( tagscanner.onFailure != null ) {
			tagscanner.state = "failed";

			tagscanner.onFailure.Invoke();
		}  


	}



	IEnumerator  onEnd () {


		yield return new WaitForSeconds(0.2f);


		if ( tagscanner.state != "failed" ) {
			tagscanner.state = "succeeded";

		}

		if ( tagscanner.onEnd != null ) {
			Debug.Log("onEnd");
			tagscanner.onEnd.Invoke();
		}
		else
		if ( !tagscanner.onSuccess.hasMissionAction() && !tagscanner.onFailure.hasMissionAction() ) {

			GameObject.Find("QuestDatabase").GetComponent<questdatabase>().endQuest();

		}
		else {


			if ( showresult ) {

				ergebnis_text.enabled = false;
				ergebnis_textbg.enabled = false;
			}


		}


	}
}

