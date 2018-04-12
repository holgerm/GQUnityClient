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
	public class FoyerMapController : MapController
	{
		#region Initialize
		protected QuestInfoManager qim;

		protected override void Start ()
		{
			// set up the inherited Map features:
			base.Start ();

			// at last we register for changes on quest infos with the quest info manager:
			qim = QuestInfoManager.Instance;
			qim.OnDataChange += OnMarkerChanged;
			qim.OnFilterChange += OnMarkerChanged;
		}
		#endregion

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

		protected override void populateMarkers() {
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

			markerGO.transform.localScale = 
				new Vector3 (markerWidth, 1.0f, markerHeight) * MapLayoutConfig.MarkerHeightUnits * MARKER_SCALE_FACTOR;

			markerGO.AddComponent<CameraFacingBillboard> ().Axis = Vector3.up;
			markerGO.name = "Markertile (" + info.Name + ")";
			markerGO.layer = QuestMarkerInteractions.MARKER_LAYER;
			BoxCollider markerBox = markerGO.GetComponent<BoxCollider> ();
			markerBox.center = new Vector3 (0.0f, 0.0f, 0.5f);
			return newMarker;
		}

		public Texture MarkerSymbolTexture;
		#endregion
	}
}