using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


using System.Collections;

public class applyColorPalette : MonoBehaviour {


	public List<ColorApplication> applications;

	public palette questdb;


	public bool hasbutton = false;

	// Use this for initialization
	void Start () {
		questdb = GameObject.Find ("QuestDatabase").GetComponent<palette> ();

		foreach (ColorApplication ca in applications) {


			Color newcolor = Color.white;


			if(ca.color == "main" || ca.color == "default"){
				newcolor = questdb.mainColor;

			} else 	if(ca.color == "background"){
				newcolor = questdb.backgroundColor;
				
			} else 	if(ca.color == "font"){



				if(!questdb.darkBG){ newcolor = Color.black; Debug.Log("black font"); } else { newcolor = Color.white; Debug.Log("white font"); }
				Debug.Log("font color is "+newcolor);
			} else 	if(ca.color == "buttonfont"){
				
				
				
				if(!questdb.darkButton){ newcolor = Color.black; } else { newcolor = Color.white; }
			} else 	if(ca.color == "negativefont" || ca.color == "!font"){
				
				
				
				if(!questdb.darkBG){ newcolor = Color.white; } else { newcolor = Color.black; }

			} else 	if(ca.color == "grey" || ca.color == "gray"){

				
				if(!questdb.darkBG){ newcolor = new Color(0.3f,0.3f,0.3f); } else { newcolor = new Color(0.6f,0.6f,0.6f); }
				//Debug.Log(newcolor);

		
			} else 	if(ca.color == "button"){
				newcolor = questdb.buttonColor;

			} else 	if(ca.color == "comp" || ca.color == "comp1"){
				newcolor = questdb.compColor;
				
			}


			if(ca.button != null){

				hasbutton = true;


				ColorBlock colors = ca.button.colors;
				colors.normalColor = questdb.buttonColor;
				colors.disabledColor = questdb.buttonDisabledColor;
				colors.pressedColor =  questdb.buttonDisabledColor;
				colors.highlightedColor =  questdb.buttonColor;
				ca.button.colors = colors;


			
				foreach(Text t in ca.button.GetComponentsInChildren<Text>()){
				
					if(!questdb.darkButton){ t.color = Color.black; } else { t.color = Color.white; }


				}


			}
			if(ca.canvasimage != null){

				ca.canvasimage.color = newcolor;

			}
			if(ca.canvastext != null){
				
				ca.canvastext.color = newcolor;
				
			}

			if(ca.camera != null){


				ca.camera.backgroundColor = newcolor;

			}


				}

	}
	
	// Update is called once per frame
	void Update () {
	

		if(hasbutton){
		foreach (ColorApplication ca in applications) {

				if(ca.button != null){
			if(!ca.button.IsActive()){

					foreach(Text t in ca.button.GetComponentsInChildren<Text>()){
						
						 t.color = Color.grey; 
						
					}
					} else {
					foreach(Text t in ca.button.GetComponentsInChildren<Text>()){

						if(!questdb.darkButton){ t.color = Color.black; } else { t.color = Color.white; }
					}

					}
				}
				}

		}


				   


	}
}





[System.Serializable]
public class ColorApplication{
	public string title;
	public string color;
	public Image canvasimage;
	public Text canvastext;
	public Camera camera;
	public Button button;
}