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
		bool clickStartedValid;
		Vector3 endPos;

		// If we drag on the map less than this distance we interpret it as click.
		private const float DISTANCE_BETWEEN_CLICK_AND_DRAG = 10.0f;

		void Start() {
			startPos = Vector3.zero;
			endPos = Vector3.zero;
		}

		void Update () {
			// if users touches buttons or header, we ignore that touch on the map and its markers
			// also we ignore if the user starts a drag move from such an ui element (hence we ask for used events, c.f. UnitySlippyMap.Input.MapInput
			if ((Event.current != null && Event.current.type == EventType.Used) 
				|| EventSystem.current.IsPointerOverGameObject () 
				|| MapController.IgnoreInteraction
				|| (UnityEngine.Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject (UnityEngine.Input.GetTouch (0).fingerId))
			) {
				MapController.IgnoreInteraction = true;
				return;
			}

			if (
				Input.GetMouseButtonDown (0) || 
				(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
				)
			{
				startPos = Input.mousePosition;
				if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) {
					// MOBILE PLATFORMS:
					clickStartedValid = !EventSystem.current.IsPointerOverGameObject (UnityEngine.Input.GetTouch (0).fingerId);
				}
				else {
					// NON_MOBILE PLATFORMS:
					clickStartedValid = !(EventSystem.current.IsPointerOverGameObject ());
				}
			}

			if (
				clickStartedValid &&
				(Input.GetMouseButtonUp (0) || 
					(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended))
			) {
				endPos = Input.mousePosition;
				float dictance = Vector3.Distance (startPos, endPos);

				if (startPos != Vector3.zero && dictance > DISTANCE_BETWEEN_CLICK_AND_DRAG) {
				}
				else {
					Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					RaycastHit hit;
					if (Physics.Raycast (ray, out hit, Mathf.Infinity, MARKER_LAYER_MASK)) {
						if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) {
							// MOBILE PLATFORMS:
							if (Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch (0).fingerId) 
								&& !EventSystem.current.IsPointerOverGameObject()) {
								hit.collider.GetComponentInParent<Marker> ().OnTouch ();
							}
						}
						else {
							// NON_MOBILE PLATFORMS:
							if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject ()) {
								hit.collider.GetComponentInParent<Marker> ().OnTouch ();
							}
						}
					}				
				}

				startPos = Vector3.zero;
				endPos = Vector3.zero;
			}
		}


	}


}
