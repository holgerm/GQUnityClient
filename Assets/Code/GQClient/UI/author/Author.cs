using System;
using GQ.Client.Conf;
using UnityEngine;

namespace GQ.Client.Util
{

    public class Author : MonoBehaviour
    {
        public void Awake()
        {
            Debug.Log("Author.Awake()");
        }

        public static event EventHandler<EventArgs> SettingsChanged;

        protected static void OnSettingsChanged()
        {
            EventHandler<EventArgs> handler = SettingsChanged;
            if (handler != null)
            {
                handler(null, null);
            }
        }



        public static bool LoggedIn
        {
            get
            {
                return !string.IsNullOrEmpty(LoggedInAs);
            }

        }

        public static string LoggedInAs
        {
            get
            {
                if (string.IsNullOrEmpty(_loggedInAs))
                {
                    if (PlayerPrefs.HasKey(GQPrefKeys.LOGGED_IN_AS.ToString()))
                    {
                        _loggedInAs = PlayerPrefs.GetString(GQPrefKeys.LOGGED_IN_AS.ToString());
                    }
                }
                return _loggedInAs;
            }
            set
            {
                if (value != _loggedInAs)
                {
                    _loggedInAs = value;
                    PlayerPrefs.SetString(GQPrefKeys.LOGGED_IN_AS.ToString(), _loggedInAs);
                    if (_loggedInAs == null || _loggedInAs == "")
                    {
                        PlayerPrefs.DeleteKey(GQPrefKeys.LOGGED_IN_AS.ToString());
                    }
                    PlayerPrefs.Save();
                    OnSettingsChanged();
                }
            }
        }
        private static string _loggedInAs = null;

        internal static bool ShowHiddenQuests
        {
            get
            {
                if (_showHiddenQuests == null)
                {
                    if (PlayerPrefs.HasKey(GQPrefKeys.SHOW_HIDDEN_QUESTS.ToString()))
                    {
                        _showHiddenQuests = PlayerPrefs.GetInt(GQPrefKeys.SHOW_HIDDEN_QUESTS.ToString()) == 1;
                    }
                    else
                    {
                        _showHiddenQuests = ConfigurationManager.Current.showHiddenQuests;
                    }
                }
                return (bool)_showHiddenQuests;
            }
            set
            {
                if (value != _showHiddenQuests)
                {
                    _showHiddenQuests = value;
                    OnSettingsChanged();
                    PlayerPrefs.SetInt(GQPrefKeys.SHOW_HIDDEN_QUESTS.ToString(), _showHiddenQuests == true ? 1 : 0);
                    PlayerPrefs.Save();
                }
            }
        }
        private static bool? _showHiddenQuests = null;

        public static bool offerManualUpdate
        {
            get
            {
                if (_offerManualUpdate == null)
                {
                    if (PlayerPrefs.HasKey(GQPrefKeys.OFFER_MANUAL_UPDATES.ToString()))
                    {
                        _offerManualUpdate = PlayerPrefs.GetInt(GQPrefKeys.OFFER_MANUAL_UPDATES.ToString()) == 1;
                    }
                    else
                    {
                        _offerManualUpdate = ConfigurationManager.Current.OfferManualUpdate4QuestInfos;
                    }
                }
                return (bool) _offerManualUpdate;
            }
            set
            {
                if (value != _offerManualUpdate)
                {
                    _offerManualUpdate = value;
                    OnSettingsChanged();
                    PlayerPrefs.SetInt(GQPrefKeys.OFFER_MANUAL_UPDATES.ToString(), _offerManualUpdate == true ? 1 : 0);
                    PlayerPrefs.Save();
                }
            }
        }
        private static bool? _offerManualUpdate = null;

        public static bool ShowEmptyMenuEntries
        {
            get
            {
                if (_showEmptyMenuEntries == null)
                {
                    if (PlayerPrefs.HasKey(GQPrefKeys.SHOW_EMPTY_MENU_ENTRIES.ToString()))
                    {
                        // use stored value when called first time this run but already called in earlier runs::
                        _showEmptyMenuEntries = PlayerPrefs.GetInt(GQPrefKeys.SHOW_EMPTY_MENU_ENTRIES.ToString()) == 1;
                    }
                    else
                    {
                        // Default value: Use config value when first time called:
                        _showEmptyMenuEntries = ConfigurationManager.Current.showEmptyMenuEntries;
                    }
                }
                return (bool)_showEmptyMenuEntries && LoggedIn;
            }
            set
            {
                if (value != _showEmptyMenuEntries)
                {
                    _showEmptyMenuEntries = value;
                    OnSettingsChanged();
                    PlayerPrefs.SetInt(GQPrefKeys.SHOW_EMPTY_MENU_ENTRIES.ToString(), _showEmptyMenuEntries == true ? 1 : 0);
                    PlayerPrefs.Save();
                }
            }
        }
        private static bool? _showEmptyMenuEntries = null;


        public enum GQPrefKeys
        {
            LOGGED_IN_AS,
            SHOW_HIDDEN_QUESTS,
            OFFER_MANUAL_UPDATES,
            SHOW_EMPTY_MENU_ENTRIES
        }

    }

}
