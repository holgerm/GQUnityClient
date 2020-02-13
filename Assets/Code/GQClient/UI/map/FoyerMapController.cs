using Code.GQClient.Conf;
using Code.GQClient.Err;
using GQClient.Model;
using Code.QM.Util;
using Code.UnitySlippyMap.Map;
using UnityEngine;

namespace Code.GQClient.UI.map
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
//				Debug.Log (string.Format("FMC.OnMarkerChanged: AddedInfo {0} #{1}", e.Message, QuestInfoManager.Instance.QuestDict.Count).Yellow());
//				qiCtrl = 
//					QuestMapMarkerController.Create (
//					root: InfoList.gameObject,
//					qInfo: e.NewQuestInfo,
//					containerController: this
//				).GetComponent<QuestMapMarkerController> ();
//				QuestInfoControllers.Add (e.NewQuestInfo.Id, qiCtrl);
//				qiCtrl.Show ();
				UpdateView ();
				break;
			case ChangeType.ChangedInfo:
//				Debug.Log (string.Format("FMC.OnMarkerChanged: ChangedInfo {0} #{1}", e.Message, QuestInfoManager.Instance.QuestDict.Count).Yellow());
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
//				Debug.Log (string.Format("FMC.OnMarkerChanged: RemovedInfo {0}", e.Message).Yellow());
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
//				Debug.Log (string.Format("FMC.OnMarkerChanged: ListChanged {0} #{1}", e.Message, QuestInfoManager.Instance.QuestDict.Count).Yellow());
				UpdateView ();
				break;							
			case ChangeType.FilterChanged:
//				Debug.Log (string.Format("FMC.OnMarkerChanged: FilterChanged {0}", e.Message).Yellow());
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
				if (Device.location.isEnabledByUser &&
					Device.location.status != LocationServiceStatus.Running) {
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