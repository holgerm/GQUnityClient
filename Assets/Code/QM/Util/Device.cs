using System;
using Code.GQClient.Util;
using UnityEngine;

namespace Code.QM.Util
{
    /// <summary>
    /// Helper class allows mocking for tests.
    /// </summary>
    public static class Device
    {
        #region Screen

        public static float dpi = Screen.dpi;
        public static int height = Screen.height;

        private static int _width;

        public static int width
        {
            get
            {
                if (_width == 0)
                {
                    var root = GameObject.FindGameObjectWithTag(Tags.ROOT_CANVAS);
                    if (root == null)
                    {
                        _width = Screen.width;
                        return _width;
                    }

                    var rt = root.GetComponent<RectTransform>();
                    if (rt == null)
                    {
                        _width = Screen.width;
                        return _width;
                    }

                    if (Math.Abs(rt.localScale.x) < float.Epsilon)
                    {
                        _width = Screen.width;
                        return _width;
                    }

                    _width = Convert.ToInt32(Screen.width / rt.localScale.x);
                    return _width;
                }

                return _width;
            }
            set => _width = value;
        }

        public static Size DisplaySize
        {
            get
            {
#if UNITY_EDITOR
                return Size.Large;
#else
                return Screen.width / Screen.dpi < 4.13f ? Size.Small : Size.Large;
#endif
            }
        }

        public enum Size
        {
            Small = 0,
            Medium = 1,
            Large = 2
        }

        public static DeviceOrientation Orientation
        {
            get { return Input.deviceOrientation; }
        }

        #endregion

        #region DataPath

        private static Void2String _getPersistentDatapath = () => { return Application.persistentDataPath; };

        public static string GetPersistentDatapath()
        {
            return _getPersistentDatapath();
        }

        public static void SetPersistentDataPathMethod(Void2String method)
        {
            _getPersistentDatapath = method;
        }

        #endregion

        #region Location Service

        private static LocationServiceExt _location;

        public static LocationServiceExt location
        {
            get
            {
                if (_location == null)
                {
#if UNITY_EDITOR
                    _location = new LocationServiceExt(true);
#else
                    _location = new LocationServiceExt ();
#endif
                }

                return _location;
            }

            set { _location = value; }
        }

        //	#if UNITY_EDITOR || UNITY_STANDALONE
        //static public void awakeLocationMock() {
        //	Device.location = new LocationServiceExt(true);
        //}

        const float LOCATION_MOCK_STEP_MIN = 0.0000001f;
        const float LOCATION_MOCK_STEP_MAX = 1.0f;
        static float locationMockStep = 0.0001f;

        static public void updateMockedLocation()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                LocationInfoExt newLoc = location.lastData.clone();
                newLoc.latitude = Math.Min(location.lastData.latitude + locationMockStep, 90.0f);
                Device.location.MockedLocation = newLoc;
                return;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                LocationInfoExt newLoc = location.lastData.clone();
                newLoc.latitude = Math.Max(location.lastData.latitude - locationMockStep, -90.0f);
                Device.location.MockedLocation = newLoc;
                return;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                LocationInfoExt newLoc = location.lastData.clone();
                newLoc.longitude -= locationMockStep;
                if (newLoc.longitude < -180.0f)
                {
                    newLoc.longitude += 360.0f;
                }

                Device.location.MockedLocation = newLoc;
                return;
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                LocationInfoExt newLoc = location.lastData.clone();
                newLoc.longitude += locationMockStep;
                if (newLoc.longitude > 180.0f)
                {
                    newLoc.longitude -= 360.0f;
                }

                Device.location.MockedLocation = newLoc;
                return;
            }

            if (Input.GetKeyDown(KeyCode.Plus) && locationMockStep < LOCATION_MOCK_STEP_MAX)
            {
                locationMockStep *= 2;
                return;
            }

            if (Input.GetKeyDown(KeyCode.Minus) && LOCATION_MOCK_STEP_MIN < locationMockStep)
            {
                locationMockStep /= 2;
                return;
            }
        }
        //#endif

        #endregion
    }
}