using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using YoutubeExtractor;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

//BASIC USAGE
// the Handheld.PlayFullScreenMovie play mobile videos *only on mobile device, not works on editor
// YoutubeVideo.Instance.RequestVideo is everithing you need to get the video
// If you want to play the video on textures, just like a outdor(example) in game or a 360 degree video(using a sphere scene of Easy Movie Texture) you need a third part asset, Easy Movie Texture works perfectly with that plugin, if you have Easy Movie Texture take a look at YoutubeEasyMovieTexture script
// I think you can do that using a free plugin too, not tested yet but you can try to use vuforia to play video as texture.

public class GetVideo : MonoBehaviour {

	public string videoId1 = "";
	public string videoId2 = "";

	void OnGUI () {
		GUI.depth = 0;
		if ( GUI.Button(new Rect(0, 0, Screen.width, Screen.height / 2), "Load Video 1") ) {
			#if UNITY_IOS || UNITY_ANDROID
            Handheld.PlayFullScreenMovie(YoutubeVideo.Instance.RequestVideo(videoId1, 720)); //if the second parameter is 0 will use the YoutubeVideo.cs quality settings
			#endif
			Debug.Log("The video only plays on mobile device, if you receive one big url on console all it's ok");
		}
		if ( GUI.Button(new Rect(0, Screen.height / 2, Screen.width, Screen.height / 2), "Load Video 2") ) {
			#if UNITY_IOS || UNITY_ANDROID
			Handheld.PlayFullScreenMovie(YoutubeVideo.Instance.RequestVideo(videoId2, 720));
			#endif
			Debug.Log("The video only plays on mobile device, if you receive one big url on console all it's ok");
		}
	}
}
