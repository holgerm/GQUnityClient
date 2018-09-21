using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Candlelight.UI;
using System.IO;
using UnityEngine.UI;
using GQ.Client.Err;

namespace GQ.UI {
	
    [RequireComponent(typeof(Text))]
	public class ShowTextFromProductResource : MonoBehaviour
	{
        public RecourceFiles recourceTextType;

		protected Text contentText;

		// Use this for initialization
		void Start ()
        {
			contentText = GetComponent<Text> ();

            string recourceTextFileName = recourceTextType.ToString().ToLower();


            TextAsset textAsset = Resources.Load<TextAsset> (recourceTextFileName);
			if (textAsset != null) {
				contentText.text = textAsset.text;
            } else {
                Log.SignalErrorToDeveloper("This product is missing the recource text " + recourceTextFileName);
            }
		
		}
		
	}

    public enum RecourceFiles {
        Imprint,
        Privacy,
        Feedback
    }
}
