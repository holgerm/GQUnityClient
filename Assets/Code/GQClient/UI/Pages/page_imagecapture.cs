using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.IO;
using System;
using System.Globalization;
using GQ.Client.Model;
using UnityEngine.SceneManagement;
using GQ.Client.Util;


public class page_imagecapture : MonoBehaviour
{

	
	public questdatabase questdb;
	public actions actioncontroller;

	public Quest quest;
	public Page imagecapture;
	
	public Text text;
	public Image textbg;

	public WebCamTexture cameraTexture;
	bool camIsRotated;
	public RawImage camRawImage;

	// Use this for initialization
	IEnumerator Start ()
	{
		GameObject questdbGO = GameObject.Find ("QuestDatabase");
		if (questdbGO != null) {
			questdb = questdbGO.GetComponent<questdatabase> ();
			actioncontroller = questdbGO.GetComponent<actions> ();
			quest = questdbGO.GetComponent<questdatabase> ().currentquest;
			imagecapture = questdbGO.GetComponent<questdatabase> ().currentquest.currentpage;
		} else {
			SceneManager.LoadScene ("questlist");
			yield break;
		}

		if (imagecapture.onStart != null) {
			
			imagecapture.onStart.Invoke ();
		}
		
		if (imagecapture.hasAttribute ("task") && imagecapture.getAttribute ("task").Length > 1) {
			text.text = questdb.GetComponent<actions> ().formatString (imagecapture.getAttribute ("task"));
		} else {
			
			text.enabled = false;
			textbg.enabled = false;
		}

		// init web cam;
		if (Application.platform == RuntimePlatform.OSXWebPlayer ||
		    Application.platform == RuntimePlatform.WindowsWebPlayer ||
		    Application.isWebPlayer) {
			yield return Application.RequestUserAuthorization (UserAuthorization.WebCam);
		}
		
		string deviceName = null;
		foreach (WebCamDevice wcd in WebCamTexture.devices) {
			if (!wcd.isFrontFacing) {
				deviceName = wcd.name;
				break;
			}
		}

		cameraTexture = new WebCamTexture (deviceName);

		cameraTexture.requestedHeight = 2000;
		cameraTexture.requestedWidth = 3000;

		cameraTexture.Play ();

		// wait for web cam to be ready which is guaranteed after first image update:
		while (!cameraTexture.didUpdateThisFrame)
			yield return null;

		// rotate if needed:
		camRawImage.transform.rotation *= Quaternion.AngleAxis (cameraTexture.videoRotationAngle, Vector3.back);

		camIsRotated = Math.Abs (cameraTexture.videoRotationAngle) == 90 || Math.Abs (cameraTexture.videoRotationAngle) == 270;
		float camHeight = (camIsRotated ? cameraTexture.width : cameraTexture.height);
		float camWidth = (camIsRotated ? cameraTexture.height : cameraTexture.width);

		float panelHeight = camRawImage.rectTransform.rect.height;
		float panelWidth = camRawImage.rectTransform.rect.width;

		float heightScale = panelHeight / camHeight;
		float widthScale = panelWidth / camWidth;
		float fitScale = Math.Min (heightScale, widthScale);

		float goalHeight = cameraTexture.height * fitScale;
		float goalWidth = cameraTexture.width * fitScale;

		heightScale = goalHeight / panelHeight;
		widthScale = goalWidth / panelWidth;

		float mirrorAdjustment = cameraTexture.videoVerticallyMirrored ? -1F : 1F;
		// TODO adjust mirror also correct if cam is not rotated:
		camRawImage.transform.localScale = new Vector3 (widthScale, heightScale * mirrorAdjustment, 1F);

		camRawImage.texture = cameraTexture;
	}


	public void TakeSnapshot ()
	{

		Texture2D photo;

		// we add 360 degrees to avoid any negative values:
		int rotatedClockwiseQuarters = 360 - cameraTexture.videoRotationAngle;

		switch (Input.deviceOrientation) {
		case DeviceOrientation.LandscapeLeft:
			rotatedClockwiseQuarters += 90;
			break;
		case DeviceOrientation.PortraitUpsideDown:
			rotatedClockwiseQuarters += 180;
			break;
		case DeviceOrientation.LandscapeRight:
			rotatedClockwiseQuarters += 270;
			break;
		case DeviceOrientation.Portrait:
		case DeviceOrientation.FaceUp:
		case DeviceOrientation.FaceDown:
		default:
			break;
		}

		rotatedClockwiseQuarters /= 90;  // going from degrees to quarters
		rotatedClockwiseQuarters %= 4; // reducing to 0, 1 ,2 or 3 quarters

		Color[] pixels = cameraTexture.GetPixels ();

		switch (rotatedClockwiseQuarters) {
		case 1:
			photo = new Texture2D (cameraTexture.height, cameraTexture.width);
			photo.SetPixels (pixels.Rotate90 (cameraTexture.height, cameraTexture.width));
			break;
		case 2:
			photo = new Texture2D (cameraTexture.width, cameraTexture.height);
			photo.SetPixels (pixels.Rotate180 (cameraTexture.width, cameraTexture.height));
			break;
		case 3:
			photo = new Texture2D (cameraTexture.height, cameraTexture.width);
			photo.SetPixels (pixels.Rotate270 (cameraTexture.height, cameraTexture.width));
			break;
		case 0:
		default:
			photo = new Texture2D (cameraTexture.width, cameraTexture.height);
			photo.SetPixels (pixels);
			break;
		}
		photo.Apply ();

		cameraTexture.Stop ();

		QuestRuntimeAsset qra = new QuestRuntimeAsset ("@_" + imagecapture.getAttribute ("file"), photo);
		actioncontroller.addPhoto (qra);

		SaveTextureToCamera (photo);

		onEnd ();
	}

	void SaveTextureToCamera (Texture2D texture)
	{
		DateTime now = DateTime.Now;
		string filename = now.ToString ("yyyy_MM_dd_HH_mm_ss_fff", CultureInfo.InvariantCulture);
		if (Application.platform == RuntimePlatform.Android) {
			GetComponent<AndroidCamera> ().SaveImageToGallery (texture, filename);
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			GetComponent<IOSCamera> ().SaveTextureToCameraRoll (texture);
		}
	}

	void onEnd ()
	{

		imagecapture.state = "succeeded";

		if (imagecapture.onEnd != null) {
			imagecapture.onEnd.Invoke ();
		} else {
			
			questdb.endQuest ();
		}
	}


}
