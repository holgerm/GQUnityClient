using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Err;
using GQ.Client.Conf;
using GQ.Client.Util;


namespace GQ.Client.UI
{

	public abstract class LayoutConfig : MonoBehaviour
	{

		public static string HEADER = "Header";
		public static string FOOTER = "Footer";

		protected abstract void layout ();

		protected void Start ()
		{
			layout ();
		}

		protected void Reset ()
		{
			layout ();
		}

		/// <summary>
		/// Resets all layout changes made in the config to all gameobjects involved in the editor, so that the changes are immediatley reflected.
		/// </summary>
		public static void ResetAll ()
		{
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

		static private void calculateHeightAdaptations ()
		{
			heightReductionInHeaderAndFooter = 0f;

			// calculate footer adaptation (reduction):
			_footerHeightUnits = 
				calculateRestrictedHeight (
				ConfigurationManager.Current.footerHeightUnits, 
				ConfigurationManager.Current.footerHeightMinMM, 
				ConfigurationManager.Current.footerHeightMaxMM
			);
			heightReductionInHeaderAndFooter += ConfigurationManager.Current.footerHeightUnits - _footerHeightUnits;

			// calculate header adaptation (reduction):
			_headerHeightUnits = calculateRestrictedHeight (
				ConfigurationManager.Current.headerHeightUnits, 
				ConfigurationManager.Current.headerHeightMinMM, 
				ConfigurationManager.Current.headerHeightMaxMM
			);
			heightReductionInHeaderAndFooter += ConfigurationManager.Current.headerHeightUnits - _headerHeightUnits;

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

		static private float calculateRestrictedHeight (float units, float minMM, float maxMM)
		{
			float result = units;

			if (minMM > 0f) {
				// check wether specified units obeye the contraint:
				if (Units2MM (units) < minMM) {
					// if not, adapt to nearest value and return that:
					result = MM2Units (minMM);
				}
			}
			if (maxMM > 0f) {
				// check wether specified units obeye the contraint:
				if (Units2MM (units) > maxMM) {
					// if not, adapt to nearest value and return that:
					result = MM2Units (maxMM);
				}
			}

			return result;
		}

		static public float MapButtonHeightUnits {
			get {
				return 
					calculateRestrictedHeight (
					ConfigurationManager.Current.mapButtonHeightUnits,
					ConfigurationManager.Current.mapButtonHeightMinMM,
					ConfigurationManager.Current.mapButtonHeightMaxMM
				);
			}
		}

		static public float MenuEntryHeightUnits {
			get {
				return 
					calculateRestrictedHeight (
					ConfigurationManager.Current.menuEntryHeightUnits,
					ConfigurationManager.Current.menuEntryHeightMinMM,
					ConfigurationManager.Current.menuEntryHeightMaxMM
				);
			}
		}

		static public float ListEntryHeightUnits {
			get {
				return 
					calculateRestrictedHeight (
					ConfigurationManager.Current.listEntryHeightUnits,
					ConfigurationManager.Current.listEntryHeightMinMM,
					ConfigurationManager.Current.listEntryHeightMaxMM
				);
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

		static public float Units2Pixels (float units)
		{
			float canvasScale = GameObject.Find ("DialogCanvas").transform.localScale.y;
			return (units * (Device.height / ScreenHeightUnits)) / canvasScale;
		}

		static public float MMperINCH = 25.4f;

		static public float Units2MM (float units)
		{
			return (units * Device.height * MMperINCH) / (ScreenHeightUnits * Device.dpi);
		}

		static public float MM2Units (float mm)
		{
			return ScreenHeightUnits * mm * Device.dpi / (Device.height * MMperINCH);
		}


		#endregion
	}

}