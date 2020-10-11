using Code.GQClient.Conf;
using Code.GQClient.Model;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Util.http;
using UnityEngine;

namespace Code.GQClient.UI.map
{
    public class QuestMapController : MapController
    {
        private QuestManager _qm;


        #region Runtime API

        protected override void Start()
        {
            base.Start();

            _qm = QuestManager.Instance;

            UpdateView();
        }

        /// <summary>
        /// Cleans up, e.g. unregisters itself as listener of map position updates.
        /// </summary>
        public void CleanUp()
        {
            Debug.Log("TODO IMPLEMENTATION MISSING");
            // LocationSensor.Instance.OnLocationUpdate -= map.UpdatePosition;
        }

        #endregion


        #region Map

        protected override void locateAtStart()
        {
            // calculate center of hotspots:
            double sumLong = 0f;
            double sumLat = 0f;
            int counter = 0;
            foreach (Hotspot h in QuestManager.Instance.CurrentQuest.AllHotspots)
            {
                //				if (!h.IsVisible())  // TODO check Visibility and / or Activity of each Hotspot
                //					continue;

                sumLong += h.Longitude;
                sumLat += h.Latitude;
                counter++;
            }

            if (counter == 0)
            {
                Debug.Log("TODO IMPLEMENTATION MISSING");
                // map.CenterOnLocation();
            }
            else
            {
                Debug.Log("TODO IMPLEMENTATION MISSING");
                // map.CenterWGS84 = new double[2]
                // {
                //     sumLong / counter,
                //     sumLat / counter
                // };
            }
        }

        #endregion


        #region Markers

        protected override void populateMarkers()
        {
            var q = _qm.CurrentQuest;
            foreach (var h in q.AllHotspots)
            {
                CreateMarker(h);
            }
        }

        /// <summary>
        /// Called as update method on hotspot change events.
        /// </summary>
        /// <param name="hotspot">Hotspot.</param>
        private void UpdateMarker(Hotspot hotspot)
        {
            if (hotspot == null)
                return;

            Markers.TryGetValue(hotspot.Id, out var marker);
            if (marker == null)
                return;

            //marker.gameObject.SetActive(false);
            Debug.Log("TODO IMPLEMENTATION MISSING");
            // map.RemoveMarker(marker);

            // do unregister for hotspot change events
            hotspot.HotspotChanged -= UpdateMarker;
            CreateMarker(hotspot);
        }

        private void CreateMarker(Hotspot hotspot)
        {
            if (hotspot == null || !hotspot.Visible)
                return;

            // do register for hotspot change events
            hotspot.HotspotChanged += UpdateMarker;

            // Hotspot-specific markers:
            if (hotspot.MarkerImageUrl != HotspotMarker.SERVER_DEFAULT_MARKER_URL)
            {
                LoadHotspotMarker(hotspot, hotspot.MarkerImageUrl);
                return;
            }

            // Quest-specific markers: they are defined as metadata:
            if (_qm.CurrentQuest.metadata.ContainsKey(HotspotMarker.QUEST_SPECIFIC_MARKER_MDKEY))
            {
                _qm.CurrentQuest.metadata.TryGetValue(HotspotMarker.QUEST_SPECIFIC_MARKER_MDKEY, out var markerUrl);
                LoadHotspotMarker(hotspot, markerUrl);
                return;
            }

            // App-specific hotspot marker (defaults to the default geoquest marker):
            var markerTexture = Resources.Load<Texture2D>(ConfigurationManager.Current.hotspotMarker.path);

            Debug.Log("TODO IMPLEMENTATION MISSING");
            // ShowLoadedMarker(hotspot, markerTexture);
        }

        private static void LoadHotspotMarker(Hotspot hotspot, string markerUrl)
        {
            AbstractDownloader loader;
            if (QuestManager.Instance.MediaStore.ContainsKey(markerUrl))
            {
                QuestManager.Instance.MediaStore.TryGetValue(markerUrl, out var mediaInfo);
                loader = new LocalFileLoader(mediaInfo.LocalPath);
            }
            else
            {
                loader = new Downloader(
                    url: markerUrl,
                    timeout: ConfigurationManager.Current.timeoutMS,
                    maxIdleTime: ConfigurationManager.Current.maxIdleTimeMS
                );
            }

            loader.OnSuccess += (AbstractDownloader d, DownloadEvent e) =>
            {
                ShowLoadedMarker(hotspot, d.Www.texture);
            };
            loader.Start();
        }

        private static void ShowLoadedMarker(Hotspot hotspot, Texture texture)
        {
            Debug.Log("TODO IMPLEMENTATION MISSING");
            // var markerGo = TileBehaviour.CreateTileTemplate(TileBehaviour.AnchorPoint.BottomCenter).gameObject;

            // var newMarker = MapBehaviour.Instance.CreateMarker<HotspotMarker>(
            //     hotspot.Id.ToString(), // if ever we intorduce hotspots names in game.xml use it here.
            //     new double[2] {hotspot.Longitude, hotspot.Latitude},
            //     markerGo
            // );
            // newMarker.Hotspot = hotspot;
            // markerGo.name = "Marker tile (hotspot #" + hotspot.Id + ")";
            // calculateMarkerDetails(texture, markerGo);
            //
            // if (newMarker != null)
            //     Markers[hotspot.Id] = newMarker;
        }

        #endregion
    }
}