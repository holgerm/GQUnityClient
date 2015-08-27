using UnityEngine;
using System.Collections;
using System;
using UnitySlippyMap;
using Vectrosity;

public class circletests : MonoBehaviour {





	public float radius = 20;

	public questdatabase questdb;

	public Map map;


	public Vector3 up;
	private VectorLine currentCircle;

	// Update is called once per frame
	void Update () {

		if (questdb == null) {
			
			if(GameObject.Find("QuestDatabase") != null){
				questdb = GameObject.Find("QuestDatabase").GetComponent<questdatabase>();
			}
			
		} else
	
		if (map == null) {
			
			
			map = GameObject.Find("PageController_Map").GetComponent<page_map>().map;
			
		} else {


			if(map.IsDirty || questdb.fixedposition ){

			float width = 3f;
			
			
			if(map.RoundedZoom == 17){
				
				width = 2.8f;
				
			} else if(map.RoundedZoom == 16){
				width = 2.6f;
				
			} else if(map.RoundedZoom == 15){
				width = 2.4f;
				
			} else if(map.RoundedZoom == 14){
				width = 2.2f;
				
			} else if(map.RoundedZoom == 13){
				width = 2.0f;
				
			} else if(map.RoundedZoom == 12){
				width = 1.8f;
			} else if(map.RoundedZoom == 11){
				width = 1.6f;

			}



				VectorLine.SetCanvasCamera (GameObject.Find("MapCam").GetComponent<Camera>());
			VectorLine.canvas.sortingLayerName = "Default";

				VectorLine.Destroy (ref currentCircle);
				var myLine = new VectorLine ("Round", new Vector3[202], null, width);


				myLine.color = new Color(questdb.GetComponent<palette>().mainColor.r,questdb.GetComponent<palette>().mainColor.g,questdb.GetComponent<palette>().mainColor.b,0.75f);
				myLine.MakeCircle ( transform.position,new Vector3(0f,90f,0f), (float)(radius/1000f), 100,1f);
				myLine.joins = Joins.Weld;

				currentCircle = myLine;

			myLine.Draw ();
			}
		}
	}
}
