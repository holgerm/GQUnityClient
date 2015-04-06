using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


using System.Collections;

public class applyColorPalette : MonoBehaviour {


	public List<ColorApplication> applications;

	public palette questdb;


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



				if(!questdb.darkBG){ newcolor = Color.black; } else { newcolor = Color.white; }
			} else 	if(ca.color == "negativefont" || ca.color == "!font"){
				
				
				
				if(!questdb.darkBG){ newcolor = Color.white; } else { newcolor = Color.black; }

			} else 	if(ca.color == "grey" || ca.color == "gray"){

				
				if(!questdb.darkBG){ newcolor = new Color(0.3f,0.3f,0.3f); } else { newcolor = new Color(0.6f,0.6f,0.6f); }
				Debug.Log(newcolor);

			} else 	if(ca.color == "DarkOrBright"){

				if(!questdb.darkBG){ newcolor = questdb.brightColor; } else { newcolor = questdb.darkColor; }

				
			} else 	if(ca.color == "light" || ca.color == "bright"){
				newcolor = questdb.brightColor;

			} else 	if(ca.color == "comp" || ca.color == "comp1"){
				newcolor = questdb.compColor;
				
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
	
	}
}





[System.Serializable]
public class ColorApplication{
	public string title;
	public string color;
	public Image canvasimage;
	public Text canvastext;
	public Camera camera;
}