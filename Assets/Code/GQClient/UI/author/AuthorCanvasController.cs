using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Util;
using GQ.Client.Conf;
using GQ.Client.Model;

namespace GQ.Client.UI
{

    public class AuthorCanvasController : MonoBehaviour
    {

        public Button LoginButton;
        Text LoginButtonText;

        public Text AccountInput;
        public Text PasswordInput;
        public GameObject SettingsPanel;
        private InputField AccountEmail;
        private InputField Password;
        public Text StatusText;

        const string LOGIN_TEXT = "Login";
        const string LOGOUT_TEXT = "Logout";

        const string NOT_LOGGED_IN_TEXT = "Sie sind <b>nicht</b> eingeloggt";
        const string LOGGED_IN_TEXTFORMAT = "Sie sind eingeloggt mit Autoren-Email\n<b>{0}</b>";


        // Use this for initialization
        void Start()
        {
            AccountEmail = AccountInput.transform.Find("InputField").GetComponent<InputField>();
            Password = PasswordInput.transform.Find("InputField").GetComponent<InputField>();
            LoginButtonText = LoginButton.transform.Find("Text").GetComponent<Text>();
            checkLoginButtonText();
            checkStatus();
            checkInput(AccountEmail.text, Password.text);
        }

        public void EmailChanged(string newMail)
        {
            checkInput(newMail, Password.text);
        }


        public void PasswordChanged(string newPassword)
        {
            checkInput(AccountEmail.text, newPassword);
        }

        void checkInput(string mail, string passwd)
        {
            LoginButton.interactable = (
                (
                    Author.LoggedInAs != null &&
                    Author.LoggedInAs != ""
                ) ||
                (
                    mail.Contains("@") &&
                    mail.Length >= 6 &&
                    passwd.Length >= 3
                )
            );
        }

        /// <summary>
        /// Depending on state of the logged-in-state we set the status text and show or hide the input fields.
        /// </summary>
        void checkStatus()
        {
            if (Author.LoggedInAs == null)
            {
                StatusText.text = NOT_LOGGED_IN_TEXT;
                AccountInput.gameObject.SetActive(true);
                PasswordInput.gameObject.SetActive(true);
                SettingsPanel.gameObject.SetActive(false);
            }
            else
            {
                StatusText.text = string.Format(LOGGED_IN_TEXTFORMAT, Author.LoggedInAs);
                AccountInput.gameObject.SetActive(false);
                PasswordInput.gameObject.SetActive(false);
                SettingsPanel.gameObject.SetActive(true);
                LoginButton.interactable = true;
            }
        }

        void checkLoginButtonText()
        {
            LoginButtonText.text =
                Author.LoggedInAs == null ?
                LOGIN_TEXT :
                LOGOUT_TEXT;
        }

        bool tryToLogin(string email, string password)
        {
            // TODO ask server for permissions ...
            if (ConfigurationManager.Current.defineAuthorBackDoor) {
                if (email == ConfigurationManager.Current.acceptedAuthorEmail &&
                    password == ConfigurationManager.Current.acceptedAuthorPassword) 
                {
                    Author.LoggedInAs = email;
                    //QuestInfoFilter.HiddenQuestsFilter.Instance.IsActive = true;
                    return true;
                }
            }

            // check for correct login server-side:
            return false;
        }

        public void LoginPressed()
        {
            switch (LoginButtonText.text)
            {
                case LOGIN_TEXT:
                    if (tryToLogin(AccountEmail.text, Password.text))
                    {
                        AccountEmail.text = "";
                        Password.text = "";
                    }
                    checkStatus();
                    checkLoginButtonText();
                    break;
                case LOGOUT_TEXT:
                    Author.LoggedInAs = null;
                    //QuestInfoFilter.HiddenQuestsFilter.Instance.IsActive = false;
                    checkStatus();
                    checkLoginButtonText();
                    break;
            }
        }

    }
}