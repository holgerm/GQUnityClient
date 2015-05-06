//using UnityEngine;
//using System.Collections;
//
//public class GetVideo : MonoBehaviour {
//
//	public string videoId1 = "";
//	public string videoId2 = "";
//
//	//Create one YoutuveVideo Object
//	public YoutubeVideo youtube;
//
//
//	void Start ()
//	{
//		//Instance the YoutubeVideo Object
//		//youtube = new YoutubeVideo();
//	}
//	
//	void OnGUI()
//	{
//		GUI.depth = 0;
//		if(GUI.Button(new Rect(0,0,Screen.width,Screen.height/2),"Load Video 1"))
//		{
//			//Call video load using LoadVideo under StartCoroutine with video id parameter
//			StartCoroutine(youtube.LoadVideo(videoId1));
//		}
//		if(GUI.Button(new Rect(0,Screen.height/2,Screen.width,Screen.height/2),"Load Video 2"))
//		{
//			//Call video load using LoadVideo under StartCoroutine with video id parameter
//			StartCoroutine(youtube.LoadVideo(videoId2));
//		}
//	}
//}
