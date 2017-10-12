using UnityEngine;
using System.Collections;
using GQ.Client.Conf;

public class createLanguageButtons : MonoBehaviour {


	public GameObject prefab;

	// Use this for initialization
	void Start () {
	


		foreach ( Language l in Configuration.instance.languagesAvailable ) {



			if ( l.available ) {




				GameObject newButton = Instantiate(prefab) as GameObject;
				languageButton button = newButton.GetComponent <languageButton>();
				button.spr.sprite = l.sprite;
				button.name.text = l.anzeigename_de;
				button.bezeichnung = l.bezeichnung;
				newButton.transform.SetParent(transform);
				newButton.transform.localScale = new Vector3(1f, 1f, 1f);

			}



		}



	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
