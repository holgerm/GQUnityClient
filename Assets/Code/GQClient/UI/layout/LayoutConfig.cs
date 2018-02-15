using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Err;
using GQ.Client.Conf;
using GQ.Client.Util;


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


		static private float _headerHeightUnits { get; set; }

		/// <summary>
		/// Height of the header element in device-dependent units.
		/// </summary>
		/// <value>The height of the header.</value>
		static public float HeaderHeightUnits {
			get {
				calculateHeightAdaptations ();
				return _headerHeightUnits;
			}
		}

		static private float heightReductionInHeaderAndFooter { get; set; }

		static private void calculateHeightAdaptations() {
			heightReductionInHeaderAndFooter = 0f;

			// calculate footer adaptation (reduction):
			_footerHeightUnits = ConfigurationManager.Current.footerHeightUnits; // start with standard
			if (ConfigurationManager.Current.footerHeightMinMM > 0f) {
				// check wether specified units obeye the contraint:
				if (Units2MM (ConfigurationManager.Current.footerHeightUnits) < ConfigurationManager.Current.footerHeightMinMM) {
					// if not, adapt to nearest value and return that:
					_footerHeightUnits = MM2Units (ConfigurationManager.Current.footerHeightMinMM);
					heightReductionInHeaderAndFooter += ConfigurationManager.Current.footerHeightUnits - _footerHeightUnits;
				}
			}
			if (ConfigurationManager.Current.footerHeightMaxMM > 0f) {
				// check wether specified units obeye the contraint:
				if (Units2MM (ConfigurationManager.Current.footerHeightUnits) > ConfigurationManager.Current.footerHeightMaxMM) {
					// if not, adapt to nearest value and return that:
					_footerHeightUnits = MM2Units (ConfigurationManager.Current.footerHeightMaxMM);
					heightReductionInHeaderAndFooter += ConfigurationManager.Current.footerHeightUnits - _footerHeightUnits;
				}
			}

			// calculate header adaptation (reduction):
			_headerHeightUnits = ConfigurationManager.Current.headerHeightUnits; // start with standard
			if (ConfigurationManager.Current.headerHeightMinMM > 0f) {
				// check wether specified units obeye the contraint:
				if (Units2MM (ConfigurationManager.Current.headerHeightUnits) < ConfigurationManager.Current.headerHeightMinMM) {
					// if not, adapt to nearest value and return that:
					_headerHeightUnits = MM2Units (ConfigurationManager.Current.headerHeightMinMM);
					heightReductionInHeaderAndFooter += ConfigurationManager.Current.headerHeightUnits - _headerHeightUnits;
				}
			}
			if (ConfigurationManager.Current.headerHeightMaxMM > 0f) {
				// check wether specified units obeye the contraint:
				if (Units2MM (ConfigurationManager.Current.headerHeightUnits) > ConfigurationManager.Current.headerHeightMaxMM) {
					// if not, adapt to nearest value and return that:
					_headerHeightUnits = MM2Units (ConfigurationManager.Current.headerHeightMaxMM);
					heightReductionInHeaderAndFooter += ConfigurationManager.Current.headerHeightUnits - _headerHeightUnits;
				}
			}

			// adapt content height units based on footer and header adaptations:
			_contentHeightUnits = ConfigurationManager.Current.contentHeightUnits + heightReductionInHeaderAndFooter;
		}

		static private float _footerHeightUnits { get; set; }
		/// <summary>
		/// Height of the footer element in device-dependent units.
		/// </summary>
		/// <value>The height of the footer.</value>
		static public float FooterHeightUnits {
			get {
				calculateHeightAdaptations ();
				return _footerHeightUnits;
			}
		}

		static private float _contentHeightUnits { get; set; }

		/// <summary>
		/// Height of the whole content are in device-dependent units.
		/// </summary>
		/// <value>The height of the header.</value>
		static public float ContentHeightUnits {
			get {
				calculateHeightAdaptations ();
				return _contentHeightUnits;
			}
		}

		static public float ScreenHeightUnits {
			get {
				return (
					ConfigurationManager.Current.headerHeightUnits +
					ConfigurationManager.Current.contentHeightUnits +
					ConfigurationManager.Current.footerHeightUnits
				);
			}
		}

		static public float ScreenWidthUnits {
			get {
				float rawScreenWidthUnits = (Device.width * ScreenHeightUnits) / Device.height;
				// TODO adjust to device diplay format, raw config data should be ideal for 16:9.
				return rawScreenWidthUnits;
			}
		}

		static public int Units2Pixels (float units)
		{
			return (int)(units * (Device.height / ScreenHeightUnits));
		}

		static public float MMperINCH = 25.4f;

		static public float Units2MM (float units) {
			return (units * Device.height * MMperINCH) / (ScreenHeightUnits * Device.dpi);
		}

		static public float MM2Units (float mm) {
			return ScreenHeightUnits * mm * Device.dpi / (Device.height * MMperINCH);
		}


		#endregion
	}

}