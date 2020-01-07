//#define DEBUG_LOG

using UnityEngine;
using GQ.Client.Model;
using UnityEngine.Android;

namespace QM.Util
{

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
					InitLocationMock ();
				}
				return _mockedLocation;
			}
			set {
				_mockedLocation = value;
			}
		}


		public void InitLocationMock() {
			Quest curQuest = QuestManager.Instance.CurrentQuest;
			// Initialize mocked location near relevant hotspots, i.e. in-quest hotspots or quest starting points.
			if (curQuest == Quest.Null || curQuest == null) {
				// we are in foyer situation and use the central location of all quests:
				float sumOfLongitudes = 0f;
				float sumOfLatitudes = 0f;
				int nrOfQuests = 0;
				foreach (QuestInfo qi in QuestInfoManager.Instance.GetFilteredQuestInfos()) {
					if (qi.MarkerHotspot != HotspotInfo.NULL) {
						sumOfLongitudes += (float)qi.MarkerHotspot.Longitude;
						sumOfLatitudes += (float)qi.MarkerHotspot.Latitude;
						nrOfQuests++;
					}
				}
				_mockedLocation.longitude = nrOfQuests > 0 ? sumOfLongitudes / nrOfQuests : sumOfLongitudes;
				_mockedLocation.latitude = nrOfQuests > 0 ? sumOfLatitudes / nrOfQuests : sumOfLatitudes;
			}
			else {
				// we are in a quest and use a position south of the southern-most hotspot:
				float sumOfLongitudes = 0f;
				float southernMostLatitude = 90f;
				float radiusOfSelectedHotspot = 0f;
				int nrOfQuests = 0;
				foreach (Hotspot h in curQuest.AllHotspots) {
					// we search the southern most hotpot latitude:
					if (h.Latitude < southernMostLatitude) {
						southernMostLatitude = (float) h.Latitude;
						radiusOfSelectedHotspot = (float) h.Radius;
					}
					sumOfLongitudes += (float) h.Longitude;
					nrOfQuests++;
				}
				// we go a bit more than the radius south of the hotspot, 
				// by dividing the radius through 100000m instead of 111000m and subtracting it:
				southernMostLatitude -= radiusOfSelectedHotspot / 100000;
				_mockedLocation.longitude = nrOfQuests > 0 ? sumOfLongitudes / nrOfQuests : sumOfLongitudes;
				_mockedLocation.latitude = southernMostLatitude;
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
			}
			else
			{
				realLocation = new LocationService();
			}
		}

        static bool askingUserForPermission = false;

        public bool isEnabledByUser
		{
			//realLocation.isEnabledByUser seems to be failing on Android. Input.location.isEnabledByUser is the fix
			get {
                if (useMockLocation)
                {
#if DEBUG_LOG
                    Debug.Log("MockLocation always grants Permission for FineLocation.");
#endif
                    return mIsEnabledByUser;
                }

#if UNITY_IOS
                return true;
#endif

#if UNITY_ANDROID
                if (!askingUserForPermission && !Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                {
#if DEBUG_LOG
                    Debug.Log("Asking User for Permission for FineLocation.");
#endif
                    askingUserForPermission = true;
                    Permission.RequestUserPermission(Permission.FineLocation);
                }

                if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                {
#if DEBUG_LOG
                    Debug.Log("User granted Permission for FineLocation.");
#endif
                    askingUserForPermission = false;
                    return true;
                }
                else
                {
                    return false;
                }
#else
                return true;
#endif
            }
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