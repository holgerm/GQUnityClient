using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Err;
using GQ.Client.Conf;


namespace GQ.Client.UI {

	public abstract class LayoutConfig : MonoBehaviour {

		public static string HEADER = "Header";
		public static string FOOTER = "Footer";

		protected void Awake() {
			config ();
		}

		protected virtual void config() {
			
		}

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

		#region Static Size Functions


		/// <summary>
		/// Height of the header element in device-dependent units.
		/// </summary>
		/// <value>The height of the header.</value>
		static public float HeaderHeightUnits {
			get {
				// TODO adjust to device diplay format, raw config data should be ideal for 16:9.
				return ConfigurationManager.Current.headerHeightUnits;
			}
		}

		/// <summary>
		/// Height of the whole content are in device-dependent units.
		/// </summary>
		/// <value>The height of the header.</value>
		static public float ContentHeightUnits {
			get {
				// TODO adjust to device diplay format, raw config data should be ideal for 16:9.
				return ConfigurationManager.Current.contentHeightUnits;
			}
		}

		/// <summary>
		/// Height of the footer element in device-dependent units.
		/// </summary>
		/// <value>The height of the footer.</value>
		static public float FooterHeightUnits {
			get {
				// TODO adjust to device diplay format, raw config data should be ideal for 16:9.
				return ConfigurationManager.Current.footerHeightUnits;
			}
		}

		static public float ScreenHeightUnits {
			get {
				return (
					HeaderHeightUnits +
					ContentHeightUnits +
					FooterHeightUnits
				);
			}
		}

		static public float ScreenWidthUnits {
			get {
				float rawScreenWidthUnits = (9f / 16f) * ScreenHeightUnits;
				// TODO adjust to device diplay format, raw config data should be ideal for 16:9.
				return rawScreenWidthUnits;
			}
		}

		static public int UnitsToPixels (float units)
		{
			return (int)(units * (Screen.height / ScreenHeightUnits));
		}


		#endregion
	}

}