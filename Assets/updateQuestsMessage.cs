using UnityEngine;
using System.Collections;

public class updateQuestsMessage : MonoBehaviour {



	public questdatabase questdb;

	public void updateQuests () {

		questdb.updateAllQuests();
		GetComponent<Animator>().SetTrigger("out");


	}



	public void close () {

		GetComponent<Animator>().SetTrigger("out");



	}



	public void disableGameObject () {

		gameObject.SetActive(false);

	}

}
