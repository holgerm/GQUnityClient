using UnityEngine;
using System.Collections;

public class menucontroller : MonoBehaviour {




	public bool isActive = false;
	public questdatabase questdb;

	



	IEnumerator Start(){



		
		if (GameObject.Find ("MenuCanvas") != gameObject) {
			Destroy (gameObject);		

		} else {
			DontDestroyOnLoad (gameObject);
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();

			GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().menu = this;
		}








	}




	public void showImpressum(){

		GameObject.Find ("ImpressumCanvas").GetComponent<showimpressum> ().toggleImpressum ();

	}


	public void endQuestAnimation(){

		GetComponent<Animator> ().SetTrigger ("endmenu");
		isActive = false;

	}


	public void enQuest(){

		GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().endQuest ();


	}

	public void returntoMainMenu(){

		isActive = false;

		Application.LoadLevel (0);

	}



	public void reloadAllData(){


		GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().ReloadButtonPressed ();


	}

public void showTopBar(){
		isActive = true;


		if (this != null) {
			GetComponent<Animator> ().SetTrigger ("startMenu");
		}
	}



	public void hideTopBar(){
		isActive = false;


	}

}
