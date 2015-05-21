using UnityEngine;
using System.Collections;

using UnitySlippyMap;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.Converters.WellKnownText;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;

public class locationcontrol : MonoBehaviour {


	public page_map mapcontroller;
	public Map map;
	public LocationMarker location;

	public GPSPosition gpsdata;





	void Start(){

		gpsdata = mapcontroller.questdb.GetComponent<GPSPosition> ();

	}
	// Update is called once per frame
	void Update () {


		foreach(QuestRuntimeHotspot qrh in mapcontroller.questdb.getActiveHotspots()){



			if(qrh.entered){

				if(mapcontroller.distance(location.CoordinatesWGS84[0],location.CoordinatesWGS84[1],(double)qrh.lat,(double)qrh.lon,'M') > int.Parse(qrh.hotspot.getAttribute("radius"))){
					qrh.hotspot.onLeave.Invoke();
					
					qrh.entered = false;

				}

			} else {

			if(mapcontroller.distance(location.CoordinatesWGS84[0],location.CoordinatesWGS84[1],(double)qrh.lat,(double)qrh.lon,'M') < int.Parse(qrh.hotspot.getAttribute("radius"))){

				qrh.hotspot.onEnter.Invoke();

					qrh.entered = true;
			}

			}


		}



		if(Input.GetKey(KeyCode.A)){




			transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y - Time.deltaTime * 100f,transform.eulerAngles.z);

		}

		if(Input.GetKey(KeyCode.D)){
			
			
			
			transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y + Time.deltaTime * 100f,transform.eulerAngles.z);
			
		}

		if (Input.GetKey (KeyCode.W)) {

			location.CoordinatesEPSG900913 = new double[]{
				location.CoordinatesEPSG900913[0] + (transform.forward.x* Time.deltaTime *50f ),
				location.CoordinatesEPSG900913[1] + (transform.forward.z* Time.deltaTime *50f )
			};
			gpsdata.CoordinatesWGS84 = location.CoordinatesWGS84;

		} else 	if (Input.GetKey (KeyCode.S)) {
			
			location.CoordinatesEPSG900913 = new double[]{
				location.CoordinatesEPSG900913[0] - (transform.forward.x* Time.deltaTime *50f ),
				location.CoordinatesEPSG900913[1] - (transform.forward.z* Time.deltaTime *50f )
			};
			gpsdata.CoordinatesWGS84 = location.CoordinatesWGS84;

			
		}








	}
}
