using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class FleixibleWeightCorrection : MonoBehaviour {


	public float weightOnFourToThree = 4f;
	public float weightOfRest = 4f;

	public LayoutElement layoutElement;


	// Use this for initialization
	void Start () {



		//What to reach
		float toreach = (weightOnFourToThree + weightOfRest) / 2f;


		//What we have
		float aspectratio = (float)(Screen.height) / (float)(Screen.width);
		float fourtothree = 3f / 4f;




	
	}

}
