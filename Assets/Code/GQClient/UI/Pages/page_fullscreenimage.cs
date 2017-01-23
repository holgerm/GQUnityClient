using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using GQ.Util;
using UnityEngine.SceneManagement;
using GQ.Client.Model;

public class page_fullscreenimage : MonoBehaviour {


	public RawImage imagev;
	public RawImage imageh;
	public Button imagebuttonv;
	public Button imagebuttonh;
	private WWW www;
	public Quest quest;
	public QuestPage fullscreenimage;
	public actions questactions;
	public questdatabase questdb;


	// Use this for initialization
	void Start () {


		if ( GameObject.Find("QuestDatabase") == null ) {

			SceneManager.LoadScene("questlist");
		}
		else {
			questdb = GameObject.Find("QuestDatabase").GetComponent<questdatabase>();

			questactions = GameObject.Find("QuestDatabase").GetComponent<actions>();
			quest = GameObject.Find("QuestDatabase").GetComponent<questdatabase>().currentquest;
			fullscreenimage = GameObject.Find("QuestDatabase").GetComponent<questdatabase>().currentquest.currentpage;
			string pre = "file: /";

			if ( fullscreenimage.onStart != null ) {
			
				fullscreenimage.onStart.Invoke();
			}
		
		
			if ( Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ) {
			
				pre = "file:";
			}

			if ( Application.platform == RuntimePlatform.Android && questdb.currentquest.predeployed ) {

				pre = "";
			}





			if (
				fullscreenimage.getAttribute("image").StartsWith("http://") ||
				fullscreenimage.getAttribute("image").StartsWith("https://") ) {
			
				www = new WWW(fullscreenimage.getAttribute("image"));
			
				StartCoroutine(waitforImage());
			
			
			}
			else
			if ( fullscreenimage.getAttribute("image").StartsWith("@_") ) {


			


				foreach ( QuestRuntimeAsset qra in	questactions.photos ) {



					if ( qra.key == fullscreenimage.getAttribute("image") ) {


						Sprite s = Sprite.Create(qra.texture, new Rect(0, 0, qra.texture.width, qra.texture.height), new Vector2(0.5f, 0.5f));

						if ( s.texture.width < s.texture.height ) {
				
							imagev.texture = s.texture;
							imagev.enabled = true;
							imageh.enabled = false;
						}
						else {
							imageh.texture = s.texture;
							imageh.enabled = true;
							imagev.enabled = false;
				
						}
					}
				}





			}
			else {





				www = new WWW(pre + "" + fullscreenimage.getAttribute("image"));
				
				StartCoroutine(waitforImage());



//
//				foreach (SpriteConverter sc in questdb.convertedSprites) {
//				
//				
//				
//					if (sc.filename == fullscreenimage.getAttribute ("image")) {
//					
//						if (sc.isDone) {
//							if (sc.myTexture != null) {
//								if (sc.myTexture.width < sc.myTexture.height) {
//								
//									imagev.texture = sc.myTexture;
//									imagev.enabled = true;
//									imageh.enabled = false;
//
//
//
//									
//									float myX = (float)sc.myTexture.width;
//									
//									
//									float myY = (float)sc.myTexture.height;
//									
//									
//									float scaler = myY/1663f;
//									
//									myX = myX / scaler;
//									myY = 1663f;
//									
//									
//									imagev.GetComponent<RectTransform>().sizeDelta = new Vector2(myX , myY);
//
//
//
//								} else {
//									imageh.texture = sc.myTexture;
//									imageh.enabled = true;
//									imagev.enabled = false;
//								
//
//
//									
//									float myX = (float)sc.myTexture.width;
//									
//									
//									float myY = (float)sc.myTexture.height;
//									
//									
//									float scaler = myY/1663f;
//									
//									myX = myX / scaler;
//									myY = 1663f;
//									
//									
//									imagev.GetComponent<RectTransform>().sizeDelta = new Vector2(myX , myY);
//
//
//
//								}
//							
//							
//							
//							
//							
//							} else {
//							
//								Debug.Log ("Sprite was null");
//							}
//						} else {
//						
//							Debug.Log ("SpriteConverter was not done.");
//						
//						}
//					}
//				}
			}




			if ( fullscreenimage.getAttribute("duration") == "interactive" ) {
				imagebuttonv.interactable = true;
				imagebuttonh.interactable = true;

				//Debug.Log("interactive");
			}
			else {
				//Debug.Log(int.Parse(fullscreenimage.getAttribute ("duration")));
				StartCoroutine(duration((float)(int.Parse(fullscreenimage.getAttribute("duration")) / 1000)));
			}


		}
	}

	IEnumerator duration (float s) {

		//Debug.Log ("waiting " + s + " seconds");
		yield return new WaitForSeconds(s);


		onEnd();
	}

	IEnumerator waitforImage () {
		
		yield return www;
		
		if ( www.error == null ) {






			//Sprite s = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), new Vector2 (0.5f, 0.5f));


			imagev.texture = www.texture;
			imagev.enabled = true;
			imageh.enabled = false;
				
				
				
				
			float myX = (float)www.texture.width;
				
				
			float myY = (float)www.texture.height;
				
				
			float scaler = myY / 1837f;
				
			myX = myX / scaler;
			myY = 1837f;
				
				
			imagev.GetComponent<RectTransform>().sizeDelta = new Vector2(myX, myY);


			




		}
		else {
			Debug.Log(www.error);
		}
		
	}

	public void onEnd () {
		
		fullscreenimage.state = "succeeded";
		
		if ( fullscreenimage.onEnd != null ) {
			
			fullscreenimage.onEnd.Invoke();
		}
		else {
			
			GameObject.Find("QuestDatabase").GetComponent<questdatabase>().endQuest();

		}
		
		
	}

}
