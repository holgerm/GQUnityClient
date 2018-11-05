using UnityEngine;
using System.Collections;
using System.Globalization;
using System.Threading;
using GQ.Client.Err;
using GQ.Client.UI.Dialogs;
using UnityEngine.SceneManagement;
using GQ.Client.Conf;
using GQ.Client.Model;
using GQ.Client.UI;
using System;
using QM.Util;
using System.Collections.Generic;

namespace GQ.Client.Util
{

    public class Base : MonoBehaviour
    {
        #region Inspector Global Values

        public GameObject ListCanvas;
        public GameObject MapCanvas;
        public GameObject MapHolder;
        public GameObject MenuCanvas;
        public GameObject DialogCanvas;

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
                        Init();
                    }

                    if (baseGO.GetComponent(typeof(Base)) == null)
                        baseGO.AddComponent(typeof(Base));

                    _instance = (Base)baseGO.GetComponent(typeof(Base));

                    // Initialize QuestInfoManager:
                    QuestInfoManager.Instance.UpdateQuestInfos();
                }
                return _instance;
            }
        }

        #endregion


        #region Foyer

        public const string FOYER_SCENE = "Scenes/Foyer";
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
            GameObject[] rootGOs = UnityEngine.SceneManagement.SceneManager.GetSceneByName(FOYER_SCENE_NAME).GetRootGameObjects();
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
            GameObject[] rootGOs = UnityEngine.SceneManagement.SceneManager.GetSceneByName(FOYER_SCENE_NAME).GetRootGameObjects();
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

        public static void Init()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
        }

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

                Debug.Log("Device h: " + Device.height + ", w:" + Device.width + ", dpi: " + Device.dpi);
                Debug.Log("MapButtonHeight in pixels: " + LayoutConfig.Units2Pixels(ConfigurationManager.Current.mapButtonHeightUnits));
                Debug.Log("MapButtonHeight in mm: " + LayoutConfig.Units2MM(ConfigurationManager.Current.mapButtonHeightUnits));
                Debug.Log("MapButtonHeight calculated: " + LayoutConfig.Units2Pixels(FoyerMapScreenLayout.MapButtonHeightUnits));
                Debug.Log("MarkerHeight calculated: " + LayoutConfig.Units2Pixels(FoyerMapScreenLayout.MarkerHeightUnits));
            }

            DontDestroyOnLoad(Instance);
            SceneManager.sceneLoaded += SceneAdapter.OnSceneLoaded;
            canvasStates = new Dictionary<string, bool>();

#if UNITY_EDITOR || UNITY_STANDALONE
            Device.awakeLocationMock();
#endif

            //			#if UNITY_STANDALONE
            //			Screen.SetResolution(1080,1920,true);
            //			#endif
        }

        void Update()
        {
#if UNITY_EDITOR
            Device.updateMockedLocation();
#endif
        }

        #endregion


        #region Global Runtime State

        string loggedInAs = null;

        public string LoggedInAs
        {
            get
            {
                if (loggedInAs == null || loggedInAs == "")
                {
                    if (PlayerPrefs.HasKey(GQPrefKeys.LOGGED_IN_AS.ToString()))
                    {
                        loggedInAs = PlayerPrefs.GetString(GQPrefKeys.LOGGED_IN_AS.ToString());
                    }
                }
                return loggedInAs;
            }
            set
            {
                loggedInAs = value;
                PlayerPrefs.SetString(GQPrefKeys.LOGGED_IN_AS.ToString(), loggedInAs);
                if (loggedInAs == null || loggedInAs == "")
                {
                    PlayerPrefs.DeleteKey(GQPrefKeys.LOGGED_IN_AS.ToString());
                }
                PlayerPrefs.Save();
            }
        }

        public bool EmulationMode
        {
            get
            {
                return (LoggedInAs != null);
            }
        }

        bool? _showHiddenQuests = null;

        public bool ShowHiddenQuests
        {
            get
            {
                if (_showHiddenQuests == null) {
                    if (PlayerPrefs.HasKey(GQPrefKeys.SHOW_HIDDEN_QUESTS.ToString()))
                    {
                        _showHiddenQuests = PlayerPrefs.GetInt(GQPrefKeys.SHOW_HIDDEN_QUESTS.ToString()) == 1;
                    }
                    else
                    {
                        _showHiddenQuests = !ConfigurationManager.Current.hideHiddenQuests;
                    }
                }
                return (bool)_showHiddenQuests;
            }
            set
            {
                _showHiddenQuests = value;
                PlayerPrefs.SetInt(GQPrefKeys.SHOW_HIDDEN_QUESTS.ToString(), _showHiddenQuests == true ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        #endregion


        #region PlayerPrefs Keys

        public enum GQPrefKeys
        {
            LOGGED_IN_AS,
            SHOW_HIDDEN_QUESTS
        }

        #endregion
    }

}
