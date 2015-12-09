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

public class Locationcontrol : MonoBehaviour {


	public page_map mapcontroller;
	public Map map;
	public LocationMarker location;

	public GPSPosition gpsdata;





	void Start () {

		gpsdata = mapcontroller.questdb.GetComponent<GPSPosition>();

	}
	// Update is called once per frame
	void Update () {


		foreach ( QuestRuntimeHotspot qrh in mapcontroller.questdb.getActiveHotspots() ) {



			if ( qrh.entered ) {

				if ( distance(location.CoordinatesWGS84[0], location.CoordinatesWGS84[1], (double)qrh.lat, (double)qrh.lon, 'M') > int.Parse(qrh.hotspot.getAttribute("radius")) ) {
					qrh.hotspot.onLeave.Invoke();
					
					qrh.entered = false;

				}

			}
			else {

				if ( distance(location.CoordinatesWGS84[0], location.CoordinatesWGS84[1], (double)qrh.lat, (double)qrh.lon, 'M') < int.Parse(qrh.hotspot.getAttribute("radius")) ) {

					if ( qrh.hotspot.onEnter != null ) {
						qrh.hotspot.onEnter.Invoke();
					}
					qrh.entered = true;
				}

			}


		}




//		bool updateposition = false;

		if ( Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ) {



//			updateposition = true;
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - Time.deltaTime * 100f, transform.eulerAngles.z);

		}

		if ( Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ) {
			
			
			
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + Time.deltaTime * 100f, transform.eulerAngles.z);
			
		}

		if ( Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ) {

			location.CoordinatesEPSG900913 = new double[] {
				location.CoordinatesEPSG900913[0] + (transform.forward.x * Time.deltaTime * 50f),
				location.CoordinatesEPSG900913[1] + (transform.forward.z * Time.deltaTime * 50f)
			};
			gpsdata.CoordinatesWGS84 = location.CoordinatesWGS84;

		}
		else
		if ( Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ) {
			
			location.CoordinatesEPSG900913 = new double[] {
				location.CoordinatesEPSG900913[0] - (transform.forward.x * Time.deltaTime * 50f),
				location.CoordinatesEPSG900913[1] - (transform.forward.z * Time.deltaTime * 50f)
			};
			gpsdata.CoordinatesWGS84 = location.CoordinatesWGS84;

		}
	}

	public double distance (double lat1, double lon1, double lat2, double lon2, char unit) {
		
		double theta = lon1 - lon2;
		
		double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
		
		dist = Math.Acos(dist);
		
		dist = rad2deg(dist);
		
		dist = dist * 60 * 1.1515;
		
		if ( unit == 'K' ) {
			
			dist = dist * 1.609344;
			
		}
		else
		if ( unit == 'N' ) {
			
			dist = dist * 0.8684;
			
		}
		else
		if ( unit == 'M' ) {
			
			dist = dist * 1609;
			
		}
		
		return (dist);
		
	}
	
	private double deg2rad (double deg) {
		
		return (deg * Math.PI / 180.0);
		
	}
	
	private double rad2deg (double rad) {
		
		return (rad / Math.PI * 180.0);
		
	}

}
