using UnityEngine;
using System.Collections;
using System;

public class GPSPosition : MonoBehaviour {



	public double[] CoordinatesWGS84;

	void Update () {

		if ( Input.location.isEnabledByUser ) {
			if ( Input.location.status == LocationServiceStatus.Running ) {
				float newLong = Input.location.lastData.longitude;
				float newLat = Input.location.lastData.latitude;

				if ( differsSignifcantly(CoordinatesWGS84, newLong, newLat) )
					CoordinatesWGS84 = new double[] {
						newLong,
						newLat
					};

				Debug.Log("GPS Location Info changed significantly to: (" + newLong + ", " + newLat);
			}
		}
	}

	bool differsSignifcantly (double[] current, float newLong, float newLat) {
		float minDiff = 1f;

		return (Math.Abs(current[0] - newLong) > minDiff && Math.Abs(current[1] - newLat) > minDiff);
	}
}
