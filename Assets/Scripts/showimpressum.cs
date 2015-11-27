using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Linq;
using System;

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

			StartCoroutine(loadImpressum());

			impressum_text.text = Configuration.instance.impressum.Replace("\\n","\n");


		}

	}


	public string RemoveWhitespace(this string input)
	{
		return new string(input.ToCharArray()
		                  .Where(c => !Char.IsWhiteSpace(c))
		                  .ToArray());
	}


	IEnumerator loadImpressum(){
		
		WWW www = new WWW ("http://qeevee.org:9091/"+Configuration.instance.portalID+"/imprint");
		yield return www;
		
		
		if (www.error != null && www.error != "") {
			
			Debug.Log("Couldn't load imprint: "+www.error);

			
		} else {
			

				
			string imprint = Configuration.instance.impressum;
				
			if(www.text != null && RemoveWhitespace(www.text) != ""){

				imprint = www.text;
			}

			imprint = GameObject.Find("QuestDatabase").GetComponent<actions>().formatString(imprint);


			GameObject.Find("QuestDatabase").GetComponent<actions>().localizeStringToDictionary(imprint);

			imprint = 	GameObject.Find("QuestDatabase").GetComponent<actions>().localizeString(imprint);

			
			impressum_text.text = imprint;

		}
		
		
		
	}

	public void toggleImpressum(){


		GetComponent<Animator> ().SetTrigger ("toggle");

	}




	public void closeImpressum(){
		GetComponent<Animator> ().SetTrigger ("close");


	}

}
