using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.UI;
using GQ.Client.Err;

namespace GQ.Client.UI
{

	public class QuestMarkerInteractions : MonoBehaviour {

		public const int MARKER_LAYER = 8;
		public const int MARKER_LAYER_MASK = 1 << MARKER_LAYER;

		void Update () {
			if (
				Input.GetMouseButtonDown (0) || 
				(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
			) {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, Mathf.Infinity, MARKER_LAYER_MASK))
					hit.collider.GetComponentInParent<Marker> ().OnTouch ();
			}
		}


	}


}
