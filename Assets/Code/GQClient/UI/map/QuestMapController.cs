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

			// Get the category name for the given info regarding the current filter selection ...
			Renderer markerRenderer = markerGO.GetComponent<Renderer> ();
			markerRenderer.material.renderQueue = 4001;
			markerRenderer.material.mainTexture = texture;

			// scale the marker so that it fits inside the surrouding tile holder which is a square:
			float markerWidth = Math.Min (1.0f, (float)texture.width / (float)texture.height);
			float markerHeight = Math.Min (1.0f, (float)texture.height / (float)texture.width);

			markerGO.transform.localScale = 
				new Vector3 (markerWidth, 1.0f, markerHeight) * MapLayoutConfig.MarkerHeightUnits * MARKER_SCALE_FACTOR;

			markerGO.AddComponent<CameraFacingBillboard> ().Axis = Vector3.up;
			markerGO.name = "Markertile (hotspot #" + hotspot.Id + ")";
			markerGO.layer = QuestMarkerInteractions.MARKER_LAYER;
			BoxCollider markerBox = markerGO.GetComponent<BoxCollider> ();
			markerBox.center = new Vector3 (0.0f, 0.0f, 0.5f);

			if (newMarker != null)
				Markers.Add (hotspot.Id, newMarker);
			
			GameObject b = GameObject.Find ("MapCanvas/MapScreen/MapArea (invisible)/MapButtonPanel/CenteringButton");
			Image img = b.GetComponent<Image> ();
			Debug.Log(string.Format ("Resulting Color for Center Button: {0}, {1}, {2}, {3}", 
				img.color.r, img.color.g, img.color.b, img.color.a));
		}

	}
}
