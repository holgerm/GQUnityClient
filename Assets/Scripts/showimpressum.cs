using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class showimpressum : MonoBehaviour {





	public Image impressum_bg;
	public Text impressum_text;
	public Button impressum_closebutton;
	public Text impressum_closebuttontext;
	public Image impressum_reloadbutton;
	public Image impressum_scrollpanel;
	public Image impressum_geoquestbutton;


	public void Start(){
		if (GameObject.Find ("ImpressumCanvas") != gameObject) {
			Destroy (gameObject);		
		} else {
			DontDestroyOnLoad (gameObject);
			Debug.Log (Application.persistentDataPath);

			impressum_text.text = Configuration.instance.impressum.Replace("\\n","\n");


		}

	}

	public void toggleImpressum(){


		GetComponent<Animator> ().SetTrigger ("toggle");

	}




	public void closeImpressum(){
		GetComponent<Animator> ().SetTrigger ("close");


	}

}
