using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class colorMiniGameInfo : MonoBehaviour
{



	public colorMiniGame gameController;

	public bool punkte;
	public bool zeit;

	
	// Update is called once per frame
	void Update ()
	{
		if (zeit) {
			
			GetComponent<Text> ().text = "Zeit: " + gameController.time.ToString ("0.00");

		} 
		if (punkte) {

			GetComponent<Text> ().text = "Punkte: " + gameController.score;
		}
	
	}
}
