using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySlippyMap.Markers;
using GQ.Client.UI;

public class Map : MonoBehaviour {

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


	#region Centering Button

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

	#endregion


	#region Runtime

	void Start() {
		Frame ();
	}

	#endregion
}
