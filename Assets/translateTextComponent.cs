using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using Candlelight.UI;

public class translateTextComponent : MonoBehaviour {


	public bool debug = false;


	public bool keepTranslatingAtRuntime;

	public string translateOnlyPart = "";

	string textcontent;


	string originaltext;

	public string currentLanguage;


	// Use this for initialization
	void Start () {

		translate();

	}



	void translate () {



		if ( translateOnlyPart == "" ) {

			if ( GetComponent<Text>() != null ) {
				GetComponent<Text>().text = GameObject.Find("QuestDatabase").GetComponent<Dictionary>().getTranslation(GetComponent<Text>().text);
			}
			else
			if ( GetComponent<HyperText>() != null ) {
				GetComponent<HyperText>().text = GameObject.Find("QuestDatabase").GetComponent<Dictionary>().getTranslation(GetComponent<HyperText>().text);
			}
			textcontent = GetComponent<Text>().text;
			
		}
		else {
			
			string translation = translateOnlyPart;

			string translationAfter = GameObject.Find("QuestDatabase").GetComponent<Dictionary>().getTranslation(translation);
			if ( GetComponent<Text>() != null ) {
				GetComponent<Text>().text = GetComponent<Text>().text.Replace(translation, translationAfter);
			}
			else
			if ( GetComponent<HyperText>() != null ) {
				GetComponent<HyperText>().text = GetComponent<HyperText>().text.Replace(translation, translationAfter);

			}
			textcontent = GetComponent<Text>().text;
				
				
		}


		currentLanguage = GameObject.Find("QuestDatabase").GetComponent<Dictionary>().language;

	}



	void Update () {

		bool done = false;
		if ( keepTranslatingAtRuntime && !GetComponent<Text>().text.Equals(textcontent) ) {
			translate();
			done = true;
		}


		if ( !done && currentLanguage != GameObject.Find("QuestDatabase").GetComponent<Dictionary>().language ) {

			translate();

		}


	}
	

}
