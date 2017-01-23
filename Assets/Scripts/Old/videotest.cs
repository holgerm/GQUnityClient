using UnityEngine;
using System.Collections;

public class videotest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		#if !UNITY_WEBPLAYER && !UNITY_STANDALONE_OSX

		//Debug.Log (Application.streamingAssetsPath + "/MovieSamples/1_Code_7_-_Trailer_(1080p).mp4");
		Handheld.PlayFullScreenMovie ("/MovieSamples/1_Code_7_-_Trailer_(1080p).mp4");

#endif                      
	

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
