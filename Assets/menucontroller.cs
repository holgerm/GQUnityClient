using UnityEngine;
using System.Collections;
using GQ.Client.Conf;

public class menucontroller : MonoBehaviour {




	public bool isActive = false;
	public questdatabase questdb;




	public GameObject impressumbutton;
	public GameObject quitquestbutton;
	public GameObject deletedatabutton;
	public GameObject impressumduringmapbutton;
	public GameObject categoriesformap;


	IEnumerator Start () {


		questdb = GameObject.Find("QuestDatabase").GetComponent<questdatabase>();
		
		if ( GameObject.Find("MenuCanvas") != gameObject ) {
			Destroy(gameObject);		

		}
		else {
			DontDestroyOnLoad(gameObject);
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();

			GameObject.Find("QuestDatabase").GetComponent<questdatabase>().menu = this;
		}




		if ( Configuration.instance.questvisualization == "map" ) {

			showTopBar();

		}





	}


	void Update () {


		if ( questdb == null ) {


			if ( GameObject.Find("QuestDatabase") != null ) {

				questdb = GameObject.Find("QuestDatabase").GetComponent<questdatabase>();
			}

		}
		else {
			if ( Configuration.instance.questvisualization == "map" ) {
				//impressumbutton.SetActive(false);


				if ( questdb.currentquest == null ) {
					deletedatabutton.SetActive(true);
					quitquestbutton.SetActive(false);
					categoriesformap.SetActive(true);

				}
				else {
					deletedatabutton.SetActive(false);
					quitquestbutton.SetActive(true);
					categoriesformap.SetActive(false);



				}
				//impressumduringmapbutton.SetActive(true);

			}
			else {
				impressumbutton.SetActive(true);
				if ( questdb.currentquest == null ) {
					quitquestbutton.SetActive(false);
					deletedatabutton.SetActive(true);
				}
				else {
					quitquestbutton.SetActive(true);
					deletedatabutton.SetActive(false);
				}

				//impressumduringmapbutton.SetActive(false);
				categoriesformap.SetActive(false);


			}


		}
	}


	public void lookForUpdates () {
		if ( questdb.allquests == null || questdb.allquests.Count == 0 ) {
			questdb.showmessage("Keine Verbindung zum Server möglich.");
			return;
		}

		if ( questdb.questsHaveUpdates() ) {

			questdb.askUserForUpdatingQuests();


		}
		else {

			questdb.showmessage("Deine App ist auf dem neusten Stand.");

		}


	}


	public void showMenu () {


		if ( GameObject.Find("QuestDatabase").GetComponent<questdatabase>().currentquest != null && GameObject.Find("QuestDatabase").GetComponent<questdatabase>().currentquest.currentpage.type == "WebPage" ) {

			GameObject.Find("PageController").GetComponent<page_webpage>().deactivateWebView();

		}

	}


	public void hideMenu () {

		if ( GameObject.Find("QuestDatabase").GetComponent<questdatabase>().currentquest != null && GameObject.Find("QuestDatabase").GetComponent<questdatabase>().currentquest.currentpage.type == "WebPage" ) {
			
			GameObject.Find("PageController").GetComponent<page_webpage>().activateWebView();
			
		}
		
		
	}

	public void showImpressum () {

		GameObject.Find("ImpressumCanvas").GetComponent<showimpressum>().toggleImpressum();

	}


	public void endQuestAnimation () {

		if ( Configuration.instance.questvisualization == "map" ) {
			GetComponent<Animator>().SetTrigger("endquestbutnotmenu");

		

		}
		else {
			GetComponent<Animator>().SetTrigger("endmenu");
			isActive = false;
		}

	}


	public void enQuest () {

		GameObject.Find("QuestDatabase").GetComponent<questdatabase>().endQuest();


	}

	public void returntoMainMenu () {

		isActive = false;

		Application.LoadLevel(0);

	}



	public void reloadAllData () {


		bool b = GameObject.Find("QuestDatabase").GetComponent<questdatabase>().ReloadButtonPressed();

		if ( b ) {
			hideMenu();
		}

	}

	public void showTopBar () {


		isActive = true;


		if ( this != null ) {
			GetComponent<Animator>().SetTrigger("startMenu");
		}


	}



	public void hideTopBar () {


		if ( Configuration.instance.questvisualization != "map" ) {

			isActive = false;
		}

	}

}
