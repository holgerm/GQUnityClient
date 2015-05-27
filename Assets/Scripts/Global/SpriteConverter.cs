using UnityEngine;
using System.Collections;
using System.Threading;
using System.IO;

namespace GQ.Util
{
	[System.Serializable]
	public class SpriteConverter
	{




		public bool isDone;

		public string filename;

		public Sprite sprite;

		WWW myWWW;
		Thread waitForWWW;

		public SpriteConverter (string s)
		{
			isDone = false;

			filename = s;

		}


		public void startConversion(){
			string pre = "file: /";
			if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
				
				pre = "file:";
			}
			
			if (filename != null && filename != "") {



				if(File.Exists(filename)){
				Debug.Log("opening www: "+filename);
				myWWW = new WWW (pre + filename);
				
				waitForCompletion (myWWW);
				}
			} else {
				
				isDone = true;
			}


		}

		void waitForCompletion (WWW givenWWW)
		{

			Debug.Log ("trying to acces: " + givenWWW.url);
	
			if (givenWWW.url == null || givenWWW.url == "") {

				isDone = true;


			} else {
				while (!givenWWW.isDone) {

					if (givenWWW.error != null) {

						break;
					}
				}
				if (givenWWW.error == null) {
					if(givenWWW.texture != null){
					//Sprite Conversion

						Debug.Log("starting sprite conversion");
					sprite = Sprite.Create (givenWWW.texture, new Rect (0, 0, givenWWW.texture.width, givenWWW.texture.height), new Vector2 (0.5f, 0.5f));

					isDone = true;
					} else {

						isDone = true;
					}
				} else {

					Debug.Log (givenWWW.error);
				}
			}
			}

	}
}