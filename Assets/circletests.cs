using UnityEngine;
using System.Collections;
using System;
using UnitySlippyMap;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.Converters.WellKnownText;

public class circletests : MonoBehaviour {





	public int radius = 20;
	
	// Update is called once per frame
	void Update () {




		float theta_scale = 0.1f;             //Set lower to add more points
		int size = (int)((2.0f * Mathf.PI) / theta_scale); //Total number of points in circle.

		LineRenderer lineRenderer = new LineRenderer ();

				if (gameObject.GetComponent<LineRenderer> () != null) {
						lineRenderer = gameObject.GetComponent<LineRenderer> ();
				} else {
						lineRenderer = gameObject.AddComponent<LineRenderer> ();
				}


		//lineRenderer.material = new Material(Shader.Find("Default"));
		lineRenderer.SetColors(Color.blue, Color.blue);
		lineRenderer.SetWidth(0.001F, 0.001F);
		lineRenderer.SetVertexCount(0);
		lineRenderer.SetVertexCount(size+1);


		int i = 0;
		for(float theta = 0f; theta < (2f * Mathf.PI); theta += 0.1f) {
			//Debug.Log (i);

			float x = 0.035f* GeoHelpers.MetersPerInch* radius* Mathf.Cos(theta);
			float y = 0.035f* GeoHelpers.MetersPerInch* radius*Mathf.Sin(theta);
			//Debug.Log(i);
			Vector3 pos = new Vector3(transform.position.x + y, transform.position.y +0.001f , transform.position.z + x );
			lineRenderer.SetPosition(i, pos);
			i+=1;
		}
	}
}
