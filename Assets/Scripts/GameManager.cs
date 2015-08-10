using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	
	private MessageReceiver receiver;

	// Use this for initialization
	void Start () {
		receiver = MessageReceiver.Instance();
	}
	
	// Update is called once per frame
	void Update () {
		if (receiver.QRInfo.Equals("scene1")) {
			Application.LoadLevel(1);
		}
		else if (receiver.QRInfo.Equals("scene2")) {
			Application.LoadLevel(2);
		}
	}
}
