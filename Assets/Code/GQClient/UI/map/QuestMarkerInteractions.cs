using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.UI;
using GQ.Client.Err;
using UnityEngine.EventSystems;

namespace GQ.Client.UI
{

	public class QuestMarkerInteractions : MonoBehaviour {

		public const int MARKER_LAYER = 8;
		public const int MARKER_LAYER_MASK = 1 << MARKER_LAYER;

		Vector3 startPos;
		Vector3 endPos;

		// If we drag on the map less than this distance we interpret it as click.
		private const float DISTANCE_BETWEEN_CLICK_AND_DRAG = 10.0f;

		void Start() {
			startPos = Vector3.zero;
			endPos = Vector3.zero;
		}

		void Update () {
			if (EventSystem.current.IsPointerOverGameObject ())
				return;

			if (
				Input.GetMouseButtonDown (0) || 
				(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
				)
			{
				startPos = Input.mousePosition;
			}

			if (
				Input.GetMouseButtonUp (0) || 
				(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
			) {
				endPos = Input.mousePosition;
				float dictance = Vector3.Distance (startPos, endPos);

				if (startPos != Vector3.zero && dictance > DISTANCE_BETWEEN_CLICK_AND_DRAG) {
				}
				else {
					Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					RaycastHit hit;
					if (Physics.Raycast (ray, out hit, Mathf.Infinity, MARKER_LAYER_MASK))
						hit.collider.GetComponentInParent<Marker> ().OnTouch ();
				}

				startPos = Vector3.zero;
				endPos = Vector3.zero;
			}
		}


	}


}
