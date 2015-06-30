using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.IO;
public class page_imagecapture : MonoBehaviour {

	
	public questdatabase questdb;
	public actions actioncontroller;

	public Quest quest;
	public QuestPage imagecapture;
	
	public Text text;
	public Image textbg;

	
	
	
	
	
	
	WebCamTexture cameraTexture;
	
	Material cameraMat;
	GameObject plane;
	


	
	// Use this for initialization
	IEnumerator Start ()
	{
		
		
		
		questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
		actioncontroller = GameObject.Find ("QuestDatabase").GetComponent<actions> ();
		quest = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest;
		imagecapture = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest.currentpage;




		if (imagecapture.onStart != null) {
			
			imagecapture.onStart.Invoke ();
		}
		
		if (imagecapture.hasAttribute ("task") && imagecapture.getAttribute ("task").Length > 1) {
			text.text = imagecapture.getAttribute ("task");
		} else {
			
			text.enabled = false;
			textbg.enabled = false;
			
		}




		// get render target;
		plane = GameObject.Find ("Plane");
		cameraMat = plane.GetComponent<MeshRenderer> ().material;
		

		// init web cam;
		if (Application.platform == RuntimePlatform.OSXWebPlayer ||
			Application.platform == RuntimePlatform.WindowsWebPlayer) {
			yield return Application.RequestUserAuthorization (UserAuthorization.WebCam);
		}
		
		var devices = WebCamTexture.devices;
		var deviceName = devices[0].name;


		if (Application.platform == RuntimePlatform.Android) {
			cameraTexture = new WebCamTexture (deviceName, 1280	, 720);

			
		} else {
			cameraTexture = new WebCamTexture (deviceName, 1920, 1080);
		}



		cameraTexture.Play();
		

		cameraMat.mainTexture = cameraTexture;
		

	}





	IEnumerator takeSnapshotAndroid(WebCamTexture cameraTexture2){

		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();


		Texture2D snap = new Texture2D (cameraTexture2.width, cameraTexture2.height);
		snap.SetPixels (cameraTexture2.GetPixels ());
		snap.Apply ();
		
		cameraTexture2.Stop ();
		
		cameraMat.mainTexture = snap;
		
		QuestRuntimeAsset qra = new QuestRuntimeAsset ("@_" + imagecapture.getAttribute ("file"), snap);
		
		actioncontroller.photos.Add (qra);
		onEnd ();


	}
	public void TakeSnapshot()
	{


		Debug.Log ("starting photo");

		if (Application.platform == RuntimePlatform.Android) {
			var devices = WebCamTexture.devices;
			var deviceName = devices [0].name;
			WebCamTexture cameraTexture2 = new WebCamTexture (deviceName, 1920, 1080);
			cameraTexture.Stop();
			cameraTexture2.Play();

			StartCoroutine(takeSnapshotAndroid(cameraTexture2));


		} else {
		
			Texture2D snap = new Texture2D (cameraTexture.width, cameraTexture.height);
			snap.SetPixels (cameraTexture.GetPixels ());
			snap.Apply ();
	
			cameraTexture.Stop ();

			cameraMat.mainTexture = snap;
			QuestRuntimeAsset qra = new QuestRuntimeAsset ("@_" + imagecapture.getAttribute ("file"), snap);
			
			actioncontroller.photos.Add (qra);


			onEnd ();


		}
	
	





	




	}
	


	
	void onEnd ()
	{


	

		imagecapture.state = "succeeded";

		
		if (imagecapture.onEnd != null) {
			Debug.Log ("onEnd");
			imagecapture.onEnd.Invoke ();
		} else {
			
			GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().endQuest ();
			
		}
		
		
	}
}
