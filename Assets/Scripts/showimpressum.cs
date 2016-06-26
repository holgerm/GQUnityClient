using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Linq;
using System;
using GQ.Client.Conf;

public class showimpressum : MonoBehaviour {





	public Image impressum_bg;
	public Text impressum_text;
	public Text privacy_text;
	public Text agbs_text;





	public Button impressum_closebutton;
	public Text impressum_closebuttontext;
	public Image impressum_reloadbutton;
	public Image impressum_scrollpanel;
	public Image impressum_geoquestbutton;

	public GameObject upperPanel;

	public void Start () {
		if ( GameObject.Find("ImpressumCanvas") != gameObject ) {
			Destroy(gameObject);		
		}
		else {
			DontDestroyOnLoad(gameObject);

			StartCoroutine(loadImpressum());

			impressum_text.text = Configuration.instance.impressum.Replace("\\n", "\n");
		}

	}

	// TODO maybe we should really make an extension of string of this method here: (hm), i.e. add "this" to param list
	public string RemoveWhitespace (string input) {
		return new string(input.ToCharArray()
		                  .Where(c => !Char.IsWhiteSpace(c))
		                  .ToArray());
	}


	public void loadPrivacy () {
		string s = Configuration.instance.privacyAgreement;

		GameObject.Find("QuestDatabase").GetComponent<actions>().localizeStringToDictionary(s);
		s = GameObject.Find("QuestDatabase").GetComponent<actions>().formatString(GameObject.Find("QuestDatabase").GetComponent<actions>().localizeString(s));

		privacy_text.text = s;


	}

	public void loadAGBs () {
		string s = Configuration.instance.agbs;

		GameObject.Find("QuestDatabase").GetComponent<actions>().localizeStringToDictionary(s);
		s = GameObject.Find("QuestDatabase").GetComponent<actions>().formatString(GameObject.Find("QuestDatabase").GetComponent<actions>().localizeString(s));
		
		agbs_text.text = s;
		
		
	}



	IEnumerator loadImpressum () {
		
		WWW www = new WWW("http://qeevee.org:9091/" + Configuration.instance.portalID + "/imprint");
		yield return www;


		if ( PlayerPrefs.HasKey("imprint") ) {

			Configuration.instance.impressum = PlayerPrefs.GetString("imprint");

		}

		string imprint = Configuration.instance.impressum;

		
		if ( www.error != null && www.error != "" && www.text != "" ) {
			
			Debug.Log("Couldn't load imprint: " + www.error);


			
		}
		else {
			

				
				
			if ( www.text != null && RemoveWhitespace(www.text) != "" ) {

				imprint = www.text;
				PlayerPrefs.SetString("imprint", imprint);
			}


		}
		
		imprint = GameObject.Find("QuestDatabase").GetComponent<actions>().formatString(imprint);
		
		
		GameObject.Find("QuestDatabase").GetComponent<actions>().localizeStringToDictionary(imprint);
		
		imprint = GameObject.Find("QuestDatabase").GetComponent<actions>().localizeString(imprint);
		
		
		impressum_text.text = imprint;

		
	}

	public void toggleImpressum () {


		GetComponent<Animator>().SetTrigger("toggle");

	}




	public void closeImpressum () {
		GetComponent<Animator>().SetTrigger("close");


	}

}
