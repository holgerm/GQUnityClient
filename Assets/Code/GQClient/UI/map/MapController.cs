using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySlippyMap.Markers;
using GQ.Client.UI;
using UnityEngine.UI;
using UnitySlippyMap.Map;
using GQ.Client.Conf;
using System;
using GQ.Client.Err;
using GQ.Client.Util;
using UnitySlippyMap.Layers;

namespace GQ.Client.UI
{

	public abstract class MapController : MonoBehaviour
	{

		private List<LayerBehaviour> layers;
		protected MapBehaviour map;

		#region Markers
		private Dictionary<int, Marker> markers;

		protected Dictionary<int, Marker> Markers {
			get {
				if (markers == null) {
					markers = new Dictionary<int, Marker> ();
				}
				return markers;
			}
		}

		protected float MARKER_SCALE_FACTOR {
			get {
				// We empirically found this to be close to a correct scaling factor in order to resize the markers according to 
				// UI elements like buttons etc.:
				return (400000f / Device.height);
			}
		}
		#endregion


		#region Global static behaviour
		private static bool _ignoreInteraction = false;

		public static bool IgnoreInteraction {
			get {
				return _ignoreInteraction;
			}
			set {
				if (value == true) {
					_ignoreInteraction = true;
				} else {
					Base.Instance.StartCoroutine (_setIgnoreInteractionToFalseAsCoroutine ());
				}
			}

		}

		private static IEnumerator _setIgnoreInteractionToFalseAsCoroutine ()
		{
			yield return new WaitForEndOfFrame ();
			yield return new WaitForEndOfFrame ();
			_ignoreInteraction = false;
			yield break;
		}
		#endregion


		#region Centering
		public GameObject MapButtonPanel;
		public Texture CenterTexture;
		public Texture FrameTexture;
		public Texture	LocationTexture;

		public enum Centering
		{
			Centered,
			Framed,
			Manual
		}

		public Centering CenteringState {
			get;
			protected set;
		}

		public void Frame ()
		{
			// center the map so it frames all currently visible markers: TODO

			// let the center button show the centering button icon now, unless local position is not available, in that case show the frame icon and disbale it.

			CenteringState = Centering.Framed;
		}

		public void Center ()
		{
			// center the map so it is centered to the current users position: TODO
			map.CenterOnLocation ();

			// let the center button show the centering button icon now

			CenteringState = Centering.Centered;
		}

		public void CenterButtonPressed ()
		{
			Center ();
		}
		#endregion


		#region Zooming
		public void ZoomInButtonPressed ()
		{
			map.CurrentZoom = Math.Min (map.CurrentZoom + ConfigurationManager.Current.mapDeltaZoom, map.MaxZoom);
			map.Zoom (0f);
			UpdateZoomButtons ();
		}

		public void ZoomOutButtonPressed ()
		{
			map.CurrentZoom = Math.Max (map.CurrentZoom - ConfigurationManager.Current.mapDeltaZoom, map.MinZoom);
			map.Zoom (0f);
			UpdateZoomButtons ();
		}

		protected void UpdateZoomButtons ()
		{
			// If further zooming IN is not possible disable ZoomInButton: 
			zoomInButton.Enabled = (map.MaxZoom > map.CurrentZoom);

			// If further zooming OUT is not possible disable ZoomOutButton: 
			zoomOutButton.Enabled = (map.MinZoom < map.CurrentZoom);
		}
		#endregion

		#region Map Layers
		private LayerBehaviour MapLayer {
			get {
				LayerBehaviour mapLayer;

				switch (ConfigurationManager.Current.mapProvider) {
				case MapProvider.OpenStreetMap:
					mapLayer = OsmMapLayer;
					break;
				case MapProvider.MapBox:
					mapLayer = MapBoxLayer;
					break;
				default:
					Log.SignalErrorToDeveloper (
						"Unknown map provider defined in configuration: {0}. We use OpenStreetMap instead.", 
						ConfigurationManager.Current.mapProvider
					);
					mapLayer = OsmMapLayer;
					break;
				}

				return mapLayer;
			}
		}

		private LayerBehaviour OsmMapLayer {
			get {
				OSMTileLayer osmLayer = map.CreateLayer<OSMTileLayer> (ConfigurationManager.Current.mapProvider.ToString ());
				osmLayer.BaseURL = ConfigurationManager.Current.mapBaseUrl + "/";
				return osmLayer;
			}
		}

		private LayerBehaviour MapBoxLayer {
			get {
				OSMTileLayer mapBoxLayer = map.CreateLayer<OSMTileLayer> (ConfigurationManager.Current.mapProvider.ToString ());
				mapBoxLayer.BaseURL = "http://api.tiles.mapbox.com/v4/" + ConfigurationManager.Current.mapID + "/";
				mapBoxLayer.TileImageExtension = "@2x.png?access_token=" + ConfigurationManager.Current.mapKey;
				return mapBoxLayer;
			}
		}
		#endregion

		#region Runtime
		OverlayButtonLayoutConfig zoomInButton;
		OverlayButtonLayoutConfig zoomOutButton;

		protected virtual void Start ()
		{
			// create the map singleton
			map = MapBehaviour.Instance;
			map.CurrentCamera = Camera.main;
			map.InputDelegate += UnitySlippyMap.Input.MapInput.BasicTouchAndKeyboard;
			map.CurrentZoom = 15.0f;

			Frame ();
			GameObject zibGo = MapButtonPanel.transform.Find ("ZoomInButton").gameObject;
			zoomInButton = zibGo.GetComponent<OverlayButtonLayoutConfig> ();
			zoomOutButton = MapButtonPanel.transform.Find ("ZoomOutButton").GetComponent<OverlayButtonLayoutConfig> ();

			LocationSensor.Instance.OnLocationUpdate += 
				(object sender, LocationSensor.LocationEventArgs e) => {
				if (e.Kind == LocationSensor.LocationEventType.Update) {
					Debug.Log (
						string.Format ("--- LOC: {0}, {1}", e.Location.longitude, e.Location.latitude)
						.Yellow ()
					);
				}
				if (e.Kind == LocationSensor.LocationEventType.NotAvailable) {
					Debug.Log (("--- LOC: Unavailable. enabled: " + Input.location.isEnabledByUser).Yellow ());
				}
			};

			map.UsesLocation = true;
			map.InputsEnabled = true;
			map.ShowsGUIControls = false;

			// Locate at Start:
			if (ConfigurationManager.Current.mapStartAtLocation) {
				map.CenterOnLocation ();
			} else {
				map.CenterWGS84 = new double[2] { 
					ConfigurationManager.Current.mapStartAtLongitude, 
					ConfigurationManager.Current.mapStartAtLatitude 
				};
			}

			layers = new List<LayerBehaviour> ();

			layers.Add (MapLayer);

			// create the location marker
			GameObject go = TileBehaviour.CreateTileTemplate ().gameObject;
			go.GetComponent<Renderer> ().material.mainTexture = LocationTexture;
			go.GetComponent<Renderer> ().material.renderQueue = 4000;
			go.transform.localScale /= 4f; 

			GameObject markerGO = Instantiate (go) as GameObject;
			map.SetLocationMarker<LocationMarkerBehaviour> (markerGO);
			DestroyImmediate (go);
		}

		protected abstract void populateMarkers ();

		public void UpdateView ()
		{
			if (this == null) {
				return;
			}

			// hide and delete all list elements:
			foreach (KeyValuePair<int, Marker> kvp in Markers) {
				kvp.Value.Hide ();
				kvp.Value.Destroy ();
				map.RemoveMarker (kvp.Value);
			}

			Markers.Clear ();

			populateMarkers ();
		}

		void OnApplicationQuit ()
		{
			map = null;
		}
		#endregion
	}
}
