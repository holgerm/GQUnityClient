
using UnityEngine;
using UnityEngine.UI;

using System;

using UnitySlippyMap;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.Converters.WellKnownText;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using GQ.Geo;
using UnityEngine.EventSystems;
using System.Xml.Serialization;
using System.Text;
using System.Globalization;

public class page_map : MonoBehaviour
{
	public Map		map;
	public questdatabase questdb;
	public Quest quest;
	public QuestPage mappage;
	public actions questactions;
	public GPSPosition gpsdata;
	public Texture	LocationTexture;
	public Texture	MarkerTexture;
	private float	guiXScale;
	private float	guiYScale;
	private Rect	guiRect;
	private bool 	isPerspectiveView = false;
	private float	perspectiveAngle = 30.0f;
	private float	destinationAngle = 0.0f;
	private float	currentAngle = 0.0f;
	private float	animationDuration = 0.5f;
	private float	animationStartTime = 0.0f;
	private List<Layer> layers;
	private int     currentLayerIndex = 0;
	public LocationMarker location;
	public Transform radiusprefab;
	private bool zoomin = false;
	private bool zoomout = false;
	public bool fixedonposition = true;
	public bool gotgps = true;
	public bool showquests = false;
	private string pre;
	public checkmarkcolor positionCheckmark;
	public Toggle positionToggle;
	public bool togglebuttontouched = false;
	public float togglebuttoncounter = 0f;
	public float mapmovedcounter = 0f;
	public bool fixedpositionbeforemapmovement = true;
	public bool onStartInvoked = false;
	public Route currentroute;
	//private List<Marker> allmarker;
	private
		
		void
			
			Start ()
	{




		if (GameObject.Find ("QuestDatabase") == null) {
			
			Application.LoadLevel (0);
			
		} else {


			questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();

			gpsdata = questdb.GetComponent<GPSPosition> ();

			if (questdb.currentquest != null && questdb.currentquest.id != 0) {

				quest = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest;
				mappage = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest.currentpage;
				questactions = GameObject.Find ("QuestDatabase").GetComponent<actions> ();
			}
		

			pre = "file: /";
		
			//allmarker = new List<Marker>();
		
			if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			
				pre = "file:";
			}

			if (questdb.currentquest != null) {
				if (Application.platform == RuntimePlatform.Android && questdb.currentquest.predeployed) {
			
					pre = "";
				}
			}

		
			// setup the gui scale according to the screen resolution
			guiXScale = (Screen.orientation == ScreenOrientation.Landscape ? Screen.width : Screen.height) / 480.0f;
			guiYScale = (Screen.orientation == ScreenOrientation.Landscape ? Screen.height : Screen.width) / 640.0f;
			// setup the gui area
			guiRect = new Rect (16.0f * guiXScale, 4.0f * guiXScale, Screen.width / guiXScale - 32.0f * guiXScale, 32.0f * guiYScale);
		
			// create the map singleton
			map = Map.Instance;
			map.CurrentCamera = Camera.main;
			map.InputDelegate += UnitySlippyMap.Input.MapInput.BasicTouchAndKeyboard;
	

			map.gameObject.AddComponent<mapdisplaytoggle> ();




			if (questdb.getActiveHotspots ().Count > 0) {

				QuestRuntimeHotspot first = questdb.getActiveHotspots () [0];
				map.CurrentZoom = 18.0f;

				map.CenterWGS84 = new double[2] {(double)first.lat,(double)first.lon };


			} else {


				map.CurrentZoom = 18.0f;

				map.CenterWGS84 = new double[2] { 51	, 8 };
			}

			map.UseLocation = true;




	
			map.UseOrientation = true;
			//map.CameraFollowsOrientation = true;
			map.CenterOnLocation ();


			map.MaxZoom = 20.0f;
			map.MinZoom = 13.0f;



			map.InputsEnabled = true;
			map.ShowGUIControls = true;
		
			//map.GUIDelegate += Toolbar;
		
			layers = new List<Layer> ();
		
			// create an OSM tile layer
			OSMTileLayer osmLayer = map.CreateLayer<OSMTileLayer> ("OSM");
			//osmLayer.BaseURL = "http://a.tile.openstreetmap.org/";
			osmLayer.BaseURL = "http://api.tiles.mapbox.com/v4/" + Configuration.instance.mapboxMapID + "/";
			osmLayer.TileImageExtension = "@2x.png?access_token=" + Configuration.instance.mapboxKey;
			layers.Add (osmLayer);
			Debug.Log ("MAP URL: " + osmLayer.BaseURL + osmLayer.TileImageExtension);


//		osmLayer.TileCacheSizeLimit = osmLayer.TileCacheSizeLimit * 2;

		
			//layers.Add(osmLayer);
		





			updateMapMarker ();




			double a = 0;
			double b = 0;
			if (gpsdata.CoordinatesWGS84.Length > 1) {


				a = gpsdata.CoordinatesWGS84 [0];
				b = gpsdata.CoordinatesWGS84 [1];
				map.CenterWGS84 = new double[2] { a	, b };



			} else {


				if (Application.isWebPlayer || Application.isEditor) {

					//questdb.showmessage("Starte Positions-Simulation. Zum Bewegen der Position benutze die Tasten W,A,S und D.");
					QuestRuntimeHotspot minhotspot = null;
					double[] minhotspotposition = null;
					Debug.Log ("Hotspot Count: " + questdb.getActiveHotspots ().Count);


					if (questdb.getActiveHotspots ().Count > 0) {

						foreach (QuestRuntimeHotspot qrh in questdb.getActiveHotspots()) {
							if ((qrh.lon < 1f && qrh.lon > -1f) && qrh.lat < 1f && qrh.lat > -1f) {

								Debug.Log ("null hotspot");
							} else {

								if (minhotspot == null) {

									minhotspot = qrh;
				
									double[] xy = new double[] {qrh.lon, qrh.lat };
									double lon = (0 / 20037508.34) * 180;                        
									double lat = (((-1) * ((double.Parse (qrh.hotspot.getAttribute ("radius")) * 2d) + 5d)) / 20037508.34) * 180;
									lat = 180 / Math.PI * (2 * Math.Atan (Math.Exp (lat * Math.PI / 180)) - Math.PI / 2);                        
									double[] xy1 = new double[] { xy [0] + lon, xy [1] + lat };
									minhotspotposition = xy1;

				
								} else {

									// Lon/Lat offset by (x// 20037508.34) * 180;
									// i only use lat right now.

									double[] xy = new double[] {qrh.lon, qrh.lat };
									double lon = (0 / 20037508.34) * 180;                        
									double lat = (((-1) * ((double.Parse (qrh.hotspot.getAttribute ("radius")) * 2d) + 5d)) / 20037508.34) * 180;

									lat = 180 / Math.PI * (2 * Math.Atan (Math.Exp (lat * Math.PI / 180)) - Math.PI / 2);                        
									double[] xy1 = new double[] { xy [0] + lon, xy [1] + lat };


				
				
									if (xy1 [1] < minhotspotposition [1]) {
										minhotspot = qrh;
										minhotspotposition = xy1;
									}


								}
							}
						}


//						Debug.Log ("LONLAT:" + minhotspotposition [0] + "," + minhotspotposition [1]);
		
						a = minhotspotposition [1];
						b = minhotspotposition [0];

						gpsdata.CoordinatesWGS84 = new double[]{
							minhotspotposition [1],
							minhotspotposition [0]};

						map.CenterWGS84 = new double[2] { a	, b };
					} else {

						map.CenterWGS84 = new double[2] { 51	, 8 };

					}
				} else {

					// disable location
					setFixedPosition (false);

				}


			}


			if (gpsdata.CoordinatesWGS84.Length > 1 || Application.isWebPlayer || Application.isEditor) {




				// create the location marker
				var posi = Tile.CreateTileTemplate ().gameObject;
				posi.GetComponent<Renderer> ().material.mainTexture = LocationTexture;
				posi.GetComponent<Renderer> ().material.renderQueue = 4000;
				posi.transform.localScale /= 8.0f;
		
				GameObject markerPosi = Instantiate (posi) as GameObject;
				location = map.SetLocationMarker<LocationMarker> (markerPosi, a, b);
				location.OrientationMarker = location.transform;
				location.GetComponentInChildren<MeshRenderer> ().material.color = Color.blue;


				location.gameObject.AddComponent <locationcontrol> ();
				locationcontrol lc = location.GetComponent<locationcontrol> ();
				lc.mapcontroller = this;
				lc.map = map;
				lc.location = location;

				DestroyImmediate (posi);



			} else {
				setFixedPosition (false);

				gotgps = false;

			}


		


		}
	}

	public void updateMapMarker ()
	{

		// DELETE ALL MARKERS


		List<Marker> allmarker = new List<Marker> ();
		allmarker.AddRange (map.Markers);

		foreach (Marker m in allmarker) {

			Destroy (m.gameObject);
			map.Markers.Remove (m);

		}


		foreach (QuestRuntimeHotspot qrh in questdb.hotspots) {

			bool show = true;
			
			if (qrh.category != null && qrh.category != "") {

				foreach (MarkerCategorySprite mcs in Configuration.instance.categoryMarker) {

					if (mcs.category == qrh.category) {

						if (!mcs.showOnMap) {

							show = false;
						}

					}

				}

			}

			if (show) {
				WWW www = null;
			
			
			
				if (qrh.hotspot.getAttribute ("img").StartsWith ("@_")) {
				
				
					www = new WWW (pre + "" + questactions.getVariable (qrh.hotspot.getAttribute ("img")).string_value [0]);
				
				
				} else if (qrh.hotspot.getAttribute ("img") != "") {
				
				
				
				
					string url = qrh.hotspot.getAttribute ("img");
					if (!url.StartsWith ("http:") && !url.StartsWith ("https:")) {
						url = pre + "" + qrh.hotspot.getAttribute ("img");
					}
				
					//				Debug.Log(url);
				
				
					if (url.StartsWith ("http:") || url.StartsWith ("https:")) {
						//Debug.Log("webimage");
					
						www = new WWW (url);
						StartCoroutine (createMarkerAfterImageLoaded (www, qrh));
					
					
					} else if (File.Exists (qrh.hotspot.getAttribute ("img"))) {
						www = new WWW (url);
						StartCoroutine (createMarkerAfterImageLoaded (www, qrh));
					} else if (questdb.currentquest.predeployed) {
						www = new WWW (url);
						StartCoroutine (createMarkerAfterImageLoaded (www, qrh));
					}
				
				} else {
				
				
				
				
					createMarker (qrh, qrh.getMarkerImage ().texture);
				
				
				}
			
			}
			
		}





	}

	public void unDrawCurrentRoute ()
	{

		Debug.Log ("unloading Route");
		foreach (RoutePoint rp in currentroute.points) {

			if (rp.marker != null) {
				map.RemoveMarker (rp.marker);
				DestroyImmediate (rp.waypoint);
				rp.marker = null;
				rp.waypoint = null;
			}

		}
			
			
	}

	public void drawCurrentRoute ()
	{


//		Debug.Log (questdb.currentquest.currentpage.type);
		if (questdb.currentquest.currentpage.type == "MapOSM") {


			foreach (RoutePoint rp in currentroute.points) {



				//string lon = rp.lon;
				//string lat = rp.lat;

				float lat = float.Parse (rp.lon, CultureInfo.InvariantCulture);
				float lon = float.Parse (rp.lat, CultureInfo.InvariantCulture);


		
				GameObject waypoint = new GameObject ();
				
				Marker m1 = map.CreateMarker<Marker> ("", new double[2] {
					lat,
					lon
				}, waypoint);
				rp.marker = m1;
				rp.waypoint = waypoint;



			}


			GameObject.Find ("RouteRender").GetComponent<routerender> ().started = false;
		}
	}

	public void togglePositionClicked (bool b)
	{


		if (questdb != null) {

			if (mapmovedcounter > 0) {

				positionCheckmark.setMode (!fixedpositionbeforemapmovement);
				//positionToggle.isOn = false;
			
				setFixedPosition (!fixedpositionbeforemapmovement);




			} else if (!questdb.fixedposition) {

				positionCheckmark.setMode (true);
				//positionToggle.isOn = false;

				setFixedPosition (true);


			} else {

				positionCheckmark.setMode (false);
				//	positionToggle.isOn = true;

				setFixedPosition (false);

				
			}
				

		}


	}

	private double deg2rad (double deg)
	{
		
		return (deg * Math.PI / 180.0);
		
	}

	private double rad2deg (double rad)
	{
		
		return (rad / Math.PI * 180.0);
		
	}
	
	public double distance (double lat1, double lon1, double lat2, double lon2, char unit)
	{
		
		double theta = lon1 - lon2;
		
		double dist = Math.Sin (deg2rad (lat1)) * Math.Sin (deg2rad (lat2)) + Math.Cos (deg2rad (lat1)) * Math.Cos (deg2rad (lat2)) * Math.Cos (deg2rad (theta));
		
		dist = Math.Acos (dist);
		
		dist = rad2deg (dist);
		
		dist = dist * 60 * 1.1515;
		
		if (unit == 'K') {
			
			dist = dist * 1.609344;
			
		} else if (unit == 'N') {
			
			dist = dist * 0.8684;
			
		} else if (unit == 'M') {
			
			dist = dist * 1609;
			
		}
		
		return (dist);
		
	}

	void createMarker (QuestRuntimeHotspot qrh, Texture image)
	{

		if (qrh.lon != 0f || qrh.lat != 0f) {


//			Debug.Log(qrh.lon+","+qrh.lat);
		
			// Prefab
			GameObject go = Tile.CreateTileTemplate (Tile.AnchorPoint.BottomCenter).gameObject;
		
		
			go.GetComponent<Renderer> ().material.mainTexture = image;
			go.GetComponent<Renderer> ().material.renderQueue = 4001;
		
		
		
			int height = go.GetComponent<Renderer> ().material.mainTexture.height;
			int width = go.GetComponent<Renderer> ().material.mainTexture.width;
		



			float scale = 1.0f;


			if (Application.isMobilePlatform) {

				scale = 2.0f;

			}
		
			if (height > width) {
			
				//Debug.Log(width+"/"+height+"="+width/height);
				go.transform.localScale = new Vector3 ((scale * ((float)width) / ((float)height)), scale, scale);
			
			} else {
			
				go.transform.localScale = new Vector3 (scale, (scale * ((float)width) / ((float)height)), scale);
			
			}
		
			go.transform.localScale /= 512f;
			go.transform.localScale *= width;
		
			int screenWidth;
			#if UNITY_WEBPLAYER || UNITY_EDITOR 
			screenWidth = 1080;
			#else
			screenWidth = Screen.width;
			#endif
		
			go.transform.localScale *= screenWidth / 600f;
		
			go.AddComponent<onTapMarker> ();
			go.GetComponent<onTapMarker> ().hotspot = qrh;
		

			if (questdb.currentquest != null && questdb.currentquest.id != 0) {

		
				go.AddComponent<circletests> ();
		
				if (qrh.hotspot.hasAttribute ("radius")) {
					go.GetComponent<circletests> ().radius = int.Parse (qrh.hotspot.getAttribute ("radius"));
				}

			}

			go.GetComponent<BoxCollider> ().center = new Vector3 (0f, 0f, 0.5f);
			go.GetComponent<BoxCollider> ().size = new Vector3 (1f, 0.1f, 1f);
		
			go.AddComponent<CameraFacingBillboard> ().Axis = Vector3.up;
		
		
			// Instantiate
			GameObject markerGO;
			markerGO = Instantiate (go) as GameObject;
		
		
		
			qrh.renderer = markerGO.GetComponent<MeshRenderer> ();
		
		
		
		
		
		
			// CreateMarker(Name,longlat,prefab)
			Marker m = map.CreateMarker<Marker> (qrh.hotspot.getAttribute ("name"), new double[2] {
			qrh.lat,
			qrh.lon
		}, markerGO);
		
		

		
		
			// Destroy Prefab
			DestroyImmediate (go);
		
		
			if (!qrh.visible) {
			
				qrh.renderer.enabled = false;
			
			
			}

		}
	}

	IEnumerator createMarkerAfterImageLoaded (WWW www, QuestRuntimeHotspot qrh)
	{
		
		yield return www;
		
		if (www.error == null) {

			createMarker (qrh, www.texture);


		} else {
			Debug.Log (www.error);
			
		}
		
	}

	public void initiateZoomIn ()
	{

		zoomin = true;

	}

	public void initiateZoomOut ()
	{
		
		zoomout = true;
		
	}

	public void endZoomIn ()
	{
		
		zoomin = false;
		
	}

	public void endZoomOut ()
	{
		
		zoomout = false;
		
	}

	public void setFixedPosition (bool b)
	{
		togglebuttoncounter = 0.2f;
		togglebuttontouched = true;


		Debug.Log ("toggle clicked: " + b);


		if (b == false) {

				


			Debug.Log ("untoggle");
			map.CameraFollowsOrientation = false;
			questdb.getActiveHotspots ();
			questdb.fixedposition = false;
			GeoPosition center = questdb.getCenter ();

			map.CenterWGS84 = new double[] {
				center.Lat,
				center.Long
//				Configuration.instance.fixedMapCenterLong,
//				Configuration.instance.fixedMapCenterLat
			};

			if (map.CurrentZoom < 17.0f) {
				map.CurrentZoom = 17.0f;
			}
			map.Zoom (1.0f);
				
		} else {
			questdb.fixedposition = true;
			if (map != null) {
				map.CameraFollowsOrientation = false;
			
				map.CenterWGS84 = new double[] {
					location.CoordinatesWGS84 [0],
					location.CoordinatesWGS84 [1]
				//				Configuration.instance.fixedMapCenterLong,
				//				Configuration.instance.fixedMapCenterLat
				};



			}
		}
		fixedonposition = b;
				

	}

	void OnApplicationQuit ()
	{
		map = null;
	}
	
	void Update ()
	{

	

		if (!onStartInvoked && mappage != null) {

			if (mappage.onStart != null) {
				Debug.Log ("invoking on Start of Map");

				mappage.onStart.Invoke ();
				onStartInvoked = true;

			}

			if (currentroute != null && currentroute.points != null & currentroute.points.Count > 1 && currentroute.points [0].waypoint == null) {

				drawCurrentRoute ();


			}


		}

		
		if (mapmovedcounter > 0f) {

			mapmovedcounter -= Time.deltaTime;
		}


		if (togglebuttoncounter > 0f) {
			
			togglebuttoncounter -= Time.deltaTime;
			
		} else {
			
			
			if (Input.GetMouseButtonDown (0)) {

				fixedpositionbeforemapmovement = questdb.fixedposition;
				
				//questdb.fixedposition = false;
				positionCheckmark.setMode (false);
				//positionToggle.isOn = false;
				questdb.fixedposition = false;
				mapmovedcounter = 0.3f;

			}
		}

	


		
			








		if (gpsdata != null && gpsdata.CoordinatesWGS84.Length > 1 && !gotgps) {


			
			double a = gpsdata.CoordinatesWGS84 [0];
			double b = gpsdata.CoordinatesWGS84 [1];
			map.CenterWGS84 = new double[2] { a	, b };
			
			// create the location marker
			var posi = Tile.CreateTileTemplate ().gameObject;
			posi.GetComponent<Renderer> ().material.mainTexture = LocationTexture;
			posi.GetComponent<Renderer> ().material.renderQueue = 4000;
			posi.transform.localScale /= 8.0f;
			
			GameObject markerPosi = Instantiate (posi) as GameObject;
			location = map.SetLocationMarker<LocationMarker> (markerPosi, a, b);
			location.OrientationMarker = location.transform;
			location.GetComponentInChildren<MeshRenderer> ().material.color = Color.blue;
			
			
			location.gameObject.AddComponent <locationcontrol> ();
			locationcontrol lc = location.GetComponent<locationcontrol> ();
			lc.mapcontroller = this;
			lc.map = map;
			lc.location = location;
			
			DestroyImmediate (posi);
			setFixedPosition (true);

			gotgps = true;


		}



		if (questdb != null && questdb.fixedposition) {

			map.CenterWGS84 = gpsdata.CoordinatesWGS84;


		} 



		if (zoomin) {

//			Debug.Log(map.CurrentZoom);
			map.Zoom (1.0f);

			
		} else if (zoomout) {

			map.Zoom (-1.0f);

			
		}


		if (destinationAngle != 0.0f) {
			Vector3 cameraLeft = Quaternion.AngleAxis (-90.0f, Camera.main.transform.up) * Camera.main.transform.forward;
			if ((Time.time - animationStartTime) < animationDuration) {
				float angle = Mathf.LerpAngle (0.0f, destinationAngle, (Time.time - animationStartTime) / animationDuration);
				Camera.main.transform.RotateAround (Vector3.zero, cameraLeft, angle - currentAngle);
				currentAngle = angle;
			} else {
				Camera.main.transform.RotateAround (Vector3.zero, cameraLeft, destinationAngle - currentAngle);
				destinationAngle = 0.0f;
				currentAngle = 0.0f;
				map.IsDirty = true;
			}
			
			map.HasMoved = true;
		}
	}
	
	#if DEBUG_PROFILE
	void LateUpdate()
	{
		Debug.Log("PROFILE:\n" + UnitySlippyMap.Profiler.Dump());
		UnitySlippyMap.Profiler.Reset();
	}
	#endif





	public Route LoadFromText (WWW routewww)
	{
		
		string xmlcontent = routewww.text;
		
		if (xmlcontent != null && xmlcontent.StartsWith ("<error>")) {
			string errMsg = xmlcontent;
			
			GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().showmessage (errMsg);
			return null;
		}
		
	
		if (xmlcontent == null) {
			
			xmlcontent = " ";
		}
		
		
		
		Encoding enc = System.Text.Encoding.UTF8;
		
		
		TextReader txr = new StringReader (xmlcontent);
		

		
		
		XmlSerializer serializer = new XmlSerializer (typeof(Route));
		
		Route r = serializer.Deserialize (txr) as Route; 
		
		
		

		
		return r;
	}
	
	
	
	
	
	
}

[System.Serializable]
public class Route
{
	
	
	public string version;
	public List<RoutePoint> points;

	public void addPoint (string a, string b)
	{

		RoutePoint rp = new RoutePoint ();
		rp.lon = a;
		rp.lat = b;


		if (points == null) {

			points = new List<RoutePoint> ();
		}
		points.Add (rp);

	}

	
}

[System.Serializable]
public class RoutePoint
{


	public string lon;
	public string lat;
	public string description;
	public Marker marker;
	public GameObject waypoint;
	
}

