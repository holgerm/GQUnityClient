using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;
using UnitySlippyMap.Map;
using System;
using GQ.Client.Util;
using GQ.Client.Conf;
using UnityEngine.UI;


namespace GQ.Client.UI
{
	public class QuestMapController : MapController {
		protected QuestManager qm;

		protected override void Start() {
			base.Start ();

			qm = QuestManager.Instance;

			UpdateView ();
		}

		#region Map

		protected override void locateAtStart ()
		{
			// calculate center of hotspots:
			double sumLong = 0f;
			double sumLat = 0f;
			int counter = 0;
			foreach (Hotspot h in QuestManager.Instance.CurrentQuest.AllHotspots) {
//				if (!h.IsVisible())  // TODO check Visibility and / or Activity of each Hotspot
//					continue;

				sumLong += h.Longitude;
				sumLat += h.Latitude;
				counter++;
			}
			if (counter == 0) {
				map.CenterOnLocation ();
			}
			else {
				map.CenterWGS84 = new double[2] {
					sumLong / counter,
					sumLat / counter
				};
			}
		}

		#endregion


		#region Markers
		protected override void populateMarkers() {
			Quest q = qm.CurrentQuest;
			foreach (Hotspot h in q.AllHotspots) {
				CreateMarker (h);
			}
		}

		private void CreateMarker (Hotspot hotspot)
		{
			AbstractDownloader loader;
			if (qm.CurrentQuest.MediaStore.ContainsKey (hotspot.MarkerImageUrl)) {
				MediaInfo mediaInfo;
				qm.CurrentQuest.MediaStore.TryGetValue (hotspot.MarkerImageUrl, out mediaInfo);
				loader = new LocalFileLoader (mediaInfo.LocalPath);
			} else {
				loader = new Downloader (
					url: hotspot.MarkerImageUrl, 
					timeout: ConfigurationManager.Current.timeoutMS,
					maxIdleTime: ConfigurationManager.Current.maxIdleTimeMS
				);
			}
			loader.OnSuccess += (AbstractDownloader d, DownloadEvent e) => {
				showLoadedMarker(hotspot, d.Www.texture);
			};
			loader.Start ();
		}

		private void showLoadedMarker(Hotspot hotspot, Texture texture) {
			GameObject markerGO = TileBehaviour.CreateTileTemplate (TileBehaviour.AnchorPoint.BottomCenter).gameObject;

			HotspotMarker newMarker = map.CreateMarker<HotspotMarker> (
				hotspot.Id.ToString(), // if ever we intorduce hotspots names in game.xml use it here.
				new double[2] { hotspot.Longitude, hotspot.Latitude }, 
				markerGO
			);
			newMarker.Hotspot = hotspot;
			markerGO.name = "Markertile (hotspot #" + hotspot.Id + ")";
			calculateMarkerDetails (texture, markerGO);

			if (newMarker != null)
				Markers.Add (hotspot.Id, newMarker);
		}
		#endregion
	}
}
