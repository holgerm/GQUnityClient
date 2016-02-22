using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class colorMiniGame : MonoBehaviour
{




	public page_custom controller;

	public string colorName1;
	public string colorName2;
	public string colorName3;
	public string colorName4;



	public Color color1;
	public Color color2;
	public Color color3;
	public Color color4;


	public colorMiniGameButton color1Button;
	public colorMiniGameButton color2Button;
	public colorMiniGameButton color3Button;
	public colorMiniGameButton color4Button;

	public Text actionText;


	public float time = 60f;
	public int score = 0;



	public bool running = false;

	string solution = "";
	public float timeBetween = 3f;

	IEnumerator Start ()
	{


		controller = GameObject.Find ("PageController").GetComponent<page_custom> ();


		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();



		colorName1 = controller.param1;
		colorName2 = controller.param3;
		colorName3 = controller.param5;
		colorName4 = controller.param7;

		color1 = hexToColor (controller.param2);
		color2 = hexToColor (controller.param4);
		color3 = hexToColor (controller.param6);
		color4 = hexToColor (controller.param8);


		time = float.Parse (controller.param9);


		color1Button.GetComponent<Image> ().color = color1;
		color2Button.GetComponent<Image> ().color = color2;
		color3Button.GetComponent<Image> ().color = color3;
		color4Button.GetComponent<Image> ().color = color4;

		color1Button.color = colorName1;
		color2Button.color = colorName2;
		color3Button.color = colorName3;
		color4Button.color = colorName4;

		nextColor ();
		running = false;
	}

	private static Color hexToColor (string hex)
	{


		if (hex == "") {

			return Color.white;

		}

		hex = hex.Replace ("0x", "");//in case the string is formatted 0xFFFFFF
		hex = hex.Replace ("#", "");//in case the string is formatted #FFFFFF
		byte a = 255;//assume fully visible unless specified in hex
		byte r = byte.Parse (hex.Substring (0, 2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse (hex.Substring (2, 2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse (hex.Substring (4, 2), System.Globalization.NumberStyles.HexNumber);
		//Only use alpha if the string has enough characters
		if (hex.Length == 8) {
			a = byte.Parse (hex.Substring (4, 2), System.Globalization.NumberStyles.HexNumber);
		}
		return new Color32 (r, g, b, a);
	}



	public void nextColor ()
	{
		string currentColorName = actionText.text;
		Color currentColor = actionText.color;


		while (actionText.text == currentColorName &&
		       actionText.color == currentColor) {

			int c =	Random.Range (1, 4);

			if (c == 1) {


				actionText.text = colorName1;

				c =	Random.Range (1, 3);

				if (c == 1) {

					actionText.color = color2;
				} else if (c == 2) {

					actionText.color = color3;
				} else if (c == 3) {

					actionText.color = color4;
				} 
		



			} else if (c == 2) {

				actionText.text = colorName2;

				c =	Random.Range (1, 3);

				if (c == 1) {

					actionText.color = color1;
				} else if (c == 2) {

					actionText.color = color3;
				} else if (c == 3) {

					actionText.color = color4;
				} 

			} else if (c == 3) {

				actionText.text = colorName3;

				c =	Random.Range (1, 3);

				if (c == 1) {

					actionText.color = color1;
				} else if (c == 2) {

					actionText.color = color2;
				} else if (c == 3) {

					actionText.color = color4;
				} 

			} else if (c == 4) {

				actionText.text = colorName4;
				c =	Random.Range (1, 3);

				if (c == 1) {

					actionText.color = color1;
				} else if (c == 2) {

					actionText.color = color2;
				} else if (c == 3) {

					actionText.color = color3;
				} 

			} 

		}
	}




	public void checkColor (string cname)
	{


		if (!running) {
			if (actionText.text == cname) {
				
				running = true;
			}
		}


		if (running) {
			if (actionText.text == cname) {

				score += 100;

			}

			nextColor ();
		}

	

	}


	// Update is called once per frame
	void Update ()
	{

		if (running) {
			time -= Time.deltaTime;



			if (time <= 0f) {

				controller.questactions.setVariable (controller.param10, (float)score);
				controller.onEnd ();


			}

		}

	
	}
}
