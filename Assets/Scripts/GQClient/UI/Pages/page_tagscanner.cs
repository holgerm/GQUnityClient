using UnityEngine;
using UnityEngine.UI;

using System.Collections;

//using CielaSpike.Unity.Barcode;
using System.Threading;

using ZXing;

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



	WebCamTexture camTexture;
	WebCamTexture camTexture2;

	private Thread qrThread;
	
	private Color32[] c;
	private sbyte[] d;
	private int W, H, WxH;
	private int x, y, z;
	
	Material cameraMat;
	public MeshRenderer plane;
	
	
	private string qrcontent;
	WebCamDecoder decoder;


	public MessageReceiver receiver;

	void Update () {


		if ( Application.platform == RuntimePlatform.IPhonePlayer ) {
			qrcontent = receiver.QRInfo;

			Debug.Log("QR RESULT:" + receiver.QRInfo);

		}
		else {

			if ( camTexture != null ) {
				c = camTexture.GetPixels32();
				W = camTexture.width;
				H = camTexture.height;
				
			}

		}
		if ( qrcontent != null && qrcontent != "" && qrcontent != "!XEMPTY_GEOQUEST_QRCODEX!28913890123891281283012" ) {

			checkResult(qrcontent);
		}
	}


	// Use this for initialization
	IEnumerator Start () {

		if ( GameObject.Find("QuestDatabase") == null ) {

			Application.LoadLevel(0);

		}
		else {

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


			// get render target;
			//plane = GameObject.Find("Plane");
			//cameraMat = plane.GetComponent<MeshRenderer>().material;
		
			// get a reference to web cam decoder component;
			//decoder = GetComponent<WebCamDecoder>();
		
		


			if ( Application.platform != RuntimePlatform.IPhonePlayer ) {
				// init web cam;
				if ( Application.isWebPlayer ) {
					yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
				}
		
				var devices = WebCamTexture.devices;

				string debugdevices = "WEBCAMDEVICES: ";

				foreach ( WebCamDevice wcd in devices ) {

					debugdevices += wcd.name + ", ";


				}

				Debug.Log(debugdevices);

				var deviceName = devices[0].name;
				camTexture = new WebCamTexture(deviceName);
				camTexture.requestedHeight = Screen.height; // 480;
				camTexture.requestedWidth = Screen.width; //640;
				plane.material.mainTexture = camTexture;

				camTexture.Play();

				// old tagscanner
				//yield return StartCoroutine(decoder.StartDecoding(camTexture));
	

				//image.renderer.material.mainTexture = cameraTexture;
		
				StartCoroutine(OnEnableCam());

				qrThread = new Thread(DecodeQR);
				qrThread.Start();
		
		
				// adjust texture orientation;
				// plane.transform.rotation = plane.transform.rotation * 
				//   Quaternion.AngleAxis(cameraTexture.videoRotationAngle, Vector3.up);
	



			}
			else {

				UZBarReaderViewController zBar = new UZBarReaderViewController();
				zBar.cameraDevice = kCameraDevice.ZBAR_CAMERA_DEVICE_REAR;
				zBar.symbolType = kScanSymbolType.ZBAR_I25;
				zBar.configSymbolValue = 0;
				zBar.cameraFlashMode = kCameraFlashMode.ZBAR_CAMERA_FLASH_MODE_AUTO;
				zBar.showsZBarControls = true;
				UIBinding.ActivateUI(zBar.getZBarInfos());

				//onEnd();


			}
		}



	}



	IEnumerator OnEnableCam () {
		if ( camTexture != null ) {
			//camTexture.Play ();



			if ( camTexture.didUpdateThisFrame && c != null ) {

				W = camTexture.width;
				H = camTexture.height;
				WxH = W * H;
				
				
				//Debug.Log ("WebcamTexture: " + W + ":" + H + "=" + WxH);


				if ( c != null ) {
					//	Debug.Log ("Webcam actual pixels: " + c.Length);

				}
			}
			else {
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();

				StartCoroutine(OnEnableCam());

			}



		}
	}

	void OnDisable () {
		if ( camTexture != null ) {
			camTexture.Pause();
		}
	}

	void OnDestroy () {


		if ( qrThread != null ) {
			qrThread.Abort();
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
