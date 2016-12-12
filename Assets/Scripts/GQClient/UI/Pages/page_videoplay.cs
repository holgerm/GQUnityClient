using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class page_videoplay : MonoBehaviour {
	
	
	private WWW www;
	public questdatabase questdb;
	public Quest quest;
	public QuestPage npctalk;
	//	public YoutubeVideo youtube;
	private bool videoplayed = false;

	static string filepath;

	public const string YOUTUBE_URL_PREFIX = "https://www.youtube.com/watch?v=";



	IEnumerator Start () {

		questdb = GameObject.Find("QuestDatabase").GetComponent<questdatabase>();
		quest = GameObject.Find("QuestDatabase").GetComponent<questdatabase>().currentquest;
		npctalk = GameObject.Find("QuestDatabase").GetComponent<questdatabase>().currentquest.currentpage;

		if ( npctalk.onStart != null ) {
			
			npctalk.onStart.Invoke();
		}
		
		

		#if !UNITY_WEBPLAYER && !UNITY_STANDALONE_OSX

		Screen.orientation = ScreenOrientation.LandscapeLeft;

		
		

		string url = npctalk.getAttribute("file");
		Debug.Log("We want to play video url = " + url);

		if ( !url.StartsWith("http:") && !url.StartsWith("https:") ) {

			Debug.Log("Starting video url = " + url);

			if ( Application.platform == RuntimePlatform.Android && questdb.currentquest.predeployed ) {

				if ( url.StartsWith(YOUTUBE_URL_PREFIX) ) {
					PlayYoutubeVideo(url);
				}
				else {
					url = url.Replace(questdb.PATH_2_PREDEPLOYED_QUESTS, "predeployed/quests");
					Debug.Log("We will play an ordinary Video link: " + url);
					Handheld.PlayFullScreenMovie(url);
				}
			}
			else {
				if ( url.StartsWith(YOUTUBE_URL_PREFIX) ) {
					PlayYoutubeVideo(url);

				}
				else {
					Debug.Log("We will play an ordinary Video link: " + url);
					Handheld.PlayFullScreenMovie("file://" + url);
				}
			}

			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			videoplayed = true;

		}
		else {
			if ( url.StartsWith(YOUTUBE_URL_PREFIX) ) {
				PlayYoutubeVideo(url);
			}


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

	private IEnumerator PlayStreamingVideo (string url) {
		//        Handheld.PlayFullScreenMovie(url, Color.black, FullScreenMovieControlMode.Full);
		yield return new WaitForSeconds(1.0f);

		#if !UNITY_WEBPLAYER && !UNITY_STANDALONE_OSX

		Handheld.PlayFullScreenMovie(url);

#endif

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		StartCoroutine(onEnd());
	}

	private void PlayYoutubeVideo (string url) {
		// YouTube Videos
		string id = url.Substring(YOUTUBE_URL_PREFIX.Length);
		// cut off rest of url if there is more than the vid id, e.g. language etc.:
		int endIndex = id.IndexOf('&');
		if ( endIndex > 0 ) {
			id = id.Substring(0, endIndex);
		}
		Debug.Log("We will play a YouTube Video id = " + id);
		Handheld.PlayFullScreenMovie(YoutubeVideo.Instance.RequestVideo(id, 720));

		// TODO catch VideoNotAvailableException
	}



	void Update () {


		if ( videoplayed ) {


			StartCoroutine(onEnd());

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

	public IEnumerator onEnd () {
		yield return new WaitForSeconds(0.1f);
		Screen.orientation = ScreenOrientation.Portrait;
		yield return new WaitForSeconds(0.1f);


		npctalk.state = "succeeded";
		//questdb.AllowAutoRotation (false);

		
		if ( npctalk.onEnd != null ) {
			Debug.Log("onEnd");
			npctalk.onEnd.Invoke();
		}
		else {
			
			GameObject.Find("QuestDatabase").GetComponent<questdatabase>().endQuest();
			
		}

		
	}
}
