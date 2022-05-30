using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.QM.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.GQClient.UI.layout
{

    public abstract class LayoutConfig : MonoBehaviour
	{

		public static string HEADER = "Header";
		public static string FOOTER = "Footer";

		/// <summary>
		/// Actually does the layout, i.e. loads graphics and connects them to the Images etc.
		/// </summary>
		public abstract void layout ();


        protected virtual void Start()
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
			var objects = Resources.FindObjectsOfTypeAll (typeof(LayoutConfig));
			foreach (var item in objects) {
				((LayoutConfig)item).Reset ();
			}
		}

		private static float _headerHeightUnits { get; set; }

		/// <summary>
		/// Height of the header element in device-dependent units.
		/// </summary>
		/// <value>The height of the header.</value>
		public static float HeaderHeightUnits {
			get {
				calculateHeightAdaptations ();
				return _headerHeightUnits;
			}
		}

		private static float heightReductionInHeaderAndFooter { get; set; }

		private static void calculateHeightAdaptations ()
		{
			heightReductionInHeaderAndFooter = 0f;

			// calculate footer adaptation (reduction):
			_footerHeightUnits = 
				calculateRestrictedHeight (
				Config.Current.footerHeightUnits, 
				Config.Current.footerHeightMinMM, 
				Config.Current.footerHeightMaxMM
			);
			heightReductionInHeaderAndFooter += Config.Current.footerHeightUnits - _footerHeightUnits;

			// calculate header adaptation (reduction):
			_headerHeightUnits = calculateRestrictedHeight (
				Config.Current.headerHeightUnits, 
				Config.Current.headerHeightMinMM, 
				Config.Current.headerHeightMaxMM
			);
			heightReductionInHeaderAndFooter += Config.Current.headerHeightUnits - _headerHeightUnits;

			// adapt content height units based on footer and header adaptations:
			_contentHeightUnits = Config.Current.contentHeightUnits + heightReductionInHeaderAndFooter;
		}

		private static float _footerHeightUnits { get; set; }

		/// <summary>
		/// Height of the footer element in device-dependent units.
		/// </summary>
		/// <value>The height of the footer.</value>
		public static float FooterHeightUnits {
			get {
				calculateHeightAdaptations ();
				return _footerHeightUnits;
			}
		}

		private static float _contentHeightUnits { get; set; }

		/// <summary>
		/// Height of the whole content are in device-dependent units.
		/// </summary>
		/// <value>The height of the header.</value>
		public static float ContentHeightUnits {
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

		public static float ScreenHeightUnits {
			get {
				return (
				    Config.Current.headerHeightUnits +
				    Config.Current.contentHeightUnits +
				    Config.Current.footerHeightUnits
				);
			}
		}

		public static float ScreenWidthUnits {
			get {
				var rawScreenWidthUnits = (Device.width * ScreenHeightUnits) / Device.height;
				// TODO adjust to device diplay format, raw config data should be ideal for 16:9.
				return rawScreenWidthUnits;
			}
		}

		private static float CanvasScale {
			get {
				foreach (var go in GameObject.FindGameObjectsWithTag("RootCanvas")) {
					if (go.activeSelf && go.GetComponent<Canvas> () != null && go.name.EndsWith ("Canvas")) {
						return go.transform.localScale.y;
					}
				}

				string sceneName = SceneManager.GetActiveScene().name;
				if ("Foyer" != sceneName)
				{
					Log.SignalErrorToDeveloper(
						$"No root canvas found in scene {SceneManager.GetActiveScene().name} hence canvas scale factor is set to 1.0f"
					);
				}

				return 1f;
			}
		}

		public static float Units2Pixels (float units)
		{
			var pixels = (units * (Device.height / ScreenHeightUnits)) / CanvasScale;
			return pixels;
		}

		protected static float MMperINCH = 25.4f;

		public static float Units2MM (float units)
		{
			return (units * Device.height * MMperINCH) / (ScreenHeightUnits * Device.dpi);
		}

		public static float MM2Units (float mm)
		{
			return ScreenHeightUnits * mm * Device.dpi / (Device.height * MMperINCH);
		}

		protected static void SetLayoutElementHeight (LayoutElement layElem, float height)
		{
			layElem.minHeight = height;	
			layElem.preferredHeight = height;	
			layElem.flexibleHeight = 0f;
		}

		protected static void SetLayoutElementWidth (LayoutElement layElem, float width)
		{
			layElem.minWidth = width;	
			layElem.preferredWidth = width;	
			layElem.flexibleWidth = 0f;
		}

		#endregion
	}

}