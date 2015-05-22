using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class page_videoplay : MonoBehaviour
{
	
	
	private WWW www;
	public questdatabase questdb;
	public Quest quest;
	public QuestPage npctalk;
//	public YoutubeVideo youtube;
	private bool videoplayed = false;

	static string filepath;


	IEnumerator BeginDownload ()
	{

		#if !UNITY_WEBPLAYER

		filepath = Application.persistentDataPath + "/test.mp4";
		WWW www = new WWW ("https://quest-mill.com/tests/Geburtshaus.mp4");
		while (!www.isDone) {
			yield return null;
		}

		System.IO.File.WriteAllBytes (filepath, www.bytes);
		FileInfo finfo = new FileInfo (filepath);
		Debug.Log ("filepath = " + filepath);
		Debug.Log ("file length is " + finfo.Length);
		Debug.Log ("www had read bytes: " + www.bytes.Length);
		Debug.Log ("WWW Error :" + www.error);
		if (www.responseHeaders.Count > 0) {
			foreach (KeyValuePair<string, string> entry in www.responseHeaders) {
				Debug.Log ("Response Header: " + entry.Value + "=" + entry.Key);
			}
		}

		Handheld.PlayFullScreenMovie ("file://" + filepath);

#else

		yield return null;

		questdb.debug("Video can't be previewed in Web-Editor");
		StartCoroutine(onEnd());


#endif
	}


	IEnumerator Start ()
	{

		questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
		quest = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest;
		npctalk = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest.currentpage;

		if (npctalk.onStart != null) {
			
			npctalk.onStart.Invoke ();
		}
		
		

		#if !UNITY_WEBPLAYER

		Screen.orientation = ScreenOrientation.LandscapeLeft;

		
		

		string url = npctalk.getAttribute ("file");
		Debug.Log ("We want to play video url = " + url);

		if (!url.StartsWith ("http:") && !url.StartsWith ("https:")) {

			Debug.Log ("Starting video url = " + url);
			Handheld.PlayFullScreenMovie ("file://" + url);

			yield return new WaitForEndOfFrame ();
			yield return new WaitForEndOfFrame ();
			videoplayed = true;

		} else {


			// YOUTUBE OR URL


			/*


			
			StartCoroutine (youtube.LoadVideo (url));
			


			*/


		}


		/*

		if (npctalk.hasAttribute ("url")) {


			Debug.Log("has url: "+npctalk.getAttribute ("url"));


				
				string[] splitted = npctalk.getAttribute ("url").ToString ().Split ('=');
				
				url = splitted [splitted.Length - 1];
				
				
				StartCoroutine(youtube.LoadVideo(url));

			
				}
*/

	
				



		
#else

		yield return null;

		questdb.debug("Video Playback can't be simulated in web-preview right now");

		StartCoroutine(onEnd());


#endif




	}

	private IEnumerator PlayStreamingVideo (string url)
	{
		//        Handheld.PlayFullScreenMovie(url, Color.black, FullScreenMovieControlMode.Full);
		yield return new WaitForSeconds (1.0f);

		#if !UNITY_WEBPLAYER

		Handheld.PlayFullScreenMovie (url);

#endif

		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();
		StartCoroutine (onEnd ());
	}

	void Update ()
	{


		if (videoplayed) {


			StartCoroutine (onEnd ());

		}

	}
		
//	public void playMovie (string x)
//	{
//
//		StartCoroutine (playMovieFullscreen (x));
//		//onEnd();
//		videoplayed = true;
//		
//	}
//
//	public IEnumerator playMovieFullscreen (string x)
//	{
//
//#if !UNITY_WEBPLAYER
//		Handheld.PlayFullScreenMovie (x, Color.black, FullScreenMovieControlMode.Full);
//#endif
//		yield return 0;
//	}

	public IEnumerator onEnd ()
	{
		yield return new WaitForSeconds (0.1f);
		Screen.orientation = ScreenOrientation.Portrait;
		yield return new WaitForSeconds (0.1f);


		npctalk.state = "succeeded";
		//questdb.AllowAutoRotation (false);

		
		if (npctalk.onEnd != null) {
			Debug.Log ("onEnd");
			npctalk.onEnd.Invoke ();
		} else {
			
			GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().endQuest ();
			
		}

		
	}
}
