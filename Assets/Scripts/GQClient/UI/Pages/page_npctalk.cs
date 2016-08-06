using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using System.IO;
using System;
using GQ.Util;
using Candlelight.UI;
using System.Text.RegularExpressions;

namespace GQ.Client.UI.Pages {

	public class page_npctalk : PageController {

	
	
		private WWW www;
		public RawImage image;
		public Image image_hochkant;
		public Text text;
		public Button nextbutton;
		public Button backbutton;
		public Text buttontext;
		public int dialogitem_state = 0;
		public string texttoticker;
		public float tickertime;
		private float savedtickertime;
		public int indexoflink = 0;
		public string link;
		public List<Link> links;

		protected override void Start () { 

			base.Start();

			string pre = "file: /";

			if ( Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ) {

				pre = "file:";
			}

			if ( Application.platform == RuntimePlatform.Android && questdb.currentquest.predeployed ) {

				pre = "";
			}

			if ( page.getAttribute("tickerspeed").Length > 0 ) {

				tickertime = float.Parse(page.getAttribute("tickerspeed")) / 1000;

			}
			savedtickertime = tickertime;
			DateTime start = DateTime.Now;

			if ( page.getAttribute("image") != null && page.getAttribute("image") != "" ) {

				if (
					page.getAttribute("image").StartsWith("http://") ||
					page.getAttribute("image").StartsWith("https://") ) {

					www = new WWW(page.getAttribute("image"));

					StartCoroutine(waitforImage());

				}
				else
				if ( page.getAttribute("image").StartsWith("@_") ) {

					foreach ( QuestRuntimeAsset qra in questactions.photos ) {

						if ( qra.key == page.getAttribute("image") ) {

							image.texture = qra.texture;
							float myX = (float)qra.texture.width;
							float myY = (float)qra.texture.height;
							float scaler = myY / 604f;
							myX = myX / scaler;
							myY = 604f;

							image.GetComponent<RectTransform>().sizeDelta = new Vector2(myX, myY);

						}
					}
				}
				else {

					www = new WWW(pre + "" + page.getAttribute("image"));
					StartCoroutine(waitforImage());

				}

			}
			else {
				deactivateImage();
			}

			text.text = "";
			text.fontSize = FontSize;

			if ( page.hasAttribute("text") ) {

				string toadd = questdb.GetComponent<actions>().formatString(page.getAttribute("text"));

				text.text += convertStringForHypertext(toadd);
				nextbutton.interactable = true;
				buttontext.text = page.getAttribute("endbuttontext");


			}
			else {
				nextdialogitem();
			}

		}

		protected override void InitBackButton (bool show) {
			if ( !show ) {
				Destroy(backbutton.gameObject);
			}
		}

		protected override int FontSize {
			get {
				string resultString = Regex.Match(page.getAttribute("textsize"), @"\d+").Value;
				int size = int.Parse(resultString);
				return size * 3;
			}
		}


		void Update () {
			if ( texttoticker != null ) {



				if ( page.getAttribute("skipwordticker") == "true" && Input.GetMouseButtonDown(0) ) {


					tickertime = savedtickertime;
					text.text += texttoticker;
					texttoticker = "";


				}
				else {
			
			
					if ( tickertime > 0f ) {

						tickertime -= Time.deltaTime;
					}
					else
					if ( texttoticker.Length > 0 ) {



						tickertime = savedtickertime;
						char[] tickeringtext = texttoticker.ToCharArray();
						text.text += tickeringtext[0];


						if ( tickeringtext.Length > 0 ) {
							texttoticker = new string(tickeringtext, 1, tickeringtext.Length - 1);
			
						}



				
					}
					else {

						if ( dialogitem_state > 0 && page.contents_dialogitems[dialogitem_state - 1].getAttribute("blocking") == "true" ) {


							if ( questactions.npcaudio != null ) {
								if ( questactions.npcaudio.GetComponent<AudioSource>() != null && !questactions.npcaudio.GetComponent<AudioSource>().isPlaying ) {

									nextbutton.interactable = true;
								}

							}


						}
						else {
							nextbutton.interactable = true;

						}

						texttoticker = null;

					}

			
				}
			
			
			}
			else {


				//Debug.Log(dialogitem_state-1);

				if ( questactions.npcaudio != null ) {

					if ( (page.contents_dialogitems[dialogitem_state - 1].getAttribute("blocking") == "true" && !questactions.npcaudio.GetComponent<AudioSource>().isPlaying) ) {
						nextbutton.interactable = true;
					}
				}
				else {

					nextbutton.interactable = true;

				}

			}
		
		
		}

		/// <summary>
		/// Deactivates the image. Useful when there is no image given, hence the text can start at the top of the screen.
		/// </summary>
		void deactivateImage () {
			image_hochkant.transform.parent.gameObject.SetActive(false);
			LayoutElement scrollLayout = GameObject.Find("/Canvas/Panel/Scroll").GetComponent<LayoutElement>();
			scrollLayout.flexibleHeight = 7;
		}

		void nextdialogitem () {
//		Debug.Log ("nextdialogitem()");

			if ( page.contents_dialogitems.Count > 0 ) {

				if ( page.contents_dialogitems[dialogitem_state].getAttribute("sound") != "" ) {


					questdb.GetComponent<actions>().PlayNPCAudio(page.contents_dialogitems[dialogitem_state].getAttribute("sound"));


				}


				if ( page.getAttribute("mode") == "Wordticker" ) {

					if ( page.contents_dialogitems[dialogitem_state].getAttribute("speaker").Length > 0 ) {

						text.text += "<b>" + page.contents_dialogitems[dialogitem_state].getAttribute("speaker") + "</b>: ";
					}

					texttoticker = questdb.GetComponent<actions>().formatString(page.contents_dialogitems[dialogitem_state].content) + "\n";
					texttoticker = convertStringForHypertext(texttoticker);
					nextbutton.interactable = false;

				}
				else {




					if ( !questdb.GetComponent<palette>().darkBG ) {
						text.text = "<color=#5c5c5c>" + text.text + "</color>";
					}
					else {

						text.text = "<color=#989898>" + text.text + "</color>";

					}
					if ( page.contents_dialogitems[dialogitem_state].getAttribute("speaker").Length > 0 ) {
					
						text.text += "<b>" + page.contents_dialogitems[dialogitem_state].getAttribute("speaker") + "</b>: ";
					}

					string toadd = questdb.GetComponent<actions>().formatString(page.contents_dialogitems[dialogitem_state].content) + "\n";

					// evtl. auf Regexp ändern: https://regex101.com/r/cC4vF0/7
					text.text += convertStringForHypertext(toadd);

					questdb.debug("Dialog Item is Blocking? -> " + page.contents_dialogitems[dialogitem_state].getAttribute("blocking"));

					if ( page.contents_dialogitems[dialogitem_state].getAttribute("blocking") != "true" ) {
						nextbutton.interactable = true;
					}
				}
				dialogitem_state++;

//			Debug.Log ("scrolling?");

				if ( page.contents_dialogitems.Count == dialogitem_state ) {
					buttontext.text = page.getAttribute("endbuttontext");
			
				}
				else {
					buttontext.text = page.getAttribute("nextdialogbuttontext");
				}

			}
			else {

				buttontext.text = page.getAttribute("endbuttontext");

			}



			if ( dialogitem_state > 1 ) {
				Canvas.ForceUpdateCanvases();
		
				text.transform.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
				Canvas.ForceUpdateCanvases();
			}


		}

		string convertStringForHypertext (string toadd) {
			int i = 0;
			int l = 0;
			while ( toadd.IndexOf("<a href=") > -1 ) {
				int aStartTagStartIndex = toadd.IndexOf("<a href=", i);
				int aStartTagEndIndex = toadd.IndexOf(">", aStartTagStartIndex);
				int aEndTagStartIndex = toadd.IndexOf("</a>", aStartTagEndIndex);
				i = aEndTagStartIndex;
				string textBeforeLink = toadd.Substring(0, aStartTagStartIndex);
				string url = toadd.Substring(aStartTagStartIndex + "<a href=".Length + 1, aStartTagEndIndex - (aStartTagStartIndex + "<a href=".Length + 2));
				string linkText = toadd.Substring(aStartTagEndIndex + 1, aEndTagStartIndex - aStartTagEndIndex - 1);
				string textAfterLink = toadd.Substring(aEndTagStartIndex + 4, toadd.Length - aEndTagStartIndex - 4);
				Regex urlRegex = new Regex(@"^(?:https?:\/\/)?(([a-z\d-]+)\.)+([a-z\d]+)([\/\?]\S*)?$");
				if ( urlRegex.IsMatch(url) ) {
					links.Add(new Link("link" + l, url));
					if ( linkText.Length > 27 ) {
						linkText = linkText.Substring(0, 25) + "...";
					}
					toadd = textBeforeLink + "<a name=\"link" + l + "\"><quad class=\"link\">  " + linkText + "</a>" + textAfterLink;
					l++;
				}
				else {
					toadd = textBeforeLink + " " + textAfterLink;
				}
			}

			return toadd;
		}

		public void clickLink (HyperText ht, Candlelight.UI.HyperText.LinkInfo li) {



			foreach ( Link l in links ) {


				if ( l.name == li.Name ) {


					string url = l.url;

					if ( !url.StartsWith("http") ) {

						url = "http://" + url;

					}

					Debug.Log("Opening url: " + url);
					Application.OpenURL(url);

				}

			}





		}

		IEnumerator waitforImage () {

			DateTime startWWW = DateTime.Now;

			yield return www;

			if ( www.error == null ) {

				//DateTime start = DateTime.Now;

				//Sprite s = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), new Vector2 (0.5f, 0.5f));
		
//			Debug.Log ("Sprite creation took: " + DateTime.Now.Subtract (start).Milliseconds);
				//	Debug.Log ("All took: " + DateTime.Now.Subtract (startWWW).Milliseconds);

				//Debug.Log (www.texture.height + "," + www.texture.width);
				
				image.texture = www.texture;
				
			
				float myX = (float)www.texture.width;
			
			
				float myY = (float)www.texture.height;
			
			
				float scaler = myY / 604f;
			
				myX = myX / scaler;
				myY = 604f;
			
			
				image.GetComponent<RectTransform>().sizeDelta = new Vector2(myX, myY);
			
											
		
		
		
		
		
		
			}
			else {



				Debug.Log(www.error + " loading took: " + DateTime.Now.Subtract(startWWW).Milliseconds);

				image.enabled = false;
			}


			yield return 0;
		
		}

		public void nextButton () {


//		Debug.Log ("nextButton()");
			if ( page.contents_dialogitems.Count == dialogitem_state ) {

				onEnd();

			}
			else {

						
				nextdialogitem();

			}




		}

		public void backButton () {



			QuestPage show = questdb.currentquest.previouspages[questdb.currentquest.previouspages.Count - 1];
			questdb.currentquest.previouspages.Remove(questdb.currentquest.previouspages[questdb.currentquest.previouspages.Count - 1]);
			questdb.changePage(show.id);
		

		
		}

	}


	[System.Serializable]
	public class Link {

		// TODO move this class together with text and hypertext in UI namespace.


		public string name;
		public string url;

		public Link (string n, string u) {


			name = n;
			url = u;


		}

	}
}