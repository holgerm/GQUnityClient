using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class languageButton : MonoBehaviour
{



	public string bezeichnung;

	public Text name;
	public Image spr;
	// Use this for initialization
	void Start ()
	{
	
	}



	public void setLanguage ()
	{

		if (GameObject.Find ("QuestDatabase") != null) {

			GameObject.Find ("QuestDatabase").GetComponent<Dictionary> ().setLanguage (bezeichnung);
			GameObject.Find ("LanguageAnimator").GetComponent<Animator> ().SetTrigger ("p_out");

		}


	}
	

}
