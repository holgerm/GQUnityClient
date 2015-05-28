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

		public WWW myWWW;

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









	}
}