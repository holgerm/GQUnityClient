using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;
using UnitySlippyMap.Map;
using System;


namespace GQ.Client.UI
{
	public class QuestMapController : MapController {
		protected QuestManager qm;

		protected override void Start() {
			base.Start ();

			qm = QuestManager.Instance;

			UpdateView ();
		}

		protected override void populateMarkers() {
			Quest q = qm.CurrentQuest;
			foreach (Hotspot h in q.AllHotspots) {
				Marker newMarker = CreateMarker (h);
				if (newMarker != null)
					Markers.Add (h.Id, newMarker);
			}
		}

		private Marker CreateMarker (Hotspot hotspot)
		{
			GameObject markerGO = TileBehaviour.CreateTileTemplate (TileBehaviour.AnchorPoint.BottomCenter).gameObject;

			HotspotMarker newMarker = map.CreateMarker<HotspotMarker> (
				hotspot.Id.ToString(), // if ever we intorduce hotspots names in game.xml use it here.
				new double[2] { hotspot.Longitude, hotspot.Latitude }, 
				markerGO
			);
			newMarker.Hotspot = hotspot;

			// Get the category name for the given info regarding the current filter selection ...
			Renderer markerRenderer = markerGO.GetComponent<Renderer> ();
			markerRenderer.material.mainTexture = newMarker.Texture; 
			markerRenderer.material.renderQueue = 4001;

			// scale the marker so that it fits inside the surrouding tile holder which is a square:
			float markerWidth = Math.Min (1.0f, (float)newMarker.Texture.width / (float)newMarker.Texture.height);
			float markerHeight = Math.Min (1.0f, (float)newMarker.Texture.height / (float)newMarker.Texture.width);

			markerGO.transform.localScale = 
				new Vector3 (markerWidth, 1.0f, markerHeight) * (MapLayoutConfig.MarkerHeightUnits / MARKER_SCALE_FACTOR);

			markerGO.AddComponent<CameraFacingBillboard> ().Axis = Vector3.up;
			markerGO.name = "Markertile (hotspot #" + hotspot.Id + ")";
			markerGO.layer = QuestMarkerInteractions.MARKER_LAYER;
			BoxCollider markerBox = markerGO.GetComponent<BoxCollider> ();
			markerBox.center = new Vector3 (0.0f, 0.0f, 0.5f);
			return newMarker;
		}

	}
}
