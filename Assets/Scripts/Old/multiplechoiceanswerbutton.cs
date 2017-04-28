using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class multiplechoiceanswerbutton : MonoBehaviour
{


	public Text text;


	public string label;


	public bool correct = false;



	public page_multiplechoicequestion controller;


	void Start ()
	{
		GameObject pageController = GameObject.Find ("PageController");
		if (pageController != null) {
			controller = pageController.GetComponent<page_multiplechoicequestion> ();
		}

	}


	public void setText (string l)
	{
		text.text = GameObject.Find ("QuestDatabase").GetComponent<actions> ().formatString (l);

	}






	public void press ()
	{

		controller.multiplechoicequestion.result = text.text;


		if (correct) {

			controller.onSuccess ();
		} else {
			controller.onFailure ();

		}

		controller.onEnd ();

	}

}
