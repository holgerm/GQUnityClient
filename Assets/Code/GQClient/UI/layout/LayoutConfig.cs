using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Err;


namespace GQ.Client.UI {

	public abstract class LayoutConfig : MonoBehaviour {

		public static string HEADER = "Header";
		public static string FOOTER = "Footer";

		protected abstract void layout ();

		protected void Start () {
			layout ();
		}
		
		protected void Reset () {
			layout ();
		}

		/// <summary>
		/// Resets all layout changes made in the config to all gameobjects involved in the editor, so that the changes are immediatley reflected.
		/// </summary>
		public static void ResetAll() {
			Object[] objects = Resources.FindObjectsOfTypeAll (typeof(LayoutConfig));
			foreach (var item in objects) {
				((LayoutConfig)item).Reset ();
			}
		}
	}

}