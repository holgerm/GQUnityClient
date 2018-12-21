using System;
using GQ.Client.Conf;
using UnityEngine;

namespace GQ.Client.Util
{

    public class Author : MonoBehaviour
    {

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
                return !string.IsNullOrEmpty(loggedInAs);
            }

        }

        public static string LoggedInAs
        {
            get
            {
                if (string.IsNullOrEmpty(loggedInAs))
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
                if (value != loggedInAs)
                {
                    loggedInAs = value;
                    OnSettingsChanged();
                    PlayerPrefs.SetString(GQPrefKeys.LOGGED_IN_AS.ToString(), loggedInAs);
                    if (loggedInAs == null || loggedInAs == "")
                    {
                        PlayerPrefs.DeleteKey(GQPrefKeys.LOGGED_IN_AS.ToString());
                    }
                    PlayerPrefs.Save();
                }
            }
        }
        private static string loggedInAs = null;

        public static bool EmulationMode
        {
            get
            {
                return (LoggedInAs != null);
            }
        }

        public static bool ShowHiddenQuests
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
                        _showHiddenQuests = !ConfigurationManager.Current.hideHiddenQuests;
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

        public static bool OfferManualUpdate
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
                        _offerManualUpdate = !ConfigurationManager.Current.hideHiddenQuests;
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


        public enum GQPrefKeys
        {
            LOGGED_IN_AS,
            SHOW_HIDDEN_QUESTS,
            OFFER_MANUAL_UPDATES
        }

    }

}
