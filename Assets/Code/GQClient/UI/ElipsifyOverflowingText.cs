using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Err;

[RequireComponent(typeof(Text))]
public class ElipsifyOverflowingText : MonoBehaviour {

	public Text Text;
    public int maxLineNumbers = 1; // TODO: DOES NOT WORK WITH DOUBLE LINES!

	public string FullText {
		get;
		set;
	}

	void Reset () {
		Text foundText = gameObject.GetComponent<Text> ();
		if (foundText != null) {
			Text = foundText;
		}
		else {
			Log.SignalErrorToDeveloper ("Script " + GetType ().Name + " needs a Text GameObject to be set to its text variable.");
		}
	}

	// Use this for initialization
	void Start () {
		ElipsifyText ();
	}

	public void ElipsifyText ()
	{
		// init FullText here lazily because Start() is called too late for prefab initalizations
		if (FullText == null)
			FullText = Text.text;
		
		// start with the original full text:
		Text.text = FullText;


		Canvas.ForceUpdateCanvases ();

        float maxWidth = maxLineNumbers * Text.rectTransform.rect.width;

        float preferredWidth = LayoutUtility.GetPreferredWidth(Text.rectTransform);

        if (preferredWidth <= maxWidth) {
			// text just fits well:
			return;
		}
		// we have to elipsify the text:
		int reduceLastChars = 2;

		// maybe if the text ends with two long characters like "mm" we just need to replace these with the elipse ("...").
		do {
			Text.text = Text.text.Substring (0, Text.text.Length - reduceLastChars) + "...";
			reduceLastChars++;
            // already increase for next round in this loop.
            preferredWidth = LayoutUtility.GetPreferredWidth(Text.rectTransform);
        }
		while (preferredWidth > maxWidth);

        return;
	}
}
