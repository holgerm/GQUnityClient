using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class translateSprite : MonoBehaviour
{


	GameObject questDBGO;

	public Sprite de;
	public Sprite en;
	public Sprite fr;
	public Sprite es;


	public string currentLanguage;

	void Start ()
	{
		questDBGO = GameObject.Find ("QuestDatabase");
		translate ();
	}

	public void translate ()
	{

		if (GameObject.Find ("QuestDatabase").GetComponent<Dictionary> ().language == "de") {

			if (GetComponent<SpriteRenderer> () != null) {
				GetComponent<SpriteRenderer> ().sprite = de;
			}
			if (GetComponent<Image> () != null) {
				GetComponent<Image> ().sprite = de;
			}

		} else if (GameObject.Find ("QuestDatabase").GetComponent<Dictionary> ().language == "en") {
			
			if (GetComponent<SpriteRenderer> () != null) {
				GetComponent<SpriteRenderer> ().sprite = en;
			}
			if (GetComponent<Image> () != null) {
				GetComponent<Image> ().sprite = en;
			}
		} else if (GameObject.Find ("QuestDatabase").GetComponent<Dictionary> ().language == "fr") {
			
			if (GetComponent<SpriteRenderer> () != null) {
				GetComponent<SpriteRenderer> ().sprite = fr;
			}
			if (GetComponent<Image> () != null) {
				GetComponent<Image> ().sprite = fr;
			}
		} else if (GameObject.Find ("QuestDatabase").GetComponent<Dictionary> ().language == "es") {
			
			if (GetComponent<SpriteRenderer> () != null) {
				GetComponent<SpriteRenderer> ().sprite = es;
			}
			if (GetComponent<Image> () != null) {
				GetComponent<Image> ().sprite = es;
			}
		} 

		currentLanguage = GameObject.Find ("QuestDatabase").GetComponent<Dictionary> ().language;

	}


	void Update ()
	{
		if (questDBGO == null) {
			questDBGO = GameObject.Find ("QuestDatabase");
			if (questDBGO == null)
				return;
		}
 
		if (currentLanguage != questDBGO.GetComponent<Dictionary> ().language) {
			
			translate ();
			
		}
		
		
	}





}
