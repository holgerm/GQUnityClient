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

		showresult = false;
		if ( tagscanner.hasAttribute("showTagContent") ) {

			if ( tagscanner.getAttribute("showTagContent") == "true" ) {

				showresult = true;
			}
		}

		// init web cam;
		if ( Application.isWebPlayer ) {
			yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
		}

		string deviceName = null;
		foreach ( WebCamDevice wcd in WebCamTexture.devices ) {
			if ( !wcd.isFrontFacing ) {
				deviceName = wcd.name;
				break;
			}
		}

		camTexture = new WebCamTexture(deviceName);
		camTexture.requestedHeight = 480;
		camTexture.requestedWidth = 640;

		camTexture.Play();

		// wait for web cam to be ready:
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		// correct shown texture according to webcam details:
		camQRImage.transform.rotation *= Quaternion.AngleAxis(camTexture.videoRotationAngle, Vector3.back);
		float yScale = ((float)camTexture.height / (float)camTexture.width) * (camTexture.videoVerticallyMirrored ? -1F : 1F);
		camQRImage.transform.localScale = new Vector3(1F, yScale, 1F);

		camQRImage.texture = camTexture;
		W = camTexture.width;
		H = camTexture.height;

		qrThread = new Thread(DecodeQR);
		qrThread.Start();
	}

	void Update () {
		if ( camTexture != null && camTexture.didUpdateThisFrame ) {
			c = camTexture.GetPixels32();
		}

		if ( qrcontent != null && qrcontent != "" && qrcontent != "!XEMPTY_GEOQUEST_QRCODEX!28913890123891281283012" ) {
			checkResult(qrcontent);
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
			decoderRunning = false;
		}

		if ( camTexture != null ) {

			camTexture.Stop();
		}
	}


	private bool decoderRunning = false;

	void DecodeQR () {  
		// create a reader with a custom luminance source

		var barcodeReader = new BarcodeReader {
			AutoRotate = false,
			TryHarder = false
		};
				
		decoderRunning = true;

		while ( decoderRunning ) {


			try {
				string result = ""; 

				Debug.Log("decode thread running");

				// decode the current frame

				if ( c != null ) {

					result = barcodeReader.Decode(c, W, H).Text; 
					Debug.Log("THREAD: DecodeQR() ##1 result given: >" + result + "<");
				}        
				if ( result != null ) {           
					qrcontent = result;   

					Debug.Log("THREAD: DecodeQR() result given: >" + result + "<");
				}
				// Sleep a little bit and set the signal to get the next frame
				c = null;
				Thread.Sleep(200); 
			} catch {   
				continue;
			}

		}

		Debug.Log("THREAD: DecodeQR() ENDED");
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


				if ( tagscanner.contents_expectedcode != null && tagscanner.contents_expectedcode.Count > 0 ) {

					bool foundCorrectResult = false;

					foreach ( QuestContent qc in tagscanner.contents_expectedcode ) {

						if ( qc.content == r ) {

							foundCorrectResult = true;

							text.enabled = false;
							textbg.enabled = false;

							break;
						}
					}

					if ( foundCorrectResult ) {

						onSuccess();
					}
					else {

						onFailure();
					}
				}
				StartCoroutine(onEnd());
			}
		}
	}


	void onSuccess () {

		if ( tagscanner.onSuccess != null ) {

			tagscanner.state = "succeeded";
			tagscanner.onSuccess.Invoke();
		}  


	}


	void onFailure () {

		if ( tagscanner.onFailure != null ) {
		
			tagscanner.state = "failed";
			tagscanner.onFailure.Invoke();
		}  
	}



	IEnumerator  onEnd () {

		yield return new WaitForSeconds(0.2f);

		if ( !GQML.RESULT_FAILED.Equals(tagscanner.state) ) {

			tagscanner.state = GQML.RESULT_SUCCEEDED;
		}

		if ( tagscanner.onEnd != null ) {
			
			tagscanner.onEnd.Invoke();
		}
		else
		if ( !tagscanner.onSuccess.hasMissionAction() && !tagscanner.onFailure.hasMissionAction() ) {

			questdb.endQuest();
			// TODO looks like an ERROR
			Debug.LogWarning("A QR Scan page did neither have onSucceed nor onFail change page actions, so we end the quest. Shouldn't we simply perform onEnd()?");
		}
		else {

			if ( showresult ) {

				ergebnis_text.enabled = false;
				ergebnis_textbg.enabled = false;
			}
		}
	}
}

