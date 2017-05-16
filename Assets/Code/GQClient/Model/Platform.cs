using UnityEngine;
using System.Collections;

namespace GQ.Client.Model
{

	/// <summary>
	/// Utility functions that help to differentiate between the platforms (iOS, Android, Web). Might later also involve different App versions.
	/// </summary>
	public class Platform
	{

		// TODO TEST!
		public static bool CanPlay (Quest q)
		{
			// TODO checks for iOS, Android and Webplayer via precompiler directive

			// TODO check for Android that NFC is availabe if NFCRead page is used

			bool playable = true;
			foreach (Page qp in q.PageList) {


				if (qp.type != "StartAndExitScreen" &&
				    qp.type != "NPCTalk" &&
				    qp.type != "MultipleChoiceQuestion" &&
				    qp.type != "VideoPlay" &&
				    qp.type != "TagScanner" &&
				    qp.type != "ImageCapture" &&
				    qp.type != "AudioRecord" &&
				    qp.type != "TextQuestion" &&
				    qp.type != "ImageWithText" &&
				    qp.type != "Menu" &&
				    qp.type != "MapOSM" &&
				    qp.type != "MetaData" &&
				    qp.type != "Custom" &&
				    qp.type != "Navigation" &&
				    qp.type != "WebPage" &&
				    qp.type != "ReadNFC") {



					Debug.Log ("Can't play because it includes mission of type " + qp.type);
					playable = false;
				}



			}
			return playable;

		}

	}
}
