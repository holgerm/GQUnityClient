using UnityEngine;
using System.Collections;
using System.Threading;
using ZXing.QrCode;
using ZXing;


public class qrCam : MonoBehaviour {
	
	private WebCamTexture camTexture;
	private Thread qrThread;

	private Color32[] c;
	private sbyte[] d;
	private int W, H, WxH;
	private int x, y, z;
	
	void OnEnable () {
		if(camTexture != null) {
			camTexture.Play();
			W = camTexture.width;
			H = camTexture.height;
			WxH = W * H;
		}
	}
	
	void OnDisable () {
		if(camTexture != null) {
			camTexture.Pause();
		}
	}
	
	void OnDestroy () {
		qrThread.Abort();
		camTexture.Stop();
	}
	
	void Start () {
		camTexture = new WebCamTexture();
		OnEnable();
		
		qrThread = new Thread(DecodeQR);
		qrThread.Start();
	}
	
	void Update () {
		c = camTexture.GetPixels32();
	}
	
	void DecodeQR()
	{  
		// create a reader with a custom luminance source
		
		var barcodeReader = new BarcodeReader {AutoRotate=false, TryHarder=false};
		
		while (true)
			
		{

			
			try
				
			{
				string result = "Cry if you see this."; 
				
				// decode the current frame
				
				if (c != null){
					
					print ("Start Decode!");
					result = barcodeReader.Decode(c, W, H).Text; //This line of code is generating unknown exceptions for some arcane reason
					print ("Got past decode!");
				}       
				if (result != null)
				{           
					print(result);
				}
				// Sleep a little bit and set the signal to get the next frame
				c = null;
				Thread.Sleep(200); 
			}
			catch 
			{   
				continue;
			}
			
		}
	}
}
