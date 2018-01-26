using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;
using UnityEngine.SceneManagement;
using GQ.Client.Util;
using GQ.Client.Err;
using GQ.Client.Conf;

namespace GQ.Client.UI
{

	public abstract class PageController : UIController
	{

		protected Page page;
		protected QuestManager qm;

		#region MonoBehaviour

		public virtual void Start ()
		{
			bool resumingToFoyer = false;

			qm = QuestManager.Instance;
			if (qm.CurrentQuest == null || qm.CurrentPage == Page.Null) {
				SceneManager.LoadSceneAsync (Base.FOYER_SCENE);
				resumingToFoyer = true;
				return;
			}

			page = qm.CurrentPage;

			if (page == null) {
				if (!resumingToFoyer)
					Log.SignalErrorToDeveloper (
						"Page is null in quest {0}", 
						QuestManager.Instance.CurrentQuest.Id.ToString ()
					);
				return;
				// TODO What should we do now? End quest?
			}

			page.PageCtrl = this;

			Initialize ();
		}

		#endregion


		#region Runtime API

		public abstract void Initialize ();

		/// <summary>
		/// Override this method to react on Back Button CLick (or similar events).
		/// </summary>
		public virtual void OnBack ()
		{
			Debug.Log ("OnBack() not yet implemented for page controller " + GetType ().Name);
		}

		/// <summary>
		/// Override this method to react on Forward Button CLick (or similar events).
		/// </summary>
		public virtual void OnForward ()
		{
			page.End ();
		}

		#endregion


		#region Layout


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
		/// Margin between header and content in device-dependent units.
		/// </summary>
		static public float ContentTopMarginUnits {
			get {
				// TODO adjust to device diplay format, raw config data should be ideal for 16:9.
				return ConfigurationManager.Current.contentTopMarginUnits;
			}
		}

		/// <summary>
		/// Margin between content and footer in device-dependent units.
		/// </summary>
		static public float ContentBottomMarginUnits {
			get {
				// TODO adjust to device diplay format, raw config data should be ideal for 16:9.
				return ConfigurationManager.Current.contentBottomMarginUnits;
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

		static public float ContentDividerUnits {
			get {
				// TODO adjust to device diplay format, raw config data should be ideal for 16:9.
				return ConfigurationManager.Current.contentDividerUnits;
			}
		}

		public abstract int NumberOfSpacesInContent ();

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

		static public float BorderWidthUnits {
			get {
				// TODO adjust to device diplay format, raw config data should be ideal for 16:9.
				return ConfigurationManager.Current.borderWidthUnits;
			}
		}

		static public float ContentWidthUnits {
			get {
				return ScreenWidthUnits - (2 * BorderWidthUnits);
			}
		}

		static public int UnitsToPixels (float units)
		{
			return (int)(units * (Screen.height / ScreenHeightUnits));
		}

		protected float ImageRatioMinimum {
			get {
				return ContentWidthUnits / ConfigurationManager.Current.imageAreaHeightMaxUnits;
			}
		}

		protected float ImageRatioMaximum {
			get {
				return ContentWidthUnits / ConfigurationManager.Current.imageAreaHeightMinUnits;

			}
		}

		protected float CalculateMainAreaHeight (float imageAreaHeight)
		{
			return (	
			    ContentHeightUnits -
			    (
			        ContentTopMarginUnits +
			        imageAreaHeight +
			        ContentDividerUnits +
			        ContentBottomMarginUnits
			    )
			);
		}

		#endregion
	}
}
