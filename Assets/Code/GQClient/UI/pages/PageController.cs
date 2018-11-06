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

		#region Start

		bool resumingToFoyer;

		public virtual void Awake ()
		{
			resumingToFoyer = false;

			qm = QuestManager.Instance;
			if (qm.CurrentQuest == null || qm.CurrentPage == Page.Null) {
				SceneManager.LoadSceneAsync (Base.FOYER_SCENE);
				resumingToFoyer = true;
				return;
			}
		}

		public virtual void Start ()
		{
			InitPage ();
		}

		/// <summary>
		/// Must be called manually if you want to reuse the page model, i.e. for the second of two consecutive pages 
		/// of the same model type.
		/// </summary>
		public void InitPage ()
		{
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
		/// Override this method to react on Forward Button Click (or similar events).
		/// </summary>
		public virtual void OnForward ()
		{
			page.End ();
		}

		/// <summary>
		/// Clean up just before the page controlled by this controller is left, e.g. when starting a new page.
		/// </summary>
		public virtual void CleanUp() {
			
		}

		#endregion


		#region Layout

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

		static public float ContentDividerUnits {
			get {
				// TODO adjust to device diplay format, raw config data should be ideal for 16:9.
				return ConfigurationManager.Current.contentDividerUnits;
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
				return LayoutConfig.ScreenWidthUnits - (2 * BorderWidthUnits);
			}
		}

        static public float ImageRatioMinimum {
			get {
				return ContentWidthUnits / ConfigurationManager.Current.imageAreaHeightMaxUnits;
			}
		}

        static public float ImageRatioMaximum {
			get {
				return ContentWidthUnits / ConfigurationManager.Current.imageAreaHeightMinUnits;

			}
		}

		protected float CalculateMainAreaHeight (float imageAreaHeight)
		{
			float units = LayoutConfig.ContentHeightUnits -
			              (
			                  ContentTopMarginUnits +
			                  imageAreaHeight +
			                  ContentDividerUnits +
			                  ContentBottomMarginUnits
			              );
			return LayoutConfig.Units2Pixels (units);
		}

		#endregion
	}
}
