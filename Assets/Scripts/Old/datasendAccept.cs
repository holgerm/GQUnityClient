using UnityEngine;
using System.Collections;

public class datasendAccept : MonoBehaviour {


	public int pageid;
	public questdatabase questdb;

	public void accept(){


		questdb.acceptedDatasend (pageid);

		GetComponent<Animator> ().SetTrigger ("out");

	}


	public void decline(){


		questdb.rejectedDatasend (pageid);
		GetComponent<Animator> ().SetTrigger ("out");

	}


}
