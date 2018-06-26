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
using QM.Util;
using GQ.Client.Model;

namespace GQ.Client.UI
{

	public abstract class MapController : MonoBehaviour
	{

		private List<LayerBehaviour> layers;
		public MapBehaviour map;

		#region Markers

		private static Dictionary<int, Marker> markers;

		/// <summary>
		/// Marker dictionary is static to support the singleton MapBehaviour from slippy maps well. 
		/// When maps change all markers must be removed from the MapBehaviour as well as from this dictionary.
		/// </summary>
		/// <value>The markers.</value>
		protected static Dictionary<int, Marker> Markers {
			get {
				if (markers == null) {
					markers = new Dictionary<int, Marker> ();
				}
				return markers;
			}
		}

		public static float MARKER_SCALE_FACTOR {
			get {
				// We empirically found this to be close to a correct scaling factor in order to resize the markers according to 
				// UI elements like buttons etc.:
				return (Device.height / 800000f);
			}
		}

		#endregion


		#region Global static behaviour

		static protected void calculateMarkerDetails (Texture texture, GameObject markerGO)
		{
			// Get the category name for the given info regarding the current filter selection ...
			Renderer markerRenderer = markerGO.GetComponent<Renderer> ();
			markerRenderer.material.renderQueue = 4001;
			markerRenderer.material.mainTexture = texture;
			// scale the marker so that it fits inside the surrouding tile holder which is a square:
			float markerWidth = LayoutConfig.Units2Pixels (Math.Min (1.0f, (float)texture.width / (float)texture.height));
			float markerHeight = LayoutConfig.Units2Pixels (Math.Min (1.0f, (float)texture.height / (float)texture.width));
			markerGO.transform.localScale = new Vector3 (markerWidth, 1.0f, markerHeight) * FoyerMapScreenLayout.MarkerHeightUnits * MARKER_SCALE_FACTOR;
			markerGO.AddComponent<CameraFacingBillboard> ().Axis = Vector3.up;
			markerGO.layer = QuestMarkerInteractions.MARKER_LAYER;
			BoxCollider markerBox = markerGO.GetComponent<BoxCollider> ();
			markerBox.center = new Vector3 (0.0f, 0.0f, 0.5f);
		}

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

		private OSMTileLayer _osmMapLayer;

		private LayerBehaviour OsmMapLayer {
			get {
				if (_osmMapLayer == null) {
					_osmMapLayer = map.CreateLayer<OSMTileLayer> (ConfigurationManager.Current.mapProvider.ToString ());
					_osmMapLayer.BaseURL = ConfigurationManager.Current.mapBaseUrl + "/";
				}
				return _osmMapLayer;
			}
		}

		private OSMTileLayer _mapBoxLayer;

		private LayerBehaviour MapBoxLayer {
			get {
				if (_mapBoxLayer == null) {
					_mapBoxLayer = map.CreateLayer<OSMTileLayer> (ConfigurationManager.Current.mapProvider.ToString ());
					_mapBoxLayer.BaseURL = "http://api.tiles.mapbox.com/v4/" + ConfigurationManager.Current.mapID + "/";
					_mapBoxLayer.TileImageExtension = "@2x.png?access_token=" + ConfigurationManager.Current.mapKey;
				}
				return _mapBoxLayer;
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
//					Debug.Log (
//						string.Format ("--- LOC: {0}, {1}", e.Location.longitude, e.Location.latitude)
//						.Yellow ()
//					);
				}
				if (e.Kind == LocationSensor.LocationEventType.NotAvailable) {
//					Debug.Log (("--- LOC: Unavailable. enabled: " + Input.location.isEnabledByUser).Yellow ());
				}
			};


			LocationSensor.Instance.OnLocationUpdate += map.UpdatePosition;
//			map.UsesLocation = true;
			map.InputsEnabled = true;
			map.ShowsGUIControls = false;

			locateAtStart ();

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

		protected abstract void locateAtStart ();

		protected abstract void populateMarkers ();

		public void UpdateView ()
		{
			if (this == null) {
				return;
			}

			// hide and delete all list elements:
			foreach (KeyValuePair<int, Marker> kvp in Markers) {
				// TODO CLARIFY WHY THIS CONTINUE IS NECESSARY:
				if (kvp.Value == null)
					continue;
				
				kvp.Value.Hide ();
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
