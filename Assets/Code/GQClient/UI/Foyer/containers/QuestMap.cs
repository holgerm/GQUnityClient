using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using System.Collections.Generic;
using System;
using GQ.Client.UI.Dialogs;
using GQ.Client.Util;
using GQ.Client.Conf;
using UnityEngine.UI;
using GQ.Client.Err;
using UnitySlippyMap.Map;
using UnitySlippyMap.Layers;
using UnitySlippyMap.Markers;
using QM.Util;

namespace GQ.Client.UI.Foyer
{

	/// <summary>
	/// Shows all Quest Info objects, on a map within the foyer. Refreshing its content silently (no dialogs shown etc.).
	/// </summary>
	public class QuestMap : Map
	{

		#region React on Events

		public void OnMarkerChanged (object sender, QuestInfoChangedEvent e)
		{
			Marker m;
			switch (e.ChangeType) {
			case ChangeType.AddedInfo:
//				qiCtrl = 
//					QuestMapMarkerController.Create (
//					root: InfoList.gameObject,
//					qInfo: e.NewQuestInfo,
//					containerController: this
//				).GetComponent<QuestMapMarkerController> ();
//				QuestInfoControllers.Add (e.NewQuestInfo.Id, qiCtrl);
//				qiCtrl.Show ();
				break;
			case ChangeType.ChangedInfo:
				if (!Markers.TryGetValue (e.OldQuestInfo.Id, out m)) {
					Log.SignalErrorToDeveloper (
						"Quest Info Controller for quest id {0} not found when a Change event occurred.",
						e.OldQuestInfo.Id
					);
					break;
				}
				m.UpdateView ();
				m.Show ();
				break;
			case ChangeType.RemovedInfo:
				if (!Markers.TryGetValue (e.OldQuestInfo.Id, out m)) {
					Log.SignalErrorToDeveloper (
						"Quest Info Controller for quest id {0} not found when a Remove event occurred.",
						e.OldQuestInfo.Id
					);
					break;
				}
				m.Hide ();
				Markers.Remove (e.OldQuestInfo.Id);
				break;							
			case ChangeType.ListChanged:
				UpdateView ();
				break;							
			case ChangeType.FilterChanged:
				UpdateView ();
				break;							
			}
		}

		public void UpdateView ()
		{
			if (this == null) {
				Debug.Log ("QuestMap is null".Red ());
				return;
			}

			// hide and delete all list elements:
			foreach (KeyValuePair<int, Marker> kvp in Markers) {
				kvp.Value.Hide ();
				kvp.Value.Destroy ();
				map.RemoveMarker (kvp.Value);
			}

			Markers.Clear ();

			foreach (QuestInfo info in QuestInfoManager.Instance.GetFilteredQuestInfos()) {
				// create new list elements
				Marker newMarker = CreateMarker (info);
				if (newMarker != null)
					Markers.Add (info.Id, newMarker);
			}
		}

		private Marker CreateMarker (QuestInfo info)
		{
			if (info.MarkerHotspot.Equals (HotspotInfo.NULL)) {
				return null;
			}

			GameObject markerGO = TileBehaviour.CreateTileTemplate (TileBehaviour.AnchorPoint.BottomCenter).gameObject;

			QuestMarker newMarker = map.CreateMarker<QuestMarker> (
				                        info.Name, 
				                        new double[2] { info.MarkerHotspot.Longitude, info.MarkerHotspot.Latitude }, 
				                        markerGO
			                        );
			newMarker.Data = info;

			// Get the category name for the given info regarding the current filter selection ...
			Renderer markerRenderer = markerGO.GetComponent<Renderer> ();
			markerRenderer.material.mainTexture = newMarker.Texture; 
			markerRenderer.material.renderQueue = 4001;

			// scale the marker so that it fits inside the surrouding tile holder which is a square:
			float markerWidth = Math.Min (1.0f, (float)newMarker.Texture.width / (float)newMarker.Texture.height);
			float markerHeight = Math.Min (1.0f, (float)newMarker.Texture.height / (float)newMarker.Texture.width);
			int longScreenSide = Math.Max (Screen.width, Screen.height);
			float longMarkerSide = Math.Max (markerRenderer.bounds.size.z, markerRenderer.bounds.size.x);
//			Debug.Log ("MARKER: long Screen: " + longScreenSide + " long marker side: " + longMarkerSide);

			markerGO.transform.localScale = new Vector3 (markerWidth, 1.0f, markerHeight) *
			(ConfigurationManager.Current.markerScale / 5.0f);

			markerGO.AddComponent<CameraFacingBillboard> ().Axis = Vector3.up;
			markerGO.name = "Markertile (" + info.Name + ")";
			markerGO.layer = QuestMarkerInteractions.MARKER_LAYER;
			BoxCollider markerBox = markerGO.GetComponent<BoxCollider> ();
			markerBox.center = new Vector3 (0.0f, 0.0f, 0.5f);
			return newMarker;
		}

		public Texture MarkerSymbolTexture;

		#endregion


		#region Map (SlippyMaps)

		private bool isPerspectiveView = false;
		private float	perspectiveAngle = 30.0f;
		private float	destinationAngle = 0.0f;
		private float	currentAngle = 0.0f;
		private float	animationDuration = 0.5f;
		private float	animationStartTime = 0.0f;

		private List<LayerBehaviour> layers;
		private int currentLayerIndex = 0;

		private string utfGridJsonString = "";

		protected QuestInfoManager qim;

		protected override void Start ()
		{
			// set up the inherited Map features:
			base.Start ();

			// create the map singleton
			map = MapBehaviour.Instance;
			map.CurrentCamera = Camera.main;
			map.InputDelegate += UnitySlippyMap.Input.MapInput.BasicTouchAndKeyboard;
			map.CurrentZoom = 15.0f;
			// 9 rue Gentil, Lyon
			map.CenterWGS84 = new double[2] { 7.0090314, 50.9603868 };

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

			layers = new List<LayerBehaviour> ();

			layers.Add (MapLayer);

			// create the location marker
			GameObject go = TileBehaviour.CreateTileTemplate ().gameObject;
			go.GetComponent<Renderer> ().material.mainTexture = LocationTexture;
			go.GetComponent<Renderer> ().material.renderQueue = 4000;
			go.transform.localScale /= 27.0f;

			GameObject markerGO = Instantiate (go) as GameObject;
			map.SetLocationMarker<LocationMarkerBehaviour> (markerGO);

			DestroyImmediate (go);

			// at last we register for changes on quest infos with the quest info manager:
			qim = QuestInfoManager.Instance;
			qim.OnDataChange += OnMarkerChanged;
			qim.OnFilterChange += OnMarkerChanged;
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

		void OnApplicationQuit ()
		{
			map = null;
		}

		void Update ()
		{
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

		#endregion
	}
}