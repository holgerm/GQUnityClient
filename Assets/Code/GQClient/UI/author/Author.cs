using System;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using GQClient.Model;
using UnityEngine;

namespace Code.GQClient.UI.author
{

    public class Author : MonoBehaviour
    {
        public void Awake()
        {
            Debug.Log("Author.Awake()");
        }

        public static event EventHandler<EventArgs> SettingsChanged;

        public static void OnSettingsChanged()
        {
            if (SettingsChanged != null)
            {
                Debug.Log("AuthorSettings Changed - Event Fired!".Green());
                SettingsChanged(null, null);
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
                    if (PlayerPrefs.HasKey(PREFKEY_LOGGED_IN_AS))
                    {
                        _loggedInAs = PlayerPrefs.GetString(PREFKEY_LOGGED_IN_AS);
                    }
                }
                return _loggedInAs;
            }
            set
            {
                if (value == _loggedInAs)
                    return;

                _loggedInAs = value;

                if (string.IsNullOrEmpty(value))
                {
                    PlayerPrefs.DeleteKey(PREFKEY_LOGGED_IN_AS);
                }
                else
                {
                    PlayerPrefs.SetString(PREFKEY_LOGGED_IN_AS, _loggedInAs);
                }
                PlayerPrefs.Save();

                OnSettingsChanged();
            }
        }
        private static string _loggedInAs = null;

        private static bool? _offerManualUpdate = null;
        public static bool OfferManualUpdate
        {
            get
            {
                if (_offerManualUpdate == null)
                {
                    if (PlayerPrefs.HasKey(PREFKEY_OFFER_MANUAL_UPDATES))
                    {
                        _offerManualUpdate = PlayerPrefs.GetInt(PREFKEY_OFFER_MANUAL_UPDATES) == 1;
                    }
                    else
                    {
                        _offerManualUpdate = false;
                    }
                }
                return (bool)_offerManualUpdate && LoggedIn;
            }
            set
            {
                if (_offerManualUpdate != value)
                {
                    _offerManualUpdate = value;
                    PlayerPrefs.SetInt(
                        PREFKEY_OFFER_MANUAL_UPDATES,
                        _offerManualUpdate == true ? 1 : 0
                    );
                    PlayerPrefs.Save();
                    Author.OnSettingsChanged();
                }
            }
        }

        private static bool? _showHiddenQuests = null;
        public static bool ShowHiddenQuests
        {
            get
            {
                if (_showHiddenQuests == null)
                {
                    if (PlayerPrefs.HasKey(PREFKEY_SHOW_HIDDEN_QUESTS))
                    {
                        _showHiddenQuests = PlayerPrefs.GetInt(PREFKEY_SHOW_HIDDEN_QUESTS) == 1;
                    }
                    else
                    {
                        _showHiddenQuests = ConfigurationManager.Current.localQuestsDeletable;
                    }
                }
                return (bool)_showHiddenQuests && LoggedIn;
            }
            set
            {
                if (_showHiddenQuests != value)
                {
                    _showHiddenQuests = value;
                    PlayerPrefs.SetInt(
                        PREFKEY_SHOW_HIDDEN_QUESTS,
                        _showHiddenQuests == true ? 1 : 0
                    );
                    PlayerPrefs.Save();
                    Author.OnSettingsChanged();

                    // obeye: filter logic is reverse to Base instance flag logic here:
                    QuestInfoFilter.HiddenQuestsFilter.Instance.IsActive = !value;
                }
            }
        }

        private static bool? _showOnlyLocalQuests = null;
        public static bool ShowOnlyLocalQuests
        {
            get
            {
                if (_showOnlyLocalQuests == null)
                {
                    if (PlayerPrefs.HasKey(PREFKEY_SHOW_ONLY_LOCAL_QUESTS))
                    {
                        _showOnlyLocalQuests = PlayerPrefs.GetInt(PREFKEY_SHOW_ONLY_LOCAL_QUESTS) == 1;
                    }
                    else
                    {
                        _showOnlyLocalQuests = ConfigurationManager.Current.showOnlyLocalQuests;
                    }
                }
                return (bool)_showOnlyLocalQuests; // this flag does not need author to be logged in.
            }
            set
            {
                if (_showOnlyLocalQuests != value)
                {
                    _showOnlyLocalQuests = value;
                    PlayerPrefs.SetInt(
                        PREFKEY_SHOW_ONLY_LOCAL_QUESTS,
                        _showOnlyLocalQuests == true ? 1 : 0
                    );
                    PlayerPrefs.Save();
                    Author.OnSettingsChanged();
                    QuestInfoFilter.LocalQuestInfosFilter.Instance.IsActive = value;
                }
            }
        }

        private static bool? _showEmptyMenuEntries = null;
        public static bool ShowEmptyMenuEntries
        {
            get
            {
                if (_showEmptyMenuEntries == null)
                {
                    if (PlayerPrefs.HasKey(PREFKEY_SHOW_EMPTY_MENU_ENTRIES))
                    {
                        _showEmptyMenuEntries = PlayerPrefs.GetInt(PREFKEY_SHOW_EMPTY_MENU_ENTRIES) == 1;
                    }
                    else
                    {
                        _showEmptyMenuEntries = false;
                    }
                }
                return (bool)_showEmptyMenuEntries && LoggedIn;
            }
            set
            {
                if (_showEmptyMenuEntries != value)
                {
                    _showEmptyMenuEntries = value;
                    PlayerPrefs.SetInt(
                        PREFKEY_SHOW_EMPTY_MENU_ENTRIES,
                        _showEmptyMenuEntries == true ? 1 : 0
                    );
                    PlayerPrefs.Save();
                    Author.OnSettingsChanged();
                }
            }
        }

        private static bool? _showDeleteOptionForLocalQuests = null;
        public static bool ShowDeleteOptionForLocalQuests
        {
            get
            {
                if (_showDeleteOptionForLocalQuests == null)
                {
                    if (PlayerPrefs.HasKey(PREFKEY_SHOW_DELETE_OPTION))
                    {
                        _showDeleteOptionForLocalQuests = PlayerPrefs.GetInt(PREFKEY_SHOW_DELETE_OPTION) == 1;
                    }
                    else
                    {
                        _showDeleteOptionForLocalQuests = ConfigurationManager.Current.localQuestsDeletable;
                    }
                }
                return (bool)_showDeleteOptionForLocalQuests; // this flag does not need author to be logged in.
            }
            set
            {
                if (_showDeleteOptionForLocalQuests != value)
                {
                    _showDeleteOptionForLocalQuests = value;
                    PlayerPrefs.SetInt(
                        PREFKEY_SHOW_DELETE_OPTION,
                        _showDeleteOptionForLocalQuests == true ? 1 : 0
                    );
                    PlayerPrefs.Save();
                }
            }
        }

        public const string PREFKEY_LOGGED_IN_AS = "gq.settings.login";
        public const string PREFKEY_SHOW_HIDDEN_QUESTS = "gq.settings.show_hidden_quests";
        public const string PREFKEY_SHOW_ONLY_LOCAL_QUESTS = "gq.settings.show_only_local_quests";
        public const string PREFKEY_OFFER_MANUAL_UPDATES = "gq.settings.offer_manual_updates";
        public const string PREFKEY_SHOW_EMPTY_MENU_ENTRIES = "gq.settings.show_empty_menu_entries";
        public const string PREFKEY_SHOW_DELETE_OPTION = "gq.settings.show_delete_option";
    }

}
