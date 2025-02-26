﻿using System.Collections.Generic;
using UnityEngine;

namespace Code.GQClient.UI.map
{
    public class LocationMarkerOnTop : MonoBehaviour
    {
        /// <summary>
        /// Defines a new comparer.
        /// </summary>
        public class MarkerComparerLocationAboveAll : IComparer<OnlineMapsMarker>
        {
            public int Compare(OnlineMapsMarker m1, OnlineMapsMarker m2)
            {
                if (m1 == m2)
                {
                    return 0;
                }

                if (m1 == null)
                {
                    return -1;
                }

                if (m2 == null)
                {
                    return 1;
                }
                
                if (m1 == OnlineMapsLocationServiceBase.marker)
                {
                    return 1;
                }

                if (m2 == OnlineMapsLocationServiceBase.marker)
                {
                    return -1;
                }
                
                if (m1.position.y > m2.position.y) {   
                    return -1;
                }

                if (m1.position.y < m2.position.y)
                {
                    return 1;
                }
                
                return 0;
            }
        }

        private void Start()
        {
            // OnlineMaps map = OnlineMaps.instance;

            // Sets a the comparer that shows location marker on top of all.
            OnlineMapsMarkerFlatDrawer drawer = (OnlineMapsTileSetControl.instance.markerDrawer as OnlineMapsMarkerFlatDrawer);
            if (drawer != null) drawer.markerComparer = new MarkerComparerLocationAboveAll();
        }
    }
}
