using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class palette : MonoBehaviour {

	public List<ColorPalette> profiles;

	public Color mainColor;

	public Color backgroundColor;

	public bool darkBG = false;
	
	public Color buttonColor;

	public bool darkButton = false;

	public Color buttonDisabledColor;

	public Color compColor;

	public Color secondCompColor;

	// Use this for initialization
	void Awake () {
		foreach ( ColorPalette cp in profiles ) {
			if ( cp.id == Configuration.instance.colorProfile ) {
				mainColor = cp.mainColor;
				backgroundColor = cp.backgroundColor;
				darkBG = cp.darkBG;
				buttonColor = cp.buttonColor;
				darkButton = cp.darkButton;
				buttonDisabledColor = cp.buttonDisabledColor;
				compColor = cp.compColor;
				secondCompColor = cp.secondCompColor;
			}
		}
	}
}






[System.Serializable]
public class ColorPalette {


	public string id;

	public Color mainColor;
	
	public Color backgroundColor;
	
	public bool darkBG = false;
	
	public Color buttonColor;
	
	public bool darkButton = false;
	
	public Color buttonDisabledColor;
	
	public Color compColor;
	
	public Color secondCompColor;
}