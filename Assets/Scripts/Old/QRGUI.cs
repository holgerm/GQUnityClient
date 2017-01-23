using UnityEngine;
using System.Collections;

public class QRGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
		GUI.Label(new Rect(10, 10, 500, 20), MessageReceiver.Instance().QRInfo);
		
		if (GUI.Button(new Rect(10, 70, 70, 30), "QR Start!")) {
			Debug.Log("Clicked the button with text");
			UZBarReaderViewController zBar = new UZBarReaderViewController();
			zBar.cameraDevice = kCameraDevice.ZBAR_CAMERA_DEVICE_REAR;
			zBar.symbolType = kScanSymbolType.ZBAR_I25;
			zBar.configOpt = kScanConfigOptions.ZBAR_CFG_ENABLE;
			zBar.configSymbolValue = 0;
			zBar.cameraFlashMode = kCameraFlashMode.ZBAR_CAMERA_FLASH_MODE_AUTO;
			zBar.showsZBarControls = true;
			UIBinding.ActivateUI (zBar.getZBarInfos());
		}
	}
}
