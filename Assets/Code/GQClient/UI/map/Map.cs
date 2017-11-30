using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySlippyMap.Markers;
using GQ.Client.UI;
using UnityEngine.UI;
using UnitySlippyMap.Map;
using GQ.Client.Conf;
using System;
using GQ.Client.Err;

namespace GQ.Client.UI
{

	public class Map : MonoBehaviour {

		protected MapBehaviour map;

		public Texture	LocationTexture;


		#region Markers

		private Dictionary<int, Marker> markers;

		protected Dictionary<int, Marker> Markers {
			get {
				if (markers == null) {
					markers = new Dictionary<int, Marker> ();
				}
				return markers;
			}
		}

		#endregion


		#region Centering

		public GameObject MapButtonPanel;
		public Texture CenterTexture;
		public Texture FrameTexture;

		public enum Centering {
			Centered,
			Framed,
			Manual
		}
			
		public Centering CenteringState {
			get;
			protected set;
		}

		public void Frame() {
			// center the map so it frames all currently visible markers: TODO

			// let the center button show the centering button icon now, unless local position is not available, in that case show the frame icon and disbale it.

			CenteringState = Centering.Framed;
		}

		public void Center() {
			// center the map so it is centered to the current users position: TODO

			// let the center button show the centering button icon now

			CenteringState = Centering.Centered;
		}

		public void CenterButtonPressed() {
			Debug.Log ("Center Button Pressed");
		}

		#endregion


		#region Zooming

		public void ZoomInButtonPressed() {
			map.CurrentZoom = Math.Min(map.CurrentZoom + ConfigurationManager.Current.mapDeltaZoom, map.MaxZoom);
			map.Zoom (0f);
			UpdateZoomButtons ();
		}

		public void ZoomOutButtonPressed() {
			map.CurrentZoom = Math.Max(map.CurrentZoom - ConfigurationManager.Current.mapDeltaZoom, map.MinZoom);
			map.Zoom (0f);
			UpdateZoomButtons ();
		}

		protected void UpdateZoomButtons() {
			// If further zooming IN is not possible disable ZoomInButton: 
			zoomInButton.Enabled = (map.MaxZoom > map.CurrentZoom);

			// If further zooming OUT is not possible disable ZoomOutButton: 
			zoomOutButton.Enabled = (map.MinZoom < map.CurrentZoom);
		}


		#endregion


		#region Runtime

		OverlayButtonLayoutConfig zoomInButton;
		OverlayButtonLayoutConfig zoomOutButton;

		protected virtual void Start() {
			Frame ();
			GameObject zibGo = MapButtonPanel.transform.Find ("ZoomInButton").gameObject;
			zoomInButton = zibGo.GetComponent<OverlayButtonLayoutConfig> ();
			zoomOutButton = MapButtonPanel.transform.Find ("ZoomOutButton").GetComponent<OverlayButtonLayoutConfig> ();
		}

		#endregion
	}
}
