using UnityEngine;
using System.Collections;
using System.Globalization;
using System.Threading;
using GQ.Client.Err;
using GQ.Client.UI.Dialogs;
using UnityEngine.SceneManagement;
using GQ.Client.Conf;
using GQ.Client.Model;
using QM.Util;
using GQ.Client.FileIO;
using System;

namespace GQ.Client.Util
{

	/// <summary>
	/// Helper class allows mocking for tests.
	/// </summary>
	public class Device
	{
		#region Screen
		public static float dpi = Screen.dpi;
		public static int height = Screen.height; 
		public static int width = Screen.width; 
		#endregion

		#region DataPath
		private static Void2String _getPersistentDatapath = () => {
			return Application.persistentDataPath;
		};

		public static string GetPersistentDatapath() {
			return _getPersistentDatapath();
		}

		public static void SetPersistentDataPathMethod(Void2String method) {
			_getPersistentDatapath = method;
		}
		#endregion

		#region Location Service
		private static LocationServiceExt _location;
		public static LocationServiceExt location {
			get {
				if (_location == null) {
					_location = new LocationServiceExt ();
				}
				return _location;
			}
			set {
				_location = value;
			}
		}

		#if UNITY_EDITOR
		static public void awakeLocationMock() {
			Device.location = new LocationServiceExt(true);
		}

		const float LOCATION_MOCK_STEP_MIN = 0.0000001f;
		const float LOCATION_MOCK_STEP_MAX = 1.0f;
		static float locationMockStep = 0.0001f;

		static public void updateMockedLocation() {
			if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
				LocationInfoExt newLoc = location.lastData.clone ();
				newLoc.latitude = Math.Min(location.lastData.latitude + locationMockStep, 90.0f);
				Device.location.MockedLocation = newLoc;
				return;
			}
			
			if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
				LocationInfoExt newLoc = location.lastData.clone ();
				newLoc.latitude = Math.Max(location.lastData.latitude - locationMockStep, -90.0f);
				Device.location.MockedLocation = newLoc;
				return;
			}

			if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
				LocationInfoExt newLoc = location.lastData.clone ();
				newLoc.longitude -= locationMockStep;
				if (newLoc.longitude < -180.0f) {
					newLoc.longitude += 360.0f;
				}
				Device.location.MockedLocation = newLoc;
				return;
			}

			if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
				LocationInfoExt newLoc = location.lastData.clone ();
				newLoc.longitude += locationMockStep;
				if (newLoc.longitude > 180.0f) {
					newLoc.longitude -= 360.0f;
				}
				Device.location.MockedLocation = newLoc;
				return;
			}

			if (Input.GetKeyDown(KeyCode.Plus) && locationMockStep < LOCATION_MOCK_STEP_MAX) {
				locationMockStep *= 2;
				return;
			}

			if (Input.GetKeyDown(KeyCode.Minus) &&  LOCATION_MOCK_STEP_MIN < locationMockStep) {
				locationMockStep /= 2;
				return;
			}
		}
		#endif

		#endregion
	}

}
