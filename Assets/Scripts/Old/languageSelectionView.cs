using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using GQ.Client.Conf;

public class languageSelectionView : MonoBehaviour {





	string lang = "de";




	// Use this for initialization
	void Start () {
	
		if ( !Configuration.instance.languageChangableByUser ) {

			gameObject.SetActive(false);

		}
		else {




			foreach ( Language l in Configuration.instance.languagesAvailable ) {

				if ( l.bezeichnung.Equals(GameObject.Find("QuestDatabase").GetComponent<Dictionary>().language) ) {

			
					GetComponentInChildren<Image>().sprite = l.sprite;

				}

			}

		}
	}


	void Update () {





		if ( Configuration.instance.languageChangableByUser && lang != GameObject.Find("QuestDatabase").GetComponent<Dictionary>().language ) {
			
			lang = GameObject.Find("QuestDatabase").GetComponent<Dictionary>().language;
			
			foreach ( Language l in Configuration.instance.languagesAvailable ) {
				
				if ( l.bezeichnung.Equals(GameObject.Find("QuestDatabase").GetComponent<Dictionary>().language) ) {
					
					
					GetComponentInChildren<Image>().sprite = l.sprite;
					
				}
				
			}
			
		}




	}


	public void openLanguageChooser () {

		GameObject.Find("LanguageAnimator").GetComponent<Animator>().ResetTrigger("p_out");

		GameObject.Find("LanguageAnimator").GetComponent<Animator>().SetTrigger("in");

	}
	

}
