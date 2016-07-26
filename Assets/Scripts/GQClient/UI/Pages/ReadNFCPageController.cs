using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using System.IO;
using System;
using GQ.Util;
using Candlelight.UI;
using System.Text.RegularExpressions;
using QM.NFC;

namespace GQ.Client.UI.Pages {

	public class ReadNFCPageController : PageController {

		private WWW www;
		public RawImage image;
		public Image image_hochkant;
		public Text text;
		public string saveToVar;
		public Button nextbutton;
		public Text nextButtontext;
		public Button backbutton;
		public int dialogitem_state = 0;
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
			}

			// init variable name where the nfc payload will be stored:
			saveToVar = page.getAttribute("saveToVar");

			// disable nextButton until NFC Chip is read:
			nextbutton.interactable = false;
			nextButtontext.text = ">";
		}

		protected override void InitBackButton (bool show) {
			if ( !show ) {
				Destroy(backbutton.gameObject);
			}
		}

		void Update () {

			// TODO Read from NFC Chip etc.
		
		}

		/// <summary>
		/// Deactivates the image. Useful when there is no image given, hence the text can start at the top of the screen.
		/// </summary>
		void deactivateImage () {
			image_hochkant.transform.parent.gameObject.SetActive(false);
			LayoutElement scrollLayout = GameObject.Find("/Canvas/Panel/Scroll").GetComponent<LayoutElement>();
			scrollLayout.flexibleHeight = 7;
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



				Debug.Log(www.error);

				image.enabled = false;
			}


			yield return 0;
		
		}

		public void backButton () {
			QuestPage show = questdb.currentquest.previouspages[questdb.currentquest.previouspages.Count - 1];
			questdb.currentquest.previouspages.Remove(questdb.currentquest.previouspages[questdb.currentquest.previouspages.Count - 1]);
			questdb.changePage(show.id);
		}

		public void nextButton () {
			onEnd();
		}

		public void onNFCRead (NFC_Info nfcInfo) {
			QuestVariable payloadVar = new QuestVariable(saveToVar, nfcInfo.Payload);
			questactions.setVariable(saveToVar, payloadVar);

			// TODO: Replace by argument that the develop can specify in NFC Reader UI Component. (hm)
			// TODO AND give as argument in GQEditor
			text.text = "NFC Chip wurde erfolgreich ausgelesen.";

			nextbutton.interactable = true;
			nextButtontext.text = "OK";
		}

	}
}

