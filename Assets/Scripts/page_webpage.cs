using UnityEngine;
using System.Collections;

public class page_webpage : MonoBehaviour {
	
	public questdatabase questdb;
	public actions actioncontroller;
	public Quest quest;
	public QuestPage webpage;



	public GameObject nextButtonObject;
	//Just let it compile on platforms beside of iOS and Android
	//If you are just targeting for iOS and Android, you can ignore this
	#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
	
	//1. First of all, we need a reference to hold an instance of UniWebView
	private UniWebView webView;
	
	private string _errorMessage;
	private GameObject _cube;
	private Vector3 _moveVector;

	[SerializeField]
	public float[] insets = new float[]{0,0,0,0};
#endif

	public void backButton ()
	{
		
		
		
		QuestPage show = questdb.currentquest.previouspages [questdb.currentquest.previouspages.Count - 1];
		questdb.currentquest.previouspages.Remove (questdb.currentquest.previouspages [questdb.currentquest.previouspages.Count - 1]);
		questdb.changePage (show.id);
		
		
		
	}
	public void nextButton ()
	{
		
		
		onEnd ();
		
		
		
	}
	// Use this for initialization
	void Start () {


		if (GameObject.Find ("QuestDatabase") != null) {
			questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
			actioncontroller = GameObject.Find ("QuestDatabase").GetComponent<actions> ();
			quest = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest;
			webpage = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest.currentpage;
		} else {
			Application.LoadLevel(0);

		}


		if (questdb.currentquest.previouspages.Count == 0) {

			Destroy(nextButtonObject);
		}


		if(webpage.onStart != null){
			
			webpage.onStart.Invoke();
		}
		
		
		#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8

	
		webView = GetComponent<UniWebView>();
		if (webView == null) {


			GameObject go = new GameObject("web_");
			webView = go.AddComponent<UniWebView>();
			webView.OnReceivedMessage += OnReceivedMessage;
			webView.OnLoadComplete += OnLoadComplete;
			webView.OnWebViewShouldClose += OnWebViewShouldClose;
			webView.OnEvalJavaScriptFinished += OnEvalJavaScriptFinished;
			webView.InsetsForScreenOreitation += InsetsForScreenOreitation;

		}
		


		if(webpage.getAttribute ("url") != null && webpage.getAttribute("url") != ""){

			Debug.Log("URL:"+webpage.getAttribute ("url"));
			webView.url = webpage.getAttribute ("url");
		webView.Load();
		
		_errorMessage = null;
		} else {

			onEnd();

		}
#else

		questdb.debug("WebView kann nur auf mobilen Geräten angezeigt werden. Rufe direkt den Beenden-Trigger der Seite auf.");
		onEnd();


#endif
		
	}

	
	public void deactivateWebView(){

		#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8

		webView.enabled = false;
#endif
	}
	
	public void activateWebView(){

		#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8

		webView.enabled = true;
#endif
	}

	
	public void onEnd(){

		#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8

		webView.enabled = false;
#endif

			webpage.state = "succeeded";
		
		
		if (webpage.onEnd != null) {
			
			webpage.onEnd.Invoke ();
		} else {
			
			GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().endQuest();
			
		}
		
		
	}
	
	
	#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8

	
	
	//5. When the webView complete loading the url sucessfully, you can show it.
	//   You can also set the autoShowWhenLoadComplete of UniWebView to show it automatically when it loads finished.
	void OnLoadComplete(UniWebView webView, bool success, string errorMessage) {
		if (success) {
			webView.Show();
		} else {
			Debug.Log("Something wrong in webview loading: " + errorMessage);
			_errorMessage = errorMessage;
		}
	}
	
	//6. The webview can talk to Unity by a url with scheme of "uniwebview". See the webpage for more
	//   Every time a url with this scheme clicked, OnReceivedMessage of webview event get raised.
	void OnReceivedMessage(UniWebView webView, UniWebViewMessage message) {
		Debug.Log("Received a message from native");
		Debug.Log(message.rawMessage);
		//7. You can get the information out from the url path and query in the UniWebViewMessage
		//For example, a url of "uniwebview://move?direction=up&distance=1" in the web page will 
		//be parsed to a UniWebViewMessage object with:
		//				message.scheme => "uniwebview"
		//              message.path => "move"
		//              message.args["direction"] => "up"
		//              message.args["distance"] => "1"
		// "uniwebview" scheme is sending message to Unity by default.
		// If you want to use your customized url schemes and make them sending message to UniWebView,
		// use webView.AddUrlScheme("your_scheme") and webView.RemoveUrlScheme("your_scheme")
//		if (string.Equals(message.path, "close")) {
//			//8. When you done your work with the webview, 
//			//you can hide it, destory it and do some clean work.
//			webView.Hide();
//			Destroy(webView);
//			webView.OnReceivedMessage -= OnReceivedMessage;
//			webView.OnLoadComplete -= OnLoadComplete;
//			webView.OnWebViewShouldClose -= OnWebViewShouldClose;
//			webView.OnEvalJavaScriptFinished -= OnEvalJavaScriptFinished;
//			webView.InsetsForScreenOreitation -= InsetsForScreenOreitation;
//			_webView = null;
//		}
	}
	
	//9. By using EvaluatingJavaScript method, you can talk to webview from Unity.
	//It can evel a javascript or run a js method in the web page.
	//(In the demo, it will be called when the cube hits the sphere)
	public void ShowAlertInWebview(float time, bool first) {
		_moveVector = Vector3.zero;
		if (first) {
			//Eval the js and wait for the OnEvalJavaScriptFinished event to be raised.
			//The sample(float time) is written in the js in webpage, in which we pop 
			//up an alert and return a demo string.
			//When the js excute finished, OnEvalJavaScriptFinished will be raised.
			webView.EvaluatingJavaScript("sample(" + time +")");
		}
	}
	
	//In this demo, we set the text to the return value from js.
	void OnEvalJavaScriptFinished(UniWebView webView, string result) {
		Debug.Log("js result: " + result);
	}
	
	//10. If the user close the webview by tap back button (Android) or toolbar Done button (iOS), 
	//    we should set your reference to null to release it. 
	//    Then we can return true here to tell the webview to dismiss.
	bool OnWebViewShouldClose(UniWebView webView) {
		if (webView == webView) {
			webView = null;
			return true;
		}
		return false;
	}
	
	// This method will be called when the screen orientation changed. Here we returned UniWebViewEdgeInsets(5,5,bottomInset,5)
	// for both situation. Although they seem to be the same, screenHeight was changed, leading a difference between the result.
	// eg. on iPhone 5, bottomInset is 284 (568 * 0.5) in portrait mode while it is 160 (320 * 0.5) in landscape.
	UniWebViewEdgeInsets InsetsForScreenOreitation(UniWebView webView, UniWebViewOrientation orientation) {

		int topInset = (int)(UniWebViewHelper.screenHeight * insets[0]);
		int bottomInset = (int)(UniWebViewHelper.screenHeight * insets[1]);

		int rightInset = (int)(UniWebViewHelper.screenWidth* insets[2]);
		int leftInset = (int)(UniWebViewHelper.screenWidth* insets[3]);

			return new UniWebViewEdgeInsets(topInset,leftInset,bottomInset,rightInset);
		
	}



	#endif
	

}
