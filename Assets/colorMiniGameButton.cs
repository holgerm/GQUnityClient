using UnityEngine;
using System.Collections;

public class colorMiniGameButton : MonoBehaviour
{




	public string color;

	public colorMiniGame gameController;




	public void checkMe ()
	{


		gameController.checkColor (color);

	}


}
