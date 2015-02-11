using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using CielaSpike.Unity.Barcode;
using System.Threading;

public class ExampleBarcodeController : MonoBehaviour
{
    WebCamTexture cameraTexture;

    Material cameraMat;
    GameObject plane;



    WebCamDecoder decoder;

    IBarcodeEncoder qrEncoder, pdf417Encoder;

    GUIContent qrImage = new GUIContent();
    GUIContent pdf417Image = new GUIContent();

    GUIContent resultString = new GUIContent();

    Vector2 scroll = Vector2.zero;
	




	void Update(){



		var result = decoder.Result;
		if (result.Success) {

		
			Debug.Log(result.Text);

		}


	}
    IEnumerator Start()
    {
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

  
}
