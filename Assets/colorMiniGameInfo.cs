using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class colorMiniGameInfo : MonoBehaviour
{



	public colorMiniGame gameController;


	
	// Update is called once per frame
	void Update ()
	{

		GetComponent<Text> ().text = "Zeit: " + Mathf.RoundToInt (gameController.time) + "\nPunkte: " + gameController.score;
	
	}
}
