using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.GQClient.Conf;
using Code.GQClient.UI.Dialogs;
using Code.GQClient.UI.layout;
using Code.GQClient.Util;
using Code.QM.Util;
using GQClient.Model;
using UnityEngine;

namespace Code.GQClient.UI.map
{
    public abstract class MapController : MonoBehaviour
    {
        public OnlineMapsMarkerManager markerManager;
        public OnlineMaps map;

        private static Dictionary<int, Marker> markers;

        public float maxZoomLevel = 20f;
        public float minZoomLevel = 0f;

        /// <summary>
        /// Marker dictionary is static to support the singleton MapBehaviour from slippy maps well. 
        /// When maps change all markers must be removed from the MapBehaviour as well as from this dictionary.
        /// </summary>
        /// <value>The markers.</value>
        protected static Dictionary<int, Marker> Markers
        {
            get
            {
                if (markers == null)
                {
                    markers = new Dictionary<int, Marker>();
                }

                return markers;
            }
        }

        private static float MARKER_SCALE_FACTOR
        {
            get
            {
                // We empirically found this to be close to a correct scaling factor in order to resize the markers according to 
                // UI elements like buttons etc.:
                return (Device.height / 800000f);
            }
        }

        private static bool _ignoreInteraction;

        public static bool IgnoreInteraction
        {
            get { return _ignoreInteraction; }
            set
            {
                if (value == true)
                {
                    _ignoreInteraction = true;
                }
                else
                {
                    Base.Instance.StartCoroutine(_setIgnoreInteractionToFalseAsCoroutine());
                }
            }
        }

        private static IEnumerator _setIgnoreInteractionToFalseAsCoroutine()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            _ignoreInteraction = false;
            yield break;
        }


        public GameObject MapButtonPanel;
        public Texture CenterTexture;
        public Texture FrameTexture;
        public Texture LocationTexture;

        #region Center

        public enum Centering
        {
            Centered,
            Framed,
            Manual
        }

        public Centering CenteringState { get; protected set; }

        public void Frame()
        {
            // center the map so it frames all currently visible markers: TODO

            // let the center button show the centering button icon now, unless local position is not available, in that case show the frame icon and disbale it.

            CenteringState = Centering.Framed;
        }

        private OnlineMapsLocationService _locService = null;

        private OnlineMapsLocationService locService
        {
            get
            {
                if (_locService == null)
                {
                    _locService = map.GetComponent<OnlineMapsLocationService>();
                }

                return _locService;
            }
        }

        private bool _isInitialized = false;

        private bool IsInitialized
        {
            get => _isInitialized || !Vector2.zero.Equals(locService.position);
            set => _isInitialized = value;
        }

        public void Center()
        {
            if (Vector2.zero.Equals(locService.position))
            {
                var dialog =
                    new MessageDialog("Die Lokalisierung funktioniert nicht - ist das GPS inaktiv?", "Ok");
                dialog.Start();
                return;
            }

            // center the map so it is centered to the current users position:
            if (IsInitialized)
                map.SetPosition(locService.position.x, locService.position.y);

            // let the center button show the centering button icon now
            CenteringState = Centering.Centered;
        }

        public void CenterButtonPressed()
        {
            Center();
        }

        #endregion


        #region Zoom

        OverlayButtonLayoutConfig zoomInButton;
        OverlayButtonLayoutConfig zoomOutButton;

        public float zoomDeltaFactor = 1.03f;

        public void ZoomIn()
        {
            map.floatZoom *= zoomDeltaFactor;
            map.Redraw();
        }

        public void ZoomOut()
        {
            map.floatZoom /= zoomDeltaFactor;
            map.Redraw();
        }

        #endregion

        private void OnEnable()
        {
            map = Base.Instance.Map;
            Base.Instance.Map.gameObject.SetActive(true);

            markerManager = map.GetComponent<OnlineMapsMarkerManager>();
            locService.OnLocationInited += () => { IsInitialized = true; };

            UpdateView();
        }

        protected void locateAtStart()
        {
            switch (ConfigurationManager.Current.mapStartPositionType)
            {
                case MapStartPositionType.CenterOfMarkers:
                    // calculate center of markers / quests:
                    SetLocationToMiddleOfHotspots();
                    break;
                case MapStartPositionType.FixedPosition:
                    map.SetPositionAndZoom(
                        ConfigurationManager.Current.mapStartAtLongitude,
                        ConfigurationManager.Current.mapStartAtLatitude,
                        ConfigurationManager.Current.mapStartZoom);
                    break;
                case MapStartPositionType.PlayerPosition:
                    if (null == locService || !IsInitialized || Vector2.zero.Equals(locService.position))
                    {
                        var dialog =
                            new MessageDialog("Die Lokalisierung funktioniert nicht - ist das GPS inaktiv?", "Ok");
                        dialog.Start();
                        map.SetPositionAndZoom(
                            ConfigurationManager.Current.mapStartAtLongitude,
                            ConfigurationManager.Current.mapStartAtLatitude,
                            ConfigurationManager.Current.mapStartZoom);
                    }
                    else
                    {
                        map.SetPositionAndZoom(
                            locService.position.x,
                            locService.position.y,
                            ConfigurationManager.Current.mapStartZoom);
                    }

                    break;
            }
        }

        protected abstract void SetLocationToMiddleOfHotspots();

        protected abstract void populateMarkers();

        private static bool _alreadyLocatedAtStart = false;

        public void UpdateView()
        {
            if (this == null)
            {
                return;
            }
            
            // hide and delete all list elements:
            foreach (var kvp in Markers)
            {
                if (kvp.Value == null)
                    continue;

                kvp.Value.Hide();
                // remove marker update as listener to questInfo Changed Events:
                QuestInfo qi = QuestInfoManager.Instance.GetQuestInfo(kvp.Key);
                if (null != qi)
                {
                    qi.OnChanged -= kvp.Value.UpdateView;
                }
            }

            foreach (var marker in markerManager.items.ToList())
            {
                if (marker == OnlineMapsLocationServiceBase.marker)
                {
                    continue;
                }

                markerManager.Remove(marker);
            }

            Markers.Clear();

            populateMarkers();

            if (!_alreadyLocatedAtStart)
            {
                _alreadyLocatedAtStart = true;

                locateAtStart();
            }
        }
    }
}