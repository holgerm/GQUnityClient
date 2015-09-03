using UnityEngine;
using System.Collections;
using UnitySlippyMap;
using System.Globalization;
using System.Collections.Generic;

using Vectrosity;

public class routerender : MonoBehaviour
{







	public Map map;
	public page_map mapController;

	public Material material;
	public bool started = false;

	public questdatabase questdb;


	VectorLine currentLine;



	void Update ()
	{


		if (questdb == null) {

			if (GameObject.Find ("QuestDatabase") != null) {
				questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
			}

		} else if (map == null) {


			map = mapController.map;

		} else {




			if (mapController.currentroute != null){
				//Debug.Log ("Route is not null");
		}

			if (questdb.currentquest != null && questdb.currentquest.currentpage != null && questdb.currentquest.currentpage.type == "MapOSM" && (map.IsDirty || questdb.fixedposition || !started) && mapController.currentroute != null) {


//				Debug.Log("redoing");
				VectorLine.Destroy (ref currentLine);
//				Debug.Log("map is dirty");

				started = true;


				float width = 4f;


				if (map.RoundedZoom == 17) {

					width = 6f;

				} else if (map.RoundedZoom == 16) {
					width = 7f;

				} else if (map.RoundedZoom == 15) {
					width = 10f;

				} else if (map.RoundedZoom == 14) {
					width = 12f;

				} else if (map.RoundedZoom == 13) {
					width = 14f;

				} else if (map.RoundedZoom == 12) {
					width = 16f;
				} else if (map.RoundedZoom == 11) {

				}


//				if(map.RoundedZoom <= 15){
//
//					material.color = new Color(material.color.r,material.color.g,material.color.b,1f);
//
//				} else {
//
//					material.color = new Color(material.color.r,material.color.g,material.color.b,0.5f);
//
//				}

//				Debug.Log(map.RoundedZoom);

//				GetComponent<LineRenderer>().SetWidth(width,width);
//
//
//				GetComponent<LineRenderer> ().SetVertexCount (mapController.currentroute.points.Count);

				var linePoints = new List<Vector3> ();



				foreach (RoutePoint rp in mapController.currentroute.points) {
				
				
//			
					if (rp.waypoint != null) {
//				

						linePoints.Add (rp.waypoint.transform.position);
					}
				
				}

				if (linePoints.Count > 0 && linePoints.Count % 2 != 0) {

					linePoints.Add (linePoints [linePoints.Count - 1]);

				}
				if (linePoints.Count > 0) {
					//VectorLine.canvas.transform.SetParent(map.gameObject.transform);
					//VectorLine.canvas.renderMode = RenderMode.WorldSpace;
					VectorLine.SetCanvasCamera (GameObject.Find ("MapCam").GetComponent<Camera> ());
					VectorLine myLine = new VectorLine ("MyLine", linePoints, material, width, LineType.Continuous);
					myLine.joins = Joins.Fill;
					myLine.Draw ();

					currentLine = myLine;

				} 
			}

		}

	}





}
