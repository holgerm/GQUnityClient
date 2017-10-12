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
		GameObject qDBgo = GameObject.Find("QuestDatabase");
		if ( qDBgo == null )
			return;

		Dictionary dictionary = qDBgo.GetComponent<Dictionary>();


		if ( translateOnlyPart == "" ) {

			if ( GetComponent<Text>() != null ) {
				GetComponent<Text>().text = dictionary.getTranslation(GetComponent<Text>().text);
			}
			else
			if ( GetComponent<HyperText>() != null ) {
				GetComponent<HyperText>().text = dictionary.getTranslation(GetComponent<HyperText>().text);
			}
			textcontent = GetComponent<Text>().text;
			
		}
		else {
			
			string translation = translateOnlyPart;

			string translationAfter = dictionary.getTranslation(translation);
			if ( GetComponent<Text>() != null ) {
				GetComponent<Text>().text = GetComponent<Text>().text.Replace(translation, translationAfter);
			}
			else
			if ( GetComponent<HyperText>() != null ) {
				GetComponent<HyperText>().text = GetComponent<HyperText>().text.Replace(translation, translationAfter);

			}
			textcontent = GetComponent<Text>().text;
				
				
		}


		currentLanguage = qDBgo.GetComponent<Dictionary>().language;

	}



	void Update () {
		GameObject qDBgo = GameObject.Find("QuestDatabase");
		if ( qDBgo == null )
			return;

		bool done = false;
		if ( keepTranslatingAtRuntime && !GetComponent<Text>().text.Equals(textcontent) ) {
			translate();
			done = true;
		}


		if ( !done && currentLanguage != qDBgo.GetComponent<Dictionary>().language ) {

			translate();

		}


	}
	

}
