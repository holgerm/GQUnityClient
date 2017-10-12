using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using GQ.Client.Model;

public class SampleButton : MonoBehaviour
{
	
	public Text nameLabel;
	public Quest q;
	private float clickTime;
	// time of click
	public bool onClick = true;
	// is click allowed on button?
	public bool onDoubleClick = false;
	// is double-click allowed on button?


	public Button removeButton;





	void Start ()
	{

		if (q.hasData) {

			removeButton.enabled = true;
			removeButton.GetComponent<Image> ().enabled = true;
			foreach (Image i in	removeButton.GetComponentsInChildren<Image> ()) {
				i.enabled = true;
			}

		} else {

			removeButton.enabled = false;
			removeButton.GetComponent<Image> ().enabled = false;
			foreach (Image i in	removeButton.GetComponentsInChildren<Image> ()) {
				i.enabled = false;
			}

		}

	}

	public void startquest ()
	{

		StartCoroutine (GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().startQuest (q));
		//Debug.Log ("starting");

	}



	public void removeQuest ()
	{


		GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().removeQuest (q);

	}



}