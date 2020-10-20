using Code.GQClient.Util;
using Code.QM.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.Progress
{

    /// <summary>
    /// Connects the Progress UI with the behaviour implemented in a subclass of DialogBehaviour. 
    /// These behaviours are NOT MonoBehaviours but one of them must be set as connected in this component.
    /// 
    /// Why is this? It allows to use the dialog prefab for multiple purposes. 
    /// Therefore one has to instantiate the one dialog prefab and initialize it with aone of the available behaviours
    /// in a separate step by setting the connection. 
    /// 
    /// This can both be done by script. Manually in the editor only the first step can be done right now. 
    /// We would need a little custom editor to enable selection of available behaviours in the gui.
    /// 
    /// Anyway, we typically drive the dialog by calling some functionality, 
    /// hence it should be dynamically initialized and setup by script anyway
    /// 
    /// For details on how to link UI elements like this Dialog to Tasks cf. @ref TasksAndUI

    /// </summary>
    public class ProgressController : PrefabController
	{

        #region Content and Structure
        protected static readonly string PREFAB_ASSETBUNDLE = "prefabs";
        protected static readonly string PREFAB_NAME = "Progress";

		public TextMeshProUGUI Title;
		protected const string TITLE_PATH = "Title Label";
		public Slider ProgressSlider;

		public const string PROGRESS_CANVAS_PATH = "/ProgressCanvas";

		public ProgressBehaviour Behaviour { get; set; }
		#endregion


		#region Singleton
		private static ProgressController instance = null;

		/// <summary>
		/// Gets the instance. If the instance is used for the first time, 
		/// it will be created from the prefab and will be inactive.
		/// </summary>
		/// <value>The instance.</value>
		public static ProgressController Instance {
			get {
				if (instance == null)
                {
                    GameObject progresPanel = GameObject.Find(PROGRESS_CANVAS_PATH + "/" + PREFAB_NAME);
                    if (progresPanel != null)
                    {
                        instance = progresPanel.GetComponent<ProgressController>();
                    }
                }

                if (instance == null)
                    return Create (PREFAB_ASSETBUNDLE, PREFAB_NAME, GameObject.Find (PROGRESS_CANVAS_PATH)).GetComponent<ProgressController> ();
				else
					return instance;
			}
		}
        #endregion


        #region Runtime
        public override void Show(GameObject go = null)
        {
            Base.Instance.ProgressCanvas.SetActive(true);
            GameObject.Find(PROGRESS_CANVAS_PATH).GetComponent<Canvas>().enabled = true;
        }

        public override void Hide(GameObject go = null)
        {
            GameObject.Find(PROGRESS_CANVAS_PATH).GetComponent<Canvas>().enabled = false;
        }
        #endregion
    }
}