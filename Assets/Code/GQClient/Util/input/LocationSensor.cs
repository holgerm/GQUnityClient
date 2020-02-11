//#define DEBUG_LOG

using System;
using System.Collections;
using Code.GQClient.Err;
using Code.QM.Util;
using UnityEngine;

namespace Code.GQClient.Util.input
{

    /*
	 * Wenn sich ein Listener anmeldet, starten wir eine Coroutine (PollData), die solange in einer Schleife läuft, 
	 * wie Listener angemeldet sind. In dieser Schleife wird gewartet wie die Frequenz es vorgibt (WaitForSecondsRealtime().
	 * Dann wird ein neuer LocationInfo Wert erhoben.
	 * Wenn dieser Wert um mehr als die Minimalsdistanz vom bisher letzten Wert abweicht (oder der letzte Wert null ist)
	 * werden die Listener über ein Update mit dem neuen Wert informiert (event.Invoke();
	 * Der neue Wert wird als letzter Wert gespeichert und weiter geht es mit der Schleife.
	 * 
	 * Wird der letzte Listener abgemeldet, endet die Schleife und damit die Coroutine. Somit werden auch keine LocationInfos mehr
	 * am Sensor abgegriffen. Erst mit erneuter Anmeldung eines Listeners geht es wieder los.
	 * 
	 * Außerdem wird jeder neu angemledete Listener einzeln entweder mit einem Update oder mit der Info, 
	 * dass LocationInfo aktuell nicht verfügbar sind "begrüsst". 
	 *
	 * Evtl. kann man zusätzlich noch über ein Flag den LocationSensor von aussen stoppen und starten. 
	 * Dies würde dann zusätzlich inder Whileschleife abgefragt.
	 */

    public class LocationSensor
    {

        #region Singleton

        private static LocationSensor _instance = null;

        private LocationSensor() { }

        public static LocationSensor Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocationSensor();
                    _instance.StartPolling();
                }
                return _instance;
            }
        }

        #endregion


        #region Fields

        // How often is the location polled (in seconds)?
        private float _pollingIntervall = 2.0f;
        public float PollingInterval
        {
            get
            {
                return _pollingIntervall;
            }
            set
            {
                _pollingIntervall = value;
            }
        }

        // The minimal distance (in m) that the location has to chnage in order to trigger a LocationEvent.
        public double UpdateDistance
        {
            get;
            set;
        }

        // Timeout for starting a location service in seconds.
        public int InitializationTimeOut
        {
            get;
            set;
        } = 2;

        /// <summary>
        /// Used to count down when waiting for Intialization to finish.
        /// </summary>
        private int stillWait;

        private void StartWaitingForInitialization()
        {
            stillWait = InitializationTimeOut;
        }

        private bool WaitingForInitialization
        {
            get
            {
                return (Device.location.status == LocationServiceStatus.Initializing && stillWait > 0);
            }
        }

        private void WaitedForInitialization()
        {
            stillWait--;
        }

        public static readonly LocationInfoExt NullLocationInfo = new LocationInfoExt();

        /// <summary>
        /// We remember the last location received from the location service and initialize this with a Null object.
        /// </summary>
        private LocationInfoExt lastLocation = NullLocationInfo;

        private bool _activated = true;
        public bool Activated
        {
            get
            {
                return _activated;
            }
            private set
            {
                _activated = value;
            }
        }

        public void Activate()
        {
            Activated = true;
        }

        public void Deactivate()
        {
            Activated = false;
        }

        #endregion


        #region Events

        public delegate void LocationUpdateCallback(object sender, LocationEventArgs e);

        public class LocationEventArgs : EventArgs
        {
            public LocationEventType Kind { get; set; }

            public LocationInfoExt Location { get; set; }

            public LocationEventArgs(
                LocationEventType kind,
                LocationInfoExt location)
            {
                Kind = kind;
                Location = location;
            }
        }

        public enum LocationEventType
        {
            Update,
            NotAvailable,
        }

        private event LocationUpdateCallback _onLocationUpdate;
        public event LocationUpdateCallback OnLocationUpdate
        {
            add
            {
                if (!ListenersAttached)
                {
                    _onLocationUpdate += value;
                    StartPolling();
                }
                else
                {
                    _onLocationUpdate += value;
                }
            }

            remove
            {
                _onLocationUpdate -= value;
            }
        }

        #endregion


        #region Runtime

        private bool currentlyPolling = false;

        private void StartPolling()
        {
#if DEBUG_LOG
            Debug.Log("LocationSensor starts polling data");
#endif

            if (!currentlyPolling)
                Base.Instance.StartCoroutine(PollData());
        }

        private IEnumerator PollData()
        {
            try
            {
#if DEBUG_LOG
                Debug.Log("LocationSensor is polling data... mocked?: " + Device.location.useMockLocation);
#endif
                currentlyPolling = true;
                bool failed = false;

                Device.location.Stop();

                while (Activated && ListenersAttached)
                {
#if DEBUG_LOG
                    Debug.Log("LocationSensor has listeners attached and is activated.");
#endif
                    switch (Device.location.status)
                    {
                        case LocationServiceStatus.Running:
#if DEBUG_LOG
                            Debug.Log("LocationSensor is running.");
#endif
                            LocationInfoExt newLocation = Device.location.lastData;
                            if (failed || !lastLocation.WithinDistance(UpdateDistance, newLocation))
                            {
                                _onLocationUpdate(this, new LocationEventArgs(LocationEventType.Update, Device.location.lastData));
                                failed = false;
                            }
                            lastLocation = newLocation;
                            break;
                        case LocationServiceStatus.Stopped:
#if DEBUG_LOG
                            Debug.Log("LocationSensor is stopped.");
                            Debug.Log("Device.location.isEnabledByUser: " + Device.location.isEnabledByUser);
                            Debug.Log("Input.location.isEnabledByUser: " + Input.location.isEnabledByUser);
#endif
                            if (Device.location.isEnabledByUser)
                            {
                                Device.location.Start(1f, 1f);
                            }
                            else
                            {
                                if (!failed)
                                {
                                    _onLocationUpdate(
                                        this,
                                        new LocationEventArgs(
                                            LocationEventType.NotAvailable,
                                            NullLocationInfo
                                        )
                                    );
                                    failed = true;
                                }
                            }
                            break;
                        case LocationServiceStatus.Initializing:
                            StartWaitingForInitialization();
#if DEBUG_LOG
                            Debug.Log("LocationSensor is initializing. To wait: " + stillWait);
#endif
                            do
                            {
                                yield return new WaitForSeconds(1f);
                                WaitedForInitialization();
                            } while (WaitingForInitialization);
                            //						continue;
                            break;
                        case LocationServiceStatus.Failed:
#if DEBUG_LOG
                            Debug.Log("LocationSensor is failed.");
#endif
                            if (!failed)
                            {
                                _onLocationUpdate(
                                    this,
                                    new LocationEventArgs(
                                        LocationEventType.NotAvailable,
                                        NullLocationInfo
                                    )
                                );
                                failed = true;
                            }
                            break;
                        default:
                            Log.SignalErrorToDeveloper("LocationService in unknown state {0}.", Device.location.status.ToString());
                            break;
                    }

#if DEBUG_LOG
                    Debug.LogFormat("LocationSensor polling again after {0}s.", PollingInterval);
#endif
                    yield return new WaitForSecondsRealtime(PollingInterval);
                }
            }
            finally
            {
                currentlyPolling = false;
            }
        }

        private bool ListenersAttached
        {
            get
            {
                return
                    (
                        _onLocationUpdate != null &&
                        _onLocationUpdate.GetInvocationList().Length > 0
                    );
            }
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Distance between the two given geopoints in Meters.
        /// </summary>
        /// <param name="lat1">Lat1.</param>
        /// <param name="lon1">Lon1.</param>
        /// <param name="lat2">Lat2.</param>
        /// <param name="lon2">Lon2.</param>
        public static double distance(double lat1, double lon1, double lat2, double lon2)
        {

            double theta = lon1 - lon2;

            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515 * 1609;

            return (dist);
        }

        private static double deg2rad(double deg)
        {

            return (deg * Math.PI / 180.0);
        }

        private static double rad2deg(double rad)
        {

            return (rad / Math.PI * 180.0);
        }

        #endregion

    }

    public static class LocationExtensions
    {

        public static bool WithinDistance(this LocationInfoExt thisLoc, double distance, LocationInfoExt otherLoc)
        {
            // TODO calculate weather the distance is larger than UpdateDistance
            return false;
        }
    }

}