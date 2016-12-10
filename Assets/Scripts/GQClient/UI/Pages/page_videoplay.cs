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



				url = url.Replace(questdb.PATH_2_PREDEPLOYED_QUESTS, "predeployed/quests");

				 

				Handheld.PlayFullScreenMovie(url);

			}
			else {
				Handheld.PlayFullScreenMovie("file://" + url);
			}

			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			videoplayed = true;

		}
		else {


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
