using UnityEngine;
using System.Collections;
using UnitySlippyMap;
using System.Globalization;



public class routerender : MonoBehaviour {







	public Map map;
	public page_map mapController;



	void Update(){


		if (map == null) {


			map = mapController.map;

		} else {







			if (map.IsDirty && mapController.currentroute != null) {




				float width = 0.0025f;


				if(map.RoundedZoom == 17){

					width = 0.005f;

				} else if(map.RoundedZoom == 16){
					width = 0.01f;

				} else if(map.RoundedZoom == 15){
					width = 0.03f;

				} else if(map.RoundedZoom == 14){
					width = 0.05f;

				} else if(map.RoundedZoom == 13){
					width = 0.1f;

				} else if(map.RoundedZoom == 12){
					width = 0.5f;
				} else if(map.RoundedZoom == 11){

				}


				Debug.Log(map.RoundedZoom);

				GetComponent<LineRenderer>().SetWidth(width,width);


				GetComponent<LineRenderer> ().SetVertexCount (mapController.currentroute.points.Count);

				int i = 0;

				foreach (RoutePoint rp in mapController.currentroute.points) {
				
				
//					Debug.Log (i);
				
					//string lon = rp.lon;
					//string lat = rp.lat;
				
					float lat = float.Parse (rp.lon, CultureInfo.InvariantCulture);
					float lon = float.Parse (rp.lat, CultureInfo.InvariantCulture);
				
				
				
					GameObject waypoint = new GameObject ();
				
					Marker m1 = map.CreateMarker<Marker> ("", new double[2] {
					lat,
					lon
				}, waypoint);
				
					rp.waypoint = waypoint;
				
				
				
					GetComponent<LineRenderer> ().SetPosition (i, m1.transform.position);
				
					GetComponent<LineRenderer> ().sortingLayerName = "Foreground";
					i++;
				
				}



			}

		}

	}





}
