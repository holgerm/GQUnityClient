using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using GQ.Client.Model;

public class page_videoplay : MonoBehaviour {
	
	
	public questdatabase questdb;
	public Quest quest;
	public Page page;

	public Texture2D bgImage;

	public const int RESOLUTION = 720;

	static string filepath;

	public const string YOUTUBE_URL_PREFIX = "https://www.youtube.com/watch?v=";

	private enum VideoKind {
		Local,
		HTTP,
		YouTube
	}

	private VideoKind URL2VideoKind (string url) {
		
		if ( url.StartsWith(YOUTUBE_URL_PREFIX) ) {
			return VideoKind.YouTube;
		}		
		if ( url.StartsWith("http:") || url.StartsWith("https:") ) {
			return VideoKind.HTTP;
		}
		else {
			return VideoKind.Local;
		}
	}

	public IEnumerator Start () {

		// try to let image and canvas be shown quickly
		yield return new WaitForEndOfFrame();

		questdb = GameObject.Find("QuestDatabase").GetComponent<questdatabase>();
		quest = GameObject.Find("QuestDatabase").GetComponent<questdatabase>().currentquest;
		page = GameObject.Find("QuestDatabase").GetComponent<questdatabase>().currentquest.currentpage;

		if ( page.onStart != null ) {
			page.onStart.Invoke();
		}
		
#if UNITY_WEBPLAYER 
		questdb.debug("Video Playback can't be simulated in web-preview right now");
		OnEnd();

#elif UNITY_STANDALONE_OSX
		OnEnd();

#else
		PlayVideo();

#endif
	}


	private void PlayVideo () {

		string url = page.getAttribute("file");

		switch ( URL2VideoKind(url) ) {
			case VideoKind.YouTube:
				StartCoroutine(PlayYoutubeVideo(url));
				break;
			case VideoKind.HTTP:
				StartCoroutine(PlayHTTPVideo(url));
				break;
			case VideoKind.Local:
				StartCoroutine(PlayLocalVideo(url));
				break;
			default:
				Debug.LogError("Can NOT play this unknown Video kind. Url: " + url);
				OnEnd();
				break;
		}
	}


	IEnumerator PlayYoutubeVideo (string url) {
		yield return new WaitForEndOfFrame();

		// Extract youtube id from url:
		string id = url.Substring(YOUTUBE_URL_PREFIX.Length);
		// cut off rest of url if there is more than the vid id, e.g. language etc.:
		int endIndex = id.IndexOf('&');
		if ( endIndex > 0 ) {
			id = id.Substring(0, endIndex);
		}

		// TODO Test for internet connection or wrap the next call by a timeouted thread:
		Debug.Log("BEFORE");
		string link = YoutubeVideo.Instance.RequestVideo(id, RESOLUTION);
		Debug.Log("AFTER");

		if ( bgImage != null ) {
			YoutubeVideo.Instance.backgroundImage = bgImage;
			YoutubeVideo.Instance.drawBackground = true;
		}
		else {
			YoutubeVideo.Instance.drawBackground = false;
		}

		if ( link == null ) {
			Debug.LogError("COULD NOT PLAY Video: " + id);
			OnEnd();
			yield break;
		}
		else {
			Screen.orientation = ScreenOrientation.LandscapeLeft;
			yield return new WaitForEndOfFrame();
			Handheld.PlayFullScreenMovie(link, Color.black, FullScreenMovieControlMode.Full);
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
		}

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		Screen.orientation = ScreenOrientation.Portrait;
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		OnEnd();
	}

	IEnumerator PlayHTTPVideo (string url) {
		Debug.Log("We will play an HTTP Video link: " + url);

		Screen.orientation = ScreenOrientation.LandscapeLeft;
		yield return new WaitForEndOfFrame();
		Handheld.PlayFullScreenMovie(url);
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Debug.Log("Finished the HTTP Video from url: " + url);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		Screen.orientation = ScreenOrientation.Portrait;
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		OnEnd();
	}


	IEnumerator PlayLocalVideo (string url) {
		Debug.Log("We will play a LOCAL Video from file: " + url);

		Screen.orientation = ScreenOrientation.LandscapeLeft;
		yield return new WaitForEndOfFrame();
		Handheld.PlayFullScreenMovie("file://" + url);
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Debug.Log("Finished the LOCAL Video from url: " + url);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		Screen.orientation = ScreenOrientation.Portrait;
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		OnEnd();
	}


	public void OnEnd () {
		Debug.Log("VIDEO: ON_END()");
		Screen.orientation = ScreenOrientation.Portrait;
		page.state = "succeeded";
		//questdb.AllowAutoRotation (false);


		if ( page.onEnd != null ) {
			Debug.Log("onEnd");
			page.onEnd.Invoke();
		}
		else {

			GameObject.Find("QuestDatabase").GetComponent<questdatabase>().endQuest();

		}
	}
}
