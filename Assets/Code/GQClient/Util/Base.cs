// #define DEBUG_LOG

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.UI;
using Code.GQClient.UI.author;
using Code.GQClient.UI.Dialogs;
using Code.GQClient.UI.Progress;
using Code.GQClient.Util.http;
using Code.GQClient.Util.tasks;
using Code.QM.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.GQClient.Util
{
    public class Base : MonoBehaviour
    {
        #region Inspector Global Values

        public GameObject ListCanvas;
        public GameObject TopicTreeCanvas;
        public GameObject MapCanvas;
        public GameObject MapHolder;
        public GameObject MenuCanvas;
        public GameObject DialogCanvas;
        public GameObject ProgressCanvas;

        public Canvas partnersCanvas;
        public Canvas imprintCanvas;
        public Canvas feedbackCanvas;
        public Canvas privacyCanvas;
        public Canvas authorCanvas;

        public GameObject MenuTopLeftContent;
        public Canvas canvas4TopLeftMenu;
        public GameObject MenuTopRightContent;
        public Canvas canvas4TopRightMenu;

        public GameObject InteractionBlocker;

        #endregion


        #region Singleton

        public const string BASE = "Base";

        private static Base _instance = null;

        public static Base Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject baseGO = GameObject.Find(BASE);

                    if (baseGO == null)
                    {
                        baseGO = new GameObject(BASE);
                        Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
                    }

                    if (baseGO.GetComponent(typeof(Base)) == null)
                        baseGO.AddComponent(typeof(Base));

                    // initialize the instance:
                    _instance = (Base) baseGO.GetComponent(typeof(Base));
                    _instance.ProgressCanvas.SetActive(true);
                    _instance.ProgressCanvas.GetComponent<Canvas>().enabled = false;
                }

                return _instance;
            }
        }

        #endregion


        #region Foyer

        public const string FOYER_SCENE_NAME = "Foyer";

        private bool listShown;
        private bool mapShown;
        private bool menuShown;
        private bool imprintShown;

        private Dictionary<string, bool> canvasStates;

        /// <summary>
        /// Called when we leave the foyer towards a page.
        /// </summary>
        public void HideFoyerCanvases()
        {
            // store current show state and hide:
            GameObject[] rootGOs = UnityEngine.SceneManagement.SceneManager.GetSceneByName(FOYER_SCENE_NAME)
                .GetRootGameObjects();
            foreach (GameObject rootGo in rootGOs)
            {
                Canvas canv = rootGo.GetComponent<Canvas>();
                if (canv != null)
                {
                    canvasStates[canv.name] = canv.isActiveAndEnabled;
                    canv.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Called when we return to the foyer from a page.
        /// </summary>
        public void ShowFoyerCanvases()
        {
            // show again accordingg to stored state:
            GameObject[] rootGOs = UnityEngine.SceneManagement.SceneManager.GetSceneByName(FOYER_SCENE_NAME)
                .GetRootGameObjects();
            foreach (GameObject rootGo in rootGOs)
            {
                Canvas canv = rootGo.GetComponent<Canvas>();
                bool oldCanvState;
                if (canv != null)
                {
                    if (canvasStates.TryGetValue(canv.name, out oldCanvState))
                    {
                        canv.gameObject.SetActive(canvasStates[canv.name]);
                    }
                }
            }
        }

        #endregion


        #region LifeCycle

        void Awake()
        {
            // hide all canvases at first, we show the needed ones in initViews()
            GameObject[] rootGOs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject rootGo in rootGOs)
            {
                Canvas canv = rootGo.GetComponent<Canvas>();
                if (canv != null)
                {
                    if ("DialogCanvas".Equals(canv.name))
                    {
                        canv.gameObject.SetActive(true);
                    }
                    else
                    {
                        canv.gameObject.SetActive(false);
                    }
                }
            }

            DontDestroyOnLoad(Instance);
            SceneManager.sceneLoaded += SceneAdapter.OnSceneLoaded;
            canvasStates = new Dictionary<string, bool>();
        }

        private void Start()
        {
            partnersCanvas.gameObject.SetActive(ConfigurationManager.Current.showPartnersInfoAtStart);

#if DEBUG_LOG
            DisplaySizeInfo();

            void DisplaySizeInfo()
            {
                float w = Device.width / Device.dpi;
                float h = Device.height / Device.dpi;
                double d = Math.Sqrt(w * w + h * h);

                Resolution[] resolutions = Screen.resolutions;
                string resText = $"{resolutions.Length} resolutions: ";
                int i = 0;

                // Print the resolutions
                foreach (var res in resolutions)
                {
                    resText += $"res {++i} w: {res.width} h: {res.height}";
                }

                MessageDialog dialog =
                    new MessageDialog(
                        $"diagonale: {d} zoll, dpi: {Device.dpi}, width: {Device.width}, height: {Device.height} \n {resText}",
                        "Ok");
                dialog.Start();
            }
#endif
        }

        void Update()
        {
            //#if UNITY_EDITOR || UNITY_STANDALONE
            if (Author.LoggedIn)
            {
                Device.updateMockedLocation();
            }
            //#endif

            // Debug.Log($"Frame {Time.frameCount}, Time: {Time.time}, Deltatime: {Time.deltaTime}".Yellow());
        }

        #endregion


        #region Global Functions

        public void BlockInteractions(bool block)
        {
            if (block)
            {
                InteractionBlocker.SetActive(true);
            }
            else
            {
                CoroutineStarter.Run(UnblockInCoroutine());
            }
        }

        private IEnumerator UnblockInCoroutine()
        {
            yield return new WaitForEndOfFrame();
            InteractionBlocker.SetActive(false);
        }

        public DownloadBehaviour GetDownloadBehaviour(AbstractDownloader downloader, string title)
        {
            switch (ConfigurationManager.Current.taskUI)
            {
                case TaskUIMode.Dialog:
                    return new DownloadDialogBehaviour(downloader, title);
                case TaskUIMode.ProgressAtBottom:
                    return new DownloadProgressBehaviour(downloader, title);
                default:
                    Log.SignalErrorToDeveloper("Downloader TaskUI mode {0} is unknown, using default dialog instead.",
                        ConfigurationManager.Current.taskUI);
                    return new DownloadDialogBehaviour(downloader, title);
            }
        }

        public SimpleBehaviour GetSimpleBehaviour(Task task, string title, string details)
        {
            switch (ConfigurationManager.Current.taskUI)
            {
                case TaskUIMode.Dialog:
                    return new SimpleDialogBehaviour(task, title, details);
                case TaskUIMode.ProgressAtBottom:
                    return new SimpleProgressBehaviour(task, title, details);
                default:
                    Log.SignalErrorToDeveloper("Downloader TaskUI mode {0} is unknown, using default dialog instead.",
                        ConfigurationManager.Current.taskUI);
                    return new SimpleDialogBehaviour(task, title, details);
            }
        }

        #endregion
    }
}