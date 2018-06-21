using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace QM.Util {

	public class LocationServiceExt
	{
		private LocationService realLocation;

		private bool useMockLocation = false;

		public static LocationInfoExt NULL_LOCATION = new LocationInfoExt ();
		private LocationInfoExt _mockedLocation = NULL_LOCATION;
		public LocationInfoExt MockedLocation {
			get {
				if (_mockedLocation.Equals(NULL_LOCATION)) {
					_mockedLocation = new LocationInfoExt ();

				}
				return _mockedLocation;
			}
			set {
				_mockedLocation = value;
			}
		}

		private LocationServiceStatus mockedStatus;
		private bool mIsEnabledByUser = false;

		public LocationServiceExt(bool mockLocation = false)
		{
			this.useMockLocation = mockLocation;

			if (mockLocation)
			{
				mIsEnabledByUser = true;
//				MockedLocation = getMockLocation();
			}
			else
			{
				realLocation = new LocationService();
			}
		}

		public bool isEnabledByUser
		{
			//realLocation.isEnabledByUser seems to be failing on Android. Input.location.isEnabledByUser is the fix
			get { return useMockLocation ? mIsEnabledByUser : Input.location.isEnabledByUser; }
			set { mIsEnabledByUser = value; }
		}


		public LocationInfoExt lastData
		{
			get { return useMockLocation ? MockedLocation : getRealLocation(); }
		}

		public LocationServiceStatus status
		{
			get { return useMockLocation ? mockedStatus : realLocation.status; }
			set { mockedStatus = value; }
		}

		public void Start()
		{
			if (useMockLocation)
			{
				mockedStatus = LocationServiceStatus.Running;
			}
			else
			{
				realLocation.Start();
			}
		}

		public void Start(float desiredAccuracyInMeters)
		{
			if (useMockLocation)
			{
				mockedStatus = LocationServiceStatus.Running;
			}
			else
			{
				realLocation.Start(desiredAccuracyInMeters);
			}
		}

		public void Start(float desiredAccuracyInMeters, float updateDistanceInMeters)
		{
			if (useMockLocation)
			{
				mockedStatus = LocationServiceStatus.Running;
			}
			else
			{
				realLocation.Start(desiredAccuracyInMeters, updateDistanceInMeters);
			}
		}

		public void Stop()
		{
			if (useMockLocation)
			{
				mockedStatus = LocationServiceStatus.Stopped;
			}
			else
			{
				realLocation.Stop();
			}
		}

		//Predefined Location. You always override this by overriding lastData from another class 
		private LocationInfoExt getMockLocation()
		{
			LocationInfoExt location = new LocationInfoExt();
			location.latitude = 59.000f;
			location.longitude = 18.000f;
			location.altitude = 0.0f;
			location.horizontalAccuracy = 5.0f;
			location.verticalAccuracy = 5.0f;
			location.timestamp = 0f;
			return location;
		}

		private LocationInfoExt getRealLocation()
		{
			if (realLocation == null)
				return new LocationInfoExt();

			LocationInfo realLoc = realLocation.lastData;
			LocationInfoExt location = new LocationInfoExt();
			location.latitude = realLoc.latitude;
			location.longitude = realLoc.longitude;
			location.altitude = realLoc.altitude;
			location.horizontalAccuracy = realLoc.horizontalAccuracy;
			location.verticalAccuracy = realLoc.verticalAccuracy;
			location.timestamp = realLoc.timestamp;
			return location;
		}

	}

	public struct LocationInfoExt
	{
		public float altitude { get; set; }
		public float horizontalAccuracy { get; set; }
		public float latitude { get; set; }
		public float longitude { get; set; }
		public double timestamp { get; set; }
		public float verticalAccuracy { get; set; }

		public LocationInfoExt clone() {
			return (LocationInfoExt) MemberwiseClone();
		}
	}

}