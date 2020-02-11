using Code.GQClient.Conf;
using Code.GQClient.Model;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Util.http;
using Code.GQClient.Util.input;
using Code.UnitySlippyMap.Map;
using UnityEngine;

namespace Code.GQClient.UI.map
{
    public class QuestMapController : MapController
    {
        protected QuestManager qm;


        #region Runtime API

        protected override void Start()
        {
            base.Start();

            qm = QuestManager.Instance;

            UpdateView();
        }

        /// <summary>
        /// Cleans up, e.g. unregisters itself as listener of map position updates.
        /// </summary>
        public void CleanUp()
        {
            LocationSensor.Instance.OnLocationUpdate -= map.UpdatePosition;
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
                map.CenterOnLocation();
            }
            else
            {
                map.CenterWGS84 = new double[2] {
                    sumLong / counter,
                    sumLat / counter
                };
            }
        }

        #endregion


        #region Markers
        protected override void populateMarkers()
        {
            Quest q = qm.CurrentQuest;
            foreach (Hotspot h in q.AllHotspots)
            {
                CreateMarker(h);
            }
        }

        /// <summary>
        /// Called as update method on hotspot change events.
        /// </summary>
        /// <param name="hotspot">Hotspot.</param>
        public void UpdateMarker(Hotspot hotspot)
        {
            if (hotspot == null)
                return;

            Marker marker;
            Markers.TryGetValue(hotspot.Id, out marker);
            if (marker == null)
                return;

            //marker.gameObject.SetActive(false);
            map.RemoveMarker(marker);
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
                loadHotspotMarker(hotspot, hotspot.MarkerImageUrl);
                return;
            }

            // Quest-specific markers: they are defined as metadata:
            if (qm.CurrentQuest.metadata.ContainsKey(HotspotMarker.QUEST_SPECIFIC_MARKER_MDKEY))
            {
                string markerUrl;
                qm.CurrentQuest.metadata.TryGetValue(HotspotMarker.QUEST_SPECIFIC_MARKER_MDKEY, out markerUrl);
                loadHotspotMarker(hotspot, markerUrl);
                return;
            }

            // App-specific hotspot marker (defaults to the default geoqeust marker):
            Texture2D markerTexture = Resources.Load<Texture2D>(ConfigurationManager.Current.hotspotMarker.path);
            showLoadedMarker(hotspot, markerTexture);
        }

        private void loadHotspotMarker(Hotspot hotspot, string markerUrl)
        {
            AbstractDownloader loader;
            if (qm.CurrentQuest.MediaStore.ContainsKey(markerUrl))
            {
                MediaInfo mediaInfo;
                qm.CurrentQuest.MediaStore.TryGetValue(markerUrl, out mediaInfo);
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
                showLoadedMarker(hotspot, d.Www.texture);
            };
            loader.Start();
        }

        private void showLoadedMarker(Hotspot hotspot, Texture texture)
        {
            GameObject markerGO = TileBehaviour.CreateTileTemplate(TileBehaviour.AnchorPoint.BottomCenter).gameObject;

            HotspotMarker newMarker = MapBehaviour.Instance.CreateMarker<HotspotMarker>(
                hotspot.Id.ToString(), // if ever we intorduce hotspots names in game.xml use it here.
                new double[2] { hotspot.Longitude, hotspot.Latitude },
                markerGO
            );
            newMarker.Hotspot = hotspot;
            markerGO.name = "Markertile (hotspot #" + hotspot.Id + ")";
            calculateMarkerDetails(texture, markerGO);

            if (newMarker != null)
                Markers[hotspot.Id] = newMarker;
        }
        #endregion

    }
}
