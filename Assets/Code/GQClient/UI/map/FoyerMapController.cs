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

		#endregion

		#region Map & Markers

		protected override void populateMarkers ()
		{
			foreach (QuestInfo info in QuestInfoManager.Instance.GetFilteredQuestInfos()) {
				// create new list elements
				CreateMarker (info);
			}
		}

		private void CreateMarker (QuestInfo info)
		{
			if (info.MarkerHotspot.Equals (HotspotInfo.NULL)) {
				return;
			}

			GameObject markerGO = TileBehaviour.CreateTileTemplate (TileBehaviour.AnchorPoint.BottomCenter).gameObject;

			QuestMarker newMarker = map.CreateMarker<QuestMarker> (
				                        info.Name, 
				                        new double[2] { info.MarkerHotspot.Longitude, info.MarkerHotspot.Latitude }, 
				                        markerGO
			                        );
			if (newMarker == null)
				return;
			
			newMarker.Data = info;
			markerGO.name = "Markertile (" + info.Name + ")";

			calculateMarkerDetails (newMarker.Texture, markerGO);

			Markers.Add (info.Id, newMarker);
		}

		public Texture MarkerSymbolTexture;

		protected override void locateAtStart ()
		{
			switch (ConfigurationManager.Current.mapStartPositionType) {
			case MapStartPositionType.CenterOfMarkers:
				// calculate center of markers / quests:
				double sumLong = 0f;
				double sumLat = 0f;
				int counter = 0;
				foreach (QuestInfo qi in QuestInfoManager.Instance.GetListOfQuestInfos()) {
					HotspotInfo hi = qi.MarkerHotspot;
					if (hi == HotspotInfo.NULL)
						continue;

					sumLong += hi.Longitude;
					sumLat += hi.Latitude;
					counter++;
				}
				if (counter == 0) {
					locateAtFixedConfiguredPosition ();
				}
				else {
					map.CenterWGS84 = new double[2] {
						sumLong / counter,
						sumLat / counter
					};
				}
				break;
			case MapStartPositionType.FixedPosition:
				locateAtFixedConfiguredPosition();
				break;
			case MapStartPositionType.PlayerPosition:
				if (Input.location.isEnabledByUser &&
					Input.location.status != LocationServiceStatus.Running) {
					map.CenterOnLocation ();
				} else {
					locateAtFixedConfiguredPosition();
				}
				break;
			}
		}

		private void locateAtFixedConfiguredPosition() {
			map.CenterWGS84 = new double[2] {
				ConfigurationManager.Current.mapStartAtLongitude,
				ConfigurationManager.Current.mapStartAtLatitude
			};
		}



		#endregion
	}
}