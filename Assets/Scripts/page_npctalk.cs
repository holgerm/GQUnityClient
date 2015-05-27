using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System;
using GQ.Util;

public class page_npctalk : MonoBehaviour
{

	
	
	private WWW www;
	public questdatabase questdb;
	public Quest quest;
	public QuestPage npctalk;
	public actions 	questactions;
	public Image image;
	public Image image_hochkant;
	public Text text;
	public Button nextbutton;
	public Button backbutton;
	public Text buttontext;
	public int dialogitem_state = 0;
	public string texttoticker;
	public float tickertime;
	private float savedtickertime;

	void Start ()
	{ 
		if (GameObject.Find ("QuestDatabase") == null) {
			
			Application.LoadLevel (0);
			
		} else {


			questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
			quest = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest;
			npctalk = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest.currentpage;
			questactions = GameObject.Find ("QuestDatabase").GetComponent<actions> ();

			string pre = "file: /";

			if (npctalk.onStart != null) {
			
				npctalk.onStart.Invoke ();
			}



			if (questdb.currentquest.previouspages.Count == 0 || questdb.currentquest.previouspages [questdb.currentquest.previouspages.Count - 1] == null) {
				Destroy (backbutton.gameObject);
			}



			if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {

				pre = "file:";
			}


			if (npctalk.getAttribute ("tickerspeed").Length > 0) {

				tickertime = float.Parse (npctalk.getAttribute ("tickerspeed")) / 1000;

			}
			savedtickertime = tickertime;
			DateTime start = DateTime.Now;

			if (npctalk.getAttribute ("image") != "") {
				if (npctalk.getAttribute ("image").StartsWith ("@_")) {
				



					foreach (QuestRuntimeAsset qra in questactions.photos) {
					
						//Debug.Log("KEY:"+qra.key);
					
						if (qra.key == npctalk.getAttribute ("image")) {


					
						
							Sprite s = Sprite.Create (qra.texture, new Rect (0, 0, qra.texture.width, qra.texture.height), new Vector2 (0.5f, 0.5f));
						

							
							
						Debug.Log ("npctalk.image: (h, w) " + qra.texture.height + "," + qra.texture.width);
						if (qra.texture.height > qra.texture.width) {

								image_hochkant.sprite = s;
								image_hochkant.enabled = true;
								image.enabled = false;


							} else {

								image.sprite = s;
								image.enabled = true;


							}

						}
					}
					Debug.Log ("donewithforeach");
				
				
				
				
				} else {
		

					
					foreach(SpriteConverter sc in questdb.convertedSprites){



						if(sc.filename == npctalk.getAttribute ("image")){

							if (sc.sprite.texture.height > sc.sprite.texture.width) {
								
								image_hochkant.sprite = sc.sprite;
								image_hochkant.enabled = true;
								image.enabled = false;
								
								
							} else {
								
								image.sprite = sc.sprite;
								image.enabled = true;
								
								
							}
							



						}

					}



//					string url = npctalk.getAttribute ("image");
//					if (!url.StartsWith ("http:") && !url.StartsWith ("https:")) {
//						url = pre + "" + npctalk.getAttribute ("image");
//					}
//			
//					Debug.Log ("myfile:" + url + " (" + npctalk.getAttribute ("image") + ")");
//
//
//					if (url.StartsWith ("http:") || url.StartsWith ("https:")) {
//						Debug.Log ("webimage");
//
//						www = new WWW (url);
//						StartCoroutine (waitforImage ());
//
//
//					} else if (File.Exists (npctalk.getAttribute ("image"))) {
//
//						Debug.Log ("File Exists");
//						www = new WWW (url);
//						StartCoroutine (waitforImage ());
//					}

				}

			}

//		Debug.Log ("after npc talk image");
			text.text = "";



		
				
				
			Debug.Log ("Sprite display took: " + DateTime.Now.Subtract(start).Milliseconds);

			string resultString = Regex.Match (npctalk.getAttribute ("textsize"), @"\d+").Value;
			int size = int.Parse (resultString);
			text.fontSize = size * 3;




			if (npctalk.hasAttribute ("text")) {
				text.text += npctalk.getAttribute ("text");
				nextbutton.interactable = true;
				buttontext.text = npctalk.getAttribute ("endbuttontext");


			} else {
				nextdialogitem ();
			}




		}

	}

	void Update ()
	{
		Debug.Log ("npctalk.Update() entered");
		
//		Debug.Log (text.transform.parent.GetComponent<ScrollRect> ().verticalScrollbar.value);
		
		if (texttoticker != null) {



			if (npctalk.getAttribute ("skipwordticker") == "true" && Input.GetMouseButtonDown (0)) {


				tickertime = savedtickertime;
				text.text += texttoticker;
				texttoticker = "";


			} else {
			
			
				if (tickertime > 0f) {

					tickertime -= Time.deltaTime;
				} else if (texttoticker.Length > 0) {



					tickertime = savedtickertime;
					char[] tickeringtext = texttoticker.ToCharArray ();
					text.text += tickeringtext [0];


					if (tickeringtext.Length > 0) {
						texttoticker = new string (tickeringtext, 1, tickeringtext.Length - 1);
			
					}



				
				} else {

					if (dialogitem_state > 0 && npctalk.contents_dialogitems [dialogitem_state - 1].getAttribute ("blocking") == "true") {


						if (questactions.npcaudio != null) {
							if (questactions.npcaudio.GetComponent<AudioSource> () != null && !questactions.npcaudio.GetComponent<AudioSource> ().isPlaying) {

								nextbutton.interactable = true;
							}

						}


					} else {
						nextbutton.interactable = true;

					}

					texttoticker = null;

				}

			
			}
			
			
		} else {


			//Debug.Log(dialogitem_state-1);

			if (questactions.npcaudio != null) {

				if ((npctalk.contents_dialogitems [dialogitem_state - 1].getAttribute ("blocking") == "true" && !questactions.npcaudio.GetComponent<AudioSource> ().isPlaying)) {
					nextbutton.interactable = true;
				}
			} else {

				nextbutton.interactable = true;

			}

		}
		
		
	}

	void nextdialogitem ()
	{
//		Debug.Log ("nextdialogitem()");

		if (npctalk.contents_dialogitems.Count > 0) {

			if (npctalk.contents_dialogitems [dialogitem_state].getAttribute ("sound") != "") {


				questdb.GetComponent<actions> ().PlayNPCAudio (npctalk.contents_dialogitems [dialogitem_state].getAttribute ("sound"));


			}


			if (npctalk.getAttribute ("mode") == "Wordticker") {

				if (npctalk.contents_dialogitems [dialogitem_state].getAttribute ("speaker").Length > 0) {

					text.text += "<b>" + npctalk.contents_dialogitems [dialogitem_state].getAttribute ("speaker") + "</b>: ";
				}

				texttoticker = questdb.GetComponent<actions> ().formatString (npctalk.contents_dialogitems [dialogitem_state].content) + "\n";
				nextbutton.interactable = false;

			} else {




				if (!questdb.GetComponent<palette> ().darkBG) {
					text.text = "<color=#5c5c5c>" + text.text + "</color>";
				} else {

					text.text = "<color=#989898>" + text.text + "</color>";

				}
				if (npctalk.contents_dialogitems [dialogitem_state].getAttribute ("speaker").Length > 0) {
					
					text.text += "<b>" + npctalk.contents_dialogitems [dialogitem_state].getAttribute ("speaker") + "</b>: ";
				}
				text.text += questdb.GetComponent<actions> ().formatString (npctalk.contents_dialogitems [dialogitem_state].content) + "\n";
								


				questdb.debug ("Dialog Item is Blocking? -> " + npctalk.contents_dialogitems [dialogitem_state].getAttribute ("blocking"));

				if (npctalk.contents_dialogitems [dialogitem_state].getAttribute ("blocking") != "true") {
					nextbutton.interactable = true;
				}
			}
			dialogitem_state++;

			Debug.Log ("scrolling?");

			if (npctalk.contents_dialogitems.Count == dialogitem_state) {
				buttontext.text = npctalk.getAttribute ("endbuttontext");
			
			} else {
				buttontext.text = npctalk.getAttribute ("nextdialogbuttontext");
			}

		} else {

			buttontext.text = npctalk.getAttribute ("endbuttontext");

		}



		if (dialogitem_state > 1) {
			Canvas.ForceUpdateCanvases ();
		
			text.transform.parent.GetComponent<ScrollRect> ().verticalNormalizedPosition = 0f;
			Canvas.ForceUpdateCanvases ();
		}


	}
	
	IEnumerator waitforImage ()
	{

		DateTime startWWW = DateTime.Now;

		yield return www;

		if (www.error == null) {

			DateTime start = DateTime.Now;

			Sprite s = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), new Vector2 (0.5f, 0.5f));
		
			Debug.Log ("Sprite creation took: " + DateTime.Now.Subtract(start).Milliseconds);
			Debug.Log ("All took: " + DateTime.Now.Subtract(startWWW).Milliseconds);

			Debug.Log (www.texture.height + "," + www.texture.width);
			if (www.texture.height > www.texture.width) {
				
				image_hochkant.sprite = s;
				image_hochkant.enabled = true;
				image.enabled = false;
				
				
			} else {
				
				image.sprite = s;
				image.enabled = true;
				
				
			}
		
		
		
		
		
		
		
		} else {



			Debug.Log (www.error);

			image.enabled = false;
		}


		yield return 0;
		
	}

	public void nextButton ()
	{


//		Debug.Log ("nextButton()");
		if (npctalk.contents_dialogitems.Count == dialogitem_state) {

			onEnd ();

		} else {

						
			nextdialogitem ();

		}




	}

	public void backButton ()
	{



		QuestPage show = questdb.currentquest.previouspages [questdb.currentquest.previouspages.Count - 1];
		questdb.currentquest.previouspages.Remove (questdb.currentquest.previouspages [questdb.currentquest.previouspages.Count - 1]);
		questdb.changePage (show.id);
		

		
	}

	public void onEnd ()
	{

		npctalk.state = "succeeded";

		if (npctalk.onEnd != null && npctalk.onEnd.actions != null && npctalk.onEnd.actions.Count > 0) {

			npctalk.onEnd.Invoke ();

		} else {
			//Debug.Log ("ending");
			GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().endQuest ();

		}


	}

}
