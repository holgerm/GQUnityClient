using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class page_fullscreenimage : MonoBehaviour
{


		public Image imagev;
	public Image imageh;

		public Button imagebuttonv;
	public Button imagebuttonh;

		private WWW www;
		public Quest quest;
		public QuestPage fullscreenimage;
		public actions questactions;

		// Use this for initialization
		void Start ()
		{
				questactions = GameObject.Find ("QuestDatabase").GetComponent<actions> ();
				quest = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest;
				fullscreenimage = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest.currentpage;
				string pre = "file: /";

				if (fullscreenimage.onStart != null) {
			
						fullscreenimage.onStart.Invoke ();
				}
		
		
				if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			
						pre = "file:";
				}





				if (fullscreenimage.getAttribute ("image").StartsWith ("@_")) {


			


		foreach(QuestRuntimeAsset qra in	questactions.photos){



				if(qra.key == fullscreenimage.getAttribute ("image")){


					Sprite s =  Sprite.Create (qra.texture, new Rect (0, 0, qra.texture.width, qra.texture.height), new Vector2 (0.5f, 0.5f));

			if(s.texture.width < s.texture.height){
				
				imagev.sprite = s;
				imagev.enabled = true;
				imageh.enabled = false;
			} else {
				imageh.sprite = s;
				imageh.enabled = true;
				imagev.enabled = false;
				
			}
				}
			}





				} else {


						string url = fullscreenimage.getAttribute ("image");
						if (!url.StartsWith ("http:") && !url.StartsWith ("https:")) {
								url = pre + "" + fullscreenimage.getAttribute ("image");
						}

						www = new WWW (url);
				
						StartCoroutine (waitforImage ());
				}




				if (fullscreenimage.getAttribute ("duration") == "interactive") {
						imagebuttonv.interactable = true;
			imagebuttonh.interactable = true;

						//Debug.Log("interactive");
				} else {
						//Debug.Log(int.Parse(fullscreenimage.getAttribute ("duration")));
						StartCoroutine (duration ((float)(int.Parse (fullscreenimage.getAttribute ("duration")) / 1000)));
				}



		}

		IEnumerator duration (float s)
		{

				//Debug.Log ("waiting " + s + " seconds");
				yield return new WaitForSeconds (s);


				onEnd ();
		}

		IEnumerator waitforImage ()
		{
		
				yield return www;
		
				if (www.error == null) {






						Sprite s =  Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), new Vector2 (0.5f, 0.5f));

			if(s.texture.width < s.texture.height){

				imagev.sprite = s;
				imagev.enabled = true;
				imageh.enabled = false;
			} else {
				imageh.sprite = s;
				imageh.enabled = true;
				imagev.enabled = false;

			}




				} else {
						Debug.Log (www.error);
				}
		
		}

		public void onEnd ()
		{
		
				fullscreenimage.state = "succeeded";
		
				if (fullscreenimage.onEnd != null) {
			
						fullscreenimage.onEnd.Invoke ();
				} else {
			
						GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().endQuest ();

				}
		
		
		}

}
