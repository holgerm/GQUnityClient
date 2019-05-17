using System;
using GQ.Client.Conf;
using GQ.Client.Err;
using GQ.Client.Model;
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

        public static void OnSettingsChanged()
        {
            EventHandler<EventArgs> handler = SettingsChanged;
            if (handler != null)
            {
                Debug.Log("AuthorSettings Changed - Event Fired!".Green());
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

        private static bool? _offerManualUpdate = null;
        public static bool OfferManualUpdate
        {
            get
            {
                if (_offerManualUpdate == null)
                {
                    if (PlayerPrefs.HasKey(Author.GQPrefKeys.OFFER_MANUAL_UPDATES.ToString()))
                    {
                        _offerManualUpdate = PlayerPrefs.GetInt(Author.GQPrefKeys.OFFER_MANUAL_UPDATES.ToString()) == 1;
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
                        Author.GQPrefKeys.OFFER_MANUAL_UPDATES.ToString(),
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
                    if (PlayerPrefs.HasKey(Author.GQPrefKeys.SHOW_HIDDEN_QUESTS.ToString()))
                    {
                        _showHiddenQuests = PlayerPrefs.GetInt(Author.GQPrefKeys.SHOW_HIDDEN_QUESTS.ToString()) == 1;
                    }
                    else
                    {
                        _showHiddenQuests = false;
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
                        Author.GQPrefKeys.SHOW_HIDDEN_QUESTS.ToString(),
                        _showHiddenQuests == true ? 1 : 0
                    );
                    PlayerPrefs.Save();
                    Author.OnSettingsChanged();

                    // obeye: filter logic is reverse to Base instance flag logic here:
                    QuestInfoFilter.HiddenQuestsFilter.Instance.IsActive = !value;
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
                    if (PlayerPrefs.HasKey(Author.GQPrefKeys.SHOW_EMPTY_MENU_ENTRIES.ToString()))
                    {
                        _showEmptyMenuEntries = PlayerPrefs.GetInt(Author.GQPrefKeys.SHOW_EMPTY_MENU_ENTRIES.ToString()) == 1;
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
                        Author.GQPrefKeys.SHOW_EMPTY_MENU_ENTRIES.ToString(),
                        _showEmptyMenuEntries == true ? 1 : 0
                    );
                    PlayerPrefs.Save();
                    Author.OnSettingsChanged();
                }
            }
        }

        public enum GQPrefKeys
        {
            LOGGED_IN_AS,
            SHOW_HIDDEN_QUESTS,
            OFFER_MANUAL_UPDATES,
            SHOW_EMPTY_MENU_ENTRIES
        }

    }

}
