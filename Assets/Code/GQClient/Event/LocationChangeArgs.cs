using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GQ.Event {

	public class LocationChangeArgs : EventArgs {
		LocationInfo newLocationInfo { get; set; }

		public LocationChangeArgs (LocationInfo locInfo) {
			newLocationInfo = locInfo;
		}
	}
}
