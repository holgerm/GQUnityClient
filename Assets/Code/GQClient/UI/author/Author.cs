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

        public static void OnSettingsChanged()
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

        public enum GQPrefKeys
        {
            LOGGED_IN_AS,
            SHOW_HIDDEN_QUESTS,
            OFFER_MANUAL_UPDATES,
            SHOW_EMPTY_MENU_ENTRIES
        }

    }

}
