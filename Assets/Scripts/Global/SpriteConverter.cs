using UnityEngine;
using System.Collections;
using System.IO;


namespace GQ.Util
{
	[System.Serializable]
	public class SpriteConverter
	{




		public bool isDone;

		public string filename;

		public Sprite sprite;

		public WWW myWWW;


		public int width;
		public int height;

		public Texture2D myTexture;

	//	public Thread conversionThread;

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
				//Debug.Log("opening www: "+filename);
				myWWW = new WWW (pre + filename);
				}
			} else {
				
				isDone = true;
			}


		}





		public void convertSprite(){



			//Debug.Log ("starting sprite conversion");
			 sprite = Sprite.Create (myTexture, new Rect (0, 0, width, height), new Vector2 (0.5f, 0.5f));

			if(sprite == null){
				
				GameObject.Find("QuestDatabase").GetComponent<questdatabase>().spriteError = "Fehlerhafte Datei\nBitte lade diese Quest erneut.";
			} else {
				isDone = true;
				myWWW = null;
				myTexture = null;
			}
			
			
			
			
		}
		
		




	}
}