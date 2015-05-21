using UnityEngine;
using System.Collections;

public class GPSPosition : MonoBehaviour {



	public double[] CoordinatesWGS84;

	void Update(){

		if(Input.location.isEnabledByUser)
		{
			if(Input.location.status == LocationServiceStatus.Running)
			 {


				CoordinatesWGS84 = new double[]{
				Input.location.lastData.longitude,
					Input.location.lastData.latitude};
			}
		}


	}
}
