// #define DEBUG_LOG

using System.Collections;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Model.pages;
using Code.GQClient.UI.Foyer.header;
using Code.GQClient.UI.layout;
using Code.GQClient.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.GQClient.UI.pages
{

    public abstract class PageController : UIController
	{
        public const string FOYER_SCENE = "Scenes/Foyer";

        protected Page page;
		protected QuestManager qm;
        public PageLayout layout;

        public GameObject FooterButtonPanel;
        protected Button forwardButton;
		protected Button backButton;
		public HeaderButtonPanel HeaderButtonPanel;


        #region Start

        bool resumingToFoyer;

		public virtual void Awake ()
		{
			resumingToFoyer = false;

			qm = QuestManager.Instance;
			if (qm.CurrentQuest == null || Page.IsNull(qm.CurrentPage)) {
				Debug.Log("Switching to Foyer ...".Green());
				SceneManager.LoadScene (FOYER_SCENE);
				resumingToFoyer = true;
				return;
			}
		}

		/// <summary>
        /// Called when this page does NOT HAVE A PREDECESSOR OF SAME TYPE. 
        /// Otherwise the page scene and also this page controller gets reused. 
        /// In that case only InitPage() is called directly from the Page model class.
        /// </summary>
        /// <returns>The start.</returns>
        public IEnumerator Start ()
		{
            while (!QuestManager.Instance.PageReadyToStart) {
                Debug.Log("Waiting for Page to be ready to start ...");
                yield return null;
            }

            InitPage();
        }

        /// <summary>
        /// Must be called manually if you want to reuse the page model, i.e. for the second of two consecutive pages 
        /// of the same model type.
        /// </summary>
        public void InitPage ()
		{
#if DEBUG_LOG
            Debug.Log("InitPage() on " + GetType());
#endif
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

            if (ConfigurationManager.Current.stopAudioWhenLeavingPage)
            {
                Audio.StopAllAudio();
            }

            // Footer:
            forwardButton = FooterButtonPanel.transform.Find("ForwardButton").GetComponent<Button>();
			var backButtonGo = FooterButtonPanel.transform.Find("BackButton");
			backButton = backButtonGo.GetComponent<Button>();
            backButtonGo.gameObject.SetActive(page.Quest.History.CanGoBackToPreviousPage);

            if (ConfigurationManager.Current.hideFooterIfPossible)
            {
                var footer = FooterButtonPanel.transform.parent;
                if (!page.HasEndEvents() && !backButtonGo.gameObject.activeInHierarchy)
                {
                    footer.gameObject.SetActive(false);
                }
            }

            page.TriggerOnStart();

			InitPage_TypeSpecific ();
            Base.Instance.BlockInteractions(false);
        }

        internal virtual bool OfferLeaveQuest => ConfigurationManager.Current.OfferLeaveQuests;

        #endregion


#region Runtime API

        public abstract void InitPage_TypeSpecific();

        /// <summary>
        /// Override this method to react on Back Button CLick (or similar events).
        /// </summary>
        public virtual void OnBackward ()
		{
            int prevPageId = page.Quest.History.GoBackToPreviousPage();
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
        public virtual bool ShowsTopMargin {
            get
            {
                return false;
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
                              ConfigurationManager.Current.contentTopMarginUnits +
			                  imageAreaHeight +
			                  ContentDividerUnits +
			                  ContentBottomMarginUnits
			              );
			return LayoutConfig.Units2Pixels (units);
		}

#endregion
	}
}
