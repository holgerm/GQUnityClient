using UnityEngine;
using System.Collections;
using System.IO;

public class page_videoplay : MonoBehaviour {
	
	
	private WWW www;
	
	
	public questdatabase questdb;
	public Quest quest;
	public QuestPage npctalk;
	public YoutubeVideo youtube;
	private bool videoplayed = false;
	// Use this for initialization
	void Start () {

				questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
				quest = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest;
				npctalk = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest.currentpage;

		string pre = "file:";


		#if !UNITY_WEBPLAYER

		Screen.orientation = ScreenOrientation.LandscapeLeft;

		
		
				if (npctalk.onStart != null) {
			
						npctalk.onStart.Invoke ();
				}



				string url = npctalk.getAttribute ("file");

				if (!url.StartsWith ("http:") && !url.StartsWith ("https:")) {



			if(Application.platform != RuntimePlatform.Android){

			url = url.Replace("@streamingassets@",Application.streamingAssetsPath+"/");



				url = 	"file:/"+url;

			} else {

				url = url.Replace("@streamingassets@","");

				
			}
			 

			//	url = "file://"+Application.streamingAssetsPath+"/1_Code_7_-_Trailer_(1080p).mp4";

				Debug.Log("video url:"+url);

				StartCoroutine(PlayStreamingVideo(url));





								
				
				} else {


			// YOUTUBE OR URL


			/*

			#if !UNITY_WEBPLAYER
			
			
			StartCoroutine (youtube.LoadVideo (url));
			
			#endif


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


		questdb.Debug("Video Playback can't be simulated in web-preview right now");

		StartCoroutine(onEnd());


#endif




		}



	private IEnumerator PlayStreamingVideo(string url)
	{
		//        Handheld.PlayFullScreenMovie(url, Color.black, FullScreenMovieControlMode.Full);
		yield return new WaitForSeconds(1.0f);
		Handheld.PlayFullScreenMovie(url, Color.black, FullScreenMovieControlMode.Full);
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		StartCoroutine(onEnd ());
	}

	void Update(){


		if (videoplayed) {


			StartCoroutine(onEnd());

				}

	}
	
		
	public void playMovie(string x){

		StartCoroutine(playMovieFullscreen (x));
		//onEnd();
		videoplayed = true;
		
	}


	public IEnumerator playMovieFullscreen (string x){

#if !UNITY_WEBPLAYER
		Handheld.PlayFullScreenMovie(x,Color.black,FullScreenMovieControlMode.Full);
#endif
		yield return 0;
	}




	public IEnumerator onEnd(){
		yield return new WaitForSeconds (0.1f);
		Screen.orientation = ScreenOrientation.Portrait;
		yield return new WaitForSeconds (0.1f);


		npctalk.state = "succeeded";
		//questdb.AllowAutoRotation (false);

		
		if (npctalk.onEnd != null) {
			Debug.Log ("onEnd");
			npctalk.onEnd.Invoke ();
		} else {
			
			GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().endQuest();
			
		}

		
	}
}
