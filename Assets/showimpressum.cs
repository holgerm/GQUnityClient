using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class showimpressum : MonoBehaviour {





	public Image impressum_bg;
	public Text impressum_text;
	public Button impressum_closebutton;
	public Text impressum_closebuttontext;





	public void Start(){
		if (GameObject.Find ("ImpressumCanvas") != gameObject) {
			Destroy (gameObject);		
		} else {
			DontDestroyOnLoad (gameObject);
			Debug.Log (Application.persistentDataPath);
		}

	}

	public void toggleImpressum(){



		if (impressum_bg.enabled) {

			impressum_bg.enabled = false;
			impressum_text.enabled = false;
			impressum_closebutton.enabled = false;
			impressum_closebutton.GetComponent<Image>().enabled = false;

			impressum_closebuttontext.enabled = false;

				} else {

			impressum_bg.enabled = true;
			impressum_text.enabled = true;
			impressum_closebutton.enabled = true;
			impressum_closebuttontext.enabled = true;
			impressum_closebutton.GetComponent<Image>().enabled = true;

				}


	}




	public void closeImpressum(){
		impressum_bg.enabled = false;
		impressum_text.enabled = false;
		impressum_closebutton.enabled = false;
		impressum_closebutton.GetComponent<Image>().enabled = false;
		
		impressum_closebuttontext.enabled = false;


	}

}
