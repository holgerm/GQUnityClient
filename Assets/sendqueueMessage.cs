using UnityEngine;
using System.Collections;
using GQ.Client.Net;

public class sendqueueMessage : MonoBehaviour
{



	public void resend ()
	{

		GameObject.Find ("QuestDatabase").GetComponent<ConnectionClient> ().sendQueue.reconstructSendQueue ();
		GetComponent<Animator> ().SetTrigger ("out");

	}




	public void reset ()
	{

		//	GameObject.Find ("QuestDatabase").GetComponent<ConnectionClient> ().reset ();
		GetComponent<Animator> ().SetTrigger ("p_out");


	}


	public void disableGameObject ()
	{

		gameObject.SetActive (false);

	}



}
