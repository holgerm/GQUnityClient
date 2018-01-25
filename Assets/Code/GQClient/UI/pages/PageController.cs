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
		static public float HeaderHeight {
			get {
				// TODO adjust to device diplay format, raw config data should be ideal for 16:9.
				return ConfigurationManager.Current.headerHeightUnits;
			}
		}

		/// <summary>
		/// Margin between header and content in device-dependent units.
		/// </summary>
		static public float ContentTopMargin {
			get {
				// TODO adjust to device diplay format, raw config data should be ideal for 16:9.
				return ConfigurationManager.Current.contentTopMarginUnits;
			}
		}

		/// <summary>
		/// Margin between content and footer in device-dependent units.
		/// </summary>
		static public float ContentBottomMargin {
			get {
				// TODO adjust to device diplay format, raw config data should be ideal for 16:9.
				return ConfigurationManager.Current.contentBottomMarginUnits;
			}
		}

		/// <summary>
		/// Height of the footer element in device-dependent units.
		/// </summary>
		/// <value>The height of the footer.</value>
		static public float FooterHeight {
			get {
				// TODO adjust to device diplay format, raw config data should be ideal for 16:9.
				return ConfigurationManager.Current.footerHeightUnits;
			}
		}

		static public float ContentInnerSpaceHeightUnits {
			get {
				// TODO adjust to device diplay format, raw config data should be ideal for 16:9.
				return ConfigurationManager.Current.contentInnerSpaceHeightUnits;
			}
		}

		protected float TotalHeightUnits {
			get {
				return HeaderHeight + FooterHeight + (ContentInnerSpaceHeightUnits * (float) NumberOfSpacesInContent ());
			}
		}

		protected abstract int NumberOfSpacesInContent ();

		static public float SideMarginWidthUnits {
			get {
				// TODO adjust to device diplay format, raw config data should be ideal for 16:9.
				return ConfigurationManager.Current.sideMarginWidthUnits;
			}
		}

		#endregion
	}
}
