using UnityEngine;
using System.Collections;

namespace GQ.Client.Model {

	public class MediaManager {
		
		#region singleton

		public static MediaManager Instance {
			get {
				if ( _instance == null ) {
					_instance = new MediaManager();
				} 
				return _instance;
			}
			set {
				_instance = value;
			}
		}

		public static void Reset () {
			_instance = null;
		}

		private static MediaManager _instance = null;

		private MediaManager () {

		}

		#endregion

		#region Store Media



		#endregion
	}
}
