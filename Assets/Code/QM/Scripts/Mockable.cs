using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Util;
using QM.Util;

namespace QM.Scripts {

	public class Mockable : MonoBehaviour {

		public bool useLocationMockInEditor = true;

		public void Awake() {
			#if UNITY_EDITOR
			Device.location = new LocationServiceExt(useLocationMockInEditor);
			#endif
		}
	}
}
