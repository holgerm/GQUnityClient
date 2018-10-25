using UnityEngine;
using GQ.Client.Err;
using GQ.Client.Conf;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using QM.Util;

namespace GQ.Client.UI
{

	public abstract class LayoutConfig : MonoBehaviour
	{

		public static string HEADER = "Header";
		public static string FOOTER = "Footer";

		public abstract void layout ();

		protected void Start ()
		{
			layout ();
		}

		protected void Reset ()
		{
			layout ();
		}

		#region Static Helpers

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

		public static float calculateRestrictedHeight (float units, float minMM, float maxMM)
		{
			float result = units;

			if (minMM > 0f) {
                // check wether specified units obeye the contraint:
                float calcMM = Units2MM(units);
                if (calcMM < minMM) {
					// if not, adapt to nearest value and return that:
					result = MM2Units (minMM);
				}
			}

			if (maxMM > 0f) {
                // check wether specified units obeye the contraint:
                float calcMM = Units2MM(units);
                if (calcMM > maxMM) {
					// if not, adapt to nearest value and return that:
					result = MM2Units (maxMM);
				}
			}

			return result;
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

		static protected float CanvasScale {
			get {
				foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects()) {
					if (go.activeSelf && go.GetComponent<Canvas> () != null && go.name.EndsWith ("Canvas")) {
						return go.transform.localScale.y;
					}
				}
				Log.SignalErrorToDeveloper (
					"No root canvas found in scene {0} hence canvas scale factor is set to 1.0f", 
					SceneManager.GetActiveScene ().name
				);
				return 1f;
			}
		}

		static public float Units2Pixels (float units)
		{
			float pixels = (units * (Device.height / ScreenHeightUnits)) / CanvasScale;
			return pixels;
		}

		static protected float MMperINCH = 25.4f;

		static public float Units2MM (float units)
		{
			return (units * Device.height * MMperINCH) / (ScreenHeightUnits * Device.dpi);
		}

		static public float MM2Units (float mm)
		{
			return ScreenHeightUnits * mm * Device.dpi / (Device.height * MMperINCH);
		}

		static public void SetLayoutElementHeight (LayoutElement layElem, float height)
		{
			layElem.minHeight = height;	
			layElem.preferredHeight = height;	
			layElem.flexibleHeight = 0f;
		}

		static public void SetLayoutElementWidth (LayoutElement layElem, float width)
		{
			layElem.minWidth = width;	
			layElem.preferredWidth = width;	
			layElem.flexibleWidth = 0f;
		}

		#endregion
	}

}