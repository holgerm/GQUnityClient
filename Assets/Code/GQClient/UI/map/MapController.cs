// #define DEBUG_LOG

using System;
using System.Collections;
using System.Collections.Generic;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.UI.layout;
using Code.GQClient.Util;
using Code.GQClient.Util.input;
using Code.QM.Util;
using Code.UnitySlippyMap.Helpers;
using Code.UnitySlippyMap.Layers;
using Code.UnitySlippyMap.Map;
using Code.UnitySlippyMap.Markers;
using GQClient.Model;
using UnityEngine;

namespace Code.GQClient.UI.map
{

    public abstract class MapController : MonoBehaviour
	{
		private List<LayerBehaviour> layers;
		public MapBehaviour map;

		public OnlineMapsMarkerManager markerManager;
		
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

		private static float MARKER_SCALE_FACTOR {
			get {
				// We empirically found this to be close to a correct scaling factor in order to resize the markers according to 
				// UI elements like buttons etc.:
				return (Device.height / 800000f);
			}
		}


		static protected void calculateMarkerDetails (Texture texture, GameObject markerGO)
		{
			// Get the category name for the given info regarding the current filter selection ...
			Renderer markerRenderer = markerGO.GetComponent<Renderer> ();
			markerRenderer.material.renderQueue = 4001;
			markerRenderer.material.mainTexture = texture;
			// scale the marker so that it fits inside the surrouding tile holder which is a square:
			float markerWidth = LayoutConfig.Units2Pixels (Math.Min (1.0f, (float)texture.width / (float)texture.height));
			float markerHeight = LayoutConfig.Units2Pixels (Math.Min (1.0f, (float)texture.height / (float)texture.width));
			markerGO.transform.localScale = 
				new Vector3 (markerWidth, 1.0f, markerHeight) * (FoyerMapScreenLayout.MarkerHeightUnits * MARKER_SCALE_FACTOR) / 
				ConfigurationManager.Current.mapScale;
			markerGO.AddComponent<CameraFacingBillboard> ().Axis = Vector3.up;
			markerGO.layer = QuestMarkerInteractions.MARKER_LAYER;
			BoxCollider markerBox = markerGO.GetComponent<BoxCollider> ();
			markerBox.center = new Vector3 (0.0f, 0.0f, 0.5f);
		}

		private static bool _ignoreInteraction;

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


		public void ZoomInButtonPressed ()
		{
			map.CurrentZoom = Math.Min (map.CurrentZoom + ConfigurationManager.Current.mapDeltaZoom, map.MaxZoom);
			map.Zoom (0f);
		}

		public void ZoomOutButtonPressed ()
		{
            map.CurrentZoom = Math.Max (map.CurrentZoom - ConfigurationManager.Current.mapDeltaZoom, map.MinZoom);
            map.Zoom (0f);
		}

		internal void UpdateZoomButtons ()
		{
			// If further zooming IN is not possible disable ZoomInButton: 
			zoomInButton.Enabled = (map.MaxZoom > map.CurrentZoom);

			// If further zooming OUT is not possible disable ZoomOutButton: 
			zoomOutButton.Enabled = (map.MinZoom < map.CurrentZoom);
		}

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
					_mapBoxLayer.BaseURL = "https://api.tiles.mapbox.com/v4/" + ConfigurationManager.Current.mapID + "/";
					_mapBoxLayer.TileImageExtension = "@2x.png?access_token=" + ConfigurationManager.Current.mapKey;
				}
				return _mapBoxLayer;
			}
		}

		OverlayButtonLayoutConfig zoomInButton;
		OverlayButtonLayoutConfig zoomOutButton;

		protected virtual void Start ()
		{
#if DEBUG_LOG
			WATCH w = new WATCH("zoom", true);
#endif
			// create the map singleton
			map = MapBehaviour.Instance;
			map.CurrentCamera = Camera.main;
			map.CurrentZoom = 15.0f; // TODO remove in case we are within a quest
			map.mapCtrl = this;

			Frame ();
			GameObject zibGo = MapButtonPanel.transform.Find ("ZoomInButton").gameObject;
			zoomInButton = zibGo.GetComponent<OverlayButtonLayoutConfig> ();
			zoomOutButton = MapButtonPanel.transform.Find ("ZoomOutButton").GetComponent<OverlayButtonLayoutConfig> ();

			LocationSensor.Instance.OnLocationUpdate += map.UpdatePosition;
#if DEBUG_LOG
            Debug.Log("#### Location Update Listener added");
#endif
            map.InputsEnabled = true;

			locateAtStart ();

			layers = new List<LayerBehaviour> ();
			layers.Add (MapLayer);

			// create the location marker
			GameObject go = TileBehaviour.CreateTileTemplate ().gameObject;
			go.GetComponent<Renderer> ().material.mainTexture = LocationTexture;
			go.GetComponent<Renderer> ().material.renderQueue = 4002;
			go.transform.localScale /= (ConfigurationManager.Current.mapScale * 4f) ; 

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
			foreach (var kvp in Markers) {
				// TODO CLARIFY WHY THIS CONTINUE IS NECESSARY:
				if (kvp.Value == null)
					continue;
				
				kvp.Value.Hide ();
				// remove marker update as listener to questInfo Changed Events:
				QuestInfoManager.Instance.GetQuestInfo(kvp.Key).OnChanged -= kvp.Value.UpdateView;
				map.RemoveMarker (kvp.Value);
			}

			Markers.Clear ();

			populateMarkers ();
		}

        void OnApplicationQuit ()
		{
			map = null;
		}
	}
}
