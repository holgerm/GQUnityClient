using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System;
using QM.NFC;


public class NFCExample : MonoBehaviour
{

	public Text id;
	public Text payload;
	public Text techs;

	// Use this for initialization
	void Start ()
	{
		Screen.orientation = ScreenOrientation.Portrait;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void process (NFC_Info info)
	{
		id.text = info.Id;
		payload.text = info.Payload;
		techs.text = String.Join (",", info.Techs);
	}
}
