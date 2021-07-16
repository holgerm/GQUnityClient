using System;
using System.IO;
using Code.GQClient.UI.author;
using Code.GQClient.Util.http;
using GQ.Editor.UI;
using UnityEditor;
using UnityEngine;


namespace Code.GQClient.Conf
{
    /// <summary>
    /// The Configuration manager is the runtime access point to information about the build configuration, 
    /// e.g. build time, version, contained markers and logos, links to server resources etc. 
    /// I.e. everything the app needs for the specific product it is branded to.
    /// 
    /// This class will completely replace the currently still used class Configuration 
    /// and additionally offer any information which is currently entered manually in the Unity Inspector View.
    /// </summary>
    public class ConfigurationManager
    {
        #region Initialize

        public static void Reset()
        {
            Config.Load();
        }


        /// <summary>
        /// Flag used to optimize frequent access to configured media files, e.g. symbol icons.
        /// If false, we omit checking for update infos.
        /// </summary>
        public static bool RTProductUpdated;

        public static void Initialize()
        {
            Config.Load();
            RTConfig.LoadFromServer();
        }

        private static string _version = null;

        public static string Version
        {
            get
            {
                if (_version == null)
                {
                    // get the string part before the first dot:
                    _version = Buildtime.Substring(0, Buildtime.IndexOf('.'));
                }

                return _version;
            }
        }

        #endregion

        #region Paths and general Resources

        public const string CONFIG_FILE = "Product.json";
        private const string BUILD_TIME_FILE_NAME = "buildtime";
        public const string BUILD_TIME_FILE_PATH = Config.RUNTIME_PRODUCT_DIR + "/" + BUILD_TIME_FILE_NAME + ".txt";

        public static string GetGQServerBaseURL()
        {
#if !UNITY_EDITOR
            return GQ_SERVER_BASE_URL;
#else
            if (GQDeveloperEditor.Instance.localPortalUsed)
                return "http://localhost:9000";
            else 
                return "https://quest-mill.intertech.de";
#endif
        }

        private static string GetPortalID()
        {
#if !UNITY_EDITOR
            return Config.Current.portal.ToString();
#else
            if (GQDeveloperEditor.Instance.localPortalUsed)
            {
                return GQDeveloperEditor.Instance.LocalPortalId();
            }
            else
            {
                return Config.Current.portal.ToString();
            }
#endif
        }


        public static string UrlPublicQuestsJSON =>
            $"{GetGQServerBaseURL()}/json/{GetPortalID()}/publicgamesinfo";

        #endregion

        #region RETRIEVING THE CURRENT PRODUCT

        private static string _buildtime = null;

        private static string Buildtime
        {
            get
            {
                if (_buildtime == null)
                {
                    try
                    {
                        TextAsset buildTimeAsset = Resources.Load(BUILD_TIME_FILE_NAME) as TextAsset;

                        if (buildTimeAsset == null)
                        {
                            throw new ArgumentException("Buildtime File does not represent a loadable asset. Cf. " +
                                                        BUILD_TIME_FILE_NAME);
                        }

                        _buildtime = buildTimeAsset.text;
                    }
                    catch (Exception exc)
                    {
                        Debug.LogWarning(
                            "Could not read build time file at " + BUILD_TIME_FILE_PATH + " " + exc.Message);
                        _buildtime = "unknown";
                    }
                }

                return _buildtime;
            }
            set { _buildtime = value; }
        }

        private static string _imprint = null;

        public static string Imprint
        {
            get
            {
                if (_imprint == null)
                {
                    TextAsset ta = Resources.Load("imprint") as TextAsset;
                    if (ta == null)
                        _imprint = "";
                    else
                        _imprint = ta.text;
                }

                return _imprint;
            }
            set { _imprint = value; }
        }


        private static string _terms = null;

        public static string Terms
        {
            get
            {
                if (_terms == null)
                {
                    TextAsset ta = Resources.Load("terms") as TextAsset;
                    if (ta == null)
                        _terms = "";
                    else
                        _terms = ta.text;
                }

                return _terms;
            }
            set => _terms = value;
        }


        private static string _privacyStatement = null;

        public static string PrivacyStatement
        {
            get
            {
                if (_privacyStatement == null)
                {
                    TextAsset ta = Resources.Load("privacy") as TextAsset;
                    if (ta == null)
                        _privacyStatement = "";
                    else
                        _privacyStatement = ta.text;
                }

                return _privacyStatement;
            }
            set => _privacyStatement = value;
        }

        #endregion
    }


    public enum QuestVisualizationMethod
    {
        list,
        map
    }
}