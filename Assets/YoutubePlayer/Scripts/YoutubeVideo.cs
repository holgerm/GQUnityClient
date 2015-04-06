using UnityEngine;
using System.Collections;

public class YoutubeVideo : MonoBehaviour
{

	public static YoutubeVideo Instance;
	public bool drawBackground;
	public Texture2D backgroundImage;
	public string serverGetVideoFile = "http://www.quest-mill.com/clientyoutubehelper/getvideo.php";
	public page_videoplay controller;
	public MeshRenderer plane;

	void Start ()
	{
		Instance = this;
	}
	
	public IEnumerator LoadVideo (string vId)
	{


		//Dont change this url
		//If you change the video will not work

		Debug.Log (vId);
		if (!vId.Contains ("/")) {


			string url = serverGetVideoFile + "?videoid=" + vId + "&type=Download";
			Debug.Log (url);
			WWWForm form = new WWWForm ();
			form.AddField ("key", "youtubeDownloader");
			WWW www = new WWW (url, form);
			yield return www;
			string result = www.text;
			Debug.Log ("URL:" + result);



			if (Application.isMobilePlatform) {
				controller.playMovie (result);
			} else {
				WWW loadvideo = new WWW (result);
				StartCoroutine (videoloaded (loadvideo));
			}


		} else {
			if (Application.isMobilePlatform) {

				controller.playMovie (vId);
			} else {


				WWW loadvideo = new WWW (vId);
				StartCoroutine (videoloaded (loadvideo));

			}

		}




			
			
			
			





	}

	IEnumerator videoloaded (WWW videowww)
	{

		#if !UNITY_IPHONE && !UNITY_ANDROID

		yield return 0;









#else

		yield return 0;

		#endif


	}

	void OnGUI ()
	{
		GUI.depth = 1;
		if (drawBackground) {
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), backgroundImage);
		}
	}



}
