using UnityEngine;
using System.Collections;
using UnitySlippyMap;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

public class mylocationmarker : MonoBehaviour {


	public page_map controller;

	public Map		Map;

	public double[]	coordinatesEPSG900913 = new double[2];


	public bool started = false;


	void Start(){




	}

	// Update is called once per frame
	void Update () {
	
		if (Map == null) {
						Map = controller.map;
				}




		if (Map != null) {
						started = true;

					
		

		
						double[] offsetEPSG900913 = new double[2] {
								coordinatesEPSG900913 [0] - Map.CenterEPSG900913 [0],
								coordinatesEPSG900913 [1] - Map.CenterEPSG900913 [1]
						};
		
						double offset = offsetEPSG900913 [0];
						if (offset < 0.0)
								offset = -offset;
						if (offset > GeoHelpers.HalfEarthCircumference)
								offsetEPSG900913 [0] += GeoHelpers.EarthCircumference;
		
						/*
		Debug.LogError("DEBUG: " + this.name + ": center: " + Map.Center[0] + ", " + Map.Center[1] + " ; center meters: " + centerMeters[0] + ", " + centerMeters[1] + "\ncoordinates: " + coordinatesWGS84[0] + ", " + coordinatesWGS84[1] + " ; coordinatesWGS84 meters: " + coordinatesMeters[0] + ", " + coordinatesMeters[1] + "\noffset meters: " + offsetMeters[0] + ", " + offsetMeters[1]);
		Debug.LogError("DEBUG: offset meters: " + offsetMeters[0] + ", " + offsetMeters[1] + "\noffset multiplier: " + offsetMultiplier + " ; half map scale: " + Map.HalfMapScale + "\npos: " + (offsetMeters[0] / offsetMultiplier) + ", " + (offsetMeters[1] / offsetMultiplier));
		*/
		
						this.gameObject.transform.position = new Vector3 (
			offsetEPSG900913 [0] == 0.0 ? 0.0f : (float)offsetEPSG900913 [0] * Map.ScaleMultiplier,
			this.gameObject.transform.position.y,
			offsetEPSG900913 [1] == 0.0 ? 0.0f : (float)offsetEPSG900913 [1] * Map.ScaleMultiplier);

			if (this.gameObject.transform.localScale.x != Map.HalfMapScale)
				this.gameObject.transform.localScale = new Vector3(Map.HalfMapScale / 2f, Map.HalfMapScale /2f, Map.HalfMapScale /2f);

				}



	}
}
